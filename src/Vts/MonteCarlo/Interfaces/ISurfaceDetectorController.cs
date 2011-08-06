using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// The contract for Surface tally type DetectorControllers.
    /// Defines the signature of method Tally for these type of detectors.
    /// </summary>
    public interface ISurfaceDetectorController : IDetectorController
    {
        void Tally(PhotonDataPoint dp);
    }
}
