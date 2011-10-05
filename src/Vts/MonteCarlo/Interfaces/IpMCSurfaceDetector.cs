using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for perturbation Monte Carlo Surface Tallies.
    /// </summary>
    public interface IpMCSurfaceDetector : IDetector
    {
        /// <summary>
        /// Method to tally photon
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        /// <param name="infoList">CollisionInfo</param>
        void Tally(PhotonDataPoint dp, CollisionInfo infoList);
    }

    /// <summary>
    /// Defines a contrack for perturbation Monte Carlo Surface Tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface IpMCSurfaceDetector<T> : IDetector<T>, IpMCSurfaceDetector
    {
    }
}
