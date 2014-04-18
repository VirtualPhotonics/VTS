using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for pMC R(fx).
    /// </summary>
    public class pMCROfFxDetectorInput : IpMCDetectorInput 
    {
        /// <summary>
        /// constructor for perturbation Monte Carlo reflectance as a function of spatial frequency input
        /// </summary>
        /// <param name="fx">fx binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>
        /// <param name="name">detector name</param>
        public pMCROfFxDetectorInput(
            DoubleRange fx,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices,
            String name)
        {
            TallyType = TallyType.pMCROfFx;
            Name = name;
            Fx = fx;
            PerturbedOps = perturbedOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="fx">fx binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>>
        public pMCROfFxDetectorInput(
            DoubleRange fx,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices) 
                : this (fx, 
                      perturbedOps, 
                      perturbedRegionsIndices, 
                      TallyType.pMCROfFx.ToString()) {}

        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public pMCROfFxDetectorInput() 
            : this(
                new DoubleRange(0.0, 0.2, 21), // fx
                new List<OpticalProperties>() {  // perturbedOps
                    new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                    new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                    new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
                new List<int>() { 1 }, // perturbedRegionIndex
                TallyType.pMCROfFx.ToString()
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
        public DoubleRange Fx { get; set; }
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
