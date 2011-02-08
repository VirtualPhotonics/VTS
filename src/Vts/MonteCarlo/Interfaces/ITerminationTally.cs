using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface  ITerminationTally : ITally
    {
        void Tally(PhotonDataPoint dp);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo Termination Tallies.
    /// </summary>
    public interface ITerminationTally<T> : ITally<T>, ITerminationTally
    {
    }
}
