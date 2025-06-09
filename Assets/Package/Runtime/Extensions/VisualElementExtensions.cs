using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Extension methods that are reused across many different UI scripts. These methods 
    /// only target VisualElements
    ///     - Show() to change VisualElement display
    ///     - Hide() to change VisualElement display
    ///     - SetElementSprite() to change VisualElement background image property
    ///     - SetBackgroundColour() to change VisualElement background colours
    ///     - SetBorderColour() to change VisualElement border colours
    /// </summary>
    public static class VisualElementExtensions
    {
        /// <summary>
        /// Sets DisplayStyle of incoming VisualElement to DisplayStyle.Flex
        /// </summary>
        /// <param name="visualElement"></param> 
        public static void Show(this VisualElement visualElement)
        {
            if (visualElement == null)
            {
                Debug.LogError("VisualElementExtensions.Show() - Incoming VisualElement Is Null!");
                return;
            }

            visualElement.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Sets DisplayStyle of incoming VisualElement to DisplayStyle.None
        /// </summary>
        /// <param name="visualElement"></param>
        public static void Hide(this VisualElement visualElement)
        {
            if (visualElement == null)
            {
                Debug.LogError("VisualElementExtensions.Hide() - Incoming VisualElement Is Null!");
                return;
            }

            visualElement.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Sets the background image property of the incoming VisualElement to the incoming sprite.
        /// If the incoming sprite is null, the element will be hidden.
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="sprite"></param>
        /// <param name="hideIfNull"></param>
        public static void SetElementSprite(this VisualElement visualElement, Sprite sprite, bool hideIfNull = true)
        {
            if (visualElement == null)
            {
                Debug.LogError("VisualElementExtensions.SetElementSprite() - Incoming VisualElement is Null!");
                return;
            }

            if (sprite == null && hideIfNull)
            {
                visualElement.Hide();
            }
            else
            {
                visualElement.Show();
                visualElement.style.backgroundImage = new StyleBackground(sprite);
            }
        }

        /// <summary>
        /// Sets the border colour of the incoming VisualElement to the incoming borderColour.
        /// This was created for one of the progress indicators as style.borderColor was not
        /// an option
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="borderColour"></param>
        public static void SetBorderColour(this VisualElement visualElement, StyleColor borderColour)
        {
            if (visualElement == null)
            {
                Debug.LogError("VisualElementExtensions.SetBorderColour() - Incoming VisualElement is Null!");
                return;
            }

            visualElement.style.borderTopColor = borderColour;
            visualElement.style.borderBottomColor = borderColour;
            visualElement.style.borderRightColor = borderColour;
            visualElement.style.borderLeftColor = borderColour;
        }

        /// <summary>
        /// Sets the background colour property of the incoming VisualElement to the incoming 
        /// backgroundColour
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="backgroundColour"></param>
        public static void SetBackgroundColour(this VisualElement visualElement, StyleColor backgroundColour)
        {
            if (visualElement == null)
            {
                Debug.LogError("VisualElementExtensions.SetBackgroundColour() - Incoming VisualElement is Null!");
                return;
            }

            visualElement.style.backgroundColor = backgroundColour;
        }
    }
}