using Kamgam.UIToolkitGlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    public partial class BoxShadowManager : Shadow
    {
        private CustomStyleProperty<Color> innerShadowColourProperty = new CustomStyleProperty<Color>();
        private CustomStyleProperty<Color> outerShadowColourProperty = new CustomStyleProperty<Color>();

        private Color innerShadowColour;
        private Color outerShadowColour;

        private const string defaultString = "";
        protected string innerShadowVariable;
        protected string outerShadowVariable = defaultString;

        public string InnerShadowVariable
        {
            get => innerShadowVariable; // Explicit get and set must be exposed for UXML Traits class to be able to interact with variable
            set
            {
                if (innerShadowVariable == value)
                    return;
                innerShadowVariable = value;
            }
        }

        public string OuterShadowVariable
        {
            get => outerShadowVariable;
            set
            {
                if (outerShadowVariable == value)
                    return;
                outerShadowVariable = value;
            }
        }

        public new class UxmlFactory : UxmlFactory<BoxShadowManager, AddedUxmlTraits> { }

        public class AddedUxmlTraits : UxmlTraits // UXML Traits visible in UI Builder for entering the string values, inherits parent UxmlTraits so it doesn't replace existing fields
        {
            UxmlStringAttributeDescription innerShadowVariable = new UxmlStringAttributeDescription { name = "inner-shadow-variable", defaultValue = defaultString };
            UxmlStringAttributeDescription outerShadowVariable = new UxmlStringAttributeDescription { name = "outer-shadow-variable", defaultValue = defaultString };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var shadowManager = ve as BoxShadowManager;
                shadowManager.innerShadowVariable = innerShadowVariable.GetValueFromBag(bag, cc);
                shadowManager.OuterShadowVariable = outerShadowVariable.GetValueFromBag(bag, cc);
            }
        }

        public BoxShadowManager()
        {
            RegisterCallback<CustomStyleResolvedEvent>(ShadowColourTrigger); // This causes an error which states 'Value cannot be null. Parameter name: key'
        }

        /// <summary>
        /// Calls inner and outer colour changing functions when the custom style resolved event is triggered (theme changed)
        /// </summary>
        /// <param name="e"></param>
        private void ShadowColourTrigger(CustomStyleResolvedEvent e)
        {
            ApplyInnerShadowColour();
            ApplyOuterShadowColour();
        }

        /// <summary>
        /// Applies updated colour variable to inner shadow of object attached to box shadow manager
        /// </summary>
        private void ApplyInnerShadowColour()
        {
            if (innerShadowVariable.Length < 2 || innerShadowVariable.Substring(0, 2) != "--") // First check is to avoid reading out of bounds (string is null or only 1 length), Second check is that two dashes are used before passing it to custom style property 
                return;
            innerShadowColourProperty = new CustomStyleProperty<Color>(innerShadowVariable);
            if (this.customStyle.TryGetValue(innerShadowColourProperty, out innerShadowColour))
            {
                innerColor = innerShadowColour;
            }
        }

        /// <summary>
        /// Applies updated colour variable to outer shadow of object attached to box shadow manager
        /// </summary>
        private void ApplyOuterShadowColour()
        {
            if (outerShadowVariable.Length < 2 || outerShadowVariable.Substring(0, 2) != "--")
                return;
            outerShadowColourProperty = new CustomStyleProperty<Color>(outerShadowVariable);
            if (this.customStyle.TryGetValue(outerShadowColourProperty, out outerShadowColour)) // The key parameter error occurs at this line when the change event is attached to 'this'
            {
                outerColor = outerShadowColour;
            }
        }
    }
}