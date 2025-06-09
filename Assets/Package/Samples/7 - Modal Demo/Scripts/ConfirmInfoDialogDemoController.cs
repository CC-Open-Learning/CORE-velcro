using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace VARLab.Velcro.Demos
{
    public class DialogsAndNotificationDemoController : Toolbar
    {
        [SerializeField] private ConfirmationDialogSO confirmationDialogSO;
        [SerializeField] private ConfirmationDialogSO confirmationDialogSO2;
        [SerializeField] private InformationDialogSO informationDialogSO;
        [SerializeField] private InformationDialogSO informationDialogSO2;
        [SerializeField] private ImageDialogSO imageDialogSO;
        [SerializeField] private ImageDialogSO imageDialogSO2;
        [SerializeField] private NotificationSO notificationSO;

        public UnityEvent<ConfirmationDialogSO> DisplayConfirmationDialog;
        public UnityEvent<InformationDialogSO> DisplayInformationDialog;
        public UnityEvent<ImageDialogSO> DisplayImageDialog;
        public UnityEvent<NotificationSO> DisplayNotification;

        private void Start()
        {
            SetupBaseToolbar();

            DisplayConfirmationDialog ??= new UnityEvent<ConfirmationDialogSO>();
            DisplayInformationDialog ??= new UnityEvent<InformationDialogSO>();
            DisplayImageDialog ??= new UnityEvent<ImageDialogSO>();
            DisplayNotification ??= new UnityEvent<NotificationSO>();

            Button confirmDialogBtn = Root.Q("Confirmation1").Q<Button>();
            confirmDialogBtn.clicked += () =>
            {
                DisplayConfirmationDialog?.Invoke(confirmationDialogSO);
            };

            Button confirmDialogBtn2 = Root.Q("Confirmation2").Q<Button>();
            confirmDialogBtn2.clicked += () =>
            {
                DisplayConfirmationDialog?.Invoke(confirmationDialogSO2);
            };

            Button infoDialogBtn = Root.Q("Information1").Q<Button>();
            infoDialogBtn.clicked += () =>
            {
                DisplayInformationDialog?.Invoke(informationDialogSO);
            };

            Button infoDialogBtn2 = Root.Q("Information2").Q<Button>();
            infoDialogBtn2.clicked += () =>
            {
                DisplayInformationDialog?.Invoke(informationDialogSO2);
            };

            Button imageDialogBtn = Root.Q("Image1").Q<Button>();
            imageDialogBtn.clicked += () =>
            {
                DisplayImageDialog?.Invoke(imageDialogSO);
            };

            Button imageDialogBtn2 = Root.Q("Image2").Q<Button>();
            imageDialogBtn2.clicked += () =>
            {
                DisplayImageDialog?.Invoke(imageDialogSO2);
            };
        }

        public void HandleDismissDialog()
        {
            DisplayNotification?.Invoke(notificationSO);
        }
    }
}