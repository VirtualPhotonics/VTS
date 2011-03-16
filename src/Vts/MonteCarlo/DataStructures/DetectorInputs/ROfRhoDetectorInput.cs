using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for R(r)
    /// </summary>
    public class ROfRhoDetectorInput : IDetectorInput
    {
        public ROfRhoDetectorInput(DoubleRange rho)
        {
            TallyType = TallyType.ROfRho;
            Rho = rho;
        }

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public ROfRhoDetectorInput() : this(new DoubleRange(0.0, 10, 101))
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
    }
}
