using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IHistoryTally
    {
        void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp, IList<OpticalProperties> ops);
        void Normalize(long numPhotons);
        //bool ShouldBeTallied();
        bool ContainsPoint(PhotonDataPoint dp);
    }
}
