using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller for Surface detectors.
    /// </summary>
    public class SurfaceDetectorController : ISurfaceDetectorController
    {
        private IList<ISurfaceDetector> _detectors;

        public SurfaceDetectorController(
            IList<ISurfaceDetector> detectors)
        {
            _detectors = detectors;
        }

        public IList<ISurfaceDetector> Detectors { get { return _detectors; } }
        
        public void Tally(PhotonDataPoint dp)
        {
            foreach (var detector in _detectors)
            {
                //if (dp.StateFlag.Has(PhotonStateType.PseudoReflectedTissueBoundary) &&
                //    tally.TallyType.IsSurfaceTally())
                    detector.Tally(dp);
            }
        }

        public virtual void NormalizeDetectors(long N)
        {
            foreach (var detector in _detectors)
            {
                detector.Normalize(N);
            }
        }
    }
}
