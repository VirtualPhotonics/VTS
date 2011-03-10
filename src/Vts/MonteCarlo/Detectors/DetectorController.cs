using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Vts.Common;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.TallyActions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    public class DetectorController : IDetectorController
    {
        private ITissue _tissue;
        private IList<IDetector> _detectors;
        private IList<ITerminationDetector> _terminationDetectors;
        private IList<IHistoryDetector> _historyDetectors;
        private IDictionary<TallyType, int> _tallyTypeIndex = new Dictionary<TallyType,int>();

        public DetectorController(
            IList<IDetectorInput> detectorInputs,
            ITissue tissue)
        {
            _tissue = tissue;

            _detectors = GetDetectors(detectorInputs);

            _terminationDetectors =
                (from detector in _detectors
                 where detector.TallyType.IsTerminationTally()
                 select (ITerminationDetector) detector).ToArray();

            _historyDetectors =
                (from detector in _detectors
                 where detector.TallyType.IsHistoryTally()
                 select (IHistoryDetector)detector).ToArray();
        }

        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public DetectorController()
            : this(
                new List<IDetectorInput> { new ROfRhoDetectorInput() },
                new MultiLayerTissue() )
        {
        }

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
                var detector = DetectorFactory.GetTerminationDetector(detectorInput, _tissue);
                detectorList.Add(detector);
            }
            return detectorList;
        }

        //public virtual void SetOutputArrays(Output output)
        //{
        //    foreach (var terminationTally in _terminationDetectors)
        //    {
        //        switch (terminationTally.TallyType)
        //        {
        //            case TallyType.ROfRhoAndAngle:
        //                break;
        //            case TallyType.ROfRho:
        //                break;
        //            case TallyType.ROfAngle:
        //                break;
        //            case TallyType.ROfRhoAndOmega:
        //                break;
        //            case TallyType.ROfRhoAndTime:
        //                break;
        //            case TallyType.ROfXAndY:
        //                break;
        //            case TallyType.RDiffuse:
        //                break;
        //            case TallyType.TOfRhoAndAngle:
        //                break;
        //            case TallyType.TOfRho:
        //                break;
        //            case TallyType.TOfAngle:
        //                break;
        //            case TallyType.TDiffuse:
        //                break;
        //            case TallyType.FluenceOfRhoAndZ:
        //                break;
        //            case TallyType.FluenceOfRhoAndZAndTime:
        //                break;
        //            case TallyType.AOfRhoAndZ:
        //                break;
        //            case TallyType.ATotal:
        //                break;
        //            case TallyType.MomentumTransferOfRhoAndZ:
        //                break;
        //            case TallyType.pMuaMusInROfRhoAndTime:
        //                break;
        //            case TallyType.pMuaMusInROfRho:
        //                break;
        //            default:
        //                throw new ArgumentOutOfRangeException();
        //        }
        //    }


        //    foreach (var tallyType in TallyTypeList)
        //    {
        //        switch (tallyType)
        //        {
        //            default:
        //            case TallyType.RDiffuse:
        //                output.Rd = ((IDetector<double>)TerminationITallyList[_tallyTypeIndex[TallyType.RDiffuse]]).Mean;
        //                // the following is a workaround for now
        //                output.Rtot = output.Rd +
        //                    Helpers.Optics.Specular(_tissue.Regions[0].RegionOP.N, _tissue.Regions[1].RegionOP.N);
        //                break;
        //            case TallyType.ROfAngle:
        //                output.R_a = ((IDetector<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfAngle]]).Mean;
        //                break;
        //            case TallyType.ROfRho:
        //                output.R_r = ((IDetector<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRho]]).Mean;
        //                break;
        //            case TallyType.ROfRhoAndAngle:
        //                output.R_ra = ((IDetector<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRhoAndAngle]]).Mean;
        //                break;
        //            case TallyType.ROfRhoAndTime:
        //                output.R_rt = ((IDetector<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRhoAndTime]]).Mean;
        //                break;
        //            case TallyType.ROfXAndY:
        //                output.R_xy = ((IDetector<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfXAndY]]).Mean;
        //                break;
        //            case TallyType.ROfRhoAndOmega:
        //                output.R_rw = ((IDetector<Complex[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRhoAndOmega]]).Mean;
        //                break;
        //            case TallyType.FluenceOfRhoAndZ:
        //                output.Flu_rz = ((IHistoryDetector<double[,]>)HistoryITallyList[_tallyTypeIndex[TallyType.FluenceOfRhoAndZ]]).Mean;
        //                break;
        //            case TallyType.AOfRhoAndZ:
        //                output.A_rz = ((IHistoryDetector<double[,]>)HistoryITallyList[_tallyTypeIndex[TallyType.AOfRhoAndZ]]).Mean;
        //                break;
        //            case TallyType.ATotal:
        //                output.Atot = ((IHistoryDetector<double>)HistoryITallyList[_tallyTypeIndex[TallyType.ATotal]]).Mean;
        //                break;
        //            case TallyType.TDiffuse:
        //                output.Td = ((IDetector<double>)TerminationITallyList[_tallyTypeIndex[TallyType.TDiffuse]]).Mean;
        //                break;
        //            case TallyType.TOfAngle:
        //                output.T_a = ((IDetector<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.TOfAngle]]).Mean;
        //                break;
        //            case TallyType.TOfRho:
        //                output.T_r = ((IDetector<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.TOfRho]]).Mean;
        //                break;
        //            case TallyType.TOfRhoAndAngle:
        //                output.T_ra = ((IDetector<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.TOfRhoAndAngle]]).Mean;
        //                break;
        //        }
        //    }
        //}
    }
}
