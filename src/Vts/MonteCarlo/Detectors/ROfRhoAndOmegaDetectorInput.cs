using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for R(r,omega)
    /// </summary>
    public class ROfRhoAndOmegaDetectorInput : IDetectorInput
    {
        public ROfRhoAndOmegaDetectorInput(DoubleRange rho, DoubleRange omega)
        {
            TallyType = TallyType.ROfRhoAndOmega;
            Rho = rho;
            Omega = omega;
        }

        /// <summary>
        /// Default constructor uses default rho and omega bins
        /// </summary>
        public ROfRhoAndOmegaDetectorInput()
            : this(new DoubleRange(0.0, 10, 101), new DoubleRange(0.0, 1000, 21)) 
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Omega { get; set; }
    }
}
