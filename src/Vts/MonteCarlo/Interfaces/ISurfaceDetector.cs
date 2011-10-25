using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for Monte Carlo Surface Tallies.
    /// </summary>
    public interface  ISurfaceDetector : IDetector
    {
        /// <summary>
        /// Method to tally
        /// </summary>
        /// <param name="dp">PhotonDataPoint</param>
        void Tally(PhotonDataPoint dp);
    }
    /// <summary>
    /// Defines a contract for Monte Carlo Surface Tallies.
    /// </summary>
    /// <typeparam name="T">type of tally return (e.g. double[])</typeparam>
    public interface ISurfaceDetector<T> : IDetector<T>, ISurfaceDetector
    {
    }
}
