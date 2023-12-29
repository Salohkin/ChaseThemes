using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ChaseThemes
{
    internal class ThemeHandler
    {
        public static AudioClip theme;

        public static void PlayTheme(ref AudioSource ___creatureVoice)
        {
            ___creatureVoice.PlayOneShot(theme);
            ChaseThemesBase.Instance.logger.LogInfo("Playing theme");
        }

        public static void StopTheme(ref AudioSource ___creatureVoice)
        {
            ___creatureVoice.Stop();
            ChaseThemesBase.Instance.logger.LogInfo("Stopping theme");
        }

        void checkPlaying()
        {

        }
    }
}
