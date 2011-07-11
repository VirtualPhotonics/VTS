using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCVolumeDetector : IDetector
    {
        void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp, IList<SubRegionCollisionInfo> infoList);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCVolumeDetector<T> : IDetector<T>, IpMCVolumeDetector
    {
    }
}
