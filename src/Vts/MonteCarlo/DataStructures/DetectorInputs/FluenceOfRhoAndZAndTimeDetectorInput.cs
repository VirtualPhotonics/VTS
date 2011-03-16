using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z,t)
    /// </summary>
    public class FluenceOfRhoAndZAndTimeDetectorInput : IDetectorInput
    {
        public FluenceOfRhoAndZAndTimeDetectorInput(DoubleRange rho, DoubleRange z, DoubleRange time)
        {
            TallyType = TallyType.FluenceOfRhoAndZAndTime;
            Rho = rho;
            Z = z;
            Time = time;
        }

        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public FluenceOfRhoAndZAndTimeDetectorInput()
            : this(
            new DoubleRange(0.0, 10.0, 101), // rho
            new DoubleRange(0.0, 10.0, 101), // z
            new DoubleRange(0.0, 1, 101))   // time
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Z { get; set; }
        public DoubleRange Time { get; set; }
    }
}
