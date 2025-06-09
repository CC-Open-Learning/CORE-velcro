using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Class that represnts an individual element inside a table entry.
    /// </summary>
    public class TableElement
    {
        /// <summary>
        /// The table which the element is contained in.
        /// </summary>
        public Table Owner { get; private set; }

        /// <summary>
        /// The entry which this element is contained in.
        /// </summary>
        public TableEntry Entry { get; private set; }

        private string text;

        /// <summary>
        /// The text of the element.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                text = value;
                if (textLabel != null)
                {
                    textLabel.text = text;
                }
            }
        }

        private Sprite icon;

        /// <summary>
        /// The icon of the element.
        /// </summary>
        public Sprite Icon
        {
            get => icon;
            set
            {
                icon = value;
                if (iconElement != null)
                {
                    if (icon != null) {
                        iconElement.SetElementSprite(icon);
                        iconElement.Show();
                    }
                    else
                    {
                        iconElement.style.backgroundImage = StyleKeyword.Null;
                        iconElement.Hide();
                    }
                }
            }
        }

        private Color textColour;

        /// <summary>
        /// The colour of the text.
        /// </summary>
        public Color TextColour
        {
            get => textColour;
            set
            {
                textColour = value;
                if (textLabel != null)
                {
                    textLabel.style.color = textColour;
                }
            }
        }

        // UI references
        private Label textLabel;
        private VisualElement iconElement;

        // Sets the owner table of the element
        internal void SetOwner(Table owner)
        {
            Owner = owner;
        }

        // Sets the owner entry of the element
        internal void SetEntry(TableEntry entry)
        {
            Entry = entry;
        }

        // Sets the UI references for the table element
        internal void SetUIReferences(Label textLabel, VisualElement iconElement)
        {
            this.textLabel = textLabel;
            this.iconElement = iconElement;
            text = textLabel.text;
        }

        /// <summary>
        /// Reverts the colour of the text back to the theme's default colour.
        /// </summary>
        public void ClearTextColour()
        {
            if (textLabel != null)
            {
                textLabel.style.color = StyleKeyword.Null;
            }
        }
    }
}