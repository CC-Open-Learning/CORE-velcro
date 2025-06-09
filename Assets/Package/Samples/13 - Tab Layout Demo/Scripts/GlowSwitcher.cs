using Kamgam.UIToolkitGlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class GlowSwitcher : MonoBehaviour
    {
        [SerializeField] string elementName;

        private CustomStyleProperty<Color> glowInnerColourProperty = new CustomStyleProperty<Color>("--glow-colour-inner");
        private CustomStyleProperty<Color> glowOuterColourProperty = new CustomStyleProperty<Color>("--glow-colour-outer");
        private Color glowInnerColour;
        private Color glowOuterColour;

        private VisualElement root;
        private VisualElement element;
        private Glow glowElement;

        private void Start()
        {
            root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            element = root.Q<VisualElement>(elementName);
            glowElement = root.Q<Glow>("Glow");

            //Initialize the resolved style callback so that the glow colour can be changed based on the theme sheet
            element.RegisterCallback<CustomStyleResolvedEvent>(ApplyGlowColour);
        }

        /// <summary>
        /// Applies the glow colour based on the colour variables inside the theme
        /// </summary>
        /// <param name="e"></param>
        private void ApplyGlowColour(CustomStyleResolvedEvent e)
        {
            if (element.customStyle.TryGetValue(glowInnerColourProperty, out glowInnerColour))
            {
                glowElement.innerColor = glowInnerColour;
            }

            if (element.customStyle.TryGetValue(glowOuterColourProperty, out glowOuterColour))
            {
                glowElement.outerColor = glowOuterColour;
            }
        }

        private void OnDestroy()
        {
            element?.UnregisterCallback<CustomStyleResolvedEvent>(ApplyGlowColour);
        }
    }
}