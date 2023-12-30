using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(SandSpiderAI))]
    internal class SandSpiderAIPatch
    {
        public static bool alreadyPlaying = false;

        [HarmonyPatch("TriggerChaseWithPlayer")]
        [HarmonyPostfix]
        static void PlayChosenMainClip(ref int ___currentBehaviourStateIndex, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("CHASE THEME: Test");
            if (___currentBehaviourStateIndex == 2 && !alreadyPlaying)
            {
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenMainClip);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                alreadyPlaying = true;
            }
        }
    }

    [HarmonyPatch(typeof(EnemyAI))]
    internal class SpiderEnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopChosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if (___currentBehaviourStateIndex != 2 && ___enemyType.enemyName.ToLower() == "sandspider")
            {
                SandSpiderAIPatch.alreadyPlaying = false;
                ChaseThemesBase.Instance.logger.LogInfo("Spider stopped");
                ___creatureVoice.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
            }
        }
    }
}
