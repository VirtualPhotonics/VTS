using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for T(r)
    /// </summary>
    public class TOfRhoDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for transmittance as a function of rho detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="name">detector name</param>
        public TOfRhoDetectorInput(DoubleRange rho, String name)
        {
            TallyType = "TOfRho";
            Name = name;
            Rho = rho;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        public TOfRhoDetectorInput(DoubleRange rho) 
            : this (rho, "TOfRho") {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public TOfRhoDetectorInput() 
            : this(new DoubleRange(0.0, 10, 101), "TOfRho") {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specfied
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
    }
}
