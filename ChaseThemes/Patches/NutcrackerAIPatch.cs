using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    internal class NutcrackerAIPatch : MonoBehaviour
    {
        static bool audioPlaying = false;
        static float playedTime = 0f;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void reset()
        {
            audioPlaying = false;
            playedTime = 0f;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref AudioSource ___longRangeAudio)
        {
            if (!audioPlaying)
            {
                ___longRangeAudio.PlayOneShot(RoundManagerPatch.chosenNutcrackerClip);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                audioPlaying = true;
                playedTime = 0f;

            }
            else
            {
                playedTime += Time.deltaTime;
                if (playedTime > RoundManagerPatch.chosenNutcrackerClip.length)
                {
                    audioPlaying = false;
                }
            }

        }
    }
}
