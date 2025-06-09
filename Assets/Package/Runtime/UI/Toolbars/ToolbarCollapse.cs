using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Collapsable Toolbars have a few different customizations:
    ///     - Icons for the options starting from top to bottom
    ///     - Labels for the first three options
    ///     
    /// Intended use of this class is to connect ToolbarCollapse.HandleDisplayUI() to a UnityEvent in your DLX. Methods 
    /// show/hide have been exposed if DLX would prefer to serialize and control 
    /// individual stages of the toolbar
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ToolbarCollapse : Toolbar
    {
        private const string ClosedClass = "toolbar-collapse-closed";
        private const string OpenClass = "toolbar-collapse";
        private const string SelectedClass = "toolbar-collapse-item-group-selected";
        private const float AnimationSeconds = 0.15f;
        private const float StartPosition = 0f;
        private const float TranslateClose = -175f;

        private Button lastBtn = null; /* Used when tracking which button should be highlighted */

        [Header("Toolbar Collapse Attributes")]
        [SerializeField]
        [Tooltip("The UXML template that represents the simple button of the toolbar.")]
        private VisualTreeAsset simpleButtonTemplate;

        [Header("Action Events"), Space(4f)]
        [Tooltip("Add your own UnityEvent here for when the toolbar is expanded.")]
        public UnityEvent OnToolbarExpand;
        [Tooltip("Add your own UnityEvent here for when the toolbar is collapsed.")]
        public UnityEvent OnToolbarCollapse;

        private VisualElement simpleButtonContainer;
        private Button contractBtn;
        private Button expandBtn;
        private VisualElement seperator;

        protected override void Start()
        {
            base.Start();

            OnToolbarExpand ??= new UnityEvent();
            OnToolbarCollapse ??= new UnityEvent();

            toolbar = Root.Q("Toolbar");
            buttonsContainer = toolbar.Q("ComplexButtons");
            simpleButtonContainer = toolbar.Q("SimpleButtons");

            toolbar.AddToClassList(ClosedClass);
            toolbar.AddToClassList(OpenClass);
            toolbar.EnableInClassList(ClosedClass, false); // Disables closed class so that default state is open
            toolbar.EnableInClassList(OpenClass, true);

            contractBtn = toolbar.Q<Button>("ContractButton");
            expandBtn = toolbar.Q<Button>("ExpandButton");
            seperator = toolbar.Q("Seperator");

            contractBtn.RegisterCallback<ClickEvent>(OnContractButtonClick);
            expandBtn.RegisterCallback<ClickEvent>(OnExpandButtonClick);
        }

        /// <summary>
        /// Adds a new button to the toolbar.
        /// </summary>
        /// <param name="icon">The icon of the new button.</param>
        /// <param name="text">The text of the new button.</param>
        public override void AddButton(Sprite icon, string text)
        {
            ToolbarButton tb = new ToolbarButton(icon, text);
            buttons.Add(tb);

            VisualElement newButton;
            if (string.IsNullOrEmpty(text))
            {
                newButton = simpleButtonTemplate.CloneTree();
                simpleButtonContainer.Add(newButton);
                seperator.Show();
            }
            else
            {
                newButton = buttonTemplate.CloneTree();
                buttonsContainer.Add(newButton);

                // Complex buttons have labels while simple ones do not.
                Label label = newButton.Q<Label>("Text");
                label.SetElementText(text);
                tb.textElement = label;
            }

            tb.root = newButton;

            VisualElement iconElement = newButton.Q("Image");
            iconElement.SetElementSprite(tb.Icon);
            tb.iconElement = iconElement;

            tb.OnClick ??= new UnityEvent();

            Button button = newButton.Q<Button>("Button");
            button.RegisterCallback<ClickEvent>((e) =>
            {
                OnGroupButtonClick(e);
                tb.OnClick?.Invoke();
            });
        }

        /// <summary>
        /// Removes all buttons from the toolbar.
        /// </summary>
        public override void ClearButtons()
        {
            base.ClearButtons();
            simpleButtonContainer.Clear();
            seperator.Hide();
        }

        /// <summary>
        /// Used as a general function for all group buttons, which covers the icons and text used for each area of the item bar portion of the toolbar.
        /// </summary>
        /// <param name="cl">Event containing target button</param>
        private void OnGroupButtonClick(ClickEvent cl)
        {
            Button curBtn = cl.target as Button;
            if (lastBtn != null)
            {
                lastBtn.EnableInClassList(SelectedClass, false);
                lastBtn.RemoveFromClassList(SelectedClass);
                VisualElement oldSelBar = lastBtn.Q("SelectedBar");
                oldSelBar.visible = false;
            }
            curBtn.AddToClassList(SelectedClass);
            curBtn.EnableInClassList(SelectedClass, true);
            VisualElement newSelBar = curBtn.Q("SelectedBar");
            newSelBar.visible = true; 
            lastBtn = curBtn;
        }

        /// <summary>
        /// Used for the contract button when the item bar is shown.
        /// </summary>
        /// <param name="cl">Event containing target button</param>
        private void OnContractButtonClick(ClickEvent cl)
        {
            Collapse();
        }

        /// <summary>
        /// Triggers the animation to bring the toolbar inwards and enables the closed class.
        /// </summary>
        /// <returns></returns>
        private IEnumerator TranslateIn()
        {
            ToggleTransitionProperties(TranslateClose);
            yield return new WaitForSeconds(AnimationSeconds);
            toolbar.EnableInClassList(OpenClass, false);
            toolbar.EnableInClassList(ClosedClass, true);
            OnToolbarCollapse?.Invoke();
        }

        /// <summary>
        /// Used for the expand button when the toolbar is 'put away' (not being shown).
        /// </summary>
        /// <param name="cl">Event containing target button</param>
        private void OnExpandButtonClick(ClickEvent cl)
        {
            Expand();
        }

        /// <summary>
        /// Triggers the animation which brings the toolbar outward and enables the open class.
        /// </summary>
        /// <returns></returns>
        private IEnumerator TranslateOut()
        {
            toolbar.EnableInClassList(OpenClass, true);
            toolbar.EnableInClassList(ClosedClass, false);
            ToggleTransitionProperties(StartPosition);
            yield return new WaitForSeconds(AnimationSeconds);
            OnToolbarExpand?.Invoke();
        }

        /// <summary>
        /// applies the translation property for the desired pixel distance and direction on the x plane, and sets the length of time in seconds which the translation
        /// takes place over.
        /// </summary>
        /// <param name="translateDirection">The direction and pixel count which to translate in the x direction.</param>
        private void ToggleTransitionProperties(float translateDirection)
        {
            toolbar.style.transitionDuration = new List<TimeValue>()
            {
                new TimeValue(AnimationSeconds, TimeUnit.Second)
            };
            toolbar.style.translate = new Translate(new Length(translateDirection), new Length(StartPosition));
        }

        /// <summary>
        /// Sets the toolbar in its expand state.
        /// Triggers the <see cref="OnToolbarExpand"/> event.
        /// </summary>
        public void Expand()
        {
            StartCoroutine(TranslateOut());
        }

        /// <summary>
        /// Sets the toolbar in its collapsed state.
        /// Triggers the <see cref="OnToolbarCollapse"/> event.
        /// </summary>
        public void Collapse()
        {
            StartCoroutine(TranslateIn());
        }
    }
}