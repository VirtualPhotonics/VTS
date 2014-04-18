using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for pMC R(r).
    /// </summary>
    public class pMCROfRhoDetectorInput : IpMCDetectorInput 
    {
        /// <summary>
        /// constructor for perturbation Monte Carlo reflectance as a function of rho detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>
        /// <param name="name">detector name</param>
        public pMCROfRhoDetectorInput(
            DoubleRange rho,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices,
            String name)
        {
            TallyType = TallyType.pMCROfRho;
            Name = name;
            Rho = rho;
            PerturbedOps = perturbedOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>>
        public pMCROfRhoDetectorInput(
            DoubleRange rho,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices) 
                : this (rho, 
                      perturbedOps, 
                      perturbedRegionsIndices, 
                      TallyType.pMCROfRho.ToString()) {}

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
                new List<int>() { 1 }, // perturbedRegionIndex
                TallyType.pMCROfRho.ToString()
            ) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user-specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// list of perturbed optical properties, indexing matches tissue indexing
        /// </summary>
        public IList<OpticalProperties> PerturbedOps { get; set; }
        /// <summary>
        /// list of perturbed region indices, indexing matches tissue indexing
        /// </summary>
        public IList<int> PerturbedRegionsIndices { get; set; }
    }
}
