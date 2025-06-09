using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Intended use of this class is to pair it with the SettingsMenuComplex UXML file on the 
    /// same game object. A prefab already exists in /Package/Prefabs. Hook up the change events to 
    /// different manager scripts in your DLX
    ///     - Theme Toggle is a simple bool for a ThemeManager
    ///     - Sound Toggle is a simple bool for an AudioManager
    ///     - All volume sliders already output strings and log values for use with an audio mixer
    ///     - Camera sensitivity is a linear value from 0 to 1 for use in a CameraManager
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SettingsMenuComplex : SettingsMenu
    {
        private const string SoundOnLabel = "ON";
        private const string SoundOffLabel = "OFF";
        private const string ThemeDarkLabel = "Dark";
        private const string ThemeLightLabel = "Light";


        private Slider soundEffectsSlider;
        private Slider dialogueSlider;
        private Label themeTag;
        private Label soundTag;

        [Header("Starting Values")]
        [Range(0.0f, 1.0f), Tooltip("Slider value which represents a float, with a range 0 (minimum) to 1 (maximum).")]
        [SerializeField] private float soundEffectLevel = 0.0f;
        [Range(0.0f, 1.0f), Tooltip("Slider value which represents a float, with a range 0 (minimum) to 1 (maximum).")]
        [SerializeField] private float dialogueLevel = 0.0f;

        [Header("Event Hook-Ins")]
        public UnityEvent<string, float> OnSoundEffectsSliderChanged;
        public UnityEvent<string, float> OnDialogueSliderChanged;

        [Header("Volume Slider Variables")]
        public string MixerSoundEffectTag = "SoundEffectsVolumeParameter";
        public string MixerDialogueTag = "DialoguVolumeParameter";

        private void Start()
        {
            SetupBaseSettingsMenu();

            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            soundEffectsSlider = Root.Q<TemplateContainer>("SoundEffectSlider").Q<FillSlider>();
            dialogueSlider = Root.Q<TemplateContainer>("DialogueSlider").Q<FillSlider>();
            themeTag = Root.Q<Label>("ThemeTag");
            soundTag = Root.Q<Label>("SoundTag");

            OnSoundEffectsSliderChanged ??= new UnityEvent<string, float>();
            OnDialogueSliderChanged ??= new UnityEvent<string, float>();

            SetStartingValues();
            ToggleSliders(isSoundOn);
            RegisterEvents();
            Root.Hide();
        }

        /// <summary>
        /// Sets the starting values of the sliders and toggles in the settings menu to what is serialized in the inspector
        /// </summary>
        private void SetStartingValues()
        {
            sliders.Add(soundEffectsSlider);
            sliders.Add(dialogueSlider);
            soundEffectsSlider.value = soundEffectLevel;
            dialogueSlider.value = dialogueLevel;
            ToggleThemeLabel(isLightTheme);
            ToggleSoundLabel(isSoundOn);
            ToggleSliders(isSoundOn);
        }

        private void RegisterEvents() 
        {
            soundEffectsSlider.RegisterCallback<ChangeEvent<float>>(SoundEffectSliderCallback);
            dialogueSlider.RegisterCallback<ChangeEvent<float>>(DialogueSliderCallback);
            themeToggle.RegisterValueChangedCallback((_) => ToggleThemeLabel(themeToggle.value));
            soundToggle.RegisterValueChangedCallback((_) => ToggleSoundLabel(soundToggle.value));
        }

        /// <summary>
        /// Invokes the OnSoundEffectsSliderChanged event when the sound effect slider's value is changed
        /// </summary>
        /// <param name="evt">The new float value held by the slider</param>
        private void SoundEffectSliderCallback(ChangeEvent<float> evt)
        {
            OnSoundEffectsSliderChanged?.Invoke(MixerSoundEffectTag, evt.newValue);
        }

        /// <summary>
        /// Invokes the OnDialogueSliderChanged event when the dialogue volume slider's value is changed
        /// </summary>
        /// <param name="evt">The new float value held by the slider</param>
        private void DialogueSliderCallback(ChangeEvent<float> evt)
        {
            OnDialogueSliderChanged?.Invoke(MixerDialogueTag, evt.newValue);
        }

        /// <summary>
        /// Changes the text of the Theme Tag
        ///     - "Dark" if true
        ///     - "Light" if false
        /// </summary>
        /// <param name="enable"></param>
        public void ToggleThemeLabel(bool enable)
        {
            string newLabel = enable ? ThemeLightLabel : ThemeDarkLabel;
            themeTag.SetElementText(newLabel);
        }

        /// <summary>
        /// Changes the text of the Sound Tag
        ///     - "ON" if true
        ///     - "OFF" if false
        /// </summary>
        /// <param name="enable"></param>
        public void ToggleSoundLabel(bool enable)
        {
            string newLabel = enable ? SoundOnLabel : SoundOffLabel;
            soundTag.SetElementText(newLabel);
        }

        /// <summary>
        ///     Set the value in the <see cref="soundEffectsSlider"/>
        /// </summary>
        /// <param name="value"> Real value to assign to the slider </param>
        /// <param name="notify"> Should the NotifyPropertyChanged event be invoked for this field </param>
        public void SetSoundEffectsSlider(float value, bool notify = true)
            => SetFieldValue(soundEffectsSlider, value, notify);

        /// <summary>
        ///     Set the value in the <see cref="dialogueSlider"/>
        /// </summary>
        /// <param name="value"> Real value to assign to the slider </param>
        /// <param name="notify"> Should the NotifyPropertyChanged event be invoked for this field </param>
        public void SetDialogueSlider(float value, bool notify = true)
            => SetFieldValue(dialogueSlider, value, notify);
    }
}