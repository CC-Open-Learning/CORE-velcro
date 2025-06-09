using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class TutorialDemoController : Toolbar
    {
        public UnityEvent DisplayTutorial;
        public UnityEvent HideTutorial;

        private void Start()
        {
            SetupBaseToolbar();

            DisplayTutorial ??= new UnityEvent();
            HideTutorial ??= new UnityEvent();

            Button showBtn = Root.Q<Button>("Show");
            showBtn.clicked += () =>
            {
                DisplayTutorial?.Invoke();
            };

            Button hideBtn = Root.Q<Button>("Hide");
            hideBtn.clicked += () =>
            {
                HideTutorial?.Invoke();
            };
        }
    }
}