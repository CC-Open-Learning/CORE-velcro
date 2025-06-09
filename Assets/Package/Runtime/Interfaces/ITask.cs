namespace VARLab.Velcro
{
    public interface ITask
    {
        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        /// <returns>The name of the task.</returns>
        public string GetTaskName();

        /// <summary>
        /// Gets the current progress of the task.
        /// </summary>
        /// <returns>The current progress of the task.</returns>
        public int GetTaskProgress();

        /// <summary>
        /// Gets the max progress of the task.
        /// </summary>
        /// <returns>The amount of progress needed to complete the task.</returns>
        public int GetTaskMaxProgress();

        /// <summary>
        /// Gets the completion status of the task.
        /// </summary>
        /// <returns>Whether the task has been completed or not.</returns>
        public bool GetTaskCompleted();
    }
}
