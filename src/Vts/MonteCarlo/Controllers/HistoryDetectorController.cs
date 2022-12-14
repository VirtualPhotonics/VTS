using System.Collections.Generic;
using System.Linq;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller for detectors.
    /// </summary>
    public class HistoryDetectorController : IDetectorController
    {
        private readonly IList<IHistoryDetector> _detectors;

        /// <summary>
        /// Controller for history type detectors
        /// </summary>
        /// <param name="detectors">IEnumerable of IHistoryDetector</param>
        /// <param name="tissue">The Tissue (this is not used)</param>
        public HistoryDetectorController(IEnumerable<IHistoryDetector> detectors, ITissue tissue)
        {
            _detectors = detectors.ToList();
        }
        /// <summary>
        /// IList of IDetector
        /// </summary>
        public IList<IDetector> Detectors { get { return _detectors.Select(d => (IDetector)d).ToList(); } }
        /// <summary>
        /// method to tally to detectors managed by this controller
        /// </summary>
        /// <param name="photon">Photon</param>
        public void Tally(Photon photon)
        {
            // CKH mod 4-1-15: need to call Tally rather than call TallySingle
            // because Tally contains 2nd moment processing
            foreach (var detector in _detectors)
            {
                detector.Tally(photon);
            }
        }
        /// <summary>
        /// method to normalize detector tallies
        /// </summary>
        /// <param name="n">number of photons launched from source</param>
        public virtual void NormalizeDetectors(long n)
        {
            foreach (var detector in _detectors)
            {
                detector.Normalize(n);
            }
        }
    }
}
