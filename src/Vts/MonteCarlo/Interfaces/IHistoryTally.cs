using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IHistoryTally : ITally
    {
        void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IHistoryTally<T> : ITally<T>, IHistoryTally
    {
    }
}
