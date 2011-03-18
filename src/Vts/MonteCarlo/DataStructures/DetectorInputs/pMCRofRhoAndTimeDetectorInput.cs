using System;
using System.Linq;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for pMC R(r,time).
    /// </summary>
    public class pMCROfRhoAndTimeDetectorInput : IpMCDetectorInput 
    {
        public pMCROfRhoAndTimeDetectorInput(
            DoubleRange rho,
            DoubleRange time,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices)
        {
            TallyType = TallyType.pMCMuaMusInROfRhoAndTime;
            Rho = rho;
            Time = time;
            PerturbedOps = perturbedOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
        }
        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public pMCROfRhoAndTimeDetectorInput()
            : this(
            new DoubleRange(0.0, 10, 101), // rho
            new DoubleRange(0.0, 1, 101), // time
            new List<OpticalProperties>() {  // perturbedOps
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
            new List<int>() { 1 } // perturbedRegionIndex
        ) {}

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Time { get; set; }
        public IList<OpticalProperties> PerturbedOps { get; set; }
        public IList<int> PerturbedRegionsIndices { get; set; }
    }
}
