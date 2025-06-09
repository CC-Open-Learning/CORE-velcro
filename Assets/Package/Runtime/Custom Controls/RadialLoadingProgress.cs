using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [UxmlElement]
    public partial class RadialLoadingProgress : VisualElement
    {
        private static CustomStyleProperty<Color> trackColourProperty = new CustomStyleProperty<Color>("--track-colour");
        private static CustomStyleProperty<Color> progressColourProperty = new CustomStyleProperty<Color>("--progress-colour");
        private static CustomStyleProperty<Color> borderColorProperty = new CustomStyleProperty<Color>("--border-colour");

        public static readonly string RadialBaseClass = "loading-indicator";
        public static readonly string RadialLabelClass = "loading-indicator-label";
        public static readonly string RadialFailedClass = "loading-indicator-failed";
        public static readonly string RadialFailedIconClass = "loading-indicator-failed-icon";

        private Color trackColour = Color.gray;
        private Color progressColour = Color.red;
        private Color borderColour = Color.black;

        private const float Size = 210f;
        private const float BorderWidth = 25f;
        private const float TrackerWidth = 21f;

        private VisualElement failedIcon;
        private Label label;
        private float progress;

        [Header("Radial Loading Properties")]
        [Tooltip("A value clamped between 0 an 100 inclusive")]
        [UxmlAttribute]
        public float Progress
        {
            get => progress;
            set
            {
                progress = value;
                progress = Mathf.Clamp(progress, 0f, 100f);
                label.text = $"{Mathf.Round(progress)}<size=48>%</size>";
                MarkDirtyRepaint();
            }
        }

        public RadialLoadingProgress()
        {
            label = new Label();
            label.AddToClassList(RadialLabelClass);
            Add(label);

            failedIcon = new VisualElement() { name = "FailedIcon" };
            failedIcon.AddToClassList(RadialFailedIconClass);
            Add(failedIcon);

            //Add the USS class name for the overall control.
            AddToClassList(RadialBaseClass);
            AddToClassList(RadialFailedClass);

            //This is so custom USS properties are applied
            RegisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);

            //This is so changes are applied after setting. MarkDirtyRepaint trigger
            //GenerateVisualContent without waiting a frame
            generateVisualContent += GenerateVisualContent;
            Progress = 0f;
        }

        private static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            RadialLoadingProgress element = evt.currentTarget as RadialLoadingProgress;
            element.UpdateCustomStyles();
        }

        //After the custom colors are resolved, this method uses them to color the meshes
        //and (if necessary) repaint the control.
        private void UpdateCustomStyles()
        {
            bool repaint = false;

            if (customStyle.TryGetValue(progressColourProperty, out progressColour))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(trackColourProperty, out trackColour))
            {
                repaint = true;
            }

            if (customStyle.TryGetValue(borderColorProperty, out borderColour))
            {
                repaint = true;
            }

            if (repaint)
            {
                MarkDirtyRepaint();
            }
        }

        private void GenerateVisualContent(MeshGenerationContext context)
        {
            float halfSize = Size / 2f;

            Painter2D painter = context.painter2D;
            painter.lineWidth = BorderWidth;
            painter.lineCap = LineCap.Butt;

            // Draw the border
            painter.strokeColor = borderColour;
            painter.BeginPath();
            painter.Arc(new Vector2(halfSize, halfSize), halfSize - BorderWidth / 2f, 0f, 360f);
            painter.Stroke();

            painter.lineWidth = TrackerWidth;

            // Draw the track
            painter.strokeColor = trackColour;
            painter.BeginPath();
            painter.Arc(new Vector2(halfSize, halfSize), halfSize - BorderWidth / 2f, 0f, 360f);
            painter.Stroke();

            // Draw the progress
            painter.strokeColor = progressColour;
            painter.BeginPath();
            painter.Arc(new Vector2(halfSize, halfSize), halfSize - BorderWidth / 2f, -90f, 360f * (Progress / 100f) - 90f);
            painter.Stroke();
        }

        ~RadialLoadingProgress()
        {
            UnregisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
            generateVisualContent -= GenerateVisualContent;
        }
    }
}