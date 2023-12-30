using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;
using System.CodeDom.Compiler;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(DressGirlAI))]
    internal class GhostGirlAIPatch : MonoBehaviour
    {
        static bool alreadyPlaying = false;
        public static AudioSource temp = new AudioSource();
        
        public static float posInSong = 0f;
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void resetGirl()
        {
            posInSong = 0f;
        }

        [HarmonyPatch("BeginChasing")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref AudioSource ___creatureVoice, ref float ___chaseTimer)
        {
            alreadyPlaying = false;
            temp = ___creatureVoice;
            temp.clip = RoundManagerPatch.chosenGhostGirlClip;
            temp.loop = true;
            if (posInSong > temp.clip.length)
            {
                posInSong -= temp.clip.length;
            }

            ChaseThemesBase.Instance.logger.LogInfo("Song will start playing " + posInSong + " seconds in...");
            ChaseThemesBase.Instance.logger.LogInfo("Loaded Ghost Girl Clip");
            ChaseThemesBase.Instance.logger.LogInfo("Ghost Girl Method Triggered");
            if (___currentBehaviourStateIndex == 1 && !alreadyPlaying)
            {
                ChaseThemesBase.Instance.logger.LogInfo("Should be starting " + posInSong + " into the song");
           
                temp.Play();
                temp.time = posInSong;
                ChaseThemesBase.Instance.logger.LogInfo("Starting " + temp.time + " into the song");
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                alreadyPlaying = true;
                
            } 
        }

        [HarmonyPatch("StopChasing")]
        [HarmonyPostfix]
        static void getTimeRemaining(ref float ___chaseTimer)
        {
            posInSong += 20f - ___chaseTimer;
        }
    }

    [HarmonyPatch(typeof(EnemyAI))]
    internal class GhostGirlEnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopchosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if (___currentBehaviourStateIndex == 0 && ___enemyType.enemyName.ToLower() == "girl")
            {
                
                GhostGirlAIPatch.temp.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Girl stopped");
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
                
                ChaseThemesBase.Instance.logger.LogInfo("Song will resume playing " + GhostGirlAIPatch.posInSong + " seconds in...");
            }
        }
    }
}
