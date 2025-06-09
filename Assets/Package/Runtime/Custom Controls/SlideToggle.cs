using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [UxmlElement]
    public partial class SlideToggle : BaseField<bool>
    {
        public static readonly string SlideToggleBaseClass = "slide-toggle";
        public static readonly string SlideToggleInputClass = "slide-toggle__input";
        public static readonly string SlideToggleInputCheckedClass = "slide-toggle__input--checked";
        public static readonly string SlideToggleInputHolderClass = "slide-toggle__input-knob-holder";
        public static readonly string SlideToggleKnobClass = "slide-toggle__input-knob";

        private VisualElement inputElement;
        private VisualElement knobHolderElement;
        private VisualElement knobElement;

        public SlideToggle() : this(null) { }

        /// <summary>
        /// Parameterized constructor that initializes a slide toggle element in the UI Builder
        /// with the given label.
        /// </summary>
        /// <param name="label">The label of the new slide toggle UI element</param>
        public SlideToggle(string label) : base(label, null)
        {
            // Style the control overall.
            AddToClassList(SlideToggleBaseClass);

            // Get the BaseField's visual input element and use it as the background of the slide.
            inputElement = this.Q(className: BaseField<bool>.inputUssClassName);
            inputElement.AddToClassList(SlideToggleInputClass);
            Add(inputElement);

            // Create a parent class to the knob and assign its values
            // (mainly being used for the hover effect)
            knobHolderElement = new VisualElement();
            knobHolderElement.AddToClassList(SlideToggleInputHolderClass);
            inputElement.Add(knobHolderElement);

            // Create a "knob" child element for the background to represent the actual slide of the toggle.
            knobElement = new VisualElement();
            knobElement.AddToClassList(SlideToggleKnobClass);
            knobHolderElement.Add(knobElement);

            // ClickEvent fires when a sequence of pointer down and pointer up actions occurs.
            RegisterCallback<ClickEvent>(evt => OnClick(evt));
            // KeydownEvent fires when the field has focus and a user presses a key.
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));
            // NavigationSubmitEvent detects input from keyboards, gamepads, or other devices at runtime.
            RegisterCallback<NavigationSubmitEvent>(evt => OnSubmit(evt));
        }

        private static void OnClick(ClickEvent evt)
        {
            SlideToggle slideToggle = evt.currentTarget as SlideToggle;
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        private static void OnSubmit(NavigationSubmitEvent evt)
        {
            SlideToggle slideToggle = evt.currentTarget as SlideToggle;
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        private static void OnKeydownEvent(KeyDownEvent evt)
        {
            SlideToggle slideToggle = evt.currentTarget as SlideToggle;

            // NavigationSubmitEvent event already covers keydown events at runtime, so this method shouldn't handle
            // them.
            if (slideToggle.panel?.contextType == ContextType.Player)
            {
                return;
            }

            // Toggle the value only when the user presses Enter, Return, or Space.
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                slideToggle.ToggleValue();
                evt.StopPropagation();
            }
        }

        // All three callbacks call this method.
        private void ToggleValue()
        {
            value = !value;
        }

        // Because ToggleValue() sets the value property, the BaseField class dispatches a ChangeEvent. This results in a
        // call to SetValueWithoutNotify(). This example uses it to style the toggle based on whether it's currently
        // enabled.
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            //This line of code styles the input element to look enabled or disabled.
            inputElement.EnableInClassList(SlideToggleInputCheckedClass, newValue);
        }
    }
}