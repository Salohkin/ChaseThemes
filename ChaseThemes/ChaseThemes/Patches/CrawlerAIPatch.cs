using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(CrawlerAI))]
    internal class CrawlerAIPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePatch(ref int ___currentBehaviourStateIndex, ref bool ___hasEnteredChaseMode, ref AudioSource ___creatureVoice)
        {
            if (___currentBehaviourStateIndex == 1 && !___hasEnteredChaseMode)
            {
                ThemeHandler.PlayTheme(ref ___creatureVoice);
            }
            else if (___currentBehaviourStateIndex == 0)
            {
                ThemeHandler.StopTheme(ref ___creatureVoice);
            }
        }
    }
}
