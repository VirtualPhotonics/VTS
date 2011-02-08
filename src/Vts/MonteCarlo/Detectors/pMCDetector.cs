using System;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Detectors
{
    public class pMCDetector : IDetector
    {
        private ITissue _tissue;
        
        public pMCDetector(
            List<TallyType> tallyTypeList,
            DoubleRange rho,
            DoubleRange z,
            DoubleRange angle,
            DoubleRange time,
            DoubleRange omega,
            DoubleRange x,
            DoubleRange y,
            ITissue tissue,
            List<OpticalProperties> perturbedOps,
            List<int> perturbedRegionsIndices
            ) 
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
            AWT = tissue.AbsorptionWeightingType;
            ReferenceOps = tissue.Regions.Select(r => r.RegionOP).ToList();
            PerturbedOps = perturbedOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
            SetTallyActionLists();
        }
        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public pMCDetector() : this(
            new List<TallyType>()
                {
                    TallyType.pMuaMusInROfRhoAndTime,
                },
            new DoubleRange(0.0, 10, 101), // rho
            new DoubleRange(0.0, 10, 101),  // z
            new DoubleRange(0.0, Math.PI/2, 1), // angle
            new DoubleRange(0.0, 10000, 101), // time
            new DoubleRange(0.0, 1000, 21), // omega
            new DoubleRange(-10.0, 10.0, 201), // x
            new DoubleRange(-10.0, 10.0, 201), // y
            new MultiLayerTissue(),
            new List<OpticalProperties>() {
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
            new List<int>() { 1 }
        ) {}
        public pMCDetector(pMCDetectorInput di, ITissue tissue)
            : this(
                di.TallyTypeList,
                di.Rho,
                di.Z,
                di.Angle,
                di.Time,
                di.Omega,
                di.X,
                di.Y,
                tissue,
                di.ReferenceOps,
                di.PerturbedRegionsIndices
            ) { }
        // do this all have to be properties?
        public IList<ITerminationTally> TerminationITallyList { get; set; }
        public IList<IHistoryTally> HistoryITallyList { get; set; }
        public IList<TallyType> TallyTypeList { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Angle { get; set; }
        public DoubleRange Time { get; set; }
        public DoubleRange Omega { get; set; }
        public DoubleRange X { get; set; }
        public DoubleRange Y { get; set; }
        public DoubleRange Z { get; set; }
        public AbsorptionWeightingType AWT { get; set; }
        public IList<OpticalProperties> ReferenceOps { get; set; }
        public IList<OpticalProperties> PerturbedOps { get; set; }
        public IList<int> PerturbedRegionsIndices { get; set; }
 
        public void SetTallyActionLists()
        {
            TerminationITallyList = new List<ITerminationTally>();
            HistoryITallyList = new List<IHistoryTally>();
            foreach (var tally in TallyTypeList)
            {
                if (Factories.TallyActionFactory.IsHistoryTally(tally))
                {
                    HistoryITallyList.Add(
                        Factories.TallyActionFactory.GetHistoryTallyAction(
                            tally, Rho, Z, Angle, Time, Omega, X, Y,
                            _tissue, PerturbedOps, PerturbedRegionsIndices));
                }
                else
                {
                    TerminationITallyList.Add(
                        Factories.TallyActionFactory.GetTerminationTallyAction(
                            tally, Rho, Z, Angle, Time, Omega, X, Y,
                            _tissue, PerturbedOps, PerturbedRegionsIndices));
                }
            }
        }
        public void TerminationTally(PhotonDataPoint dp)
        {
            foreach (var tally in TerminationITallyList)
            {
                if (tally.ContainsPoint(dp))
                    tally.Tally(dp);
            }
        }
        PhotonDataPoint _previousDP;
        public void HistoryTally(PhotonHistory history)
        {
            bool _firstPoint = true;
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
                        tally.Tally(_previousDP, dp);
                    }
                    _previousDP = dp;
                }
            }
        }

        // pass in Output rather than return because want instance of SimulationInput to be consistent
        public void NormalizeTalliesToOutput(long N, Output output)
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
        public void SetOutputArrays(Output output)
        {
            foreach (var tallyType in TallyTypeList)
            {
                switch (tallyType)
                {
                    default:
                    case TallyType.pMuaMusInROfRhoAndTime:
                        output.R_rt = 
                            ((ITally<double[,]>)HistoryITallyList[TallyTypeList.IndexOf(TallyType.pMuaMusInROfRhoAndTime)]).Mean;
                        break;
                    case TallyType.pMuaMusInROfRho:
                        output.R_r =
                            ((ITally<double[]>)HistoryITallyList[TallyTypeList.IndexOf(TallyType.pMuaMusInROfRho)]).Mean;
                        break;
                }
            }
        }
    }
}
