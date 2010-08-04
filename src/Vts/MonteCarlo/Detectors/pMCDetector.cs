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
            AbsorptionWeightingType awt,
            List<OpticalProperties> referenceOps,
            List<int> perturbedRegionsIndices,
            ITissue tissue
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
            AWT = awt;
            ReferenceOps = referenceOps;
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
            AbsorptionWeightingType.Discrete,
            new List<OpticalProperties>() {
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
            new List<int>() { 1 },
            new MultiLayerTissue()
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
                di.AWT,
                di.ReferenceOps,
                di.PerturbedRegionsIndices,
                tissue
            ) { }

        //public List<ITally> ITallyList { get; set; }
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
        public AbsorptionWeightingType AWT { get; set; }
        public List<OpticalProperties> ReferenceOps { get; set; }
        public List<int> PerturbedRegionsIndices { get; set; }
 
        public void SetTallyActionLists()
        {
            TerminationITallyList = new List<ITally>();
            HistoryITallyList = new List<IHistoryTally>();
            foreach (var tally in TallyTypeList)
            {
                if (Factories.TallyActionFactory.IsHistoryTally(tally))
                {
                    HistoryITallyList.Add(Factories.TallyActionFactory.GetHistoryTallyAction(tally, Rho, Z, Angle, Time, Omega, X, Y,
                        AWT, ReferenceOps, PerturbedRegionsIndices));
                }
                else
                {
                    TerminationITallyList.Add(Factories.TallyActionFactory.GetTallyAction(tally, Rho, Z, Angle, Time, Omega, X, Y,
                        AWT, ReferenceOps, PerturbedRegionsIndices));
                }
            }
        }
        public void TerminationTally(PhotonDataPoint dp)
        {
            foreach (var tally in TerminationITallyList)
            {
                if (tally.ContainsPoint(dp))
                    tally.Tally(dp, _tissue.Regions.Select(s => s.RegionOP).ToList());
            }
        }
        public void HistoryTally(PhotonHistory history)
        {
            foreach (var tally in HistoryITallyList)
            {
                // to fill in 
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
                            ((ITally<double[,]>)TerminationITallyList[TallyTypeList.IndexOf(TallyType.pMuaMusInROfRhoAndTime)]).Mean;
                        break;
                    case TallyType.pMuaMusInROfRho:
                        output.R_r =
                            ((ITally<double[]>)TerminationITallyList[TallyTypeList.IndexOf(TallyType.pMuaMusInROfRho)]).Mean;
                        break;
                }
            }
        }
    }
}
