using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,t)
    /// </summary>
    public class ROfRhoAndTimeDetectorInput : IDetectorInput
    {
        public ROfRhoAndTimeDetectorInput(DoubleRange rho, DoubleRange time)
        {
            TallyType = TallyType.ROfRhoAndTime;
            Rho = rho;
            Time = time;
        }

        /// <summary>
        /// Default constructor uses default rho and time bins
        /// </summary>
        public ROfRhoAndTimeDetectorInput()
            : this(new DoubleRange(0.0, 10, 101), new DoubleRange(0.0, 1, 101)) // time (ns=1000ps)) 
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Time { get; set; }
    }
}
