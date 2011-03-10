using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCHistoryDetector : IDetector
    {
        void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp, IList<SubRegionCollisionInfo> infoList);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCHistoryDetector<T> : IDetector<T>, IpMCTerminationDetector
    {
    }
}
