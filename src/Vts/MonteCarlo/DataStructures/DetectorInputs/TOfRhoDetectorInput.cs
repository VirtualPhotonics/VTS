using System;
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
            TallyType = TallyType.TOfRho;
            Name = name;
            Rho = rho;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        public TOfRhoDetectorInput(DoubleRange rho) 
            : this (rho, TallyType.TOfRho.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public TOfRhoDetectorInput() 
            : this(new DoubleRange(0.0, 10, 101), TallyType.TOfRho.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specfied
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
    }
}
