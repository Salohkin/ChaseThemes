using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ChaseThemes.Patches;
using UnityEngine;
using System.IO;
using LCSoundTool;
using System.Collections.Generic;
using System.Linq;

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

        public static AudioClip[] defaultAudioClips;

        public static AudioClip[] forestKeeperAudioClips;

        public static AudioClip[] ghostGirlAudioClips;

        public static AudioClip[] gooAudioClips;

        public static AudioClip[] nutcrackerAudioClips;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(pluginGUID);
            logger.LogInfo($"CHASE THEMES: Plugin {pluginGUID} is loaded!");


            harmony.PatchAll(typeof(ChaseThemesBase));
            harmony.PatchAll(typeof(RoundManagerPatch));
            harmony.PatchAll(typeof(RoundManagerPatch));

            harmony.PatchAll(typeof(CrawlerAIPatch));
            harmony.PatchAll(typeof(SandSpiderAIPatch));
            harmony.PatchAll(typeof(HoardingBugAIPatch));
            harmony.PatchAll(typeof(GhostGirlAIPatch));
            harmony.PatchAll(typeof(ForestKeeperAIPatch));
            harmony.PatchAll(typeof(GooAIPatch));
            harmony.PatchAll(typeof(NutcrackerAIPatch));
            //harmony.PatchAll(typeof(CircuitBeeAIPatch));

            harmony.PatchAll(typeof(CrawlerEnemyAIPatch));
            harmony.PatchAll(typeof(SpiderEnemyAIPatch));
            harmony.PatchAll(typeof(HoarderBugEnemyAIPatch));
            harmony.PatchAll(typeof(GhostGirlEnemyAIPatch));
            harmony.PatchAll(typeof(ForestKeeperEnemyAIPatch));
            //harmony.PatchAll(typeof(CircuitBeeEnemyAIPatch));

            loadAudioFiles();
        }

        private void loadAudioFiles()
        {
            logger.LogInfo($"Retrieving songs...");
            string path = Path.Combine(Paths.PluginPath, "LineLoad-ChaseThemes");
            string[] songPaths = Directory.GetFiles(path, "*.ogg");
            int numLoaded = 0;
            int defaultAmount = 0;
            int forestAmount = 0;
            int girlAmount = 0;
            int gooAmount = 0;
            int nutcrackerAmount = 0;



            if (songPaths.Length != 0)
            {
                logger.LogInfo($"CHASE THEMES: Retrieved Songs Successfully. Number of songs Retrieved: " + songPaths.Length);
            }
            else
            {
                logger.LogWarning($"CHASE THEMES: No Songs Found! ");
            }
            
            logger.LogInfo($"CHASE THEMES: Loading Songs...");

            for (int loadNum = 0; loadNum < songPaths.Length; loadNum++)
            {
           
                logger.LogInfo($"CHASE THEMES: Detected " + songPaths[loadNum]);
                if (songPaths[loadNum].Contains("MAIN"))
                {
                    defaultAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected Main Song: " + songPaths[loadNum]);
                } 
                if (songPaths[loadNum].Contains("FORESTKEEPER"))
                {
                    forestAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected Forest Keeper Song: " + songPaths[loadNum]);
                }
                if (songPaths[loadNum].Contains("GHOSTGIRL"))
                {
                    girlAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected Ghost Girl Song: " + songPaths[loadNum]);
                }
                if (songPaths[loadNum].Contains("GOO"))
                {
                    gooAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected Goo Song: " + songPaths[loadNum]);
                }
                if (songPaths[loadNum].Contains("NUTCRACKER"))
                {
                    nutcrackerAmount++;
                    logger.LogInfo($"CHASE THEMES: Detected Nutcracker Song: " + songPaths[loadNum]);
                } 
            }
            logger.LogInfo("CHASE THEMES: Number of songs in main:" + defaultAmount);
            logger.LogInfo("CHASE THEMES: Number of songs in Ghost Girl:" + girlAmount);
            
            defaultAudioClips = new AudioClip[defaultAmount];
            forestKeeperAudioClips = new AudioClip[forestAmount];
            ghostGirlAudioClips = new AudioClip[girlAmount];
            gooAudioClips = new AudioClip[gooAmount];
            nutcrackerAudioClips = new AudioClip[nutcrackerAmount];

            foreach (string songPath in songPaths)
            {
                bool added = false;
                numLoaded++;
                logger.LogInfo($"CHASE THEMES: Loading " + songPath);
                if (songPath.Contains("MAIN"))
                {
                    defaultAudioClips[defaultAmount-1] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded Main Song: " + songPath);
                    defaultAmount--;
                    added = true;
                }
                if (songPath.Contains("FORESTKEEPER"))
                {
                    forestKeeperAudioClips[forestAmount-1] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded Forest Keeper Song: " + songPath);
                    forestAmount--;
                    added = true;
                }
                if (songPath.Contains("GHOSTGIRL"))
                {
                    ghostGirlAudioClips[girlAmount-1] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded Ghost Girl Song: " + songPath);
                    girlAmount--;
                    added = true;
                }
                if (songPath.Contains("GOO"))
                {
                    gooAudioClips[gooAmount - 1] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded Goo Song: " + songPath);
                    gooAmount--;
                    added = true;
                }
                if (songPath.Contains("NUTCRACKER"))
                {
                    nutcrackerAudioClips[nutcrackerAmount - 1] = SoundTool.GetAudioClip(Paths.PluginPath, songPath);
                    logger.LogInfo($"CHASE THEMES: Loaded Nutcracker Song: " + songPath);
                    nutcrackerAmount--;
                    added = true;
                }
                if (!added)
                {
                    numLoaded--;
                    logger.LogWarning($"Song failed to load: " + songPath);
                }
            }


            if (numLoaded == songPaths.Length)
            {
                logger.LogInfo($"CHASE THEMES: All Songs Loaded Successfully!");
            } else if (numLoaded > 0)
            {
                logger.LogWarning($"CHASE THEMES: Some songs failed to load successfully!");
            } else
            {
                logger.LogWarning($"Failed to load songs!");
            }

        }
    }
}
