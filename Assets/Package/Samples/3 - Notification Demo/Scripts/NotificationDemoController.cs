using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    /// <summary>
    /// This script and the associated UI Document is for the sample scene only. It is not intended to be pulled
    /// into any DLX besides for learning and testing purposes
    /// </summary>
    public class NotificationDemoController : Toolbar
    {
        [SerializeField] Notification notification;
        [SerializeField] Sprite customIcon;

        public UnityEvent<NotificationType, string, FontSize, Align> DisplayNotification;
        public UnityEvent<NotificationSO> DisplayNotificationSO;

        void Start()
        {
            SetupBaseToolbar();
            DisplayNotification ??= new UnityEvent<NotificationType, string, FontSize, Align>();
            DisplayNotificationSO ??= new UnityEvent<NotificationSO>();

            Button notifSuccessBtn = Root.Q<Button>("Success");
            notifSuccessBtn.clicked += () =>
            {
                NotificationSO notificationSO = ScriptableObject.CreateInstance<NotificationSO>();
                notificationSO.NotificationType = NotificationType.Success;
                notificationSO.Alignment = Align.FlexStart;
                notificationSO.FontSize = FontSize.Medium;
                notificationSO.Message = "This Success notification is aligned to FlexStart";

                DisplayNotificationSO?.Invoke(notificationSO);
            };

            Button notifInfoBtn = Root.Q<Button>("Info");
            notifInfoBtn.clicked += () =>
            {
                DisplayNotification?.Invoke(NotificationType.Info, "This Info notification is aligned to Center", FontSize.Medium, Align.Center);
            };

            Button notifErrorBtn = Root.Q<Button>("Error");
            notifErrorBtn.clicked += () =>
            {
                DisplayNotification?.Invoke(NotificationType.Error, "This Error notification is aligned to FlexEnd", FontSize.Medium, Align.FlexEnd);
            };

            Button notifCustomBtn = Root.Q<Button>("Custom");
            notifCustomBtn.clicked += () =>
            {
                notification.SetCustomNotification(Color.black, Color.yellow, customIcon);
                DisplayNotification?.Invoke(NotificationType.Custom, "This Custom notification is aligned to Center with Large font size", FontSize.Large, Align.Center);
            };

            Button notifCustomBtn2 = Root.Q<Button>("Custom2");
            notifCustomBtn2.clicked += () =>
            {
                notification.SetCustomNotification(Color.black, Color.white);
                DisplayNotification?.Invoke(NotificationType.Custom, "*Some sort of in game action notification like PHI*", FontSize.Large, Align.Center);
            };
        }
    }
}