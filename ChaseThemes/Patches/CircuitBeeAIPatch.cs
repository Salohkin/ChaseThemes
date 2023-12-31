using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(RedLocustBees))]
    internal class CircuitBeeAIPatch
    {
        static string audioCategory = "MAIN";
        static bool alreadyPlaying = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlayChosenMainClip(ref int ___currentBehaviourStateIndex, ref AudioSource ___creatureVoice)
        {
            alreadyPlaying = false;
            if (___currentBehaviourStateIndex == 2 && !alreadyPlaying)
            {
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenThemes[audioCategory]);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                alreadyPlaying = true;
            }
        }
    }

    /* Legacy code
    [HarmonyPatch(typeof(EnemyAI))]
    internal class CircuitBeeEnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopChosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            //ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if (___currentBehaviourStateIndex == 0 && ___enemyType.enemyName.ToLower() == "red locust bees")
            {
                ___creatureVoice.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
            }
        }
    }
    */
}
