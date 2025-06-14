using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Notifications come in 4 styles and in 3 different positions. 
    ///     - Success: Checkmark icon and green background
    ///     - Info: Information icon and blue background
    ///     - Error: Warning icon and red background
    ///     - Custom: Icon is optional, background and text colour customizable
    ///     
    ///     - Align.FlexStart (Horizontally centered, vertically pinned to the top of the screen) 
    ///     - Align.Center (Horizontally centered, vertically centered) 
    ///     - Align.FlexEnd (Horizontally centered, vertically pinned to the bottom of the screen) 
    /// 
    /// Intended use of this class is to connect Notification.HandleDisplayUI() to a UnityEvent 
    /// in your DLX. Methods changing content, alignment, fade in/out, and show/hide have been exposed 
    /// if DLX would prefer to serialize and control individual stages of the notification
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class Notification : MonoBehaviour, IUserInterface
    {
        [Header("Notification Settings")]
        [SerializeField, Tooltip("Time in seconds for notification to go from 0->1 or 1->0 opacity")]
        private float fadeDuration = 0.75f;
        [SerializeField, Tooltip("Time in seconds for notification to stay at 1 opacity before fading out")]
        private float solidDuration = 2.0f;
        [SerializeField, Tooltip("Pixel count and direction of translate on enter/exit. Origin is top left")]
        private float translateDirection = -5.0f;

        [Header("Notification Icons")]
        [SerializeField] private Sprite successIcon;
        [SerializeField] private Sprite errorIcon;
        [SerializeField] private Sprite infoIcon;

        [Header("Custom Notification Settings")]
        [SerializeField] private Sprite customIcon;
        [SerializeField] private Color customBackgroundColour = Color.black;
        [SerializeField] private Color customTextColour = Color.white;
        
        [HideInInspector] public VisualElement Root { private set; get; }

        public UnityEvent OnNotificationShown;
        public UnityEvent OnNotificationHidden;

        private VisualElement notification;
        private VisualElement iconContainer;
        private VisualElement icon;
        private VisualElement labelContainer;
        private Label label;

        private const string SuccessUSSClass = "notification-success";
        private const string ErrorUSSClass = "notification-error";
        private const string InfoUSSClass = "notification-info";
        private const float FadeInOpacity = 1.0f;
        private const float FadeOutOpacity = 0.0f;
        private const float StartPosition = 0.0f;
        private const int MarginLeftCustom = 60;
        private const int MarginLeftDefault = 0;

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            label = Root.Q<Label>("Text");
            icon = Root.Q<VisualElement>("Icon");
            notification = Root.Q<VisualElement>("Container");
            iconContainer = Root.Q<VisualElement>("IconContainer");
            labelContainer = Root.Q<VisualElement>("LabelContainer");

            OnNotificationShown ??= new UnityEvent();
            OnNotificationHidden ??= new UnityEvent();

            notification.style.transitionDuration = new List<TimeValue>() { new TimeValue(fadeDuration) };
            Root.Hide();
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the notification with text
        /// and display it at the desired position (top, center, or bottom)
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="message"></param>
        /// <param name="fontSize"></param>
        /// <param name="alignment"></param>
        public void HandleDisplayUI(NotificationType notificationType, string message, FontSize fontSize = FontSize.Medium, Align alignment = Align.FlexStart)
        {
            ClearClasses();
            SetContent(notificationType, message);
            label.SetElementFontSize(fontSize);
            PositionHelper.SetAbsoluteVerticalPosition(notification, alignment);
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the notification with text
        /// and display it at the desired position (top, center, or bottom). This method accepts a SO rather 
        /// than individual properties if DLX have the ability to predefine their UI contents
        /// </summary>
        /// <param name="notificationSO"></param>
        public void HandleDisplayUI(NotificationSO notificationSO)
        {
            ClearClasses();
            SetContent(notificationSO.NotificationType, notificationSO.Message);
            label.SetElementFontSize(notificationSO.FontSize);
            PositionHelper.SetAbsoluteVerticalPosition(notification, notificationSO.Alignment);
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// There are 4 different types of notifications:
        ///     - Success: Checkmark icon and green background
        ///     - Info: Information icon and blue background
        ///     - Error: Warning icon and red background
        ///     - Custom: Icon is optional, background and text colour customizable
        ///     
        /// The different styles are already created in USS classes and applied depending on the enum NotificationType provided
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="message"></param>
        public void SetContent(NotificationType notificationType, string message)
        {
            label.SetElementText(message);

            switch (notificationType)
            {
                case NotificationType.Success:
                    icon.SetElementSprite(successIcon);
                    notification.EnableInClassList(SuccessUSSClass, true);
                    break;

                case NotificationType.Info:
                    icon.SetElementSprite(infoIcon);
                    notification.EnableInClassList(InfoUSSClass, true);
                    break;

                case NotificationType.Error:
                    icon.SetElementSprite(errorIcon);
                    notification.EnableInClassList(ErrorUSSClass, true);
                    break;

                case NotificationType.Custom:
                    if (!customIcon) 
                    {
                        iconContainer.Hide();
                        labelContainer.style.marginLeft = MarginLeftCustom;
                    }

                    icon.SetElementSprite(customIcon);
                    notification.SetBackgroundColour(customBackgroundColour);
                    label.SetElementColour(customTextColour);
                    break;

                default:
                    Debug.LogWarning("Notification.SetContent() - No notification type matched the switch statement. Default case selected!");
                    break;
            }
        }

        /// <summary>
        /// Sets the custom notification properties icon, background colour, and text colour for the next 
        /// time HandleDisplayUI(Custom) is called
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="backgroundColour"></param>
        /// <param name="textColour"></param>
        public void SetCustomNotification(Color backgroundColour, Color textColour, Sprite icon = null)
        {
            customIcon = icon;
            customBackgroundColour = backgroundColour;
            customTextColour = textColour;
        }

        /// <summary>
        /// Notifications fade in, and bump up based on the properties serialized in the script. On enter, the 
        /// notification goes from 0 to 1 opacity and bumps up translateDirection units
        /// </summary>
        public IEnumerator FadeIn()
        {
            //If the notification is currently visible, hide it and start the process again
            if (Root.style.display == DisplayStyle.Flex)
            {
                ResetAlert();
                StartCoroutine(FadeIn());
            }

            Show();
            StyleHelper.ToggleTransitionProperties(notification, FadeInOpacity, translateDirection);
            yield return new WaitForSeconds(fadeDuration + solidDuration);
            StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Notifications fade out, and bump down based on the properties serialized in the script. On exit, the 
        /// notification goes from 1 to 0 opacity and bumps down translateDirection units
        /// </summary>
        public IEnumerator FadeOut()
        {
            StyleHelper.ToggleTransitionProperties(notification, FadeOutOpacity, StartPosition);
            yield return new WaitForSeconds(fadeDuration);
            Hide();
        }

        public void Show()
        {
            Root.Show();
            OnNotificationShown?.Invoke();
        }

        public void Hide()
        {
            Root.Hide();
            OnNotificationHidden?.Invoke();
        }

        public void ClearClasses()
        {
            notification.EnableInClassList(SuccessUSSClass, false);
            notification.EnableInClassList(InfoUSSClass, false);
            notification.EnableInClassList(ErrorUSSClass, false);
            labelContainer.style.marginLeft = MarginLeftDefault;

            iconContainer.style.display = DisplayStyle.Flex;
            icon.style.backgroundImage = StyleKeyword.Null;
            notification.style.backgroundColor = StyleKeyword.Null;
            label.style.color = StyleKeyword.Null;
        }

        private void ResetAlert()
        {
            Hide();
            StopAllCoroutines();
        }
    }
}