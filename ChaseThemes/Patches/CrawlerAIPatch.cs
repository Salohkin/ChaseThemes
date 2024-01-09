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
        //static bool audioPlaying = false; // this fix works for host but not other clients
        static float volume = 0.75f;

        [HarmonyPatch("BeginChasingPlayerClientRpc")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(CrawlerAI  __instance, ref bool ___hasEnteredChaseMode)
        {
            if (__instance.currentBehaviourStateIndex == 1 && !___hasEnteredChaseMode) //&& !audioPlaying
            {
                __instance.creatureVoice.PlayOneShot(RoundManagerPatch.chosenThemes[audioCategory], volume);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                //audioPlaying = true;
            }
            /*
            else if (audioPlaying)
            {
                audioPlaying = false;
            }
            */
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
