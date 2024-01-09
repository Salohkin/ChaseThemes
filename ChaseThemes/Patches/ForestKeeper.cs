using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(ForestGiantAI))]
    internal class ForestKeeperAIPatch : MonoBehaviour
    {
        static string audioCategory = "FORESTKEEPER";
        //static bool audioPlaying = false; // this fix works for host but not other clients
        static float volume = 0.7f;

        [HarmonyPatch("BeginChasingNewPlayerClientRpc")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref bool ___chasingPlayerInLOS, ref AudioSource ___creatureVoice)
        {
            if (___currentBehaviourStateIndex == 1 && !___chasingPlayerInLOS) //&& !audioPlaying
            {
                //audioPlaying = true;
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenThemes[audioCategory], volume);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
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
    internal class ForestKeeperEnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopchosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if (___currentBehaviourStateIndex == 0 && ___enemyType.enemyName.ToLower() == "forestgiant")
            {
                ChaseThemesBase.Instance.logger.LogInfo("Crawler stopped");
                ___creatureVoice.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
            }
        }
    }
    */
}
