using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCSurfaceDetector : IDetector
    {
        void Tally(PhotonDataPoint dp, CollisionInfo infoList);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCSurfaceDetector<T> : IDetector<T>, IpMCSurfaceDetector
    {
    }
}
