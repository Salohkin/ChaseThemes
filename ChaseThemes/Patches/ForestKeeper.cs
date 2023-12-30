using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(ForestGiantAI))]
    internal class ForestKeeperAIPatch : MonoBehaviour
    {
        [HarmonyPatch("BeginChasingNewPlayerClientRpc")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref bool ___chasingPlayerInLOS, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("methodTriggered ");
            if (___currentBehaviourStateIndex == 1 && !___chasingPlayerInLOS)
            {
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenForestKeeperClip);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
            }
        }
    }

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
}
