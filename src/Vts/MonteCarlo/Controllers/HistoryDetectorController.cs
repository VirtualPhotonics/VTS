using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Controllers
{
    /// <summary>
    /// Controller for detectors.
    /// </summary>
    public class HistoryDetectorController : IDetectorController
    {
        private IList<IDetector> _detectors;
        private ITissue _tissue;

        public HistoryDetectorController(IEnumerable<IDetector> detectors, ITissue tissue)
        {
            // todo: is this needed, or can we trust that it's already fitlered?
            _detectors = (from d in detectors
                          where d is IHistoryDetector
                          select d).ToList();

            _tissue = tissue;
        }

        public IList<IDetector> Detectors { get { return _detectors; } }

        public void Tally(Photon photon)
        {
            PhotonDataPoint previousDP = photon.History.HistoryData.First();
            foreach (PhotonDataPoint dp in photon.History.HistoryData.Skip(1))
            {
                var currentRegionIndex = _tissue.GetRegionIndex(dp.Position);
                foreach (var detector in _detectors)
                {
                    ((IHistoryDetector)detector).TallySingle(previousDP, dp, currentRegionIndex);
                    //previousDP = dp;
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
