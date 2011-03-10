using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.TallyActions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    public class pMCDetectorController : IDetectorController
    {
        private ITissue _tissue;
        private IList<IDetector> _detectors;
        private IList<IpMCTerminationDetector> _terminationDetectors;
        private IList<IpMCHistoryDetector> _historyDetectors;

        public pMCDetectorController(
            IList<IpMCDetectorInput> detectorInputs,
            ITissue tissue)
        {
            _tissue = tissue;
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
                var detector = DetectorFactory.GetpMCDetector(detectorInput, _tissue);
                detectorList.Add(detector);
            }

            return detectorList;
        }

        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public pMCDetectorController()
            : this(
                new List<IpMCDetectorInput> { new pMCROfRhoAndTimeDetectorInput() },
                new MultiLayerTissue())
        {
        }

        public IList<IDetector> Detectors { get { return _detectors; } }
        public IList<OpticalProperties> ReferenceOps { get; set; }
        public IList<int> PerturbedRegionsIndices { get; set; }
        
        public void TerminationTally(PhotonDataPoint dp, IList<SubRegionCollisionInfo> collisionInfo)
        {
            foreach (var detector in _terminationDetectors)
            {
                if (detector.ContainsPoint(dp))
                    detector.Tally(dp, collisionInfo);
            }
        }

        public void HistoryTally(PhotonHistory history, IList<SubRegionCollisionInfo> collisionInfo)
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

        //public void SetOutputArrays(Output output)
        //{
        //    foreach (var tallyType in TallyTypeList)
        //    {
        //        switch (tallyType)
        //        {
        //            default:
        //            case TallyType.pMuaMusInROfRhoAndTime:
        //                output.R_rt =
        //                    ((IDetector<double[,]>)TerminationITallyList[TallyTypeList.IndexOf(TallyType.pMuaMusInROfRhoAndTime)]).Mean;
        //                break;
        //            case TallyType.pMuaMusInROfRho:
        //                output.R_r =
        //                    ((IDetector<double[]>)TerminationITallyList[TallyTypeList.IndexOf(TallyType.pMuaMusInROfRho)]).Mean;
        //                break;
        //        }
        //    }
        //}
    }
}
