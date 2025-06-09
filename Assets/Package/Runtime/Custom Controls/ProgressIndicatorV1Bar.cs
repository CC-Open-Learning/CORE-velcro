using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [UxmlElement]
    public partial class ProgressIndicatorV1Bar : VisualElement
    {
        private static readonly string ussClassName = "progress-indicator-v1-bar";
        private static readonly string backgroundUssClassName = "progress-indicator-v1-bar-background";
        private static readonly string fillUssClassName = "progress-indicator-v1-bar-fill";

        private float value = 0f;
        private float minValue = 0f;
        private float maxValue = 100f;

        private VisualElement background;
        private VisualElement fill;

        [Header("Progress Indicator Attributes")]
        [UxmlAttribute]
        public float MinValue 
        { 
            get => minValue; 
            set => minValue = value; 
        }

        [UxmlAttribute]
        public float MaxValue
        {
            get => maxValue;
            set => maxValue = value;
        }

        [UxmlAttribute]
        public float Value
        {
            get => value;
            set
            {
                this.value = value;
                this.value = Mathf.Clamp(this.value, MinValue, MaxValue);
                fill.style.width = GetFillWidth();
                StyleColor fillColour = Value > 0f ? StyleKeyword.Null : Color.clear;
                fill.SetBackgroundColour(fillColour);
                fill.SetBorderColour(fillColour);
            }
        }

        public ProgressIndicatorV1Bar()
        {
            AddToClassList(ussClassName);

            background = new VisualElement() { name = "Background" };
            background.AddToClassList(backgroundUssClassName);
            Add(background);

            fill = new VisualElement() { name = "Fill" };
            fill.AddToClassList(fillUssClassName);
            Add(fill);

            Value = 0f;
        }

        private StyleLength GetFillWidth()
        {
            return new StyleLength(new Length(value / MaxValue * 100f, LengthUnit.Percent));
        }
    }
}