namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for DetectorInput classes.
    /// </summary>
    public interface IDetectorInput
    {
        /// <summary>
        /// TallyType string identifying type of detector
        /// </summary>
       string TallyType { get; set; }
        /// <summary>
        /// Name of detector.  User can define or default is TallyType.ToString().
        /// </summary>
       string Name { get; set; }

       IDetector CreateDetector();
    }
}
