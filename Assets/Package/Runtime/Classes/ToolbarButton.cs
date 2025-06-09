using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [Serializable]
    public class ToolbarButton
    {
        [SerializeField]
        private Sprite icon;

        public Sprite Icon
        {
            get => icon;
            set
            {
                icon = value;
                iconElement.SetElementSprite(icon);
            }
        }

        [SerializeField]
        private string text;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                textElement.SetElementText(text);
            }
        }

        [HideInInspector]
        public UnityEvent OnClick;

        internal VisualElement root;
        internal VisualElement iconElement;
        internal Label textElement;

        /// <summary>
        /// Creates an empty toolbar button.
        /// </summary>
        public ToolbarButton()
        {
            icon = null;
            text = string.Empty;
        }

        /// <summary>
        /// Creates a new toolbar button with the specified icon and text.
        /// </summary>
        /// <param name="icon">The icon of the toolbar button.</param>
        /// <param name="text">THe text of the toolbar button.</param>
        public ToolbarButton(Sprite icon, string text)
        {
            this.icon = icon;
            this.text = text;
        }

        // The owning toolbar of this button.
        internal Toolbar owner;

        /// <summary>
        /// Shows the button in the toolbar.
        /// </summary>
        public void Show()
        {
            root.Show();
        }

        /// <summary>
        /// Hides the button in the toolbar.
        /// </summary>
        public void Hide()
        {
            root.Hide();
        }
    }
}