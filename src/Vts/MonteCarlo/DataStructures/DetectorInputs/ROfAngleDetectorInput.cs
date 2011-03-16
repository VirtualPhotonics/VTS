using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(angle)
    /// </summary>
    public class ROfAngleDetectorInput : IDetectorInput
    {
        public ROfAngleDetectorInput(DoubleRange angle)
        {
            TallyType = TallyType.ROfAngle;
            Angle = angle;
        }

        /// <summary>
        /// Default constructor uses default angle bins
        /// </summary>
        public ROfAngleDetectorInput()
            : this(new DoubleRange(0.0, Math.PI / 2, 2))
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
