using System.Collections.Generic;
using System.Linq;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller for detectors.
    /// </summary>
    public class DetectorController : IDetectorController
    {
        private IList<IDetector> _detectors;
        /// <summary>
        /// controller for detectors
        /// </summary>
        /// <param name="detectors">IEnumerable for IDetector</param>
        public DetectorController(
            IEnumerable<IDetector> detectors)
        {
            _detectors = detectors.Select(d => (IDetector)d).ToList();
        }
        /// <summary>
        /// IList of IDetector
        /// </summary>
        public IList<IDetector> Detectors { get { return _detectors; } }
        /// <summary>
        /// method that tallies all detectors given Photon
        /// </summary>
        /// <param name="photon">Photon</param>
        public void Tally(Photon photon)
        {
            foreach (var detector in _detectors)
            {
                detector.Tally(photon);
            }
        }
        /// <summary>
        /// method that normalizes the detector tallies
        /// </summary>
        /// <param name="N">number of photons launched from source</param>
        public virtual void NormalizeDetectors(long N)
        {
            foreach (var detector in _detectors)
            {
                detector.Normalize(N);
            }
        }
    }
}
