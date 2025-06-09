using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Data type that contains UI elements that a progress indicator category is bound to.
    /// </summary>
    public class CategoryUI
    {
        public ProgressIndicatorV1Bar ProgressBar;
        public VisualElement ProgressBarFill;
        public Label ProgressLabel;
        public VisualElement TaskHolder;
    }
}
