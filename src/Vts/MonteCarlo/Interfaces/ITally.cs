using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo Tally.
    /// </summary>
    public interface ITally
    {
        void Tally(PhotonDataPoint dp, IList<OpticalProperties> ops);
        void Normalize(long numPhotons);
        //bool ShouldBeTallied();
        bool ContainsPoint(PhotonDataPoint dp);
    }
}
