using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface  ISurfaceDetector : IDetector
    {
        void Tally(PhotonDataPoint dp);
    }

    /// <summary>
    /// Defines a contract for Monte Carlo Termination Tallies.
    /// </summary>
    public interface ISurfaceDetector<T> : IDetector<T>, ISurfaceDetector
    {
    }
}
