using System.Collections.Generic;
using System.Linq;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller for detectors.
    /// </summary>
    public class DetectorController : IDetectorController
    {
        /// <summary>
        /// controller for detectors
        /// </summary>
        /// <param name="detectors">IEnumerable for IDetector</param>
        public DetectorController(
            IEnumerable<IDetector> detectors)
        {
            Detectors = detectors.Select(d => d).ToList();
        }
        /// <summary>
        /// IList of IDetector
        /// </summary>
        public IList<IDetector> Detectors { get; }

        /// <summary>
        /// method that tallies all detectors given Photon
        /// </summary>
        /// <param name="photon">Photon</param>
        public void Tally(Photon photon)
        {
            foreach (var detector in Detectors)
            {
                detector.Tally(photon);
            }
        }
        /// <summary>
        /// method that normalizes the detector tallies
        /// </summary>
        /// <param name="n">number of photons launched from source</param>
        public virtual void NormalizeDetectors(long n)
        {
            foreach (var detector in Detectors)
            {
                detector.Normalize(n);
            }
        }
    }
}
