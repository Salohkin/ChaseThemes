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
using System.Reflection;
using static UnityEngine.UIElements.UIR.Implementation.UIRStylePainter;
using DunGen;
using UnityEngine.Assertions.Must;

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

        private static string path = Path.Combine(Paths.PluginPath, pluginGUID.Replace('.', '-'));
        private static string[] acceptedFileTypes = { "*.ogg", "*.mp3", "*.wav" };
        private static readonly char[] _invalidConfigChars = { '=', '\n', '\t', '\\', '\"', '\'', '[', ']' };
        private static string enabledEnemiesCategoryName = "*Enabled Creature Chase Themes*";
        private static string[] enemyList = ["THUMPER", "BUNKER SPIDER", "HOARDING BUG", "FORESTKEEPER", "GHOSTGIRL", "BLOB", "NUTCRACKER"];
        private static Dictionary<string, bool> enabledEnemyThemes = new Dictionary<string, bool>();
        //private static Dictionary<ConfigDefinition, bool> configExistingThemes = new Dictionary<ConfigDefinition, bool>();
        private static List<string> themePaths;
        private static List<string> themeNames;
        public static string[] themeCategories = ["MAIN", "FORESTKEEPER", "GHOSTGIRL", "BLOB", "NUTCRACKER"];
        public static Dictionary<string, AudioClip[]> themeAudioClips;


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

            Array.Sort(themeCategories);
            Array.Sort(enemyList);

            GenerateConfig();
            LoadAudioFiles();
            //UpdateConfigThemes();

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
            foreach (string enemy in enemyList)
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
            foreach (string enemy in enemyList)
            {
                AddEntryToConfig(enabledEnemiesCategoryName, enemy, true);
                Config.TryGetEntry<bool>(enabledEnemiesCategoryName, enemy, out ConfigEntry<bool> entry);
                enabledEnemyThemes.Add(enemy, entry.Value);
            }
        }

        private void UpdateConfigThemes()
        {
            logger.LogDebug("CHASE THEMES: Updating config");
            /*
            foreach (string theme in themePaths)
            {
                themeNames.Add(SanitiseString(theme.Replace(path, "")));
            }
            */
            logger.LogDebug("CHASE THEMES: Config keys: " + Config.Keys.ToString());

            foreach (ConfigDefinition key in Config.Keys)
            {
                logger.LogDebug("CHASE THEMES: Checking config entry for " + key.ToString());
                logger.LogDebug("CHASE THEMES: Entry section " + key.Section);
                logger.LogDebug("CHASE THEMES: Entry key " + key.Key);
                logger.LogDebug("CHASE THEMES: themeCategories.Contains(key.Section) " + themeCategories.Contains(key.Section));
                logger.LogDebug("CHASE THEMES: !themeNames.Contains(key.Key) " + !themeNames.Contains(key.Key));

                if (themeCategories.Contains(key.Section) && !themeNames.Contains(key.Key))
                {
                    logger.LogDebug("CHASE THEMES: Removing " + key.ToString());
                    Config.Remove(key);
                }
            }
        }

        /*
        private void ClearAllThemesFromConfig()
        {
            foreach (ConfigDefinition key in Config.Keys)
            {
                if (themeCategories.Contains(key.Section))
                {
                    Config.Remove(key);
                }
            }
        }
        */

        private void AddEntryToConfig(string category, string name, bool defaultValue)
        {
            try
            {
                logger.LogDebug("CHASE THEMES: Attempting to add entry to the config: " + category + " | " + name);
                Config.Bind(category, name, defaultValue);
            }
            catch(Exception e)
            {
                logger.LogError(e);
            }
        }

        private string SanitiseString(string s)
        {
            logger.LogDebug("CHASE THEMES: Sanitising string: " + s);

            foreach (char c in _invalidConfigChars)
            {
                s = s.Replace(c.ToString(), "");
            }

            logger.LogDebug("CHASE THEMES: Sanitised string: " + s);

            return s;
        }

        private bool isConfigEnabledEnemy(string enemy)
        {
            Config.TryGetEntry<bool>(enabledEnemiesCategoryName, enemy, out ConfigEntry<bool> enemyEnabled);
            return enemyEnabled.Value;
        }

        private void LoadAudioFiles()
        {
            themePaths = new List<string>();
            themeNames = new List<string>();

            List<string> acceptedThemePaths;
            int themesLoaded = 0;
            Dictionary<string, int> categoryEntriesAmount;
            int categoryPos;
            string currentCategory;
            string themeName;
            

            logger.LogDebug($"CHASE THEMES: Retrieving themes...");

            foreach (string fileType in acceptedFileTypes)
            {
                themePaths.AddRange(Directory.GetFiles(path, fileType));
            }
            
            if (themePaths.Count() == 0)
            {
                logger.LogWarning($"CHASE THEMES: No themes found!");
                return;
            }

            logger.LogDebug($"CHASE THEMES: Retrieved themes successfully. Number of themes retrieved: " + themePaths.Count());
            themePaths.Sort();
            categoryEntriesAmount = new Dictionary<string, int>();
            acceptedThemePaths = new List<string>();
            themeAudioClips = new Dictionary<string, AudioClip[]>();

            foreach (string s in themeCategories)
            {
                categoryEntriesAmount.Add(s, 0);
            }

            logger.LogInfo($"CHASE THEMES: Loading themes...");

            categoryPos = 0;
            currentCategory = themeCategories[categoryPos];
            for (int loadIndex = 0; loadIndex < themePaths.Count(); loadIndex++)
            {
                if (!themePaths[loadIndex].Contains(currentCategory))
                {
                    categoryPos++;
                    currentCategory = themeCategories[categoryPos];
                }

                themeName = SanitiseString(themePaths[loadIndex].Replace(path, ""));
                logger.LogDebug($"CHASE THEMES: Detected " + currentCategory + " theme: " + themeName);
                themeNames.Add(themeName);
                
                AddEntryToConfig(currentCategory, themeName, true);
                Config.TryGetEntry<bool>(currentCategory, themeName, out ConfigEntry<bool> entry);
                if (entry.Value)
                {
                    acceptedThemePaths.Add(themePaths[loadIndex]);
                    categoryEntriesAmount[currentCategory]++;
                    logger.LogDebug($"CHASE THEMES: " + currentCategory + " theme number " + categoryEntriesAmount[currentCategory] + " accepted: " + themeName);
                }
            }

            foreach (string s in themeCategories)
            {
                logger.LogDebug("CHASE THEMES: Number of " + s + " themes:" + categoryEntriesAmount[s]);
                themeAudioClips.Add(s, new AudioClip[categoryEntriesAmount[s]]);
            }

            categoryPos = 0;
            currentCategory = themeCategories[categoryPos];
            bool themeSuccessfullyAdded;
            foreach (string themePath in acceptedThemePaths)
            {
                themeSuccessfullyAdded = false;
                themesLoaded++;

                if (!themePath.Contains(currentCategory))
                {
                    categoryPos++;
                    currentCategory = themeCategories[categoryPos];
                }

                themeName = SanitiseString(themePath.Replace(path, ""));
                logger.LogDebug($"CHASE THEMES: Loading " + themeName);

                if (themePath.Contains(currentCategory))
                {
                    categoryEntriesAmount[currentCategory]--;
                    themeAudioClips[currentCategory][categoryEntriesAmount[currentCategory]] = SoundTool.GetAudioClip(Paths.PluginPath, themePath);
                    logger.LogInfo($"CHASE THEMES: Loaded " + currentCategory + " theme: " + themeName);
                    themeSuccessfullyAdded = true;
                }
                
                if (!themeSuccessfullyAdded)
                {
                    themesLoaded--;
                    logger.LogWarning($"CHASE THEMES: Theme failed to load: " + themeName);
                }
            }

            if (themesLoaded == acceptedThemePaths.Count())
            {
                logger.LogInfo($"CHASE THEMES: All themes loaded successfully!");
            }
            else if (themesLoaded > 0)
            {
                logger.LogWarning($"CHASE THEMES: Some themes failed to load!");
            }
            else
            {
                logger.LogWarning($"CHASE THEMES: Failed to load themes!");
            }
        }
    }
}
