using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Properties and methods that all IDetectors must implement
    /// </summary>
    public interface IHistoryDetector : IDetector
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