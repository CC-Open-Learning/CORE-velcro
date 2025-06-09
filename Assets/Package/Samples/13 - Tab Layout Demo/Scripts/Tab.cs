using System.Collections.Generic;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class Tab
    {
        public Button TabButton;
        public List<Tab> Tabs;
        public VisualElement TabContent;
        private TabType tabType;

        private const string HorizontalButtonSelectedClass = "navigation-horizontal-button-selected";
        private const string VerticalButtonSelectedClass = "navigation-vertical-button-selected";

        private const string FontRegular = "fw-400";
        private const string FontBold = "fw-700";

        public Tab(Button btn, VisualElement tabContent, TabType tabType)
        {
            TabButton = btn;
            this.TabContent = tabContent;
            this.tabType = tabType;
            btn.clicked += TabClicked;
        }

        public void TabClicked()
        {
            UnselectAllTabs();

            // Turn on select tab for this button
            foreach (Tab tab in Tabs)
            {
                if (tab.TabButton == TabButton)
                {
                    string expectedStyleClass = tabType == TabType.Horizontal ? HorizontalButtonSelectedClass : VerticalButtonSelectedClass;

                    tab.TabButton.AddToClassList(expectedStyleClass);
                    tab.TabButton.Q<Label>().RemoveFromClassList(FontRegular);
                    tab.TabButton.Q<Label>().AddToClassList(FontBold);
                    tab.TabButton.SetEnabled(false);
                    tab.TabContent.style.display = DisplayStyle.Flex;
                }
            }
        }

        private void UnselectAllTabs()
        {
            // Turn off selected styling
            foreach (Tab tab in Tabs)
            {
                string expectedStyleClass = tabType == TabType.Horizontal ? HorizontalButtonSelectedClass : VerticalButtonSelectedClass;

                tab.TabButton.RemoveFromClassList(expectedStyleClass);
                tab.TabButton.Q<Label>().RemoveFromClassList(FontBold);
                tab.TabButton.Q<Label>().AddToClassList(FontRegular);
                tab.TabButton.SetEnabled(true);
                tab.TabContent.style.display = DisplayStyle.None;
            }
        }
    }
}