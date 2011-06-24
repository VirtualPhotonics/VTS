using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Controllers
{
    public class pMCDetectorController //: IDetectorController // ckh need to fix this after reg. MC working
    {
        private ITissue _tissue;
        private bool _tallySecondMoment;
        private IList<IDetector> _detectors;
        private IList<IpMCTerminationDetector> _terminationDetectors;
        private IList<IpMCHistoryDetector> _historyDetectors;

        public pMCDetectorController(
            IList<IpMCDetectorInput> detectorInputs,
            ITissue tissue,
            bool tallySecondMoment)
        {
            _tissue = tissue;
            _tallySecondMoment = tallySecondMoment;
            _detectors = GetDetectors(detectorInputs);

            _terminationDetectors =
                (from detector in _detectors
                 where detector.TallyType.IsTerminationTally()
                 select (IpMCTerminationDetector)detector).ToArray();

            _historyDetectors =
                (from detector in _detectors
                 where detector.TallyType.IsHistoryTally()
                 select (IpMCHistoryDetector)detector).ToArray();

            ReferenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
        }

        private IList<IDetector> GetDetectors(IList<IpMCDetectorInput> detectorInputs)
        {
            var detectorList = new List<IDetector>(detectorInputs.Count);
            foreach (var detectorInput in detectorInputs)
            {
                var detector = DetectorFactory.GetpMCDetector(detectorInput, _tissue, _tallySecondMoment);
                detectorList.Add(detector);
            }

            return detectorList;
        }

        // commented unused overload
        ///// <summary>
        ///// Default constructor tallies all tallies
        ///// </summary>
        //public pMCDetectorController()
        //    : this(
        //        new IpMCDetectorInput[]
        //        { 
        //            new pMCROfRhoAndTimeDetectorInput(),
        //            new pMCROfRhoDetectorInput(),
        //        },
        //        new MultiLayerTissue())
        //{
        //}

        public IList<IDetector> Detectors { get { return _detectors; } }
        public IList<OpticalProperties> ReferenceOps { get; set; }
        public IList<int> PerturbedRegionsIndices { get; set; }
        
        public void TerminationTally(PhotonDataPoint dp, CollisionInfo collisionInfo)
        {
            foreach (var detector in _terminationDetectors)
            {
                //if (detector.ContainsPoint(dp))
                    detector.Tally(dp, collisionInfo);
            }
        }

        public void HistoryTally(PhotonHistory history, CollisionInfo collisionInfo)
        {
            // loop through the photon history. history tallies require information 
            // from previous and "current" collision points (including pseudo-collisions)
            PhotonDataPoint previousDP = history.HistoryData.First();
            foreach (PhotonDataPoint dp in history.HistoryData.Skip(1))
            {
                foreach (var tally in _historyDetectors)
                {
                    tally.Tally(previousDP, dp, collisionInfo);
                }
                previousDP = dp;
            }
        }

        public void NormalizeDetectors(long N)
        {
            foreach (var detector in _detectors)
            {
                detector.Normalize(N);
            }
        }
    }
}
