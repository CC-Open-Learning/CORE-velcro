using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    public abstract class SettingsMenu : MonoBehaviour, IUserInterface
    {
        protected const string DimmedBackgroundClass = "settings-menu-canvas";

        protected List<Slider> sliders = new();
        protected Slider volumeSlider;
        protected Slider cameraSensitivitySlider;
        protected SlideToggle themeToggle;
        protected SlideToggle soundToggle;
        protected Button closeBtn;

        public VisualElement Root { protected set; get; }

        [Header("Starting Values")]
        [Range(0.0f, 1.0f), Tooltip("Slider value which represents a float, with a range 0 (minimum, -80db) to 1 (maximum, 0db).")]
        [SerializeField] protected float volumeLevel = 0.5f;
        [Range(0.0f, 1.0f), Tooltip("Slider value which represents a float, with a range 0 (minimum) to 1 (maximum).")]
        [SerializeField] protected float cameraSensitivity = 0.5f;

        [SerializeField] protected bool isLightTheme = false;
        [SerializeField] protected bool isSoundOn = false;
        [SerializeField] protected bool isBackgroundDimmed = false;

        [Header("Event Hook-Ins")]
        public UnityEvent<bool> OnSoundTogglePressed;
        public UnityEvent<bool> OnThemeTogglePressed;
        public UnityEvent<string, float> OnVolumeSliderChanged;
        public UnityEvent<float> OnCameraSliderChanged;
        public UnityEvent OnSettingsMenuShown;
        public UnityEvent OnSettingsMenuHidden;

        [Header("Volume Slider Variables")]
        public string MixerGlobalVolumeTag = "GlobalVolumeParameter";

        protected void SetupBaseSettingsMenu()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            closeBtn = Root.Q<Button>("CloseBtn");
            themeToggle = Root.Q<TemplateContainer>("ThemeToggle").Q<SlideToggle>();
            soundToggle = Root.Q<TemplateContainer>("SoundToggle").Q<SlideToggle>();
            volumeSlider = Root.Q<TemplateContainer>("VolumeSlider").Q<FillSlider>();
            cameraSensitivitySlider = Root.Q<TemplateContainer>("SensitivitySlider").Q<FillSlider>();

            OnSoundTogglePressed ??= new UnityEvent<bool>();
            OnThemeTogglePressed ??= new UnityEvent<bool>();
            OnVolumeSliderChanged ??= new UnityEvent<string, float>();
            OnCameraSliderChanged ??= new UnityEvent<float>();
            OnSettingsMenuShown ??= new UnityEvent();
            OnSettingsMenuHidden ??= new UnityEvent();

            sliders.Add(volumeSlider);
            ToggleSliders(isSoundOn);
            SetStartingValues();
            RegisterEvents();
        }

        /// <summary>
        /// Sets the starting values of the sliders and toggles in the settings menu to what is serialized in the inspector
        /// </summary>
        private void SetStartingValues()
        {
            themeToggle.value = isLightTheme;
            soundToggle.value = isSoundOn;
            volumeSlider.value = volumeLevel;
            cameraSensitivitySlider.value = cameraSensitivity;

            if (isBackgroundDimmed)
            {
                VisualElement canvas = Root.Q<VisualElement>("Canvas");
                canvas.EnableInClassList(DimmedBackgroundClass, true);
            }
        }

        private void RegisterEvents()
        {
            closeBtn.clicked += Hide;
            themeToggle.RegisterCallback<ChangeEvent<bool>>(ThemeToggleCallback);
            soundToggle.RegisterCallback<ChangeEvent<bool>>(SoundToggleCallback);
            volumeSlider.RegisterCallback<ChangeEvent<float>>(VolumeSliderCallback);
            cameraSensitivitySlider.RegisterCallback<ChangeEvent<float>>(CameraSensitivitySliderCallback);
        }

        /// <summary>
        /// Invokes the OnThemeTogglePressed event when the theme toggle is changed
        /// </summary>
        /// <param name="evt">The new boolean value for the toggle.</param>
        private void ThemeToggleCallback(ChangeEvent<bool> evt)
        {
            OnThemeTogglePressed?.Invoke(evt.newValue);
        }

        /// <summary>
        /// Invokes the OnSoundTogglePressed event when the sound toggle is changed
        /// </summary>
        /// <param name="evt">The new boolean value for the toggle.</param>
        private void SoundToggleCallback(ChangeEvent<bool> evt)
        {
            OnSoundTogglePressed?.Invoke(evt.newValue);
            ToggleSliders(evt.newValue);
        }

        /// <summary>
        /// Invokes the OnVolumeSliderChanged event when the volume slider's value is changed
        /// </summary>
        /// <param name="evt">The new float value held by the slider</param>
        private void VolumeSliderCallback(ChangeEvent<float> evt)
        {
            OnVolumeSliderChanged?.Invoke(MixerGlobalVolumeTag, evt.newValue);
        }

        /// <summary>
        /// Invokes the OnCameraSliderChanged event when the camera sensitivity slider's value is changed
        /// </summary>
        /// <param name="evt">The new float value held by the slider</param>
        private void CameraSensitivitySliderCallback(ChangeEvent<float> evt)
        {
            OnCameraSliderChanged?.Invoke(evt.newValue);
        }

        /// <summary>
        /// Toggles slider interactivity in the current settings menu
        ///     - Enabled if true
        ///     - Disabled if false
        /// </summary>
        /// <param name="enable"></param>
        public void ToggleSliders(bool enable)
        {
            foreach (Slider slider in sliders)
            {
                slider.SetEnabled(enable);
            }
        }

        /// <summary>
        /// Shows the root of the settings menu and triggers OnSettingsMenuShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnSettingsMenuShown?.Invoke();
        }

        /// <summary>
        /// Hides the root of the settings menu and triggers OnSettingsMenuHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnSettingsMenuHidden?.Invoke();
        }


        // Public Mutator Methods
        // Provide the option to set value with and without notify

        /// <summary>
        ///     Set the value in the <see cref="themeToggle"/>
        /// </summary>
        /// <param name="value"> Boolean value to assign to the toggle </param>
        /// <param name="notify"> Should the NotifyPropertyChanged event be invoked for this field </param>
        public void SetThemeToggle(bool value, bool notify = true) 
            => SetFieldValue(themeToggle, value, notify);

        /// <summary>
        ///     Set the value in the <see cref="soundToggle"/>
        /// </summary>
        /// <param name="value"> Boolean value to assign to the toggle </param>
        /// <param name="notify"> Should the NotifyPropertyChanged event be invoked for this field </param>
        public void SetSoundToggle(bool value, bool notify = true)
            => SetFieldValue(soundToggle, value, notify);

        /// <summary>
        ///     Set the value in the <see cref="volumeSlider"/>
        /// </summary>
        /// <param name="value"> Real value to assign to the slider </param>
        /// <param name="notify"> Should the NotifyPropertyChanged event be invoked for this field </param>
        public void SetVolumeSlider(float value, bool notify = true) 
            => SetFieldValue(volumeSlider, value, notify);

        /// <summary>
        ///     Set the value in the <see cref="cameraSensitivitySlider"/>
        /// </summary>
        /// <param name="value"> Real value to assign to the slider </param>
        /// <param name="notify"> Should the NotifyPropertyChanged event be invoked for this field </param>
        public void SetCameraSlider(float value, bool notify = true)
            => SetFieldValue(cameraSensitivitySlider, value, notify);


        /// <summary>
        ///     Generic function used to set the value of a UIElement field 
        ///     which extends from BaseField. 
        ///     <para/>    
        ///     Setting the <paramref name="notify"/> parameter indicates 
        ///     whether the associated NotifyPropertyChanged event should 
        ///     be invoked after updating the value
        /// </summary>
        /// <param name="field"> Field to modify, extending from BaseField </param>
        /// <typeparam name="T"> A value type corresponding to the field provided </typeparam>
        /// <param name="value"> Value to assign to the <paramref name="field"/> </param>
        /// <param name="notify"> Should the NotifyPropertyChanged event be invoked for this field </param>
        protected static void SetFieldValue<T>(BaseField<T> field, T value, bool notify)
        {
            if (notify) { field.value = value; }
            else { field.SetValueWithoutNotify(value); }
        }
    }
}