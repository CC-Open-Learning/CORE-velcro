using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Tutorials only have one customization available:
    ///     - Canvas Dim: The background of the tutorial can be dimmed to bring focus to the UI and block interaction with other UI
    ///     
    /// Intended use of this class is to connect Tutorial.HandleDisplayUI() to a UnityEvent in your DLX. Methods changing
    /// content, canvas dim, and show/hide have been exposed if DLX would prefer to serialize and control individual stages of
    /// the tutorial
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class Tutorial : MonoBehaviour, IUserInterface
    {
        [Tooltip("The button template for the carousel indicators (Clickable circles)")]
        [SerializeField] private VisualTreeAsset carouselIndicatorTemplate;

        [Tooltip("Content of the tutorial")]
        [SerializeField] private TutorialSO tutorialSO;

        [HideInInspector] public VisualElement Root { private set; get; }

        [Header("Event Hook-Ins")]
        public UnityEvent OnTutorialShown;
        public UnityEvent OnTutorialHidden;
        public UnityEvent OnTutorialForward;
        public UnityEvent OnTutorialBackward;
        public UnityEvent OnTutorialComplete;
        public UnityEvent<int> OnTutorialJump;

        private VisualElement canvas;
        private VisualElement indicatorContainer;
        private VisualElement image;
        private Label nameLabel;
        private Label headerLabel;
        private Label descriptionLabel;
        private Label stepMinLabel;
        private Label stepMaxLabel;
        private Button nextBtn;
        private Button previousBtn;
        private Button skipBtn;

        private VisualElement currentActiveIndicator;
        private VisualElement previousActiveIndicator;
        private int currentIndex = StartingIndex;

        private const string ActiveCarouselIndicatorClass = "tutorial-button-carousel-active";
        private const string DimmedBackgroundClass = "tutorial-canvas";
        private const int StartingIndex = 0;

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            canvas = Root.Q<VisualElement>("Canvas");
            indicatorContainer = Root.Q<VisualElement>("IndicatorContainer");
            image = Root.Q<VisualElement>("Image");
            nameLabel = Root.Q<TemplateContainer>("Header").Q<Label>("NameLabel");
            headerLabel = Root.Q<Label>("HeaderLabel");
            descriptionLabel = Root.Q<Label>("DescriptionLabel");
            stepMinLabel = Root.Q<Label>("StepMinLabel");
            stepMaxLabel = Root.Q<Label>("StepMaxLabel");

            nextBtn = Root.Q<TemplateContainer>().Q<Button>("ButtonTutorialPrimary");
            previousBtn = Root.Q<TemplateContainer>().Q<Button>("ButtonTutorialSecondary");
            skipBtn = Root.Q<TemplateContainer>().Q<Button>("ButtonTutorialSkip");

            OnTutorialShown ??= new UnityEvent();
            OnTutorialHidden ??= new UnityEvent();
            OnTutorialForward ??= new UnityEvent();
            OnTutorialBackward ??= new UnityEvent();
            OnTutorialComplete ??= new UnityEvent();
            OnTutorialJump ??= new UnityEvent<int>();

            nextBtn.clicked += () =>
            {
                AdvanceTutorial();
                OnTutorialForward?.Invoke();
            };

            previousBtn.clicked += () =>
            {
                ReverseTutorial();
                OnTutorialBackward?.Invoke();
            };

            skipBtn.clicked += () =>
            {
                Hide();
                OnTutorialComplete?.Invoke();
            };

            Root.Hide();
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the tutorial with text, 
        /// buttons, and display it
        /// </summary>
        /// <param name="tutorialSO"></param>
        public void HandleDisplayUI()
        {
            ResetTutorial();
            SetContent();
            Show();
        }

        /// <summary>
        /// This method populates all content of the tutorial. The [Name] label, and button text is populated, then the carousel indicators 
        /// and section content is populated for the first index in the TutorialSections list
        /// 
        /// Tutorials only have one customization available:
        ///     - Canvas Dim: The background of the tutorial can be dimmed to bring focus to the UI and block interaction with other UI
        /// </summary>
        public void SetContent()
        {
            Label previousBtnLabel = previousBtn.Q<Label>();

            nameLabel.SetElementText(tutorialSO.Name);
            stepMaxLabel.SetElementText(tutorialSO.TutorialSections.Count.ToString());
            previousBtnLabel.SetElementText(tutorialSO.SecondaryBtnText);
            skipBtn.SetElementText(tutorialSO.TertiaryBtnText);
            canvas.EnableInClassList(DimmedBackgroundClass, tutorialSO.IsBackgroundDimmed);

            PopulateCarouselIndicators();
            PopulateTutorialSection(StartingIndex);
            ChangePreviousButtonState();
        }

        /// <summary>
        /// Populates the tutorial section with the current [Header]/[Description] label and [Image] VisualElement
        /// </summary>
        /// <param name="sectionIndex"></param>
        public void PopulateTutorialSection(int sectionIndex)
        {
            if (sectionIndex < StartingIndex || sectionIndex >= tutorialSO.TutorialSections.Count)
            {
                Debug.LogError("Tutorial.PopulateTutorialSection() - Out of Bounds sectionIndex!");
                return;
            }
            
            Label nextBtnLabel = nextBtn.Q<Label>();
            
            stepMinLabel.SetElementText((sectionIndex + 1).ToString());
            headerLabel.SetElementText(tutorialSO.TutorialSections[sectionIndex].Header);
            descriptionLabel.SetElementText(tutorialSO.TutorialSections[sectionIndex].Description);
            image.SetElementSprite(tutorialSO.TutorialSections[sectionIndex].Image);
            nextBtnLabel.SetElementText(tutorialSO.TutorialSections[sectionIndex].PrimaryBtnText);
        }

        /// <summary>
        /// Populates the clickable carousel indicators under the image. The template is cloned List<TutorialSections>.Count times 
        /// and index matches up with the list in the SO. First carousel indicator is set to active by default
        /// </summary>
        public void PopulateCarouselIndicators()
        {
            //Create one btn per tutorial section and set up click event to jump to that page
            for (int i = 0; i < tutorialSO.TutorialSections.Count; i++)
            {
                VisualElement template = carouselIndicatorTemplate.CloneTree();
                indicatorContainer.Add(template);

                Button button = template.Q<Button>();
                button.tooltip = i.ToString();
                button.clicked += () =>
                {
                    //Store button's sectionIndex in the tooltip so it can be used for the click event
                    JumpToIndex(Int32.Parse(button.tooltip));
                };
            }

            //Set the first btn as the active button by default. The USS class will change the background colour
            currentActiveIndicator = indicatorContainer.ElementAt(StartingIndex);
            currentActiveIndicator.Q<Button>().EnableInClassList(ActiveCarouselIndicatorClass, true);
        }

        /// <summary>
        /// Sets the current and previous active carousel indicators so the newly clicked one has the active styling
        /// </summary>
        public void ChangeActiveCarouselIndicators(int sectionIndex)
        {
            //Swap the current and previous before changing style classes which changes background colour
            previousActiveIndicator = currentActiveIndicator;
            currentActiveIndicator = indicatorContainer.ElementAt(sectionIndex);

            currentActiveIndicator.Q<Button>().EnableInClassList(ActiveCarouselIndicatorClass, true);
            previousActiveIndicator.Q<Button>().RemoveFromClassList(ActiveCarouselIndicatorClass);
        }

        /// <summary>
        /// Sets the display style of the previous button based on the currentIndex. If the index is > 0, 
        /// then the previous button will be shown, otherwise it will be hidden
        /// </summary>
        /// <param name="sectionIndex"></param>
        public void ChangePreviousButtonState()
        {
            if (currentIndex > StartingIndex)
            {
                previousBtn.Show();
            }
            else
            {
                previousBtn.Hide();
            }
        }

        /// <summary>
        /// Moves the tutorial to the desired page index without needing to go one by one forwards, or backwards. 
        /// The active styling for the currently selected button is updated. Invokes OnTutorialJump<int>
        /// </summary>
        /// <param name="sectionIndex"></param>
        public void JumpToIndex(int sectionIndex)
        {
            currentIndex = sectionIndex;

            ChangePreviousButtonState();
            ChangeActiveCarouselIndicators(sectionIndex);
            PopulateTutorialSection(sectionIndex);

            OnTutorialJump?.Invoke(sectionIndex);
        }

        /// <summary>
        /// Advances the tutorial to the next page index. This re-populates the content, and changes 
        /// the active carousel indicator
        /// </summary>
        public void AdvanceTutorial()
        {
            if (currentIndex == (tutorialSO.TutorialSections.Count - 1))
            {
                Hide();
                OnTutorialComplete?.Invoke();
                return;
            }

            currentIndex++;
            ChangePreviousButtonState();
            ChangeActiveCarouselIndicators(currentIndex);
            PopulateTutorialSection(currentIndex);
            OnTutorialForward?.Invoke();
        }

        /// <summary>
        /// Reverses the tutorial to the previous page index. This re-populates the content, and changes 
        /// the active carousel indicator
        /// </summary>
        public void ReverseTutorial()
        {
            if (currentIndex == StartingIndex)
            {
                Debug.LogWarning("Tutorial.ReverseTutorial() - Already at First Tutorial Section Index!");
                return;
            }

            currentIndex--;
            ChangePreviousButtonState();
            ChangeActiveCarouselIndicators(currentIndex);
            PopulateTutorialSection(currentIndex);
            OnTutorialBackward?.Invoke();
        }

        /// <summary>
        /// Resets the currentIndex, and clears the carouselIndicator container of all buttons. Labels, 
        /// and images will be overridden on next set
        /// </summary>
        private void ResetTutorial()
        {
            currentIndex = StartingIndex;
            indicatorContainer.Clear();
        }

        /// <summary>
        /// Shows the root of the tutorial and triggers OnTutorialShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnTutorialShown?.Invoke();
        }

        /// <summary>
        /// Hides the root of the tutorial and triggers OnTutorialHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnTutorialHidden?.Invoke();
        }
    }
}