using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class SettingsMenuDemoController : Toolbar
    {
        public UnityEvent DisplaySimpleMenu;
        public UnityEvent DisplayComplexMenu;
        public UnityEvent<AudioType> PlaySound;

        private void Start()
        {
            SetupBaseToolbar();

            DisplaySimpleMenu ??= new UnityEvent();
            DisplayComplexMenu ??= new UnityEvent();
            PlaySound ??= new UnityEvent<AudioType>();

            Button simpleBtn = Root.Q<Button>("Simple");
            simpleBtn.clicked += () =>
            {
                DisplaySimpleMenu?.Invoke();
            };

            Button complexBtn = Root.Q<Button>("Complex");
            complexBtn.clicked += () =>
            {
                DisplayComplexMenu?.Invoke();
            };

            Button soundEffectBtn = Root.Q<Button>("SoundEffect");
            soundEffectBtn.clicked += () =>
            {
                PlaySound?.Invoke(AudioType.SoundEffect);
            };

            Button dialogueBtn = Root.Q<Button>("Dialogue");
            dialogueBtn.clicked += () =>
            {
                PlaySound?.Invoke(AudioType.Dialogue);
            };
        }

        public void ToggleEvent(bool newValue)
        {
            Debug.Log("Toggle Change Event Triggered: " + newValue.ToString());
        }

        public void VolumeSliderEvent(string mixerTag, float newValue)
        {
            Debug.Log("Slider Change Event Triggered for " + mixerTag + ": " + newValue.ToString());
        }

        public void SliderEvent(float newValue)
        {
            Debug.Log("Slider Change Event Triggered: " + newValue.ToString());
        }
    }
}