using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [UxmlElement]
    public partial class CountdownTimerElement : VisualElement
    {
        private CustomStyleProperty<Color> outerBorderColourProperty = new CustomStyleProperty<Color>("--border-outer-colour");
        private CustomStyleProperty<Color> outerBorderColour2Property = new CustomStyleProperty<Color>("--border-outer-colour-2");
        private CustomStyleProperty<Color> progressColourProperty = new CustomStyleProperty<Color>("--progress-colour");
        private CustomStyleProperty<Color> trackerColourProperty = new CustomStyleProperty<Color>("--tracker-colour");

        private static readonly string ussClassName = "timer-countdown";
        private static readonly string labelUssClassName = "timer-countdown-label";
        private static readonly string labelElementName = "TimerLabel";

        private const float DefaultStartingTime = 3.0f;
        private const float InnerLineHeight = 0.12f;
        private const float InnerLineWidth = 3.0f;
        private const float InnerStartingArcAngle = 0.0f;
        private const int InnerCircleCount = 68;
        private const float MiddleLineWidth = 19.0f;
        private const float OuterLineHeight = 1.8f;
        private const float OuterLineWidth = 15.0f;
        private const float OuterStartingArcAngle = -90.0f;
        private const int NumberOfTicks = 56;
        private const float Radius = 125.0f;
        private const float Size = 400f;

        private Color outerBorderColour;
        private Color outerBorderColour2;
        private Color progressColour;
        private Color trackerColour;

        private float startTime = DefaultStartingTime;
        private float currentTime;
        private Label label;

        [Header("Countdown Timer Attributes")]
        [UxmlAttribute]
        public float StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                CurrentTime = value;
            }
        }

        [UxmlAttribute]
        public float CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                currentTime = Mathf.Clamp(currentTime, 0, startTime);
                label.text = Mathf.Ceil(currentTime).ToString();
                MarkDirtyRepaint();
            }
        }

        public CountdownTimerElement()
        {
            AddToClassList(ussClassName);

            label = new Label() { name = labelElementName };
            label.AddToClassList(labelUssClassName);
            Add(label);

            RegisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
            generateVisualContent += OnVisualContentGenerated;
            
            CurrentTime = startTime;
        }

        private static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            CountdownTimerElement element = evt.currentTarget as CountdownTimerElement;
            element.UpdateCustomStyles();
        }

        // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
        // the control.
        private void UpdateCustomStyles()
        {
            bool repaint = false;

            if (customStyle.TryGetValue(outerBorderColourProperty, out outerBorderColour))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(outerBorderColour2Property, out outerBorderColour2))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(progressColourProperty, out progressColour))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(trackerColourProperty, out trackerColour))
            {
                repaint = true;
            }

            if (repaint)
            {
                MarkDirtyRepaint();
            }
        }

        private void OnVisualContentGenerated(MeshGenerationContext context)
        {
            Vector2 center = new Vector2(Size, Size) / 2f;
            Painter2D painter = context.painter2D;

            // Inner Circle (Gray Ticks)
            float arcAngle = InnerStartingArcAngle;
            painter.lineWidth = InnerLineWidth;
            painter.lineCap = LineCap.Round;
            painter.strokeColor = trackerColour;

            for (int i = 0; i < InnerCircleCount; i++)
            {
                painter.BeginPath();
                painter.Arc(center, Radius, arcAngle, arcAngle + InnerLineHeight);
                painter.Stroke();

                arcAngle += 360f / InnerCircleCount;
            }

            // Middle Circle (Progress)
            painter.lineWidth = MiddleLineWidth;
            painter.strokeColor = progressColour;
            painter.BeginPath();
            painter.Arc(center, Radius, 90, 360f * (currentTime / startTime) + 90f);
            painter.Stroke();

            // Outer Circle (Animation)
            painter.lineWidth = OuterLineWidth;
            painter.lineCap = LineCap.Butt;
            painter.BeginPath();

            arcAngle = OuterStartingArcAngle;
            float remainder = startTime % 2f;
            float flooredTime = Mathf.Floor(currentTime);
            float percent = currentTime - flooredTime;
            float startAngle = 360f * percent - 90f;

            for (int i = 0; i < NumberOfTicks; i++)
            {
                bool remainderCheck = flooredTime % 2 == remainder;

                if (arcAngle >= startAngle || currentTime <= 0f)
                {
                    painter.strokeColor = remainderCheck ? outerBorderColour : outerBorderColour2;
                }
                else
                {
                    painter.strokeColor = remainderCheck ? outerBorderColour2 : outerBorderColour;
                }

                painter.BeginPath();
                painter.Arc(center, 175f, arcAngle, arcAngle + OuterLineHeight);
                painter.Stroke();

                arcAngle += 360f / NumberOfTicks;
            }

            painter.ClosePath();
        }

        ~CountdownTimerElement()
        {
            UnregisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
            generateVisualContent -= OnVisualContentGenerated;
        }
    }
}