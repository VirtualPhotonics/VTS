using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IHistoryDetector : IDetector
    {
        void Tally(PhotonDataPoint previousDP, PhotonDataPoint dp);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo History Tallies.
    /// </summary>
    public interface IHistoryDetector<T> : IDetector<T>, IHistoryDetector
    {
    }
}
