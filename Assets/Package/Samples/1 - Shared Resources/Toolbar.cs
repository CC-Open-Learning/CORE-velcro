using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    /// <summary>
    /// This script and the associated UI Document is for the sample scene only. It is not intended to be pulled
    /// into any DLX besides for learning and testing purposes
    /// </summary>
    public class Toolbar : MonoBehaviour
    {
        [SerializeField] protected PanelSettings panelSettings;
        [SerializeField] protected ThemeStyleSheet darkTheme;
        [SerializeField] protected ThemeStyleSheet lightTheme;

        [HideInInspector] public VisualElement Root { private set; get; }

        protected void SetupBaseToolbar ()
        {
            Root = gameObject.GetComponent<UIDocument>().rootVisualElement;

            Button toggleThemeBtn = Root.Q<Button>("ThemeToggle");
            toggleThemeBtn.clicked += () =>
            {
                if (panelSettings.themeStyleSheet == darkTheme)
                {
                    ToggleTheme(true);
                }
                else
                {
                    ToggleTheme(false);
                }
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleTheme(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ToggleTheme(true);
            }
        }

        public void ToggleTheme(bool enable)
        {
            panelSettings.themeStyleSheet = enable ? lightTheme : darkTheme;
        }
    }
}