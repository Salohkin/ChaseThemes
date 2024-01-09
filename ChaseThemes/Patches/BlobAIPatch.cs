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
        static float volume = 0.2f;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref AudioSource ___creatureSFX)
        {
            if (!audioPlaying) {
                ___creatureSFX.PlayOneShot(RoundManagerPatch.chosenThemes[audioCategory], volume);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                audioPlaying = true;
                playedTime = 0f;
            }
            else
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
