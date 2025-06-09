using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [UxmlElement]
    public partial class FillSlider : Slider
    {
        public static readonly string SliderTrackerClass = "slider-tracker";
        public static readonly string SliderFillClass = "tracker-fill";
        public static readonly string SliderTrackerFillClass = "slider-tracker-fill";

        // Values for setting the tracker fill to an inverted state
        private static readonly StyleLength trackerFillInvertedLeft = Length.Percent(100f);
        private static readonly StyleTransformOrigin trackerFillInvertedOrigin = new TransformOrigin(Length.Percent(0f), Length.Percent(50f));
        private static readonly StyleScale trackerFillInvertedScale = new Vector2(-1, 1);

        private VisualElement trackerFill;

        public FillSlider()
        {
            // Get the references to the elements we want to modify
            VisualElement dragContainer = this.Q(className: dragContainerUssClassName);
            VisualElement tracker = dragContainer.Q(className: trackerUssClassName);

            // Create the tracker fill element
            trackerFill = new VisualElement()
            {
                name = SliderFillClass
            };

            // Add the tracker fill as a child of the drag container
            dragContainer.Add(trackerFill);

            // Add the proper selectors to the tracker fill and set it as the first child in the drag container
            trackerFill.AddToClassList(trackerUssClassName);
            trackerFill.AddToClassList(SliderTrackerFillClass);
            trackerFill.SendToBack();

            // Initialize the width of the tracker fill based of the value of the slider
            trackerFill.style.width = GetTrackerFillWidth();

            // Add an additional selector to the tracker background and set it as the first child of the drag container
            // We want the tracker background to show behind the tracker fill
            tracker.AddToClassList(SliderTrackerClass);
            tracker.SendToBack();

            // Register an on change callback to update the tracker fill
            RegisterCallback<ChangeEvent<float>>(UpdateSliderFill);
        }

        /// <summary>
        /// On update callback that updates the tracker fill width based on the updated value of the slider.
        /// </summary>
        /// <param name="evt"></param>
        private void UpdateSliderFill(ChangeEvent<float> evt)
        {
            trackerFill.style.width = GetTrackerFillWidth();
        }

        /// <summary>
        /// Returns a width value based on the current value of the slider.
        /// </summary>
        /// <returns></returns>
        private StyleLength GetTrackerFillWidth()
        {
            return new StyleLength(new Length((value - lowValue) / (highValue - lowValue) * 100f, LengthUnit.Percent));
        }

        // Sets the tracker fill to an inverted state
        private void SetTrackerFillInverted(bool inverted)
        {
            trackerFill.style.left = inverted ? trackerFillInvertedLeft : StyleKeyword.Null;
            trackerFill.style.transformOrigin = inverted ? trackerFillInvertedOrigin : StyleKeyword.Null;
            trackerFill.style.scale = inverted ? trackerFillInvertedScale : StyleKeyword.Null;
        }

        /// <summary>
        /// Destructor for cleanup.
        /// </summary>
        ~FillSlider()
        {
            UnregisterCallback<ChangeEvent<float>>(UpdateSliderFill);
        }
    }
}