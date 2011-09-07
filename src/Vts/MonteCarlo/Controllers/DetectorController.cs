using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller for detectors.
    /// </summary>
    public class DetectorController : IDetectorController
    {
        private IList<IDetector> _detectors;

        public DetectorController(
            IList<IDetector> detectors)
        {
            _detectors = detectors.Select(d => (IDetector)d).ToList();
        }

        public IList<IDetector> Detectors { get { return _detectors; } }

        public void Tally(Photon photon)
        {
            foreach (var detector in _detectors)
            {
                detector.Tally(photon);
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
