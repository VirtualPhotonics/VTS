using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    public class Detector : IDetector
    {
        private ITissue _tissue;
        private IDictionary<TallyType, int> _tallyTypeIndex = new Dictionary<TallyType,int>();
        public Detector(
            List<TallyType> tallyTypeList,
            DoubleRange rho,
            DoubleRange z,
            DoubleRange angle,
            DoubleRange time,
            DoubleRange omega,
            DoubleRange x,
            DoubleRange y,
            ITissue tissue)
        {
            TallyTypeList = tallyTypeList;
            Rho = rho;
            Z = z;
            Angle = angle;
            Time = time;
            Omega = omega;
            X = x;
            Y = y;
            _tissue = tissue;
            SetTallyActionLists();
        }
        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public Detector()
            : this(
                new List<TallyType>()
                {
                    TallyType.RDiffuse,
                    TallyType.ROfAngle,
                    TallyType.ROfRho,
                    TallyType.ROfRhoAndAngle,
                    TallyType.ROfRhoAndTime,
                    TallyType.ROfXAndY,
                    TallyType.ROfRhoAndOmega,
                    TallyType.TDiffuse,
                    TallyType.TOfAngle,
                    TallyType.TOfRho,
                    TallyType.TOfRhoAndAngle,
                    TallyType.FluenceOfRhoAndZ,
                },
                new DoubleRange(0.0, 10, 101), // rho
                new DoubleRange(0.0, 10, 101),  // z
                new DoubleRange(0.0, Math.PI / 2, 1), // angle
                new DoubleRange(0.0, 10000, 101), // time
                new DoubleRange(0.0, 1000, 21), // omega
                new DoubleRange(-10.0, 10.0, 201), // x
                new DoubleRange(-10.0, 10.0, 201), // y
                new MultiLayerTissue()
                ) { }
        public Detector(IDetectorInput di, ITissue tissue)
            : this(
                di.TallyTypeList,
                di.Rho,
                di.Z,
                di.Angle,
                di.Time,
                di.Omega,
                di.X,
                di.Y,
                tissue) { }

        public List<ITally> TerminationITallyList { get; set; }
        public List<IHistoryTally> HistoryITallyList { get; set; }
        public List<TallyType> TallyTypeList { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Angle { get; set; }
        public DoubleRange Time { get; set; }
        public DoubleRange Omega { get; set; }
        public DoubleRange X { get; set; }
        public DoubleRange Y { get; set; }
        public DoubleRange Z { get; set; }

        public virtual void SetTallyActionLists()
        {
            TerminationITallyList = new List<ITally>();
            HistoryITallyList = new List<IHistoryTally>();
            foreach (var tally in TallyTypeList)
            {
                if (Factories.TallyActionFactory.IsHistoryTally(tally))
                {
                    HistoryITallyList.Add(Factories.TallyActionFactory.GetHistoryTallyAction(tally, _tissue, Rho, Z, Angle, Time, Omega, X, Y));
                    _tallyTypeIndex.Add(tally, HistoryITallyList.Count() - 1);
                }
                else
                {
                    TerminationITallyList.Add(Factories.TallyActionFactory.GetTallyAction(tally, _tissue, Rho, Z, Angle, Time, Omega, X, Y));
                    _tallyTypeIndex.Add(tally, TerminationITallyList.Count() - 1);
                }
            }
        }
        public void TerminationTally(PhotonDataPoint dp)
        {
            foreach (var tally in TerminationITallyList)
            {
                if (tally.ContainsPoint(dp))
                    tally.Tally(dp,
                        _tissue.Regions.Select(s => s.RegionOP).ToList());
            }
        }
        bool _firstPoint = true;
        PhotonDataPoint _previousDP;
        public void HistoryTally(PhotonHistory history)
        {
            foreach (PhotonDataPoint dp in history.HistoryData)
            {
                if (_firstPoint)
                {
                    _firstPoint = false;
                    _previousDP = dp;
                }
                else
                {
                    foreach (var tally in HistoryITallyList)
                    {
                        tally.Tally(_previousDP, dp,
                                _tissue.Regions.Select(s => s.RegionOP).ToList());
                    }
                    _previousDP = dp;
                }
            }
        }
        // pass in Output rather than return because want instance of SimulationInput to be consistent
        public virtual void NormalizeTalliesToOutput(long N, Output output)
        {
            foreach (var tally in TerminationITallyList)
            {
                tally.Normalize(N);
            }
            foreach (var tally in HistoryITallyList)
            {
                tally.Normalize(N);
            }
            foreach (var tallyType in TallyTypeList)
            {
                SetOutputArrays(output);
            };
        }
        public virtual void SetOutputArrays(Output output)
        {
            foreach (var tallyType in TallyTypeList)
            {
                switch (tallyType)
                {
                    default:
                    case TallyType.RDiffuse:
                        output.Rd = ((ITally<double>)TerminationITallyList[_tallyTypeIndex[TallyType.RDiffuse]]).Mean;
                        // the following is a workaround for now
                        output.Rtot = output.Rd +
                            Helpers.Optics.Specular(_tissue.Regions[0].RegionOP.N, _tissue.Regions[1].RegionOP.N);
                        break;
                    case TallyType.ROfAngle:
                        output.R_a = ((ITally<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfAngle]]).Mean;
                        break;
                    case TallyType.ROfRho:
                        output.R_r = ((ITally<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRho]]).Mean;
                        break;
                    case TallyType.ROfRhoAndAngle:
                        output.R_ra = ((ITally<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRhoAndAngle]]).Mean;
                        break;
                    case TallyType.ROfRhoAndTime:
                        output.R_rt = ((ITally<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRhoAndTime]]).Mean;
                        break;
                    case TallyType.ROfXAndY:
                        output.R_xy = ((ITally<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfXAndY]]).Mean;
                        break;
                    case TallyType.ROfRhoAndOmega:
                        output.R_rw = ((ITally<Complex[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.ROfRhoAndOmega]]).Mean;
                        break;
                    case TallyType.FluenceOfRhoAndZ:
                        output.Flu_rz = ((IHistoryTally<double[,]>)HistoryITallyList[_tallyTypeIndex[TallyType.FluenceOfRhoAndZ]]).Mean;
                        break;
                    case TallyType.AOfRhoAndZ:
                        output.A_rz = ((IHistoryTally<double[,]>)HistoryITallyList[_tallyTypeIndex[TallyType.AOfRhoAndZ]]).Mean;
                        break;
                    case TallyType.TDiffuse:
                        output.Td = ((ITally<double>)TerminationITallyList[_tallyTypeIndex[TallyType.TDiffuse]]).Mean;
                        break;
                    case TallyType.TOfAngle:
                        output.T_a = ((ITally<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.TOfAngle]]).Mean;
                        break;
                    case TallyType.TOfRho:
                        output.T_r = ((ITally<double[]>)TerminationITallyList[_tallyTypeIndex[TallyType.TOfRho]]).Mean;
                        break;
                    case TallyType.TOfRhoAndAngle:
                        output.T_ra = ((ITally<double[,]>)TerminationITallyList[_tallyTypeIndex[TallyType.TOfRhoAndAngle]]).Mean;
                        break;
                }
            }
        }
    }
}
