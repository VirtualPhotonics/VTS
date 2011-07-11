using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller for Surface detectors.
    /// </summary>
    public class VolumeDetectorController : IVolumeDetectorController
    {
        private IList<IVolumeDetector> _detectors;

        public VolumeDetectorController(
            IList<IVolumeDetector> detectors)
        {
            _detectors = detectors;
        }

        public IList<IVolumeDetector> Detectors { get { return _detectors; } }      

        public void Tally(PhotonHistory history)
        {
            // loop through the photon history. history tallies require information 
            // from previous and "current" collision points (including pseudo-collisions)
            PhotonDataPoint previousDP = history.HistoryData.First();
            foreach (PhotonDataPoint dp in history.HistoryData.Skip(1))
            {
                foreach (var tally in _detectors)
                {
                    tally.Tally(previousDP, dp);
                }
                previousDP = dp;
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
