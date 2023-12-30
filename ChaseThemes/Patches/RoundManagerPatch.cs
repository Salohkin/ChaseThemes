using BepInEx;
using HarmonyLib;
using LCSoundTool;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        public static AudioClip chosenMainClip;

        public static AudioClip chosenGhostGirlClip;

        public static AudioClip chosenForestKeeperClip;

        public static AudioClip chosenGooClip;

        public static AudioClip chosenNutcrackerClip;

        //public static int levelSeed;

        [HarmonyPatch("GenerateNewLevelClientRpc")]
        [HarmonyPostfix]
        static void LoadTheme(ref StartOfRound ___playersManager)
        {
            try
            {
                SelectRandomThemes(getLevelSeed(ref ___playersManager));
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Chase theme loaded successfully!");
            }
            catch (Exception e)
            {
                ChaseThemesBase.Instance.logger.LogWarning(e);
            }
        }

        static int getLevelSeed(ref StartOfRound ___playersManager)
        {
            //ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Map seed is " + levelSeed + " !!!");
            return (___playersManager.randomMapSeed);
        }

        static void SelectRandomThemes(int seed)
        {
            int numOfClips = ChaseThemesBase.defaultAudioClips.Count();
            ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Num of clips: " + ChaseThemesBase.defaultAudioClips.Count());
            int clipNumber;
            if (numOfClips != 0)
            {
                clipNumber = seed % numOfClips; //Random.Range(0, numOfClips-1);
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Num chosen: " + clipNumber);
                chosenMainClip = ChaseThemesBase.defaultAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Main Clip successfully chosen: " + chosenMainClip.ToString());

                numOfClips = ChaseThemesBase.forestKeeperAudioClips.Count();
                clipNumber = seed % numOfClips; //Random.Range(0, numOfClips - 1);
                chosenForestKeeperClip = ChaseThemesBase.forestKeeperAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Forest Keeper Clip successfully chosen: " + chosenForestKeeperClip.ToString());

                numOfClips = ChaseThemesBase.ghostGirlAudioClips.Count();
                clipNumber = seed % numOfClips; //Random.Range(0, numOfClips - 1);
                chosenGhostGirlClip = ChaseThemesBase.ghostGirlAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Ghost Girl Clip successfully chosen: " + chosenGhostGirlClip.ToString());

                numOfClips = ChaseThemesBase.gooAudioClips.Count();
                clipNumber = seed % numOfClips; //Random.Range(0, numOfClips - 1);
                chosenGooClip = ChaseThemesBase.gooAudioClips[clipNumber];
                ChaseThemesBase.Instance.logger.LogInfo("CHASE THEMES: Goo Clip successfully chosen: " + chosenGooClip.ToString());

                numOfClips = ChaseThemesBase.nutcrackerAudioClips.Count();
                clipNumber = seed % numOfClips; //Random.Range(0, numOfClips - 1);
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
