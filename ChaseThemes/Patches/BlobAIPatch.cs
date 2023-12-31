using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(BlobAI))]
    internal class BlobAIPatch : MonoBehaviour
    {
        static string audioCategory = "BLOB";
        static bool audioPlaying = false;
        static float playedTime = 0f;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref AudioSource ___creatureVoice)
        {
            if (!audioPlaying) {
                ___creatureVoice.PlayOneShot(RoundManagerPatch.chosenThemes[audioCategory]);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                audioPlaying = true;
                playedTime = 0f;
            } else
            {
                playedTime += Time.deltaTime;
                if (playedTime > RoundManagerPatch.chosenThemes[audioCategory].length)
                {
                    audioPlaying = false;
                }
            }
        }
    }
}
