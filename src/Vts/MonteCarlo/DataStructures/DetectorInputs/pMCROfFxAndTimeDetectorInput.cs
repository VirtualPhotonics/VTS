using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for pMC R(fx, t)
    /// </summary>
    public class pMCROfFxAndTimeDetectorInput : IpMCDetectorInput 
    {
        /// <summary>
        /// Constructor for perturbation Monte Carlo reflectance as a function of spatial frequency and time
        /// </summary>
        /// <param name="fx">fx binning</param>
        /// <param name="time">time binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>
        /// <param name="name">detector name</param>
        public pMCROfFxAndTimeDetectorInput(
            DoubleRange fx,
            DoubleRange time,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices,
            String name)
        {
            TallyType = "pMCROfFxAndTime";
            Name = name;
            Time = time;
            Fx = fx;
            PerturbedOps = perturbedOps;
            PerturbedRegionsIndices = perturbedRegionsIndices;
        }
        /// <summary>
        /// Constructor uses TallyType for name
        /// </summary>
        /// <param name="fx">fx binning</param>
        /// <param name="time">time binning</param>
        /// <param name="perturbedOps">list of perturbed optical properties, indexing matches tissue indexing</param>
        /// <param name="perturbedRegionsIndices">list of perturbed region indices, indexing matches tissue indexing</param>>
        public pMCROfFxAndTimeDetectorInput(
            DoubleRange fx,
            DoubleRange time,
            IList<OpticalProperties> perturbedOps,
            IList<int> perturbedRegionsIndices) 
                : this (fx, time,
                      perturbedOps, 
                      perturbedRegionsIndices, 
                      "pMCROfFxAndTime") {}

        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public pMCROfFxAndTimeDetectorInput() 
            : this(
                new DoubleRange(0.0, 0.2, 21), // fx
                new DoubleRange(0.0, 1, 101), // time
                new List<OpticalProperties>() {  // perturbedOps
                    new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                    new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                    new OpticalProperties(1e-10, 0.0, 0.0, 1.0) },
                new List<int>() { 1 }, // perturbedRegionIndex
                "pMCROfFxAndTime"
            ) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user-specified
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// fx binning
        /// </summary>
        public DoubleRange Fx { get; set; }
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
