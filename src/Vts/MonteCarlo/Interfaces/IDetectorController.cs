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
        IList<IDetector> Detectors { get; }
        void NormalizeDetectors(long N);
    }
}
