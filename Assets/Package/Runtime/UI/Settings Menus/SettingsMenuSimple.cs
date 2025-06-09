using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Intended use of this class is to pair it with the SettingsMenuSimple UXML file on the 
    /// same game object. A prefab already exists in /Package/Prefabs. Hook up the change events to 
    /// different manager scripts in your DLX
    ///     - Theme Toggle is a simple bool for a ThemeManager
    ///     - Sound Toggle is a simple bool for an AudioManager
    ///     - All volume sliders already output strings and log values for use with an audio mixer
    ///     - Camera sensitivity is a linear value from 0 to 1 for use in a CameraManager
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SettingsMenuSimple : SettingsMenu
    {
        private void Start()
        {
            SetupBaseSettingsMenu();
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            Root.Hide();
        }
    }
}