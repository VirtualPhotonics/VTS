using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo
{
    public interface ISurfaceDetectorController : IDetectorController
    {
        void Tally(PhotonDataPoint dp);
    }
}
