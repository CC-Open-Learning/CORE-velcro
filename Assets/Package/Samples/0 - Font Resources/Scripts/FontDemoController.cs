using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class FontDemoController : Toolbar
    {
        public UnityEvent DisplayThin;
        public UnityEvent DisplayLight;
        public UnityEvent DisplayRegular;
        public UnityEvent DisplayBold;
        public UnityEvent DisplayBlack;

        private void Start()
        {
            SetupBaseToolbar();

            DisplayThin ??= new UnityEvent();
            DisplayLight ??= new UnityEvent();
            DisplayRegular ??= new UnityEvent();
            DisplayBold ??= new UnityEvent();
            DisplayBlack ??= new UnityEvent();

            Button thinBtn = Root.Q<Button>("FontThin");
            thinBtn.clicked += () =>
            {
                DisplayThin?.Invoke();
            };

            Button lightBtn = Root.Q<Button>("FontLight");
            lightBtn.clicked += () =>
            {
                DisplayLight?.Invoke();
            };

            Button regularBtn = Root.Q<Button>("FontRegular");
            regularBtn.clicked += () =>
            {
                DisplayRegular?.Invoke();
            };

            Button boldBtn = Root.Q<Button>("FontBold");
            boldBtn.clicked += () =>
            {
                DisplayBold?.Invoke();
            };

            Button blackBtn = Root.Q<Button>("FontBlack");
            blackBtn.clicked += () =>
            {
                DisplayBlack?.Invoke();
            };
        }
    }
}