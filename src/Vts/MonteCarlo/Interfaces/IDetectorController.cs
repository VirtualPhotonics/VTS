using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Interface for Controller classes that manage the detectors.
    /// </summary>
    public interface IDetectorController
    {
        /// <summary>
        /// List of detectors controller is managing
        /// </summary>
        IList<IDetector> Detectors { get; }
        /// <summary>
        /// Method to normalize all detectors given the number of photons launched, N.
        /// </summary>
        /// <param name="N"></param>
        void NormalizeDetectors(long N);
    }
}
