using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface IpMCSurfaceDetectorController : IDetectorController
    {
        IList<IpMCSurfaceDetector> Detectors { get; }
        void Tally(PhotonDataPoint dp, CollisionInfo info);
    }
}
