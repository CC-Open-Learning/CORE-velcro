using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [RequireComponent(typeof(UIDocument))]
    public class InfoPopupSmall : MonoBehaviour, IUserInterface
    {
        [HideInInspector]
        public VisualElement Root { private set; get; }

        protected VisualElement infoPopup;
        private Label titleLabel;
        private VisualElement imageElement;

        /// <summary>
        /// Invoked when <see cref="Show"/> or <see cref="HandleDisplayUI"/> are called.
        /// </summary>
        [Header("Events"), Space(4f)]
        [Tooltip("Invoked when Show() or HandleDisplayUI() are called.")]
        public UnityEvent OnPopupShown;

        /// <summary>
        /// Invoked when <see cref="Hide"/> is called.
        /// </summary>
        [Tooltip("Invoked when Hide() is called.")]
        public UnityEvent OnPopupHidden;

        private void Awake()
        {
            UIDocument document = GetComponent<UIDocument>();
            Root = document.rootVisualElement;
            infoPopup = Root.Q("Popup");

            titleLabel = infoPopup.Q<Label>("TitleLabel");
            imageElement = infoPopup.Q("Image");

            Button closeButton = Root.Q<Button>("CloseBtn");
            closeButton.clicked += () =>
            {
                Hide();
            };

            OnPopupHidden ??= new UnityEvent();
            OnPopupShown ??= new UnityEvent();

            Root.Hide();
        }

        /// <summary>
        /// Calls SetContents to populate fields of info popup UXML, then calls Show to make UXML visible and trigger Show event
        /// </summary>
        /// <param name="titleText"></param>
        /// <param name="bodyImage"></param>
        public void HandleDisplayUI(string titleText, Sprite bodyImage, GameObject gameObject)
        {
            SetContent(titleText, bodyImage);
            Show();
            InfoPopupHelper.MovePopupBesideObject(Root, gameObject);
        }

        /// <summary>
        /// Override for HandleDIsplayUI which accepts an Info Popup Scriptable Object
        /// </summary>
        /// <param name="infoPopupSO"></param>
        public void HandleDisplayUI(InfoPopupSO infoPopupSO, GameObject gameObject)
        {
            SetContent(infoPopupSO.Title, infoPopupSO.Image);
            Show();
            InfoPopupHelper.MovePopupBesideObject(Root, gameObject);
        }

        /// <summary>
        /// Applies content variables passed in parameters to their respective
        /// fields inside the Small Info Popup UXML
        /// </summary>
        /// <param name="titleText"></param>
        /// <param name="bodyImage"></param>
        public void SetContent(string titleText, Sprite bodyImage)
        {
            titleLabel.SetElementText(titleText);
            imageElement.SetElementSprite(bodyImage);
        }

        /// <summary>
        /// Shows root element of Info Popup and triggers Show event
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnPopupShown?.Invoke();
        }

        /// <summary>
        /// Hides root element of Info Popup and triggers Hide event
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnPopupHidden?.Invoke();
        }
    }
}