using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for R(r,angle)
    /// </summary>
    public class TOfRhoAndAngleDetectorInput : IDetectorInput
    {
        public TOfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle)
        {
            TallyType = TallyType.TOfRhoAndAngle;
            Rho = rho;
            Angle = angle;
        }

        /// <summary>
        /// Default constructor uses default rho and angle bins
        /// </summary>
        public TOfRhoAndAngleDetectorInput()
            : this(new DoubleRange(0.0, 10, 101), new DoubleRange(0.0, Math.PI / 2, 2)) 
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
