using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    //[ServiceContract(Namespace = "http://Vts.MonteCarlo.Detectors")]
    /// <summary>
    /// Defines a contract for Monte Carlo tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IHistoryDetector<out T> : IHistoryDetector, IDetectorOld<T>
    {
    }

    /// <summary>
    /// Properties and methods that all IDetectors must implement
    /// </summary>
    public interface IHistoryDetector : IDetectorOld
    {
        /// <summary>
        /// Method to tally to detector using information in Photon
        /// </summary>
        /// <param name="previousDP"></param>
        /// <param name="dp"></param>
        /// <param name="currentRegionIndex"></param>
        void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex);
    }
}