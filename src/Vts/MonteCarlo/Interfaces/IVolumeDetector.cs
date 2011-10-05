using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo Volume Tallies.
    /// </summary>
    public interface IVolumeDetector : IDetector
    {
        /// <summary>
        /// Method to tally
        /// </summary>
        /// <param name="previousDP">previous PhotonDataPoint</param>
        /// <param name="dp">PhotonDataPoint</param>
        void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp);  
    }

    /// <summary>
    /// Defines a contrack for Monte Carlo Volume tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IVolumeDetector<T> : IDetector<T>, IVolumeDetector
    {
    }
}
