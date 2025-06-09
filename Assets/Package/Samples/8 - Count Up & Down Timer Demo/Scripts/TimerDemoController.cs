using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class TimerDemoController : Toolbar
    {
        [SerializeField] NotificationSO startNotification;
        [SerializeField] NotificationSO endNotification;

        public UnityEvent<bool, bool> StartTimer;
        public UnityEvent PauseTimer;
        public UnityEvent ResumeTimer;
        public UnityEvent ResetTimer;
        public UnityEvent<int, bool> StartTimer2;
        public UnityEvent<int, bool> SetContent;
        public UnityEvent ResetTimer2;
        public UnityEvent<NotificationSO> ShowNotification;

        private void Start()
        {
            SetupBaseToolbar();

            StartTimer ??= new UnityEvent<bool, bool>();
            PauseTimer ??= new UnityEvent();
            ResumeTimer ??= new UnityEvent();
            ResetTimer ??= new UnityEvent();
            StartTimer2 ??= new UnityEvent<int, bool>();
            SetContent ??= new UnityEvent<int, bool>();
            ResetTimer2 ??= new UnityEvent();
            ShowNotification ??= new UnityEvent<NotificationSO>();

            Button startTimerBtn = Root.Q("CountUpTimer").Q<Button>();
            startTimerBtn.clicked += () =>
            {
                StartTimer?.Invoke(true, false);
            };

            Button pauseTimerBtn = Root.Q("PauseTimer").Q<Button>();
            pauseTimerBtn.clicked += () =>
            {
                PauseTimer?.Invoke();
            };

            Button resumeTimerBTn = Root.Q("ResumeTimer").Q<Button>();
            resumeTimerBTn.clicked += () =>
            {
                ResumeTimer?.Invoke();
            };

            Button resetTimerBtn = Root.Q("ResetTimer").Q<Button>();
            resetTimerBtn.clicked += () =>
            {
                ResetTimer?.Invoke();
            };

            Button startTimerBtn2 = Root.Q("CountDownTimer").Q<Button>();
            startTimerBtn2.clicked += () =>
            {
                StartTimer2?.Invoke(5, true);
            };

            Button setConentBtn = Root.Q("SetContent").Q<Button>();
            setConentBtn.clicked += () =>
            {
                SetContent?.Invoke(5, true);
            };

            Button resetTimerBtn2 = Root.Q("ResetTimer2").Q<Button>();
            resetTimerBtn2.clicked += () =>
            {
                ResetTimer2?.Invoke();
            };
        }

        public void HandleTimerStarted()
        {
            ShowNotification?.Invoke(startNotification);
        }

        public void HandleTimerEnded()
        {
            ShowNotification?.Invoke(endNotification);
        }
    }
}