namespace VARLab.Velcro
{
    /// <summary>
    /// Represents specialized UI that keeps track of elapsed time.
    /// </summary>
    public interface IStopwatch
    {
        /// <summary>
        /// The time that has passed.
        /// </summary>
        public double ElapsedTime { get; }

        /// <summary>
        /// The pause state of the timer.
        /// </summary>
        public bool IsPaused { get; }

        /// <summary>
        /// Pauses the timer.
        /// </summary>
        public void Pause();

        /// <summary>
        /// Resumes the timer.
        /// </summary>
        public void Resume();
    }
}