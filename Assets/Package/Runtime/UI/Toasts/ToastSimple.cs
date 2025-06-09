using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Simple toasts come in 3 styles 
    ///     - Hint: light bulb icon and default dark/light mode colours (Expected default use)
    ///     - Location: location pin icon and default dark/light mode colours
    ///     - Custom: Icon is optional, background and text colours customizable
    ///     
    ///     - Align.FlexStart (Horizontally centered, vertically pinned to the top of the screen) 
    ///     - Align.Center (Horizontally centered, vertically centered) 
    ///     - Align.FlexEnd (Horizontally centered, vertically pinned to the bottom of the screen) 
    /// 
    /// Intended use of this class is to connect Toast.HandleDisplayUI() to a UnityEvent 
    /// in your DLX. Methods changing content, resetting, fade in/out, and show/hide have been exposed 
    /// if DLX would prefer to serialize and control individual stages of the toast
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ToastSimple : Toast, IUserInterface
    {
        private void Start()
        {
            SetupBaseToast();
        }

        /// <summary>
        /// This method is intended to be a single access method to reset the toast, populate the toast with text
        /// and display it at the desired position (top, center, or bottom)
        /// </summary>
        /// <param name="header"></param>
        /// <param name="message"></param>
        /// <param name="toastType"></param>
        /// <param name="alignment"></param>
        public void HandleDisplayUI(string header, string message, ToastType toastType = ToastType.Hint, Align alignment = Align.FlexEnd)
        {
            ResetToast();
            SetContent(toastType, header, message);
            PositionHelper.SetAbsoluteVerticalPosition(toast, alignment);
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// This method is intended to be a single access method to reset the toast, populate the toast with text
        /// and display it at the desired position (top, center, or bottom). This method only accepts new text, 
        /// and will display the default hint icon and position the toast at the bottom
        /// </summary>
        /// <param name="header"></param>
        /// <param name="message"></param>
        public void HandleDisplayUI(string header, string message)
        {
            ResetToast();
            SetContent(ToastType.Hint, header, message);
            PositionHelper.SetAbsoluteVerticalPosition(toast, Align.FlexEnd);
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// This method is intended to be a single access method to reset the toast, populate the toast with text
        /// and display it at the desired position (top, center, or bottom). This method accepts a SO rather 
        /// than individual properties if DLX have the ability to predefine their UI contents
        /// </summary>
        /// <param name="toastSimpleSO"></param>
        public void HandleDisplayUI(ToastSimpleSO toastSimpleSO)
        {
            ResetToast();
            SetContent(toastSimpleSO.ToastType, toastSimpleSO.Header, toastSimpleSO.Message);
            PositionHelper.SetAbsoluteVerticalPosition(toast, toastSimpleSO.Alignment);
            StartCoroutine(FadeIn());
        }
    }
}