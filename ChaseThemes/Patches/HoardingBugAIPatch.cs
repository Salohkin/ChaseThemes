using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(HoarderBugAI))]
    internal class HoardingBugAIPatch
    {
        [HarmonyPatch("IsHoarderBugAngry")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref AudioSource ___creatureVoice, ref bool ___inChase)
        {
            if (___currentBehaviourStateIndex == 2 && !___inChase)
            {
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenMainClip);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
            }
        }
    }

    [HarmonyPatch(typeof(EnemyAI))]
    internal class HoarderBugEnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopchosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if ((___currentBehaviourStateIndex == 0 || ___currentBehaviourStateIndex == 1) && ___enemyType.enemyName.ToLower() == "hoarding bug")
            {
                ChaseThemesBase.Instance.logger.LogInfo("Hoarder bug stopped");
                ChaseThemesBase.Instance.logger.LogInfo(___enemyType.enemyName.ToLower());
                ___creatureVoice.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
            }
        }
    }
}
