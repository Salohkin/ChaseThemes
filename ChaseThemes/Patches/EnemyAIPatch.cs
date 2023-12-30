using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("KillEnemyClientRpc")]
        [HarmonyPrefix] // prefix so the creature isn't already destroyed when this runs (postfix might work, but haven't tested it)
        static void StopThemeOnDeath(EnemyAI __instance)
        {
            StopTheme(__instance);
        }

        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopThemeOnPassiveBehaviourState(EnemyAI __instance)
        {
            if (__instance.currentBehaviourStateIndex == 0 && (isEnemy(__instance, "crawler") || isEnemy(__instance, "forestgiant")))
            {
                StopTheme(__instance);
            }
            else if (__instance.currentBehaviourStateIndex != 2 && (isEnemy(__instance, "hoarding bug") || isEnemy(__instance, "sandspider")))
            {
                StopTheme(__instance);
            }
            else if (__instance.currentBehaviourStateIndex == 0 && isEnemy(__instance, "girl"))
            {
                GhostGirlAIPatch.GirlThemeSource.Stop();
            }
        }

        static void StopTheme(EnemyAI __instance)
        {
            __instance.creatureVoice.Stop();
            ChaseThemesBase.Instance.logger.LogDebug("Chase theme stopped!");
        }

        static bool isEnemy(EnemyAI enemy, string enemyName)
        {
            return (enemy.enemyType.enemyName.ToLower() == enemyName.ToLower());
        }
    }
}
