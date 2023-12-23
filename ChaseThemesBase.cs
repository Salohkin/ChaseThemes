using BepInEx;
using BepInEx.Logging;
using ChaseThemes.Patches;
using HarmonyLib;
using UnityEngine;

namespace ChaseThemes
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class ChaseThemesBase : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static ChaseThemesBase Instance;

        internal ManualLogSource logger;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            harmony.PatchAll(typeof(ChaseThemesBase));
            harmony.PatchAll(typeof(ThemeHandler));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(CrawlerAIPatch));
            harmony.PatchAll(typeof(HoarderBugAIPatch));
        }
    }
}