using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.Velcro
{
    [CreateAssetMenu(fileName = "TaskListSO", menuName = "ScriptableObjects/TaskListSO")]
    public class TaskListSO : ScriptableObject
    {
        [Header("List position on Screen")]
        [Tooltip("Where the UI element will be positioned horizontally. TopRight the UI will be anchored to the top right of the screen, and TopLeft to the top left.")]
        public TopAnchor Alignment = TopAnchor.TopRight;
        
        [Header("Notification Position on the Screen")]
        [Tooltip("Where the notification will be positioned vertically.FlexStart is Top, Center is middle, and FlexEnd is bottom of the screen")]
        public Align NotificationAlignment = Align.Center;
        
        [Serializable]
        public struct Task
        {
            public string TaskName;
            public string TaskHint;
        }
        
        [Header("List Settings")]
        public string ListTitle;
        public string ProgressText;
        public string ListDescription;

        public List<Task> Tasks = new List<Task>();
    }
}