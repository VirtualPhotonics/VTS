using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    //[ServiceContract(Namespace = "http://Vts.MonteCarlo.Detectors")]
    /// <summary>
    /// Defines a contract for Monte Carlo tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IDetector<out T> : IDetector
    {
        T Mean { get; }
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
        /// <param name="numPhotons"></param>
        void Normalize(long numPhotons);
        //bool ContainsPoint(PhotonDataPoint dp);
    }
}