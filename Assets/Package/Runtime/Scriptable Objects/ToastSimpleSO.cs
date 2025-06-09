using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "ToastSimpleSO", menuName = "ScriptableObjects/Toasts/ToastSimpleSO")]
    public class ToastSimpleSO : ScriptableObject
    {
        [Tooltip("The [Icon] VisualElement of the toast. Hint will be a light bulb. Location will be a map pin")]
        public ToastType ToastType = ToastType.Hint;

        [Tooltip("Where the toast will be positioned vertically. FlexStart is Top, Center is middle, and FlexEnd is bottom of the screen")]
        public Align Alignment = Align.FlexStart;

        [Header("Content"), Space(5)]
        [TextArea(1, 3), Tooltip("The [Header] label of the toast")]
        public string Header;

        [TextArea(1, 10), Tooltip("The [Message] label of the toast")]
        public string Message;
    }
}