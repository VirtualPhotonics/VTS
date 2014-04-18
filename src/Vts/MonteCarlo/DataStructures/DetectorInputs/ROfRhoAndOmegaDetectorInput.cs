using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,omega)
    /// </summary>
    public class ROfRhoAndOmegaDetectorInput : IDetectorInput
    {
        /// <summary>
        /// Constructor for reflectance as a function of rho and omega detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="omega">temporal frequency sampling points (not binned)</param>
        /// <param name="name">detector name</param>
        public ROfRhoAndOmegaDetectorInput(DoubleRange rho, DoubleRange omega, String name)
        {
            TallyType = TallyType.ROfRhoAndOmega;
            Name = name;
            Rho = rho;
            Omega = omega;
        }
        /// <summary>
        /// constructor uses TallyType as name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="omega">temporal frequency sampling points (not binned)</param>
        public ROfRhoAndOmegaDetectorInput(DoubleRange rho, DoubleRange omega) 
            : this (rho, omega, TallyType.ROfRhoAndOmega.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins and omega sampling points
        /// </summary>
        public ROfRhoAndOmegaDetectorInput()
            : this (new DoubleRange(0.0, 10, 101), 
                    new DoubleRange(0.0, 1000, 21), 
                    TallyType.ROfRhoAndOmega.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// detector temporal-frequency (omega) sampling points (not binned)
        /// </summary>
        public DoubleRange Omega { get; set; }
    }
}
