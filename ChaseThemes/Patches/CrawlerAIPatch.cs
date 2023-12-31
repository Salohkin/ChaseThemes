using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(CrawlerAI))]
    internal class CrawlerAIPatch : MonoBehaviour
    {
        static string audioCategory = "MAIN";

        [HarmonyPatch("BeginChasingPlayerClientRpc")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref bool ___hasEnteredChaseMode, ref AudioSource ___creatureVoice)
        {
            if (___currentBehaviourStateIndex == 1 && !___hasEnteredChaseMode)
            {
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenThemes[audioCategory]);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
            }
        }
    }

    /* Legacy code
    [HarmonyPatch(typeof(EnemyAI))]
    internal class CrawlerEnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopchosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if (___currentBehaviourStateIndex == 0 && ___enemyType.enemyName.ToLower() == "crawler")
            {
                ChaseThemesBase.Instance.logger.LogInfo("Crawler stopped");
                ___creatureVoice.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
            }
        }
    }
    */
}
