using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Extension methods that are reused across many different UI scripts. These methods 
    /// only target TextElements such as Buttons, and labels
    ///     - SetElementText() to change TextElement text    
    ///     - SetElementFontSize() to change TextElement font size
    ///     - SetElementColour() to change TextElement color property
    /// </summary>
    public static class TextElementExtensions
    {
        /// <summary>
        /// Sets the text of the incoming TextElement to the incoming text. If the incoming string is null 
        /// or empty, the element will be hidden. Any UI element that has text inherits from TextElement 
        /// such as Labels and Buttons
        /// </summary>
        /// <param name="textElement"></param>
        /// <param name="text"></param>
        /// <param name="hideIfNull"></param>
        public static void SetElementText(this TextElement textElement, string text, bool hideIfNull = true)
        {
            if (textElement == null)
            {
                Debug.LogError("TextElementExtensions.SetElementText() - Incoming TextElement is Null!");
                return;
            }

            if (string.IsNullOrWhiteSpace(text) && hideIfNull)
            {
                textElement.Hide();
                return;
            }
            else
            {
                textElement.Show();
                textElement.text = text;
            }
        }

        /// <summary>
        /// Sets the font size of the incoming TextElement to the incoming FontSize enum. Any UI element that 
        /// has text inherits from TextElement such as Labels and Buttons
        /// </summary>
        /// <param name="textElement"></param>
        /// <param name="fontSize"></param>
        public static void SetElementFontSize(this TextElement textElement, FontSize fontSize)
        {
            if (textElement == null)
            {
                Debug.LogError("TextElementExtensions.SetElementFontSize() - Incoming TextElement is Null!");
                return;
            }

            int value = (int)fontSize;
            if (Enum.IsDefined(typeof(FontSize), value))
            {
                textElement.style.fontSize = value;
            }
            else
            {
                Debug.LogWarning("TextElementExtensions.SetElementFontSize() - Specified Font Size is not a Valid Enum Value");
            }
        }

        /// <summary>
        /// Sets the colour property of the incoming TextElement to the incoming colour
        /// </summary>
        /// <param name="textElement"></param>
        /// <param name="colour"></param>
        public static void SetElementColour(this TextElement textElement, StyleColor colour)
        {
            if (textElement == null)
            {
                Debug.LogError("TextElementExtensions.SetElementColour() - Incoming TextElement is Null!");
                return;
            }

            textElement.style.color = colour;
        }
    }
}