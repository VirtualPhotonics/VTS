using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for pMC R(r,time).
    /// </summary>
    public class pMCROfRhoAndTimeDetectorInput : IpMCDetectorInput 
    {
        /// <summary>
        /// constructor for pMC reflectance as a function of rho and time detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="time">time binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>
        /// <param name="name">detector name</param>
        public pMCROfRhoAndTimeDetectorInput(
            DoubleRange rho,
            DoubleRange time,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices,
            String name)
        {
            TallyType = TallyType.pMCROfRhoAndTime;
            Name = name;
            Rho = rho;
            Time = time;
            PerturbedOps = perturbedOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="time">time binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed regions indices, indexing matches tissue indexing</param>
        public pMCROfRhoAndTimeDetectorInput(
            DoubleRange rho,
            DoubleRange time,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices) 
            : this (rho, time, perturbedOps, perturbedRegionsIndices, TallyType.pMCROfRhoAndTime.ToString()) {}
        
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
                new List<int>() { 1 }, // perturbedRegionIndex
                TallyType.pMCROfRhoAndTime.ToString()
            ) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
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
