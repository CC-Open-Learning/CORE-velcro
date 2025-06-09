using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class TabLayout : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset tabTemplate;
        [SerializeField] private Sprite tabSprite;
        [SerializeField] private TabType tabType;

        private VisualElement root;
        private VisualElement tabContainer;

        private List<Tab> tabs;
        private List<VisualElement> tabContent;
        private bool isUIDisplayed = false;

        [Space(10)]
        public UnityEvent OnTabLayoutShown;
        public UnityEvent OnTabLayoutHidden;        

        void Start()
        {
            GetReferences();
            SetUpTabList();

            //Always have tab one selected on start
            tabs[0].TabClicked();
            root.Hide();
        }

        private void GetReferences()
        {
            root = gameObject.GetComponent<UIDocument>().rootVisualElement;
            tabContainer = root.Q<TemplateContainer>().Q<VisualElement>("TabButtonContainer");

            tabContent = new List<VisualElement>
            {
                root.Q<VisualElement>("TabOne"),
                root.Q<VisualElement>("TabTwo"),
                root.Q<VisualElement>("TabThree")
            };
        }

        /// <summary>
        /// Clones a new tab template for each required tab, changes icon/label. Functionality is in Tab.cs
        /// </summary>
        private void SetUpTabList()
        {
            tabs = new List<Tab>();

            for (int i = 0; i < 3; i++)
            {
                VisualElement newTabTemplate = tabTemplate.CloneTree();
                Button tabButton = newTabTemplate.Q<Button>();
                Label tabLabel = newTabTemplate.Q<Label>("TabLabel");
                VisualElement tabIcon = newTabTemplate.Q<VisualElement>("Icon");

                if (tabType == TabType.Vertical)
                {
                    newTabTemplate.AddToClassList("mt-12");
                    newTabTemplate.AddToClassList("mb-12");
                }

                tabLabel.text = "Tab " + i.ToString();
                tabIcon.style.backgroundImage = new StyleBackground(tabSprite);
                tabs.Add(new Tab(tabButton, tabContent[i], tabType));

                tabs[i].TabContent.Hide();
                tabContainer.Add(newTabTemplate);
            }

            tabs[0].Tabs = tabs;
            tabs[1].Tabs = tabs;
            tabs[2].Tabs = tabs;
        }

        public void ToggleVisibility()
        {
            if (isUIDisplayed)
            {
                Hide();
            }
            else 
            {
                Show();
            }

            isUIDisplayed = !isUIDisplayed;
        }

        public void Show()
        {
            tabs[0].TabClicked();
            root.Show();
            OnTabLayoutShown?.Invoke();
        }

        public void Hide()
        {
            root.Hide();
            OnTabLayoutHidden?.Invoke();
        }
    }
}