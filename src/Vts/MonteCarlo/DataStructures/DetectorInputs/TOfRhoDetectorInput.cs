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
        public TOfRhoDetectorInput(DoubleRange rho)
        {
            TallyType = TallyType.TOfRho;
            Rho = rho;
        }

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public TOfRhoDetectorInput() : this(new DoubleRange(0.0, 10, 101))
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
    }
}
