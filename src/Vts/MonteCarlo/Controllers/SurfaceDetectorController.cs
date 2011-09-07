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
        private IList<IDetector> _detectors;

        public SurfaceDetectorController(
            IList<ISurfaceDetector> detectors)
        {
            _detectors = detectors.Select(d => (IDetector)d).ToList();
        }

        public IList<IDetector> Detectors { get { return _detectors; } }

        // new?
        public void Tally(Photon photon)
        {
            foreach (var detector in _detectors)
            {
                ((ISurfaceDetector)detector).Tally(photon);
            }
        }

        public void Tally(PhotonDataPoint dp)
        {
            foreach (var detector in _detectors)
            {
                ((ISurfaceDetector)detector).Tally(dp);
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
