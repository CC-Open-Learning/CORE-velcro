using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class DisableAllControls : MonoBehaviour
    {
        [SerializeField] 
        private KeyCode disableKey = KeyCode.D;

        private List<VisualElement> controls = new List<VisualElement>();
        private bool enable = true;

        private void Update()
        {
            if (Input.GetKeyDown(disableKey))
            {
                enable = !enable;
                GetAllControlReferences();
                ToggleElements(enable);
            }
        }

        /// <summary>
        /// Gets all references to controls in the UI. This is not called on Start() since 
        /// some UI dynamically clone new templates at runtime
        /// </summary>
        private void GetAllControlReferences()
        {
            UIDocument document = GetComponent<UIDocument>();

            controls.AddRange(document.rootVisualElement.Query<Button>().ToList());
            controls.AddRange(document.rootVisualElement.Query<RadioButton>().ToList());
            controls.AddRange(document.rootVisualElement.Query<Toggle>().ToList());
            controls.AddRange(document.rootVisualElement.Query<Slider>().ToList());
            controls.AddRange(document.rootVisualElement.Query<SlideToggle>().ToList());
        }

        private void ToggleElements(bool enable)
        {
            foreach (VisualElement element in controls)
            {
                element.SetEnabled(enable);
            }
        }
    }
}