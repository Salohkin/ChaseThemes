﻿using UnityEngine;
using BepInEx;
using HarmonyLib;
using LCSoundTool;

namespace ChaseThemes.Patches
{
    [HarmonyPatch(typeof(RedLocustBees))]
    internal class CircuitBeeAIPatch
    {
        static bool alreadyPlaying = false;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void PlaychosenMainClip(ref int ___currentBehaviourStateIndex, ref AudioSource ___creatureVoice)
        {
            alreadyPlaying = false;
            if (___currentBehaviourStateIndex == 2 && !alreadyPlaying)
            {
                ___creatureVoice.PlayOneShot(StartOfRoundPatch.chosenMainClip);
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme started!");
                alreadyPlaying = true;
            }
        }
    }
    [HarmonyPatch(typeof(EnemyAI))]
    internal class CircuitBeeEnemyAIPatch : MonoBehaviour
    {
        [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
        [HarmonyPostfix]
        static void StopchosenMainClip(int ___currentBehaviourStateIndex, ref EnemyType ___enemyType, ref AudioSource ___creatureVoice)
        {
            //ChaseThemesBase.Instance.logger.LogInfo("Enemy name is: " + ___enemyType.enemyName);
            if (___currentBehaviourStateIndex == 0 && ___enemyType.enemyName.ToLower() == "red locust bees")
            {
                ___creatureVoice.Stop();
                ChaseThemesBase.Instance.logger.LogInfo("Chase theme stopped!");
            }
        }
    }
}