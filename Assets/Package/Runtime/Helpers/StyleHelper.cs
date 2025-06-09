using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Extension methods that are reused across many different UI scripts. These methods 
    /// only target VisualElements
    ///     - ToggleTransitionProperties() to change opacity and translate direction for entry/exit animations
    /// </summary>
    public static class StyleHelper
    {
        private const float StartPosition = 0.0f;

        /// <summary>
        /// Sets opacity and translate properties of the incoming visual element to allow for fading and translation at the same time
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="newOpacity">Value which will be gradually changed until hit over the duration of the translation</param>
        /// <param name="translateDirection">
        /// </param>
        public static void ToggleTransitionProperties(VisualElement visualElement, float newOpacity, float translateDirection)
        {
            if (visualElement == null)
            {
                Debug.LogError("StyleHelper.ToggleTransitionProperties() - Incoming VisualElement is Null!");
                return;
            }

            visualElement.style.opacity = newOpacity;
            visualElement.style.translate = new Translate(new Length(StartPosition), new Length(translateDirection));
        }
    }
}