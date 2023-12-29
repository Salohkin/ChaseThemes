using BepInEx;
using HarmonyLib;
using LCSoundTool;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(StartMatchLever))]
    internal class StartOfRoundPatch
    {
        public static AudioClip chosenMainClip;

        public static AudioClip chosenGhostGirlClip;

        public static AudioClip chosenForestKeeperClip;

        public static AudioClip chosenGooClip;

        public static AudioClip chosenNutcrackerClip;
        [HarmonyPatch("StartGame")]
        [HarmonyPostfix]
        static void LoadTheme()
        {
            try
            {
                getRandomClip();
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Chase theme loaded successfully!");
            }
            catch (Exception e)
            {
                ChaseThemesBase.Instance.logger.LogWarning(e);
            }
        }

        static void getRandomClip()
        {
            int numOfClips = ChaseThemesBase.defaultAudioClips.Count();
            ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Num of clips: " + ChaseThemesBase.defaultAudioClips.Count());
            int clipNumber;
            if (numOfClips != 0)
            {
                clipNumber = Random.Range(0, numOfClips-1);
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: NUm chosen: " + clipNumber);
                chosenMainClip = ChaseThemesBase.defaultAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Main Clip successfully chosen: " + chosenMainClip.ToString());

                numOfClips = ChaseThemesBase.forestKeeperAudioClips.Count();
                clipNumber = Random.Range(0, numOfClips - 1);
                chosenForestKeeperClip = ChaseThemesBase.forestKeeperAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Forest Keeper Clip successfully chosen: " + chosenForestKeeperClip.ToString());

                numOfClips = ChaseThemesBase.ghostGirlAudioClips.Count();
                clipNumber = Random.Range(0, numOfClips - 1);
                chosenGhostGirlClip = ChaseThemesBase.ghostGirlAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Ghost Girl Clip successfully chosen: " + chosenGhostGirlClip.ToString());

                numOfClips = ChaseThemesBase.gooAudioClips.Count();
                clipNumber = Random.Range(0, numOfClips - 1);
                chosenGooClip = ChaseThemesBase.gooAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Goo Clip successfully chosen: " + chosenGooClip.ToString());

                numOfClips = ChaseThemesBase.nutcrackerAudioClips.Count();
                clipNumber = Random.Range(0, numOfClips - 1);
                chosenNutcrackerClip = ChaseThemesBase.nutcrackerAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Nutcracker Clip successfully chosen: " + chosenNutcrackerClip.ToString());
            }
            else
            {
                ChaseThemesBase.Instance.logger.LogWarning("CHASE THEMES: No clips loaded or clips not found");
            }

        }
    }
}
