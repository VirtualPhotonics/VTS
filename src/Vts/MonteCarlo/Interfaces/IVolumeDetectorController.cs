using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface IVolumeDetectorController : IDetectorController
    {
        IList<IVolumeDetector> Detectors { get; }
        void Tally(PhotonHistory history);
    }
}
