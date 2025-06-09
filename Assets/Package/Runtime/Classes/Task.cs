using System;
using UnityEngine;

namespace VARLab.Velcro
{
    /// <summary>
    /// Represents a task in a progress indicator.
    /// </summary>
    [Serializable]
    public class Task
    {
        /// <summary>
        /// The name of the task.
        /// </summary>
        [field: SerializeField]
        public string Name;

        private int progress;

        /// <summary>
        /// The current progress of the task.
        /// </summary>
        public int Progress
        {
            get => progress;
            set
            {
                progress = Mathf.Clamp(value, 0, MaxProgress);
            }
        }

        private int maxProgress = 1;

        /// <summary>
        /// The maximum amount of progress needed to complete the task.
        /// </summary>
        public int MaxProgress
        {
            get => maxProgress;
            set
            {
                maxProgress = Mathf.Abs(value);
            }
        }

        /// <summary>
        /// The completion status of the task.
        /// </summary>
        public bool Completed
        {
            get => progress != 0 && progress >= MaxProgress;
            set { }
        }

        public Task(string name, int maxProgress = 1)
        {
            Name = name;
            MaxProgress = maxProgress;
            Progress = 0;
        }
    }
}
