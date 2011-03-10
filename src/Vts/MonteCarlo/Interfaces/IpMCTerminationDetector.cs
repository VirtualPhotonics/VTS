using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCTerminationDetector : IDetector
    {
        void Tally(PhotonDataPoint dp, IList<SubRegionCollisionInfo> infoList);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCTerminationDetector<T> : IDetector<T>, IpMCTerminationDetector
    {
    }
}
