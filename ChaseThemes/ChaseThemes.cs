using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using ChaseThemes.Patches;
using UnityEngine;
using System.IO;
using LCSoundTool;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Globalization;

namespace ChaseThemes
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class ChaseThemesBase : BaseUnityPlugin
    {
        private const string pluginGUID = "LineLoad.ChaseThemes";
        private const string pluginName = "Chase Themes";
        private const string pluginVersion = "1.0.0";
        private readonly Harmony harmony = new Harmony(pluginGUID);

        public static ChaseThemesBase Instance;

        internal ManualLogSource logger;

        private static readonly char[] _invalidConfigChars = { '=', '\n', '\t', '\\', '\"', '\'', '[', ']' };

        public static string enabledEnemiesCategoryName = "* Enabled Creature Chase Themes *";
        public static string[] audioCategories = ["MAIN", "FORESTKEEPER", "GHOSTGIRL", "BLOB", "NUTCRACKER"];
        public static string[] configEnemyList = ["THUMPER", "BUNKER SPIDER", "HOARDING BUG", "FORESTKEEPER", "GHOSTGIRL", "BLOB", "NUTCRACKER"];
        public static Dictionary<string, bool> enabledEnemyThemes = new Dictionary<string, bool>();
        public static Dictionary<string, AudioClip[]> themeAudioClips = new Dictionary<string, AudioClip[]>();


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(pluginGUID);
            

            harmony.PatchAll(typeof(ChaseThemesBase));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(EnemyAIPatch));

            Array.Sort(audioCategories);
            Array.Sort(configEnemyList);

            GenerateConfig();
            LoadAudioFiles();

            PatchConfigEnabledEnemies();

            //AwakeAsync();

            logger.LogInfo($"CHASE THEMES: Plugin {pluginGUID} is loaded!");
        }

        /*
        private async Task AwakeAsync()
        {
            //await Task.Run(() => LoadAudioFiles());
            await LoadAudioFiles();
        }
        */

        private void PatchConfigEnabledEnemies()
        {
            foreach (string enemy in configEnemyList)
            {
                if (isConfigEnabledEnemy(enemy))
                {
                    switch (enemy)
                    {
                        case "THUMPER":
                            {
                                harmony.PatchAll(typeof(CrawlerAIPatch));
                                logger.LogDebug("CHASE THEMES: CrawlerAI patched!");
                                break;
                            }
                        case "BUNKER SPIDER":
                            {
                                harmony.PatchAll(typeof(SandSpiderAIPatch));
                                logger.LogDebug("CHASE THEMES: SandSpiderAI patched!");
                                break;
                            }
                        case "HOARDING BUG":
                            {
                                harmony.PatchAll(typeof(HoardingBugAIPatch));
                                logger.LogDebug("CHASE THEMES: HoardingBugAI patched!");
                                break;
                            }
                        case "FORESTKEEPER":
                            {
                                harmony.PatchAll(typeof(ForestKeeperAIPatch));
                                logger.LogDebug("CHASE THEMES: ForestKeeperAI patched!");
                                break;
                            }
                        case "GHOSTGIRL":
                            {
                                harmony.PatchAll(typeof(GhostGirlAIPatch));
                                logger.LogDebug("CHASE THEMES: GhostGirlAI patched!");
                                break;
                            }
                        case "BLOB":
                            {
                                harmony.PatchAll(typeof(BlobAIPatch));
                                logger.LogDebug("CHASE THEMES: BlobAI patched!");
                                break;
                            }
                        case "NUTCRACKER":
                            {
                                harmony.PatchAll(typeof(NutcrackerAIPatch));
                                logger.LogDebug("CHASE THEMES: NutcrackerAI patched!");
                                break;
                            }
                            //harmony.PatchAll(typeof(CircuitBeeAIPatch));
                    }
                }
            }
        }

        private void GenerateConfig()
        {
            foreach (string enemy in configEnemyList)
            {
                AddEntryToConfig(enabledEnemiesCategoryName, enemy, true);
                Config.TryGetEntry<bool>(enabledEnemiesCategoryName, enemy, out ConfigEntry<bool> entry);
                enabledEnemyThemes.Add(enemy, entry.Value);
            }
        }
        
        private void AddEntryToConfig(string category, string name, bool defaultValue)
        {
            try
            {
                logger.LogDebug("Attempting to add entry to the config: " + category + " | " + name);
                Config.Bind(category, name, defaultValue);
            }
            catch(Exception e)
            {
                logger.LogError(e);
            }
        }

        private string SanitiseString(string s)
        {
            logger.LogDebug("Sanitising string: " + s);

            foreach (char c in _invalidConfigChars)
            {
                s = s.Replace(c.ToString(), "");
            }

            logger.LogDebug("Sanitised string: " + s);

            return s;
        }

        private bool isConfigEnabledEnemy(string enemy)
        {
            Config.TryGetEntry<bool>(enabledEnemiesCategoryName, enemy, out ConfigEntry<bool> enemyEnabled);
            return enemyEnabled.Value;
        }

        private void LoadAudioFiles()
        {
            logger.LogInfo($"Retrieving themes...");
            string path = Path.Combine(Paths.PluginPath, "LineLoad-ChaseThemes");
            string[] songPaths = Directory.GetFiles(path, "*.ogg");
            int numLoaded = 0;
            Dictionary<string, int> categoryEntriesAmount = new Dictionary<string, int>();
            List<string> configAcceptedSongPaths = new List<string>();
            int categoryPos;
            string currentCategory;
            string songName;

            if (songPaths.Length != 0)
            {
                logger.LogDebug($"CHASE THEMES: Retrieved themes successfully. Number of themes retrieved: " + songPaths.Length);
            }
            else
            {
                logger.LogWarning($"CHASE THEMES: No themes found! ");
                return;
            }

            Array.Sort(songPaths);
            foreach (string s in audioCategories)
            {
                categoryEntriesAmount.Add(s, 0);
            }

            logger.LogInfo($"CHASE THEMES: Loading themes...");

            categoryPos = 0;
            currentCategory = audioCategories[categoryPos];
            for (int loadNum = 0; loadNum < songPaths.Length; loadNum++)
            {
                if (!songPaths[loadNum].Contains(currentCategory))
                {
                    categoryPos++;
                    currentCategory = audioCategories[categoryPos];
                }

                songName = SanitiseString(songPaths[loadNum].Replace(path, ""));
                logger.LogDebug($"CHASE THEMES: Detected " + currentCategory + " theme: " + songName);

                AddEntryToConfig(currentCategory, songName, true);
                Config.TryGetEntry<bool>(currentCategory, songName, out ConfigEntry<bool> entry);
                if (entry.Value)
                {
                    configAcceptedSongPaths.Add(songPaths[loadNum]);
                    categoryEntriesAmount[currentCategory]++;
                    logger.LogDebug($"CHASE THEMES: " + currentCategory + " theme number " + categoryEntriesAmount[currentCategory] + " accepted: " + songName);
                }
            }

            foreach (string s in audioCategories)
            {
                logger.LogDebug("CHASE THEMES: Number of " + s + " themes:" + categoryEntriesAmount[s]);
                themeAudioClips.Add(s, new AudioClip[categoryEntriesAmount[s]]);
            }

            categoryPos = 0;
            currentCategory = audioCategories[categoryPos];
            foreach (string songPath in configAcceptedSongPaths)
            {
                bool added = false;
                numLoaded++;

                if (!songPath.Contains(currentCategory))
                {
                    categoryPos++;
                    currentCategory = audioCategories[categoryPos];
                }

                songName = SanitiseString(songPath.Replace(path, ""));
                logger.LogInfo($"CHASE THEMES: Loading " + songName);

                if (songPath.Contains(currentCategory))
                {
                    categoryEntriesAmount[currentCategory]--;
                    themeAudioClips[currentCategory][categoryEntriesAmount[currentCategory]] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded " + currentCategory + " theme: " + songName);
                    added = true;
                }
                
                if (!added)
                {
                    numLoaded--;
                    logger.LogWarning($"Theme failed to load: " + songName);
                }
            }

            if (numLoaded == configAcceptedSongPaths.Count())
            {
                logger.LogInfo($"CHASE THEMES: All themes loaded successfully!");
            }
            else if (numLoaded > 0)
            {
                logger.LogWarning($"CHASE THEMES: Some themes failed to load successfully!");
            }
            else
            {
                logger.LogWarning($"Failed to load themes!");
            }
        }
    }
}
