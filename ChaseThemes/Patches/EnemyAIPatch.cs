using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChaseThemes.Patches
{
    internal class EnemyAIPatch
    {
        [HarmonyPatch("KillEnemyClientRpc")]
        [HarmonyPrefix] // prefix so the creature isn't destroyed (postfix might work, but haven't tested it)
        static void StopThemeOnDeath(EnemyAI __instance)
        {
            StopTheme(__instance);
        }

        static void StopTheme(EnemyAI __instance)
        {
            __instance.creatureVoice.Stop();
        }
        
        /*
        static bool isEnemy(EnemyAI enemy, string enemyName)
        {
            return (enemy.enemyType.enemyName.ToLower() == enemyName.ToLower());
        } */
    }
}
