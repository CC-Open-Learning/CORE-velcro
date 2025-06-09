using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Intended use of this class is to connect SplashScreen.HandleTaskComplete() to a UnityEvent 
    /// in your DLX. Every time the DLX finishes an initialization stage (Retrieves learner ID for SCORM, 
    /// loads save file from Azure, initializes analytics, etc.) then a UnityEvent should be invoked that
    /// points to HandleTaskComplete(). Methods for increment/show/hide have been exposed if DLX would 
    /// prefer to serialize and control individual stages of the splash screen
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SplashScreen : MonoBehaviour, IUserInterface
    {
        [SerializeField, Tooltip("Time in seconds for splash screen to go from 0->1 opacity")]
        private float fadeDuration = 1.5f;
        [SerializeField, Tooltip("Minimum time in seconds for a message to stay visible before switching")]
        private float messageDuration = 1.0f;
        [SerializeField, Tooltip("Content of the Splash Screen")]
        private SplashScreenSO splashScreenSO;

        [HideInInspector] public VisualElement Root { private set; get; }
        public UnityEvent OnSplashScreenShown;
        public UnityEvent OnSplashScreenHidden;

        private Label messageLabel;
        private Label madeByLabel;
        private Label organizationLabel;
        private VisualElement canvas;

        private float timeSinceLastMessage = 0.0f;
        private int actualMessageIndex = -1;
        private int expectedMessageIndex = 0;

        private const string FadeUSSClass = "splash-canvas-fade";

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            canvas = Root.Q<VisualElement>("Canvas");
            messageLabel = Root.Q<Label>("MessageLabel");
            madeByLabel = Root.Q<Label>("IntroLabel");
            organizationLabel = Root.Q<Label>("OrganizationLabel");

            OnSplashScreenShown ??= new UnityEvent();
            OnSplashScreenHidden ??= new UnityEvent();

            madeByLabel.SetElementText(splashScreenSO.IntroText);
            organizationLabel.SetElementText(splashScreenSO.OrganizationText);

            canvas.style.transitionDuration = new List<TimeValue>() { new TimeValue(fadeDuration) };
            HandleDisplayNextMessage();
        }

        /// <summary>
        /// Only display next message if a task is completed and time since the last message is >= 1 second 
        /// (messageDuration SerializedField). There are 2 internal index counters:
        ///     - actualMessageIndex: The index of the current message that is being displayed on screen
        ///     - expectedMessageIndex: The index of the message that the splash screen can wait on. This is 
        ///     incremented every time a task is completed (Learner ID retrieved, save file loaded, etc.). This 
        ///     is where the actualMessageIndex will be in the future, it runs ahead of the actual since each 
        ///     message is displayed for a minimum of 1 second for readability even if the next task is complete
        /// </summary>
        private void Update()
        {
            timeSinceLastMessage += Time.deltaTime;

            if (timeSinceLastMessage >= messageDuration && actualMessageIndex <= expectedMessageIndex)
            {
                HandleDisplayNextMessage();
                timeSinceLastMessage = 0.0f;
            }
        }

        /// <summary>
        /// Displays the message at the next index in the list of messages only if it exists. Otherwise if we are 
        /// at the end of the list, start the fade out animation
        /// </summary>
        private void HandleDisplayNextMessage()
        {
            actualMessageIndex++;

            if (splashScreenSO.Messages.ElementAtOrDefault(actualMessageIndex) != null)
            {
                messageLabel.SetElementText(splashScreenSO.Messages[actualMessageIndex]);
            }
            else
            {
                StartCoroutine(FadeOut());
            }
        }

        /// <summary>
        /// Increment the internal task counter so that Update() will recognize it can display the next step
        /// </summary>
        public void HandleTaskComplete()
        {
            expectedMessageIndex++;
        }

        /// <summary>
        /// Increment the internal task counter to the last available index so that Update() will recognize 
        /// it can display all steps without delay
        /// </summary>
        public void HandleAllTaskComplete()
        {
            expectedMessageIndex = (splashScreenSO.Messages.Count - 1);
        }

        private IEnumerator FadeOut()
        {
            canvas.AddToClassList(FadeUSSClass);
            yield return new WaitForSeconds(fadeDuration);
            Hide();
            Destroy(this);
        }

        public void Show()
        {
            Root.Show();
            OnSplashScreenShown?.Invoke();
        }

        public void Hide()
        {
            Root.Hide();
            OnSplashScreenHidden?.Invoke();
        }
    }
}