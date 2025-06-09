using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// This monobehaviour class contains any reusable functionality from dialog UI elements, without changing the overall format of the modular components.
    /// </summary>
    public class BaseDialog : MonoBehaviour, IUserInterface
    {
        [HideInInspector] public VisualElement Root { private set; get; }
        private VisualElement canvas;

        [Header("Event Hook-Ins")]
        public UnityEvent OnDialogShown;
        public UnityEvent OnDialogHidden;

        private Label titleLabel;
        private Label nameLabel;
        private Label descriptionLabel;
        private Button primaryBtn;

        private string dimmedBackgroundClass;
        private const string BoldUssClass = "fw-700";
        private const string RegularUssClass = "fw-400";

        /// <summary>
        /// Uses the root passed in to initialize the baseline elements of a dialog UI
        /// </summary>
        public void InitializeDialog(string dimmedBackgroundClass)
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            canvas = Root.Q<VisualElement>("Canvas");

            titleLabel = Root.Q<Label>("TitleLabel");
            nameLabel = Root.Q<Label>("NameLabel");
            descriptionLabel = Root.Q<Label>("DescriptionLabel");
            primaryBtn = Root.Q<TemplateContainer>().Q<Button>("Button");

            OnDialogShown ??= new UnityEvent();
            OnDialogHidden ??= new UnityEvent();

            this.dimmedBackgroundClass = dimmedBackgroundClass;

            primaryBtn.clicked += () =>
            {
                Hide();
            };

            Root.Hide();
        }

        /// <summary>
        /// Sets the content for required components of a dialog UI.
        /// </summary>
        /// <param name="baseDialogSO">can be any scriptable object that inherits from baseDialogSO</param>
        public void SetContent(BaseDialogSO baseDialogSO)
        {
            ClearClasses();

            string fontClass = baseDialogSO.IsDescriptionBolded ? BoldUssClass : RegularUssClass;
            descriptionLabel.EnableInClassList(fontClass, true);

            canvas.EnableInClassList(dimmedBackgroundClass, baseDialogSO.IsBackgroundDimmed);

            nameLabel.SetElementText(baseDialogSO.Name);
            descriptionLabel.SetElementText(baseDialogSO.Description);
            titleLabel.SetElementText(baseDialogSO.Title);
            primaryBtn.SetElementText(baseDialogSO.PrimaryBtnText);
        }

        public void Show()
        {
            Root.Show();
            OnDialogShown?.Invoke();
        }

        public void Hide()
        {
            Root.Hide();
            OnDialogHidden?.Invoke();
        }

        /// <summary>
        /// Clears all font weight classes on the description label
        /// </summary>
        public void ClearClasses()
        {
            descriptionLabel.EnableInClassList(BoldUssClass, false);
            descriptionLabel.EnableInClassList(RegularUssClass, false);
        }
    }
}
