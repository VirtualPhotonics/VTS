using Vts.MonteCarlo.Detectors;

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
       
       /// <summary>
       /// Details of the tally - booleans that specify when they should be tallied
       /// </summary>
       TallyDetails TallyDetails { get; set; }

        /// <summary>
        /// Required factory method to create the corresponding 
        /// IDetector based on the IDetectorInput data
        /// </summary>
        /// <returns></returns>
       IDetector CreateDetector();
    }
}
