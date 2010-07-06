using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    public class pMCDetectorInput : IDetectorInput 
    {
        public pMCDetectorInput(
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
            List<int> perturbedRegionsIndices)
        {
            TallyTypeList = tallyTypeList;
            Rho = rho;
            Z = z;
            Angle = angle;
            Time = time;
            Omega = omega;
            X = x;
            Y = y;
            AWT = awt;
            ReferenceOps = referenceOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
        }
        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public pMCDetectorInput() : this(
            new List<TallyType>()
                {
                    TallyType.pMuaMusInROfRhoAndTime,
                },
            new DoubleRange(0.0, 10, 101), // rho
            new DoubleRange(0.0, 10, 101),  // z
            new DoubleRange(0.0, Math.PI / 2, 1), // angle
            new DoubleRange(0.0, 10000, 101), // time
            new DoubleRange(0.0, 1000, 21), // omega
            new DoubleRange(-10.0, 10.0, 201), // x
            new DoubleRange(-10.0, 10.0, 201), // y
            AbsorptionWeightingType.Discrete,
            new List<OpticalProperties>() {
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
            new List<int>() { 1 }
        ) {}

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
    }
}
