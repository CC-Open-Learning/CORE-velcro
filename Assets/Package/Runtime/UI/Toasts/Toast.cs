using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    public abstract class Toast : MonoBehaviour
    {
        [HideInInspector] public VisualElement Root { protected set; get; }

        [Header("Toast Settings")]
        [SerializeField, Tooltip("Time in seconds for toast to go from 0->1 or 1->0 opacity")]
        protected float fadeDuration = 0.75f;
        [SerializeField, Tooltip("Pixel count and direction of translate on enter/exit. Origin is top left")]
        protected float translateDirection = 0.0f;

        [Header("Toast Icons")]
        [SerializeField] protected Sprite hintIcon;
        [SerializeField] protected Sprite locationIcon;

        [Header("Custom Toast Settings")]
        [SerializeField] protected Sprite customIcon;
        [SerializeField] protected Color customBackgroundColour = Color.black;
        [SerializeField] protected Color customHeaderColour = Color.white;
        [SerializeField] protected Color customMessageColour = Color.white;

        [Header("Event Hook-Ins")]
        public UnityEvent OnToastShown;
        public UnityEvent OnToastHidden;

        protected VisualElement iconContainer;
        protected VisualElement icon;
        protected VisualElement toast;
        protected Label headerLabel;
        protected Label messageLabel;
        protected Button closeBtn;

        protected const float FadeInOpacity = 1.0f;
        protected const float FadeOutOpacity = 0.0f;
        protected const float StartPosition = 0.0f;

        protected void SetupBaseToast()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            closeBtn = Root.Q<Button>("CloseBtn");
            iconContainer = Root.Q<VisualElement>("IconContainer");
            icon = Root.Q<VisualElement>("Icon");
            toast = Root.Q<VisualElement>("Toast");
            headerLabel = Root.Q<Label>("HeaderLabel");
            messageLabel = Root.Q<Label>("MessageLabel");

            OnToastShown ??= new UnityEvent();
            OnToastHidden ??= new UnityEvent();

            closeBtn.clicked += () =>
            {
                CloseToast();
            };

            toast.style.transitionDuration = new List<TimeValue>() { new TimeValue(fadeDuration) };
            Root.Hide();
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
        public void SetContent(ToastType toastType, string header, string message)
        {
            headerLabel.SetElementText(header);
            messageLabel.SetElementText(message);

            switch (toastType)
            {
                case ToastType.Hint:
                    icon.SetElementSprite(hintIcon);
                    break;

                case ToastType.Location:
                    icon.SetElementSprite(locationIcon);
                    break;

                case ToastType.Custom:
                    if (!customIcon)
                    {
                        iconContainer.Hide();
                    }

                    icon.SetElementSprite(customIcon);
                    toast.SetBackgroundColour(customBackgroundColour);
                    headerLabel.SetElementColour(customHeaderColour);
                    messageLabel.SetElementColour(customMessageColour);
                    break;

                default:
                    Debug.LogWarning("Toast.SetContent() - No toast type matched the switch statement. Default case selected!");
                    break;
            }
        }

        /// <summary>
        /// Sets the custom toast properties, icon, background colour, and text colour for the next 
        /// time HandleDisplayUI(Custom) is called
        /// </summary>
        /// <param name="backgroundColour"></param>
        /// <param name="headerColour"></param>
        /// <param name="messageColour"></param>
        /// <param name="icon"></param>
        public void SetCustomToast(Color backgroundColour, Color headerColour, Color messageColour, Sprite icon = null)
        {
            customIcon = icon;
            customBackgroundColour = backgroundColour;
            customHeaderColour = headerColour;
            customMessageColour = messageColour;
        }

        /// <summary>
        /// Resets all inline styles applied to the toast. Styling will default back to those defined in USS classes
        /// </summary>
        public void ResetToast()
        {
            iconContainer.style.display = DisplayStyle.Flex;
            icon.style.backgroundImage = StyleKeyword.Null;
            toast.style.backgroundColor = StyleKeyword.Null;
            headerLabel.style.color = StyleKeyword.Null;
            messageLabel.style.color = StyleKeyword.Null;

            Hide();
            StopAllCoroutines();
        }

        /// <summary>
        /// Toasts fade in, and bump up based on the properties serialized in the script. On enter, the 
        /// toast goes from 0 to 1 opacity and bumps up translateDirection units
        /// </summary>
        public IEnumerator FadeIn()
        {
            //If the toast is currently visible, hide it and start the process again
            if (Root.style.display == DisplayStyle.Flex)
            {
                ResetToast();
                StartCoroutine(FadeIn());
            }

            Show();
            StyleHelper.ToggleTransitionProperties(toast, FadeInOpacity, translateDirection);
            yield return new WaitForSeconds(fadeDuration);
        }

       /// <summary>
       /// Starts FadeOut() coroutine of the toast
       /// </summary>
        public void CloseToast()
        {
            StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Toasts fade out, and bump down based on the properties serialized in the script. On exit, the 
        /// toast goes from 1 to 0 opacity and bumps down translateDirection units
        /// </summary>
        public IEnumerator FadeOut()
        {
            StyleHelper.ToggleTransitionProperties(toast, FadeOutOpacity, StartPosition);
            yield return new WaitForSeconds(fadeDuration);
            Hide();
        }

        /// <summary>
        /// Shows the root of the toast and triggers OnToastShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnToastShown?.Invoke();
        }

        /// <summary>
        /// Hides the root of the toast and triggers OnToastHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnToastHidden?.Invoke();
        }
    }
}