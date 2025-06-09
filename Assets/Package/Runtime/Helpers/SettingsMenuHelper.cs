using System;
using UnityEngine;
namespace VARLab.Velcro
{
    public static class SettingsMenuHelper
    {
        const int BaseTen = 10;
        const int StandardSoundPressureInAir = 20;

        /// <summary>
        /// Converts a value from a linear slider (0 - 1) in the settings menu to a logarithmic 
        /// value (0f to -80f dB) for use in an audio mixer
        /// </summary>
        /// <param name="newValue">Incoming slider of settings menu. 0 to 1</param>
        /// <returns></returns>
        [Obsolete("Conversion functions are no longer used by the settings menu slider events and will be removed in a future version.")]
        public static float ConvertLinearVolumeToLog(float newValue)
        {
            return Mathf.Log10(newValue) * StandardSoundPressureInAir;
        }

        /// <summary>
        /// Converts a value from an audio mixer log value (0f to -80f dB) to linear for use 
        /// with a slider (0 - 1) in the settings menu
        /// </summary>
        /// <param name="newValue">Incoming dB of saved settings menu. 0f to -80dB</param>
        [Obsolete("Conversion functions are no longer used by the settings menu slider events and will be removed in a future version.")]
        public static float ConvertLogVolumeToLinear(float newValue)
        {
            return Mathf.Pow(BaseTen, (newValue / StandardSoundPressureInAir));
        }
    }
}