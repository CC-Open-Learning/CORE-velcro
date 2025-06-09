using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class MainMenuDemoController : Toolbar
    {
        public UnityEvent DisplayMenuSmall;
        public UnityEvent DisplayMenuLarge;
        public UnityEvent HideMenuSmall;
        public UnityEvent HideMenuLarge;

        private bool isSmallMenuVisible = false;
        private bool isLargeMenuVisible = false;

        private void Start()
        {
            SetupBaseToolbar();

            DisplayMenuSmall ??= new UnityEvent();
            DisplayMenuLarge ??= new UnityEvent();
            HideMenuSmall ??= new UnityEvent();
            HideMenuLarge ??= new UnityEvent();

            Button menuSmallBtn = Root.Q("ToggleMenuSmall").Q<Button>();
            menuSmallBtn.clicked += () =>
            {
                if (isSmallMenuVisible)
                {
                    HideMenuSmall?.Invoke();
                }
                else
                {
                    DisplayMenuSmall?.Invoke();
                }

                isSmallMenuVisible = !isSmallMenuVisible;
            };

            Button menuBigBtn = Root.Q("ToggleMenuLarge").Q<Button>();
            menuBigBtn.clicked += () =>
            {
                if (isLargeMenuVisible)
                {
                    HideMenuLarge?.Invoke();
                }
                else
                {
                    DisplayMenuLarge?.Invoke();
                }

                isLargeMenuVisible = !isLargeMenuVisible;
            };
        }
    }
}