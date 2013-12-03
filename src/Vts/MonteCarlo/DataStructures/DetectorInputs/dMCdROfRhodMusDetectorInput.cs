using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for dMC dR(r)/dMus.
    /// </summary>
    public class dMCdROfRhodMusDetectorInput : IpMCDetectorInput 
    {
        /// <summary>
        /// constructor for differential Monte Carlo reflectance with respect to mus, as a function of rho detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>
        /// <param name="name">detector name</param>
        public dMCdROfRhodMusDetectorInput(
            DoubleRange rho,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices,
            String name)
        {
            TallyType = TallyType.dMCdROfRhodMus;
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
        public dMCdROfRhodMusDetectorInput(
            DoubleRange rho,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices) 
                : this (rho, 
                      perturbedOps, 
                      perturbedRegionsIndices, 
                      "pMCROfRho") {}

        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public dMCdROfRhodMusDetectorInput() 
            : this(
                new DoubleRange(0.0, 10, 101), // rho
                new List<OpticalProperties>() {  // perturbedOps
                    new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                    new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                    new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
                new List<int>() { 1 }, // perturbedRegionIndex
                "pMCROfRho"
            ) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
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
