using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    /// <summary>
    /// Base class that all toolbars inherit from.
    /// Contains all the required methods a toolbar needs.
    /// </summary>
    public abstract class Toolbar : MonoBehaviour, IUserInterface
    {
        /// <summary>
        /// The root visual element of the toolbar.
        /// </summary>
        [HideInInspector]
        public VisualElement Root { get; private set; }

        [Header("Main")]
        [SerializeField]
        [Tooltip("The button UXML template of the toolbar.")]
        protected VisualTreeAsset buttonTemplate;

        [Header("Base Events"), Space(4f)]
        [Tooltip("Invoked when the toolbar is shown to the screen.")]
        public UnityEvent OnToolbarShown;

        [Tooltip("Invoked when the toolbar is hidden from the screen.")]
        public UnityEvent OnToolbarHidden;

        protected VisualElement toolbar;
        protected VisualElement buttonsContainer;

        protected List<ToolbarButton> buttons;

        /// <summary>
        /// Collection of buttons contained inside the toolbar.
        /// </summary>
        public IEnumerable<ToolbarButton> Buttons => buttons;

        /// <summary>
        /// The number of buttons inside the toolbar.
        /// </summary>
        public int ButtonCount => buttons.Count;

        protected virtual void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            Root = document.rootVisualElement;

            OnToolbarShown ??= new UnityEvent();
            OnToolbarHidden ??= new UnityEvent();

            buttons = new List<ToolbarButton>();

            Root.Hide();
        }

        /// <summary>
        /// Displays and sets the toolbar's content.
        /// </summary>
        /// <param name="toolbarSO"></param>
        public void HandleDisplayUI(ToolbarSO toolbarSO)
        {
            SetContent(toolbarSO);
            Show();
        }

        /// <summary>
        /// Sets the content of the toolbar via scriptable object containing a list of buttons.
        /// </summary>
        /// <param name="toolbarSO"></param>
        public void SetContent(ToolbarSO toolbarSO)
        {
            ClearButtons();

            if (toolbarSO == null || toolbarSO.Buttons == null)
            {
                return;
            }

            foreach (ToolbarButton tb in toolbarSO.Buttons)
            {
                AddButton(tb.Icon, tb.Text);
            }
        }

        /// <summary>
        /// Adds a new button to the toolbar.
        /// </summary>
        /// <param name="icon">The icon of the new button.</param>
        /// <param name="text">The text of the new button.</param>
        public abstract void AddButton(Sprite icon, string text);

        /// <summary>
        /// Removes a button from the toolbar at the specified index.
        /// </summary>
        /// <param name="index">The index of the button inside the toolbar.</param>
        public void RemoveButton(int index)
        {
            ToolbarButton tb = buttons.ElementAtOrDefault(index);
            if (tb == null)
            {
                Debug.LogError($"Unable to remove toolbar button at invalid index {index}");
                return;
            }

            buttons.RemoveAt(index);
            tb.root.RemoveFromHierarchy();
        }

        /// <summary>
        /// Clears all buttons contained inside the toolbar.
        /// </summary>
        public virtual void ClearButtons()
        {
            buttons.Clear();
            buttonsContainer?.Clear();
        }

        /// <summary>
        /// Shows the toolbar.
        /// Triggers the <see cref="OnToolbarShown"/> event.
        /// </summary>
        public void Show()
        {
            Root.Show();
            OnToolbarShown?.Invoke();
        }

        /// <summary>
        /// Hides the toolbar.
        /// Triggers the <see cref="OnToolbarHidden"/> event.
        /// </summary>
        public void Hide()
        {
            Root.Hide();
            OnToolbarHidden?.Invoke();
        }
    }
}