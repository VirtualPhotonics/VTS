using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCTally : ITally
    {
        void Tally(PhotonDataPoint dp, IList<SubRegionCollisionInfo> infoList);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IpMCTally<T> : ITally<T>, IpMCTally
    {
    }
}
