using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Data type that contains UI elements that a progress indicator task is bound to.
    /// </summary>
    public class TaskUI
    {
        public Label ProgressLabel;
        public Label TaskNameLabel;
        public VisualElement TaskCheckmark;
        public VisualElement TaskCheckmarkIcon;
    }
}
