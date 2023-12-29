using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;
using Microsoft.Win32;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(BlobAI))]
    internal class GooAIPatch : MonoBehaviour
    {
        static bool audioPlaying = false;
        static float playedTime = 0f;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref AudioSource ___creatureVoice)
        {
            if (!audioPlaying) {
                ___creatureVoice.PlayOneShot(StartOfRoundPatch.chosenGooClip);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                audioPlaying = true;
                playedTime = 0f;

            } else
            {
                playedTime += Time.deltaTime;
                if (playedTime > StartOfRoundPatch.chosenGooClip.length)
                {
                    audioPlaying = false;
                }
            }
            
        }


    }
}
