using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Loading Indicators have 2 styles
    ///     - Dimmed: The background of the loading indicator can be dimmed
    ///     - Opaque: The background of the loading indicator can be opaque which matches default dark/light colours
    ///     
    /// Intended use of this class is to connect LoadingIndicator.HandleDisplayUI() to a UnityEvent 
    /// in your DLX. Methods changing content, starting, failing, resetting, fading in, and showing/hiding 
    /// have been exposed if DLX would prefer to serialize and control individual stages of the loading 
    /// indicator
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class LoadingIndicator : MonoBehaviour, IUserInterface
    {
        [HideInInspector] public VisualElement Root { private set; get; }

        [Header("Loading Indicator Settings")]
        [SerializeField, Tooltip("The text that shows below the indicator when it is actively loading")]
        private string loadingText = "Loading...";
        [SerializeField, Tooltip("The text that shows below the indicator when the loading process has completed")]
        private string loadingCompleteText = "Loading Complete...";
        [SerializeField, Tooltip("The text that shows below the indicator when the loading process has failed")]
        private string loadingFailedText = "Loading Failed...";
        [SerializeField, Tooltip("Time in seconds for loading indicator to go from 1->0 opacity")]
        private float fadeDuration = 0.75f;
        [SerializeField, Tooltip("Dimmed if true, Opaque if false")] 
        private bool isBackgroundDimmed = false;

        [Header("Event Hook-Ins")]
        [Tooltip("Invoked when the loading indicator is shown")]
        public UnityEvent OnLoadingShown;
        [Tooltip("Invoked when the loading indicator is hidden")]
        public UnityEvent OnLoadingHidden;
        [Tooltip("Invoked when the loading indicator fails the loading process")]
        public UnityEvent OnLoadingFailed;
        
        public bool IsLoading { private set; get;  }
        public bool HasFailed { private set; get; }

        private RadialLoadingProgress loadingIndicator;
        private VisualElement loadingIndicatorFailedIcon;
        private VisualElement canvas;
        private Label loadingProgressLabel;
        private Label loadingTextLabel;

        private const string DimmedBackgroundClass = "loading-indicator-canvas-dimmed";
        private const string OpaqueBackgroundClass = "loading-indicator-canvas-opaque";
        private const float DefaultStartingValue = 0.0f;
        private const float DefaultEndingValue = 100.0f;
        private const float Opaque = 1.0f;
        private const float Transparent = 0.0f;
        private const float DelayDuration = 1.0f;

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            loadingIndicator = Root.Q<RadialLoadingProgress>();
            loadingIndicatorFailedIcon = Root.Q<VisualElement>("FailedIcon");
            canvas = Root.Q<VisualElement>("Canvas");
            loadingProgressLabel = Root.Q<Label>();
            loadingTextLabel = Root.Q<Label>("LoadingTextLabel");

            OnLoadingShown ??= new UnityEvent();
            OnLoadingHidden ??= new UnityEvent();
            OnLoadingFailed ??= new UnityEvent();

            Root.style.transitionDuration = new List<TimeValue>() { new TimeValue(fadeDuration) };
            HasFailed = false;
            Root.Hide();
        }

        /// <summary>
        /// Intended to be a single access public method to set the loading indicator starting value,
        /// start the coroutine that begins the counter, and display it
        /// </summary>
        /// <param name="startValue"></param>
        public void HandleDisplayUI(float startValue = DefaultStartingValue)
        {
            ResetLoadingIndicator();
            SetContent(startValue);
            StartCoroutine(Load());
            Show();
        }

        /// <summary>
        /// There are 2 different customizations for the loading indicator:
        ///     - Dimmed: Semi transparent dimmed background that matches the settings menu, info dialog, etc. backgrounds
        ///     - Opaque: Solid background to hide scene initialization that matches default light/dark mode colours
        ///     
        /// Sets the load indicator progress, desired canvas style, and the loading text to loadingText
        /// </summary>
        /// <param name="startValue"></param>
        public void SetContent(float startValue)
        {
            string desiredCanvasClass = isBackgroundDimmed ? DimmedBackgroundClass : OpaqueBackgroundClass;
            canvas.EnableInClassList(desiredCanvasClass, true);
            loadingTextLabel.SetElementText(loadingText);
            loadingIndicator.Progress = startValue;
            IsLoading = true;
        }

        /// <summary>
        /// Starts the Load() coroutine of the loading indicator
        /// </summary>
        public void StartLoad()
        {
            StartCoroutine(Load());
        }

        /// <summary>
        /// Starts incrementing the progress indicator's progress and calls SetComplete() if progress = 100
        /// </summary>
        public IEnumerator Load()
        {
            while (IsLoading)
            {
                if (loadingIndicator.Progress >= DefaultEndingValue)
                {
                    SetComplete();
                }

                loadingIndicator.Progress += Time.deltaTime * Random.Range(2f, 30f);
                yield return null;
            }
        }

        /// <summary>
        /// Stops the progress indicator from incrementing, and sets progress to 100. Calls
        /// the FadeOut() coroutine, and changes loading text to loadingCompleteText
        /// </summary>
        public void SetComplete()
        {
            loadingTextLabel.SetElementText(loadingCompleteText);
            loadingIndicator.Progress = DefaultEndingValue;
            StartCoroutine(FadeOut());
            IsLoading = false;
        }

        /// <summary>
        /// Stops the progress indicator from incrementing, toggles USS classes on the loading 
        /// indicator to set the bar red, hide progress to display an error icon, and hide the 
        /// current progress amount (%). Sets loading text to loadingFailedText
        /// </summary>
        public void SetFailed()
        {
            HasFailed = true;
            IsLoading = false;

            loadingIndicator.AddToClassList(RadialLoadingProgress.RadialFailedClass);
            loadingIndicatorFailedIcon.AddToClassList(RadialLoadingProgress.RadialFailedIconClass);
            loadingIndicator.Progress = DefaultStartingValue;

            loadingProgressLabel.Hide();
            loadingTextLabel.SetElementText(loadingFailedText);
            OnLoadingFailed.Invoke();
        }

        /// <summary>
        /// Resets all values of the loading indicator, and stops all currently running coroutines. Failed
        /// USS classes are removed, progress text is shown, HasFailed/IsLoading is reset, sets Root to None,
        /// and sets opacity to 1.
        /// </summary>
        public void ResetLoadingIndicator()
        {
            StopAllCoroutines();
            HasFailed = false;
            IsLoading = false;

            loadingIndicator.EnableInClassList(RadialLoadingProgress.RadialFailedClass, false);
            loadingIndicatorFailedIcon.EnableInClassList(RadialLoadingProgress.RadialFailedIconClass, false);
            loadingIndicator.Progress = DefaultStartingValue;
            Root.style.opacity = Opaque;

            canvas.EnableInClassList(OpaqueBackgroundClass, true);
            loadingProgressLabel.Show();
            Root.Hide();
        }

        /// <summary>
        /// The loading indicator will fade out based on the properties serialized in the script. On exit, the 
        /// loading indicator goes from 1 to 0 opacity in fadeDuration seconds
        /// </summary>
        public IEnumerator FadeOut()
        {
            //Wait 1 second before starting fade so the 100% & complete message is readable
            yield return new WaitForSeconds(DelayDuration);
            Root.style.opacity = Transparent;
            yield return new WaitForSeconds(fadeDuration);
            Hide();
        }

        /// <summary>
        /// Shows the root of the loading indicator and triggers OnLoadingShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnLoadingShown?.Invoke();
        }

        /// <summary>
        /// Hides the root of the loading indicator and triggers OnLoadingHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnLoadingHidden?.Invoke();
        }
    }
}