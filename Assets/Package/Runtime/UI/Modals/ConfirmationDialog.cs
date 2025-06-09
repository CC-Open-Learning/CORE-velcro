using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    ///  Confirmation dialogs have a few different customizations:
    ///     - Close Button: The close button "X" in the top right can be enabled or disabled
    ///     - Canvas Dim: The background of the confirmation dialog can be dimmed to bring focus to the UI and block interaction with other UI
    ///     - Primary Button: The primary button type can use any ButtonType.cs template. The primary button will be the right most button
    ///     - Secondary Button: The secondary button type can use any ButtonType.cs template. The primary button will be the left most button
    /// 
    /// Intended use of this class is to connect ConfirmationDialog.HandleDisplayUI() to a UnityEvent in your DLX. Methods changing
    /// content, canvas dim, and show/hide have been exposed if DLX would prefer to serialize and control individual stages of
    /// the confirmation dialog
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class ConfirmationDialog : MonoBehaviour, IUserInterface
    {
        [Header("Starting Values"), Space(10f)]
        [Tooltip("The button templates linked to the button types.")]
        [SerializeField] private List<VisualTreeAsset> buttonTemplates;

        [HideInInspector] public VisualElement Root { private set; get; }

        [Header("Event Hook-Ins")]
        public UnityEvent OnDialogShown;
        public UnityEvent OnDialogHidden;
        public UnityEvent OnPrimaryBtnClicked;
        public UnityEvent OnSecondaryBtnClicked;

        private VisualElement buttonContainer;
        private VisualElement canvas;
        private Label nameLabel;
        private Label descriptionLabel;
        private Button closeBtn;
        private Button primaryBtn;
        private Button secondaryBtn;

        private const string DimmedBackgroundClass = "confirmation-dialog-canvas";

        private const string BoldUssClass = "fw-700";
        private const string RegularUssClass = "fw-400";
        private const string CancelButtonMarginClass = "mr-20";

        private void Start()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            buttonContainer = Root.Q<VisualElement>("ButtonContainer");
            canvas = Root.Q<VisualElement>("Canvas");

            nameLabel = Root.Q<Label>("NameLabel");
            descriptionLabel = Root.Q<Label>("DescriptionLabel");

            closeBtn = Root.Q<Button>("CloseBtn");

            OnDialogShown ??= new UnityEvent();
            OnDialogHidden ??= new UnityEvent();
            OnPrimaryBtnClicked ??= new UnityEvent();
            OnSecondaryBtnClicked ??= new UnityEvent();

            closeBtn.clicked += () =>
            {
                Hide();
            };

            Root.Hide();
        }

        /// <summary>
        /// This method is intended to be a single access public method to populate the confirmation dialog with text, 
        /// buttons, and display it
        /// </summary>
        /// <param name="confirmationDialogSO"></param>
        public void HandleDisplayUI(ConfirmationDialogSO confirmationDialogSO)
        {
            SetContent(confirmationDialogSO);
            Show();
        }

        /// <summary>
        /// Confirmation dialogs have a few different customizations:
        ///     - Close Button: The close button "X" in the top right can be enabled or disabled
        ///     - Canvas Dim: The background of the confirmation dialog can be dimmed to bring focus to the UI and block interaction with other UI
        ///     - Primary Button: The primary button type can use any ButtonType.cs template. The primary button will be the right most button
        ///     - Secondary Button: The secondary button type can use any ButtonType.cs template. The primary button will be the left most button
        /// </summary>
        /// <param name="confirmationDialogSO"></param>
        public void SetContent(ConfirmationDialogSO confirmationDialogSO)
        {
            ClearButtons();
            ClearClasses();

            string fontClass = confirmationDialogSO.IsDescriptionBolded ? BoldUssClass : RegularUssClass;
            descriptionLabel.EnableInClassList(fontClass, true);

            nameLabel.SetElementText(confirmationDialogSO.Name);
            descriptionLabel.SetElementText(confirmationDialogSO.Description);
            canvas.EnableInClassList(DimmedBackgroundClass, confirmationDialogSO.IsBackgroundDimmed);

            AddSecondaryButton(confirmationDialogSO.SecondaryBtnType, confirmationDialogSO.SecondaryBtnText);
            AddPrimaryButton(confirmationDialogSO.PrimaryBtnType, confirmationDialogSO.PrimaryBtnText);
            SetCloseButton(confirmationDialogSO.IsCloseBtnVisible);
        }

        /// <summary>
        /// Sets the closeBtn to DisplayStyle.Flex or DisplayStyle.None depending on incoming isCloseBtnVisible
        /// </summary>
        /// <param name="isCloseBtnVisible"></param>
        public void SetCloseButton(bool isCloseBtnVisible)
        {
            if (isCloseBtnVisible)
            {
                closeBtn.Show();
            }
            else
            {
                closeBtn.Hide();
            }
        }

        /// <summary>
        /// Shows the root of the confirmation dialog and triggers OnDialogShown
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnDialogShown.Invoke();
        }

        /// <summary>
        /// Hides the root of the confirmation dialog and triggers OnDialogHidden
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnDialogHidden.Invoke();
        }

        /// <summary>
        /// Adds a primary button to the confirmation dialog that matches the incoming ButtonType. 
        /// This uses VELCRO button template uxml files
        /// </summary>
        /// <param name="buttonType"></param>
        /// <param name="buttonText"></param>
        private void AddPrimaryButton(ButtonType buttonType, string buttonText)
        {
            VisualElement newButton = CreateButton(buttonType, buttonText);
            primaryBtn = newButton.Q<Button>();
            primaryBtn.RegisterCallback<ClickEvent>(PrimaryBtnClicked);
        }

        /// <summary>
        /// Adds a secondary button to the confirmation dialog that matches the incoming ButtonType. 
        /// This uses VELCRO button template uxml files
        /// </summary>
        /// <param name="buttonType"></param>
        /// <param name="buttonText"></param>
        private void AddSecondaryButton(ButtonType buttonType, string buttonText)
        {
            VisualElement newButton = CreateButton(buttonType, buttonText);
            newButton.AddToClassList(CancelButtonMarginClass);
            secondaryBtn = newButton.Q<Button>();
            secondaryBtn.RegisterCallback<ClickEvent>(SecondaryBtnClicked);
        }

        /// <summary>
        /// Clones a uxml template matching incoming ButtonType and returns the cloned Button. Internal 
        /// method for use with AddPrimaryButton/AddSecondaryButton methods
        /// </summary>
        /// <param name="buttonType"></param>
        /// <param name="buttonText"></param>
        /// <returns></returns>
        private VisualElement CreateButton(ButtonType buttonType, string buttonText)
        {
            VisualTreeAsset template = buttonTemplates[(int)buttonType];
            VisualElement buttonClone = template.CloneTree();
            Button button = buttonClone.Q<Button>();
            button.SetElementText(buttonText);
            buttonContainer.Add(buttonClone);
            return buttonClone;
        }

        /// <summary>
        /// Clears all font weight classes on the description label
        /// </summary>
        public void ClearClasses()
        {
            descriptionLabel.EnableInClassList(BoldUssClass, false);
            descriptionLabel.EnableInClassList(RegularUssClass, false);
        }

        /// <summary>
        /// Removes any existing buttons in the #ButtonContainer element
        /// </summary>
        private void ClearButtons()
        {
            primaryBtn?.UnregisterCallback<ClickEvent>(PrimaryBtnClicked);
            secondaryBtn?.UnregisterCallback<ClickEvent>(SecondaryBtnClicked);
            buttonContainer?.Clear();
        }

        /// <summary>
        /// Hides the root of the confirmation dialog and triggers OnPrimaryBtnClicked
        /// </summary>
        /// <param name="evt"></param>
        private void PrimaryBtnClicked(ClickEvent evt)
        {
            Hide();
            OnPrimaryBtnClicked.Invoke();
        }

        /// <summary>
        /// Hides the root of the confirmation dialog and triggers OnSecondaryBtnClicked
        /// </summary>
        /// <param name="evt"></param>
        private void SecondaryBtnClicked(ClickEvent evt)
        {
            Hide();
            OnSecondaryBtnClicked.Invoke();
        }

        private void OnDestroy()
        {
            ClearButtons();
        }
    }
}