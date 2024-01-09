using BepInEx;
using HarmonyLib;
using LCSoundTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        public static Dictionary<string, AudioClip> chosenThemes = new Dictionary<string, AudioClip>();

        [HarmonyPatch("GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        static void LoadTheme(ref StartOfRound ___playersManager)
        {
            try
            {
                SelectRandomThemes(getLevelSeed(ref ___playersManager));
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Chase theme loaded successfully!");
            }
            catch (Exception e)
            {
                ChaseThemesBase.Instance.logger.LogWarning(e);
            }
        }

        static int getLevelSeed(ref StartOfRound ___playersManager)
        {
            ChaseThemesBase.Instance.logger.LogDebug("CHASE THEMES: Map seed is " + ___playersManager.randomMapSeed + " !!!");
            return (___playersManager.randomMapSeed);
        }
        
        static void SelectRandomThemes(int seed)
        {
            int numOfClips;
            int clipNumber;

            chosenThemes.Clear();

            foreach (string currentCategory in ChaseThemesBase.themeCategories)
            {
                numOfClips = ChaseThemesBase.themeAudioClips[currentCategory].Length;
                clipNumber = seed % numOfClips;
                chosenThemes.TryAdd(currentCategory, ChaseThemesBase.themeAudioClips[currentCategory][clipNumber]);

                ChaseThemesBase.Instance.logger.LogDebug("CHASE THEMES: Number of clips: " + numOfClips + " Chosen clip number " + clipNumber);
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: " + currentCategory + " clip successfully chosen: " + chosenThemes[currentCategory].ToString());
            }
        }
    }
}
