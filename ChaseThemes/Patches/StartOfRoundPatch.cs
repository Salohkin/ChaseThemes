using BepInEx;
using HarmonyLib;
using System;
using LCSoundTool;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        // move load theme to theme handler and make it possible to load multiple themes
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void LoadTheme()
        {
            try
            {
                string path = Path.Combine(Paths.PluginPath, "ChaseThemes");
                ThemeHandler.theme = SoundTool.GetAudioClip(path, "Assets", "Thomas_the_Tank_Engine_Theme.ogg");
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme loaded successfully!");
            }
            catch (Exception e)
            {
                ChaseThemesBase.Instance.logger.LogWarning(e);
            }
        }
    }
}
