
namespace Vts.MonteCarlo
{
    /////[ServiceContract(Namespace = "http://Vts.MonteCarlo.Detectors")]
    /// <summary>
    /// Defines a contract for Monte Carlo tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IDetector<out T> : IDetector
    {
        /// <summary>
        /// Mean of detector tally
        /// </summary>
        T Mean { get; }
        /// <summary>
        /// Second moment of detector tally
        /// </summary>
        T SecondMoment { get; }
    }
    /// <summary>
    /// Properties and methods that all IDetectors must implement
    /// </summary>
    public interface IDetector
    {
        /// <summary>
        /// TallyType enum specification
        /// </summary>
        TallyType TallyType{ get; set; }
        /// <summary>
        /// Name string of IDetector.  Default = TallyType.ToString().
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Number of times this detector got tallied.
        /// </summary>
        long TallyCount { get; set; }
        /// <summary>
        /// Method to normalize the tally to get Mean and Second Moment estimates
        /// </summary>
        /// <param name="numPhotons">number of photons launched</param>
        void Normalize(long numPhotons);
        /// <summary>
        /// Method to tally to detector using information in Photon
        /// </summary>
        /// <param name="photon">photon data needed to tally</param>
        void Tally(Photon photon);
    }
}