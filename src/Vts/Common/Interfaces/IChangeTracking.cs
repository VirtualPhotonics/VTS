namespace Vts
{
    /// <summary>
    /// Interface to enable change tracking in POCO classes
    /// </summary>
    public interface IChangeTracking
    {
        /// <summary>
        /// Status indicating whether any settings have changed
        /// </summary>
        bool SettingsChanged { get; }

        /// <summary>
        /// Method to reset all changes once they have been handled
        /// </summary>
        void Reset();
    }
}
