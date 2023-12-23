using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(HoarderBugAI))]
    internal class HoarderBugAIPatch
    {
        static bool playing = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePatch(ref int ___currentBehaviourStateIndex, ref bool ___isAngry, ref AudioSource ___creatureVoice, ref bool ___isEnemyDead)
        {
            if (!___isEnemyDead)
            {
                if (!playing && ___isAngry)
                {
                    ThemeHandler.PlayTheme(ref ___creatureVoice);
                    playing = true;
                }
                else if (playing && !___isAngry)
                {
                    ThemeHandler.StopTheme(ref ___creatureVoice);
                    playing = false;
                }
            }
            else if (playing)
            {
                ThemeHandler.StopTheme(ref ___creatureVoice);
                playing = false;
                ChaseThemesBase.Instance.logger.LogInfo("the bug is dead :(");
            }
        }
    }
}
