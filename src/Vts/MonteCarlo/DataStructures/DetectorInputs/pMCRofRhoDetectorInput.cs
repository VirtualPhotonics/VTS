using System;
using System.Linq;
using System.Collections.Generic;
using Vts.Common;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for pMC R(r).
    /// </summary>
    public class pMCROfRhoDetectorInput : IpMCDetectorInput 
    {
        public pMCROfRhoDetectorInput(
            DoubleRange rho,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices)
        {
            TallyType = TallyType.pMCMuaMusInROfRho;
            Rho = rho;
            PerturbedOps = perturbedOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
        }
        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public pMCROfRhoDetectorInput()
            : this(
            new DoubleRange(0.0, 10, 101), // rho
            new List<OpticalProperties>() {  // perturbedOps
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
            new List<int>() { 1 } // perturbedRegionIndex
        ) {}

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public IList<OpticalProperties> PerturbedOps { get; set; }
        public IList<int> PerturbedRegionsIndices { get; set; }
    }
}
