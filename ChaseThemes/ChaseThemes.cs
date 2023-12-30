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

        public static List<string> configSections;

        public static AudioClip[] defaultAudioClips;
        public static AudioClip[] forestKeeperAudioClips;
        public static AudioClip[] ghostGirlAudioClips;
        public static AudioClip[] blobAudioClips;
        public static AudioClip[] nutcrackerAudioClips;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(pluginGUID);

            //GenerateConfig();

            harmony.PatchAll(typeof(ChaseThemesBase));
            harmony.PatchAll(typeof(RoundManagerPatch));

            harmony.PatchAll(typeof(CrawlerAIPatch));
            harmony.PatchAll(typeof(SandSpiderAIPatch));
            harmony.PatchAll(typeof(HoardingBugAIPatch));
            harmony.PatchAll(typeof(GhostGirlAIPatch));
            harmony.PatchAll(typeof(ForestKeeperAIPatch));
            harmony.PatchAll(typeof(BlobAIPatch));
            harmony.PatchAll(typeof(NutcrackerAIPatch));
            //harmony.PatchAll(typeof(CircuitBeeAIPatch));

            harmony.PatchAll(typeof(EnemyAIPatch));

            LoadAudioFiles();

            //AwakeAsync();

            logger.LogInfo($"CHASE THEMES: Plugin {pluginGUID} is loaded!");
        }
        /*
        private async Task AwakeAsync()
        {
            //await Task.Run(() => LoadAudioFiles());
            await LoadAudioFiles();
        }

        private void GenerateConfig()
        {
            configSections = new List<string> { "MAIN", "THUMPER", "BUNKER SPIDER", "HOARDING BUG", "FORESTKEEPER", "GHOSTGIRL", "BLOB", "NUTCRACKER" };
        }
        */

        private void AddEntryToConfig(string category, int count, string name)
        {
            //TODO: * Add string cleaning to make legal strings in the config. No " ' ] [ allowed (not sure about other characters - have to test it)
            //      * Make sure existing config entries aren't overwritten
            try
            {
                /*
                name.Replace("\"", "");
                name.Replace("\'", "");
                name.Replace("]", ")");
                name.Replace("[", "(");

                if (!Config.ContainsKey(new ConfigDefinition(category, name)))
                { 
                    logger.LogDebug("[" + category + "|" + name + "] does not already exist in the config... adding it now"); */
                    if (count > 0)
                    {
                        Config.Bind(category, name, true);
                    }
                    else
                    {
                        Config.Bind(new ConfigDefinition(category, name), true);
                    } /*
                }
                else
                {
                    logger.LogDebug("[" + category + "|" + name + "] already exists in the config");
                } */
            }
            catch(Exception e)
            {
                logger.LogError(e);
            }
        }
        
        private void LoadAudioFiles()
        {
            logger.LogInfo($"Retrieving themes...");
            string path = Path.Combine(Paths.PluginPath, "LineLoad-ChaseThemes");
            string[] songPaths = Directory.GetFiles(path, "*.ogg");
            int numLoaded = 0;
            int defaultAmount = 0;
            int forestAmount = 0;
            int girlAmount = 0;
            int blobAmount = 0;
            int nutcrackerAmount = 0;

            Array.Sort(songPaths);

            if (songPaths.Length != 0)
            {
                logger.LogInfo($"CHASE THEMES: Retrieved themes Successfully. Number of themes Retrieved: " + songPaths.Length);
            }
            else
            {
                logger.LogWarning($"CHASE THEMES: No themes found! ");
            }

            logger.LogInfo($"CHASE THEMES: Loading themes...");

            for (int loadNum = 0; loadNum < songPaths.Length; loadNum++)
            {
                logger.LogInfo($"CHASE THEMES: Detected " + songPaths[loadNum].Replace(path, ""));

                if (songPaths[loadNum].Contains("MAIN"))
                {
                    AddEntryToConfig("MAIN", nutcrackerAmount, songPaths[loadNum].Replace(path + "\\", ""));
                    defaultAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected MAIN theme: " + songPaths[loadNum].Replace(path, ""));
                }
                if (songPaths[loadNum].Contains("FORESTKEEPER"))
                {
                    AddEntryToConfig("FORESTKEEPER", nutcrackerAmount, songPaths[loadNum].Replace(path + "\\", ""));
                    forestAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected FORESTKEEPER theme: " + songPaths[loadNum].Replace(path, ""));
                }
                if (songPaths[loadNum].Contains("GHOSTGIRL"))
                {
                    AddEntryToConfig("GHOSTGIRL", nutcrackerAmount, songPaths[loadNum].Replace(path + "\\", ""));
                    girlAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected GHOSTGIRL Theme: " + songPaths[loadNum].Replace(path, ""));
                }
                if (songPaths[loadNum].Contains("BLOB"))
                {
                    AddEntryToConfig("BLOB", nutcrackerAmount, songPaths[loadNum].Replace(path + "\\", ""));
                    blobAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected BLOB Theme: " + songPaths[loadNum].Replace(path, ""));
                }
                if (songPaths[loadNum].Contains("NUTCRACKER"))
                {
                    AddEntryToConfig("NUTCRACKER", nutcrackerAmount, songPaths[loadNum].Replace(path + "\\", ""));
                    nutcrackerAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected NUTCRACKER Theme: " + songPaths[loadNum].Replace(path, ""));
                }
            }

            logger.LogInfo("CHASE THEMES: Number of themes in MAIN:" + defaultAmount);
            logger.LogInfo("CHASE THEMES: Number of themes in GHOSTGIRL:" + girlAmount);

            defaultAudioClips = new AudioClip[defaultAmount];
            forestKeeperAudioClips = new AudioClip[forestAmount];
            ghostGirlAudioClips = new AudioClip[girlAmount];
            blobAudioClips = new AudioClip[blobAmount];
            nutcrackerAudioClips = new AudioClip[nutcrackerAmount];

            foreach (string songPath in songPaths)
            {
                bool added = false;
                numLoaded++;

                logger.LogInfo($"CHASE THEMES: Loading " + songPath.Replace(path, ""));

                if (songPath.Contains("MAIN"))
                {
                    defaultAmount--;
                    defaultAudioClips[defaultAmount] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded MAIN theme: " + songPath.Replace(path, ""));
                    added = true;
                }
                if (songPath.Contains("FORESTKEEPER"))
                {
                    forestAmount--;
                    forestKeeperAudioClips[forestAmount] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded FORESTKEEPER theme: " + songPath.Replace(path, ""));
                    added = true;
                }
                if (songPath.Contains("GHOSTGIRL"))
                {
                    girlAmount--;
                    ghostGirlAudioClips[girlAmount] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded GHOSTGIRL theme: " + songPath.Replace(path, ""));
                    added = true;
                }
                if (songPath.Contains("BLOB"))
                {
                    blobAmount--;
                    blobAudioClips[blobAmount] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded BLOB theme: " + songPath.Replace(path, ""));
                    added = true;
                }
                if (songPath.Contains("NUTCRACKER"))
                {
                    nutcrackerAmount--;
                    nutcrackerAudioClips[nutcrackerAmount] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded NUTCRACKER theme: " + songPath.Replace(path, ""));
                    added = true;
                }
                if (!added)
                {
                    numLoaded--;
                    logger.LogWarning($"Theme failed to load: " + songPath.Replace(path, ""));
                }
            }


            if (numLoaded == songPaths.Length)
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
