using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Timers have no customization, but 2 different starting states available:
    ///     - Start Open: Whether the timer starts expanded/open or collapsed/closed
    ///     - Start Paused: Whether the timer time starts paused or not
    ///     
    /// Intended use of this class is to connect CountupTimer.HandleDisplayUI() to a UnityEvent in your DLX. Methods 
    /// pausing, resuming, resetting, and show/hide have been exposed if DLX would prefer to serialize and control 
    /// individual stages of the timer
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class CountupTimer : MonoBehaviour, IUserInterface, IStopwatch
    {
        [HideInInspector] public VisualElement Root { private set; get; }

        [Header("Event Hook-Ins")]
        public UnityEvent OnTimerShown;
        public UnityEvent OnTimerHidden;
        public UnityEvent OnTimerPaused;
        public UnityEvent OnTimerResumed;
        public UnityEvent OnTimerReset;

        private VisualElement timerContainer;
        private VisualElement arrow;
        private Button arrowBtn;
        private Label elapsedTimeLabel;

        private const string ClosedClassName = "timer-closed";
        private const string ArrowClosedClassName = "timer-arrow-closed";
        private const double StartingTime = 0d;

        private double elapsedTime = StartingTime;

        /// <summary>
        /// Time elapsed from when <see cref="HandleDisplayUI"/> was last called.
        /// </summary>
        public double ElapsedTime
        {
            get => elapsedTime;
            private set
            {
                elapsedTime = value;
                elapsedTimeLabel.SetElementText(StringHelper.FormatTime(elapsedTime));
            }
        }

        /// <summary>
        /// Whether the timer is paused or not
        /// </summary>
        public bool IsPaused { get; private set; } = true;

        /// <summary>
        /// Whether the timer is expanded or not
        /// </summary>
        public bool IsOpen { get; private set; } = false;

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            elapsedTimeLabel = Root.Q<Label>("ElapsedTimeLabel");
            timerContainer = Root.Q("CountupTimer");
            arrowBtn = Root.Q<Button>("ArrowBtn");
            arrow = Root.Q("Arrow");

            OnTimerShown ??= new UnityEvent();
            OnTimerHidden ??= new UnityEvent();
            OnTimerPaused ??= new UnityEvent();
            OnTimerResumed ??= new UnityEvent();
            OnTimerReset ??= new UnityEvent();

            arrowBtn.clicked += () =>
            {
                ToggleTimer();
            };

            ResetValues();
        }

        /// <summary>
        /// Add delta time to elapsed time if its not paused
        /// </summary>
        private void Update()
        {
            if (!IsPaused)
            {
                ElapsedTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// This method is intended to be a single access public method to reset the timer, show the root, 
        /// and open/start the timer based on the incoming startOpened/startPaused bools
        /// </summary>
        /// <param name="startOpened">Whether to start the timer in its opened or closed state.</param>
        /// <param name="startPaused">Whether to start the timer in its paused or unpaused state.</param>
        public void HandleDisplayUI(bool startOpened = true, bool startPaused = false)
        {
            ResetValues();
            Show();
            IsOpen = startOpened;
            IsPaused = startPaused;
            
            if (startOpened)
            {
                ToggleTimer();
            }
        }

        /// <summary>
        /// Sets the timer container and arrow to its open/closed state
        ///     - Closed: Arrow pointing left with body hidden
        ///     - Opened: Arrow pointing right with body shown
        /// </summary>
        public void ToggleTimer()
        {
            timerContainer.ToggleInClassList(ClosedClassName);
            arrow.ToggleInClassList(ArrowClosedClassName);
        }

        /// <summary>
        /// Pauses the timer, and triggers OnTimerPaused
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            OnTimerPaused?.Invoke();
        }

        /// <summary>
        /// Resumes the timer, and triggers OnTimerResumed
        /// </summary>
        public void Resume()
        {
            IsPaused = false;
            OnTimerResumed?.Invoke();
        }

        /// <summary>
        /// Sets the elapsed time for the timer
        /// </summary>
        /// <param name="newTime"></param>
        public void SetElapsedTime(double newTime)
        {
            ElapsedTime = newTime;
        }

        /// <summary>
        /// Resets the timer. Sets ElapsedTime to 0, IsPaused to true, IsOpen to false,
        /// applies USS classes for closed state, and triggers OnTimerReset
        /// </summary>
        public void ResetTimer()
        {
            ResetValues();
            OnTimerReset?.Invoke();
        }

        /// <summary>
        /// Resets the timer. Sets ElapsedTime to 0, IsPaused to true, IsOpen to false,
        /// applies USS classes for closed state without triggering OnTimerReset
        /// </summary>
        private void ResetValues()
        {
            timerContainer.EnableInClassList(ClosedClassName, true);
            arrow.EnableInClassList(ArrowClosedClassName, true);

            IsOpen = false;
            IsPaused = true;
            ElapsedTime = StartingTime;
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