using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Helper class with various methods that are reused across many different UI scripts
    ///     - SetAbsoluteVerticalPosition() to help position UI without a parent "Canvas" element
    ///     - SetUITopAnchor() to help position UI in the top right or top left corner without a parent "Canvas" element
    /// </summary>
    public static class PositionHelper
    {
        private const int Center = 50;

        /// <summary>
        /// This method sets the absolute positioning of the incoming VisualElement container depending on the incoming Align enum.
        ///     - Align.FlexStart (Horizontally centered, vertically pinned to the top of the screen) 
        ///     - Align.Center (Horizontally centered, vertically centered) 
        ///     - Align.FlexEnd (Horizontally centered, vertically pinned to the bottom of the screen) 
        /// </summary>
        /// <param name="visualElement"></param>
        /// <param name="alignment"></param>
        public static void SetAbsoluteVerticalPosition(VisualElement visualElement, Align alignment)
        {
            if (visualElement == null)
            {
                Debug.LogError("PositionHelper.SetAbsoluteVerticalPosition() - Incoming VisualElement Is Null!");
                return;
            }

            switch (alignment)
            {
                case Align.FlexStart:
                    visualElement.style.top = 0;
                    visualElement.style.bottom = StyleKeyword.Auto;
                    break;

                case Align.Center:
                    visualElement.style.bottom = Length.Percent(Center);
                    visualElement.style.top = Length.Percent(Center);
                    break;

                case Align.FlexEnd:
                    visualElement.style.bottom = 0;
                    visualElement.style.top = StyleKeyword.Auto;
                    break;

                default:
                    Debug.LogWarning("PositionHelper.SetAbsoluteVerticalPosition() - Incoming Align Enum is not Supported!");
                    break;
            }
        }

        /// <summary>
        /// This method anchors the incoming VisualElement container depending on the incoming TopAnchor enum.
        ///     - TopAnchor.TopLeft (Anchored to the top left corner)
        ///     - TopAnchor.TopRight (Anchored to the top right corner)
        /// </summary>
        /// <param name="visualElement">UI that needs to be anchored</param>
        /// <param name="anchor">Where the UI should be anchored</param>
        public static void SetUITopAnchor(VisualElement visualElement, TopAnchor anchor)
        {
            if (visualElement == null)
            {
                Debug.LogError("PositionHelper.SetUITopAnchor() - Incoming VisualElement Is Null!");
                return;
            }

            switch (anchor)
            {
                case TopAnchor.TopLeft:
                    visualElement.style.top = 0;
                    visualElement.style.bottom = StyleKeyword.Auto;
                    visualElement.style.alignSelf = Align.FlexStart;
                    break;

                case TopAnchor.TopRight:
                    visualElement.style.top = 0;
                    visualElement.style.bottom = StyleKeyword.Auto;
                    visualElement.style.alignSelf = Align.FlexEnd;
                    break;

                default:
                    Debug.LogWarning("PositionHelper.SetUITopAnchor() - Incoming TopAnchor Enum is not Supported!");
                    break;
            }
        }
    }
}