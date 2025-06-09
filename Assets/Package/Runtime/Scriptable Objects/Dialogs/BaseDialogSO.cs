using UnityEngine;

namespace VARLab.Velcro
{
    /// <summary>
    /// This class is a consolidated approach to handling dialog templates. Any dialog type that contains additional components can inherit from this class.
    /// This would allow the removal of InformationDialogSO and ImageDialogSO and simplify all of their associated functional classes
    /// </summary>
    [CreateAssetMenu(fileName = "BaseDialogSO", menuName = "ScriptableObjects/Dialogs/BaseDialogSO")]
    public class BaseDialogSO: ScriptableObject
    {
        [Header("Content"), Space(5)]
        [TextArea(1, 3), Tooltip("The [Name] label of the dialog")]
        public string Name;

        [TextArea(1, 3), Tooltip("The [Title] label of the dialog. If empty, DisplayStyle.None will be set")]
        public string Title;

        [TextArea(1, 10), Tooltip("The [Description] label of the dialog")]
        public string Description;

        [TextArea(1, 3), Tooltip("The text of the primary button.")]
        public string PrimaryBtnText;

        [Header("Additional Options"), Space(5)]
        [Tooltip("Whether the [Description] label is bolded or not")]
        public bool IsDescriptionBolded = true;

        [Tooltip("Whether the background behind the UI is dimmed or not")]
        public bool IsBackgroundDimmed = true;
    }
}
