using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Complex toasts come in 3 styles 
    ///     - Hint: light bulb icon and default dark/light mode colours 
    ///     - Location: location pin icon and default dark/light mode colours (Expected default use)
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
    public class ToastComplex : Toast, IUserInterface
    {
        private Label secondaryMessageLabel;

        private void Start()
        {
            SetupBaseToast();
            secondaryMessageLabel = Root.Q<Label>("SecondaryMessageLabel");
        }

        /// <summary>
        /// This method is intended to be a single access method to reset the toast, populate the toast with text
        /// and display it at the desired position (top, center, or bottom)
        /// </summary>
        /// <param name="header"></param>
        /// <param name="message"></param>
        /// <param name="secondaryMessage"></param>
        /// <param name="toastType"></param>
        /// <param name="alignment"></param>
        public void HandleDisplayUI(string header, string message, string secondaryMessage, ToastType toastType = ToastType.Location, Align alignment = Align.FlexEnd)
        {
            ResetToast();
            SetContent(toastType, header, message, secondaryMessage);
            PositionHelper.SetAbsoluteVerticalPosition(toast, alignment);
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// This method is intended to be a single access method to reset the toast, populate the toast with text
        /// and display it at the desired position (top, center, or bottom). This method only accepts new text, 
        /// and will display the default location icon and position the toast at the bottom
        /// </summary>
        /// <param name="header"></param>
        /// <param name="message"></param>
        /// <param name="secondaryMessage"></param>
        public void HandleDisplayUI(string header, string message, string secondaryMessage)
        {
            ResetToast();
            SetContent(ToastType.Location, header, message, secondaryMessage);
            PositionHelper.SetAbsoluteVerticalPosition(toast, Align.FlexEnd);
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// This method is intended to be a single access method to reset the toast, populate the toast with text
        /// and display it at the desired position (top, center, or bottom). This method accepts a SO rather 
        /// than individual properties if DLX have the ability to predefine their UI contents
        /// </summary>
        /// <param name="toastComplexSO"></param>
        public void HandleDisplayUI(ToastComplexSO toastComplexSO)
        {
            ResetToast();
            SetContent(toastComplexSO.ToastType, toastComplexSO.Header, toastComplexSO.Message, toastComplexSO.SecondaryMessage);
            PositionHelper.SetAbsoluteVerticalPosition(toast, toastComplexSO.Alignment);
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// There are 3 different types of toasts:
        ///     - Hint: light bulb icon and default dark/light mode colours
        ///     - Location: location pin icon and default dark/light mode colours
        ///     - Custom: Icon is optional, background and text colours customizable
        /// </summary>
        /// <param name="toastType"></param>
        /// <param name="header"></param>
        /// <param name="message"></param>
        /// <param name="secondaryMessage"></param>
        public void SetContent(ToastType toastType, string header, string message, string secondaryMessage)
        {
            secondaryMessageLabel.SetElementText(secondaryMessage);
            base.SetContent(toastType, header, message);

            if (toastType == ToastType.Custom)
            {
                secondaryMessageLabel.SetElementColour(customMessageColour);
            }
        }

        /// <summary>
        /// Resets all inline styles applied to the toast. Styling will default back to those defined in USS classes
        /// </summary>
        public new void ResetToast()
        {
            secondaryMessageLabel.style.color = StyleKeyword.Null;
            base.ResetToast();
        }
    }
}