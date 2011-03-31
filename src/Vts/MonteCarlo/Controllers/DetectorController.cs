using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.MonteCarlo.Controllers
{
    public class DetectorController : IDetectorController
    {
        private ITissue _tissue;
        private IList<IDetector> _detectors;
        private IList<ITerminationDetector> _terminationDetectors;
        private IList<IHistoryDetector> _historyDetectors;

        public DetectorController(
            IList<IDetectorInput> detectorInputs,
            ITissue tissue)
        {
            _tissue = tissue;

            _detectors = GetDetectors(detectorInputs);

            _terminationDetectors =
                (from detector in _detectors
                 where detector.TallyType.IsTerminationTally()
                 select (ITerminationDetector)detector).ToArray();

            _historyDetectors =
                (from detector in _detectors
                 where detector.TallyType.IsHistoryTally()
                 select (IHistoryDetector)detector).ToArray();
        }

        // Commented unused overload
        ///// <summary>
        ///// Default constructor tallies all tallies with default ranges
        ///// </summary>
        //public DetectorController()
        //    : this(
        //        new IDetectorInput[] 
        //        { 
        //            new AOfRhoAndZDetectorInput(),
        //            new ATotalDetectorInput(),
        //            new FluenceOfRhoAndZAndTimeDetectorInput(),
        //            new FluenceOfRhoAndZDetectorInput(),
        //            new RDiffuseDetectorInput(),
        //            new ROfAngleDetectorInput(),
        //            new ROfRhoAndAngleDetectorInput(),
        //            new ROfRhoAndOmegaDetectorInput(),
        //            new ROfRhoAndTimeDetectorInput(),
        //            new ROfRhoDetectorInput(),
        //            new ROfXAndYDetectorInput(),
        //            new TDiffuseDetectorInput(),
        //            new TOfAngleDetectorInput(),
        //            new TOfRhoAndAngleDetectorInput(),
        //            new TOfRhoDetectorInput(),
        //            new ROfRhoDetectorInput() 
        //        },
        //        new MultiLayerTissue() )
        //{
        //}

        public IList<IDetector> Detectors { get { return _detectors; } }

        public void TerminationTally(PhotonDataPoint dp)
        {
            foreach (var tally in _terminationDetectors)
            {
                if (tally.ContainsPoint(dp))
                    tally.Tally(dp);
            }
        }

        public void HistoryTally(PhotonHistory history)
        {
            // loop through the photon history. history tallies require information 
            // from previous and "current" collision points (including pseudo-collisions)
            PhotonDataPoint previousDP = history.HistoryData.First();
            foreach (PhotonDataPoint dp in history.HistoryData.Skip(1))
            {
                foreach (var tally in _historyDetectors)
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

        private IList<IDetector> GetDetectors(IList<IDetectorInput> detectorInputs)
        {
            var detectorList = new List<IDetector>(detectorInputs.Count);
            foreach (var detectorInput in detectorInputs)
            {
                // ckh change 3/14/11 added GetHistoryDetector
                IDetector detector;
                if (detectorInput.TallyType.IsTerminationTally())
                {
                    detector = DetectorFactory.GetTerminationDetector(detectorInput, _tissue);
                }
                else
                {
                    detector = DetectorFactory.GetHistoryDetector(detectorInput, _tissue);
                }
                detectorList.Add(detector);
            }
            return detectorList;
        }
    }
}
