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
        /// <param name="previousDP">previous photon data point</param>
        /// <param name="dp">current photon data point</param>
        /// <param name="currentRegionIndex">index of tissue region photon is current in</param>
        void TallySingle(PhotonDataPoint previousDP, PhotonDataPoint dp, int currentRegionIndex);
    }
}