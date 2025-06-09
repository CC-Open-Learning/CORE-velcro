using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "NotificationSO", menuName = "ScriptableObjects/NotificationSO")]
    public class NotificationSO : ScriptableObject
    {
        [Tooltip("Success is green, info is blue, error is red")]
        public NotificationType NotificationType = NotificationType.Success;

        [Tooltip("Where the notification will be positioned vertically. FlexStart is Top, Center is middle, and FlexEnd is bottom of the screen")]
        public Align Alignment = Align.FlexStart;

        [Tooltip("Font size of the message. Medium is 24, Large is 28")]
        public FontSize FontSize = FontSize.Medium;

        [Header("Content"), Space(5)]
        [TextArea(1, 10), Tooltip("The [Text] label of the tooltip")]
        public string Message;
    }
}