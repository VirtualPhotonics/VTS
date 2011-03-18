using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface IDetectorController
    {
        IList<IDetector> Detectors { get; }
        void NormalizeDetectors(long N);
    }
}
