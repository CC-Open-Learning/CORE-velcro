using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class TabLayoutDemoController : Toolbar
    {
        public UnityEvent ToggleHorizontalTabLayout;
        public UnityEvent ToggleVerticalTabLayout;

        private void Start()
        {
            SetupBaseToolbar();

            ToggleHorizontalTabLayout ??= new UnityEvent();
            ToggleVerticalTabLayout ??= new UnityEvent();

            Button toggleHorizontalBtn = Root.Q<Button>("ToggleHorizontal");
            toggleHorizontalBtn.clicked += () =>
            {
                ToggleHorizontalTabLayout?.Invoke();
            };

            Button toggleVerticalBtn = Root.Q<Button>("ToggleVertical");
            toggleVerticalBtn.clicked += () =>
            {
                ToggleVerticalTabLayout?.Invoke();
            };
        }
    }
}