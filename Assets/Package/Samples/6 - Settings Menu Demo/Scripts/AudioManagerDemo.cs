using UnityEngine;
using UnityEngine.Audio;

namespace VARLab.Velcro.Demos
{
    public class AudioManagerDemo : MonoBehaviour
    {
        [SerializeField] private AudioSource dialogueAudioSource;
        [SerializeField] private AudioSource soundEffectAudioSource;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioClip soundEffect;
        [SerializeField] private AudioClip dialogue;

        private float previousVolume;
        private const float MinVolume = -80.0f;
        private const float StartingVolume = -6.0f;
        private const string MasterTag = "Volume";

        void Start()
        {
            SetVolume(MasterTag, StartingVolume);
        }

        public void SetVolume(string group, float value)
        {
            audioMixer.SetFloat(group, SettingsMenuHelper.ConvertLinearVolumeToLog(value));
        }
        
        public void ToggleVolume(bool enabled)
        {
            if (!enabled)
            {
                audioMixer.GetFloat(MasterTag, out previousVolume);
                audioMixer.SetFloat(MasterTag, MinVolume);
            }
            else
            {
                audioMixer.SetFloat(MasterTag, previousVolume);
            }
        }

        public void HandlePlayAudioClip(AudioType audioType)
        {
            switch (audioType)
            {
                case AudioType.Dialogue:
                    dialogueAudioSource.clip = dialogue;
                    dialogueAudioSource.Play();
                    break;

                case AudioType.SoundEffect:
                    soundEffectAudioSource.clip = soundEffect;
                    soundEffectAudioSource.Play();
                    break;

                default:
                    Debug.LogWarning("AudioManagerDemo.HandlePlayAudioClip() - Default case selected in switch statement. No AudioType matched!");
                    break;
            }
        }
    }
}