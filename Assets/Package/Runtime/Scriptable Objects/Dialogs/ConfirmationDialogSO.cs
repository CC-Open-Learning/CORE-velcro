using UnityEngine;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "ConfirmationDialogSO", menuName = "ScriptableObjects/Dialogs/ConfirmationDialogSO")]
    public class ConfirmationDialogSO : ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 3), Tooltip("The [Name] label of the dialog")]
        public string Name;

        [TextArea(1, 10), Tooltip("The [Description] label of the dialog")]
        public string Description;

        [Header("Button Options"), Space(5)]

        [Tooltip("The type of button the primary button will be (Right most button)")]
        public ButtonType PrimaryBtnType = ButtonType.Primary;

        [TextArea(1, 3), Tooltip("The text of the primary button.")]
        public string PrimaryBtnText;

        [Tooltip("The type of button the secondary button will be (Left most button)")]
        public ButtonType SecondaryBtnType = ButtonType.Secondary1;

        [TextArea(1, 3), Tooltip("The text of the secondary button")]
        public string SecondaryBtnText;

        [Header("Additional Options"), Space(5)]
        [Tooltip("Whether the \"X\" button in the top right is visible or not")]
        public bool IsCloseBtnVisible = true;

        [Header("Additional Options"), Space(5)]
        [Tooltip("Whether the [Description] label is bolded or not")]
        public bool IsDescriptionBolded = true;

        [Tooltip("Whether the background behind the UI is dimmed or not")]
        public bool IsBackgroundDimmed = true;
    }
}