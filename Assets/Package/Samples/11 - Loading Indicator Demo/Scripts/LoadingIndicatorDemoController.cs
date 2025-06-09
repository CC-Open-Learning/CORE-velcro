using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class LoadingIndicatorDemoController : Toolbar
    {
        [SerializeField] private Notification notification;

        public UnityEvent<float> StartLoading;
        public UnityEvent FailLoading;
        public UnityEvent CompleteLoading;

        private void Start()
        {
            SetupBaseToolbar();
            StartLoading ??= new UnityEvent<float>();
            FailLoading ??= new UnityEvent();
            CompleteLoading ??= new UnityEvent();

            Button startLoadBtn = Root.Q<Button>("StartLoad");
            startLoadBtn.clicked += () =>
            {
                StartLoading?.Invoke(0.0f);
            };

            Button failLoadBtn = Root.Q<Button>("FailLoad");
            failLoadBtn.clicked += () =>
            {
                FailLoading?.Invoke();
            };

            Button completeLoadBtn = Root.Q<Button>("CompleteLoad");
            completeLoadBtn.clicked += () =>
            {
                CompleteLoading?.Invoke();
            };
        }

        public void ShowLoadingStartNotifcation()
        {
            notification.HandleDisplayUI(NotificationType.Info, "Loading Started!", FontSize.Large, Align.FlexStart);
        }

        public void ShowLoadingCompleteNotifcation()
        {
            notification.HandleDisplayUI(NotificationType.Success, "Loading Complete!", FontSize.Large, Align.FlexStart);
        }

        public void ShowLoadingFailedNotifcation()
        {
            notification.HandleDisplayUI(NotificationType.Error, "Loading Failed!", FontSize.Large, Align.FlexStart);
        }
    }
}