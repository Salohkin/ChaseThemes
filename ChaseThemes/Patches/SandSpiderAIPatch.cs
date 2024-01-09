using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(SandSpiderAI))]
    internal class SandSpiderAIPatch
    {
        static string audioCategory = "MAIN";
        static bool audioPlaying = false;
        static float playedTime = 0f;
        static float volume = 0.8f;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref AudioSource ___creatureVoice, ref float ___chaseTimer, ref bool ___watchFromDistance)
        {
            if (___currentBehaviourStateIndex == 2 && ___chaseTimer > 0 && !audioPlaying && !___watchFromDistance)
            {
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenThemes[audioCategory], volume);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                playedTime = 0f;
                audioPlaying = true;
            }
            else if (audioPlaying)
            {
                playedTime += Time.deltaTime;
                if (playedTime > RoundManagerPatch.chosenThemes[audioCategory].length)
                {
                    audioPlaying = false;
                }
            }
        }
    }

    /* Legacy Code
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
    */
}
