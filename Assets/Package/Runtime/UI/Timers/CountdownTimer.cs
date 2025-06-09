using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Timers have one customization:
    ///     - Canvas Dim: The background of the timer can be dimmed to bring focus to the UI and block interaction with other UI
    ///  
    /// Intended use of this class is to connect CountdownTimer.HandleDisplayUI() to a UnityEvent in your DLX. Methods 
    /// changing content, resetting timer, and show/hide have been exposed if DLX would prefer to serialize and control 
    /// individual stages of the timer
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class CountdownTimer : MonoBehaviour, IUserInterface
    {
        [HideInInspector] public VisualElement Root { private set; get; }

        [Header("Event Hook-Ins")]
        public UnityEvent OnTimerShown;
        public UnityEvent OnTimerHidden;
        public UnityEvent OnTimerEnded;
        public UnityEvent OnTimerReset;

        private CountdownTimerElement countdownTimer;
        private VisualElement canvas;

        private const string DimmedBackgroundClass = "timer-countdown-canvas";
        private const float EndingTime = 0f;

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            countdownTimer = Root.Q<CountdownTimerElement>();
            canvas = Root.Q<VisualElement>("Canvas");

            OnTimerShown ??= new UnityEvent();
            OnTimerHidden ??= new UnityEvent();
            OnTimerEnded ??= new UnityEvent();
            OnTimerReset ??= new UnityEvent();

            Root.Hide();
        }

        /// <summary>
        /// Resets the timer, starts the timer with the incoming startTime (must be an integer), optionally dims 
        /// the background canvas element, and shows the timer. Triggers OnTimerShown
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="isBackgroundDimmed"></param>
        public void HandleDisplayUI(int startTime = 3, bool isBackgroundDimmed = true)
        {
            ResetValues();
            SetContent(startTime, isBackgroundDimmed);
            StartCoroutine(Countdown());
            Show();
        }

        /// <summary>
        /// Sets the timer's startTime with the incoming startTime int, and optionally dims 
        /// the background canvas element.
        /// </summary>
        /// <param name="startTime"></param>
        public void SetContent(int startTime = 3, bool isBackgroundDimmed = true)
        {
            startTime = Mathf.Clamp(startTime, 0, int.MaxValue);
            countdownTimer.StartTime = startTime;
            canvas.EnableInClassList(DimmedBackgroundClass, isBackgroundDimmed);
        }

        /// <summary>
        /// Coroutine to decrement the CountdownTimerElement custom control's current time. 
        /// This causes styling to update. Triggers OnTimerEnded when the CurrentTime reaches 0
        /// </summary>
        public IEnumerator Countdown()
        {
            while (countdownTimer.CurrentTime > EndingTime)
            {
                countdownTimer.CurrentTime -= Time.deltaTime;
                yield return null;
            }

            countdownTimer.CurrentTime = EndingTime;
            OnTimerEnded?.Invoke();
            Hide();
        }

        /// <summary>
        /// Stops all coroutines, sets timer current time to 0, resets the canvas dim state, 
        /// and triggers OnTimerReset
        /// </summary>
        public void ResetTimer()
        {
            ResetValues();
            OnTimerReset.Invoke();
        }

        /// <summary>
        /// Stops all coroutines, sets timer current time to 0, resets the canvas dim state, 
        /// without triggering OnTimerReset
        /// </summary>
        private void ResetValues()
        {
            StopAllCoroutines();
            countdownTimer.CurrentTime = EndingTime;
            canvas.EnableInClassList(DimmedBackgroundClass, false);
            Root.Hide();
        }

        /// <summary>
        /// Shows the root of the timer and triggers OnTimerShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnTimerShown?.Invoke();
        }

        /// <summary>
        /// Hides the root of the timer and triggers OnTimerHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnTimerHidden?.Invoke();
        }
    }
}