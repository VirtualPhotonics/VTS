using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,angle)
    /// </summary>
    public class ROfRhoAndAngleDetectorInput : IDetectorInput
    {
        public ROfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle)
        {
            TallyType = TallyType.ROfRhoAndAngle;
            Rho = rho;
            Angle = angle;
        }

        /// <summary>
        /// Default constructor uses default rho and angle bins
        /// </summary>
        public ROfRhoAndAngleDetectorInput()
            : this(new DoubleRange(0.0, 10, 101), new DoubleRange(0.0, Math.PI / 2, 2)) 
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
