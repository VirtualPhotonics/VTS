using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for perturbation Monte Carlo (pMC) Volume tallies.
    /// </summary>
    public interface IpMCVolumeDetector : IDetector
    {
        /// <summary>
        /// Method to tally photon 
        /// </summary>
        /// <param name="previousDP">previous PhotonDataPoint</param>
        /// <param name="dp">PhotonDataPoint</param>
        /// <param name="infoList">SubRegionCollisionInfo</param>
        void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp, IList<SubRegionCollisionInfo> infoList);
    }

    /// <summary>
    /// Defines a contract for perturbation Monte Carlo (pMC) Volume tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IpMCVolumeDetector<T> : IDetector<T>, IpMCVolumeDetector
    {
    }
}
