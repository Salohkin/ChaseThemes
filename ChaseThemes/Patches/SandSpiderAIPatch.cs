using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(SandSpiderAI))]
    internal class SandSpiderAIPatch
    {
        static bool alreadyPlaying = false;
        [HarmonyPatch("TriggerChaseWithPlayer")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref AudioSource ___creatureVoice)
        {
            alreadyPlaying = false;
            ChaseThemesBase.Instance.logger.LogInfo("CAHSE THEME: Test");
            if (___currentBehaviourStateIndex == 2 && !alreadyPlaying)
            {
                ___creatureVoice.PlayOneShot(StartOfRoundPatch.chosenMainClip);
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
        static void StopchosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if (___currentBehaviourStateIndex != 2 && ___enemyType.enemyName.ToLower() == "sandspider")
            {
                ChaseThemesBase.Instance.logger.LogInfo("Spider stopped");
                ___creatureVoice.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
            }
        }
    }
}
