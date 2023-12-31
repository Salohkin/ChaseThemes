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
        static string audioCategory = "GHOSTGIRL";
        static bool alreadyPlaying = false;
        public static AudioSource GirlThemeSource = new AudioSource();
        
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
            GirlThemeSource = ___creatureVoice;
            GirlThemeSource.clip = RoundManagerPatch.chosenThemes[audioCategory];
            GirlThemeSource.loop = true;
            if (posInSong > GirlThemeSource.clip.length)
            {
                posInSong -= GirlThemeSource.clip.length;
            }

            ChaseThemesBase.Instance.logger.LogInfo("Song will start playing " + posInSong + " seconds in...");
            ChaseThemesBase.Instance.logger.LogInfo("Loaded Ghost Girl Clip");
            ChaseThemesBase.Instance.logger.LogInfo("Ghost Girl Method Triggered");
            if (___currentBehaviourStateIndex == 1 && !alreadyPlaying)
            {
                ChaseThemesBase.Instance.logger.LogInfo("Should be starting " + posInSong + " into the song");
           
                GirlThemeSource.Play();
                GirlThemeSource.time = posInSong;
                ChaseThemesBase.Instance.logger.LogInfo("Starting " + GirlThemeSource.time + " into the song");
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

    /* Legacy code
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
    */
}
