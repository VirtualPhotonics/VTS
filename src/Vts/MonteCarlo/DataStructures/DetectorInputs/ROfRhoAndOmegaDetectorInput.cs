using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,omega)
    /// </summary>
    public class ROfRhoAndOmegaDetectorInput : IDetectorInput
    {
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
        /// <param name="rho"></param>
        /// <param name="omega"></param>
        /// <param name="name"></param>
        public ROfRhoAndOmegaDetectorInput(DoubleRange rho, DoubleRange omega) 
            : this (rho, omega, TallyType.ROfRhoAndOmega.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho and omega bins
        /// </summary>
        public ROfRhoAndOmegaDetectorInput()
            : this (new DoubleRange(0.0, 10, 101), 
                    new DoubleRange(0.0, 1000, 21), 
                    TallyType.ROfRhoAndOmega.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Omega { get; set; }
    }
}
