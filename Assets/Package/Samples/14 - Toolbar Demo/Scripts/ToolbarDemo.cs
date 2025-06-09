using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class ToolbarDemo : Toolbar
    {
        [SerializeField]
        private ToolbarCollapse toolbarCollapse;

        [SerializeField]
        private ToolbarStatic toolbarStatic;

        [SerializeField]
        private ToolbarSO toolbarCollapseDemoButtons;

        [SerializeField]
        private ToolbarSO toolbarStaticDemoButtons;

        private Button toolbarCollapseButton;
        private Button toolbarStaticButton;

        private void Start()
        {
            SetupBaseToolbar();

            toolbarCollapseButton = Root.Q<Button>("ToolbarCollapseToggle");
            toolbarStaticButton = Root.Q<Button>("ToolbarStaticToggle");

            toolbarCollapseButton.RegisterCallback<ClickEvent>(ShowToolbarCollapse);
            toolbarStaticButton.RegisterCallback<ClickEvent>(ShowToolbarStatic);
        }

        private void ShowToolbarCollapse(ClickEvent evt)
        {
            toolbarCollapse.HandleDisplayUI(toolbarCollapseDemoButtons);
            toolbarStatic.Hide();
        }

        private void ShowToolbarStatic(ClickEvent evt)
        {
            toolbarStatic.HandleDisplayUI(toolbarStaticDemoButtons);
            toolbarCollapse.Hide();
        }

        public void EventDemo()
        {
            Debug.Log("Event Triggered!");
        }

        private void OnDestroy()
        {
            toolbarCollapseButton?.UnregisterCallback<ClickEvent>(ShowToolbarCollapse);
            toolbarStaticButton?.UnregisterCallback<ClickEvent>(ShowToolbarStatic);
        }
    }
}