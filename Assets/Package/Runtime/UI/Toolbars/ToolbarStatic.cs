using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Static Toolbars have a few different customizations:
    ///     - Icons for the options starting from top to bottom
    ///     - Labels for all the options
    ///     
    /// Intended use of this class is to connect ToolbarStatic.HandleDisplayUI() to a UnityEvent in your DLX. Methods 
    /// show/hide have been exposed if DLX would prefer to serialize and control 
    /// individual stages of the toolbar
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ToolbarStatic : Toolbar
    {
        private const string SelectedClass = "toolbar-static-item-group-selected";
        private const string GrowClass = "grow";

        private Button lastBtn = null; /* Used when tracking which button should be highlighted */

        protected override void Start()
        {
            base.Start();

            toolbar = Root.Q("Toolbar");
            buttonsContainer = toolbar;
        }

        /// <summary>
        /// Adds a new button to the toolbar.
        /// </summary>
        /// <param name="icon">The icon of the new button.</param>
        /// <param name="text">The text of the new button.</param>
        public override void AddButton(Sprite icon, string text)
        {
            ToolbarButton tb = new ToolbarButton(icon, text);
            buttons.Add(tb);

            VisualElement newButton = buttonTemplate.CloneTree();
            newButton.AddToClassList(GrowClass); // Fixes the toolbar resizing bug
            buttonsContainer.Add(newButton);

            tb.root = newButton;

            VisualElement iconElement = newButton.Q("Image");
            iconElement.SetElementSprite(tb.Icon);
            tb.iconElement = iconElement;

            Label label = newButton.Q<Label>("Text");
            label.SetElementText(tb.Text);
            tb.textElement = label;

            tb.OnClick ??= new UnityEvent();

            Button button = newButton.Q<Button>("Button");
            button.RegisterCallback<ClickEvent>((e) =>
            {
                OnGroupButtonClick(e);
                tb.OnClick?.Invoke();
            });
        }

        /// <summary>
        /// Used as a general function for all group buttons, which covers the icons and text used for each area of the toolbar.
        /// </summary>
        /// <param name="cl">Event containing target button</param>
        private void OnGroupButtonClick(ClickEvent cl)
        {
            Button curBtn = cl.target as Button;
            if (lastBtn != null)
            {
                lastBtn.EnableInClassList(SelectedClass, false);
                lastBtn.RemoveFromClassList(SelectedClass);
                VisualElement oldSelBar = lastBtn.Q("SelectedBar");
                oldSelBar.visible = false;
            }
            curBtn.AddToClassList(SelectedClass);
            curBtn.EnableInClassList(SelectedClass, true);
            VisualElement newSelBar = curBtn.Q("SelectedBar");
            newSelBar.visible = true;
            lastBtn = curBtn;
        }
    }
}