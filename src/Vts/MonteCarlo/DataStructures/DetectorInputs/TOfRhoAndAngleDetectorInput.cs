using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,angle)
    /// </summary>
    public class TOfRhoAndAngleDetectorInput : IDetectorInput
    {
        public TOfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle, String name)
        {
            TallyType = TallyType.TOfRhoAndAngle;
            Name = name;
            Rho = rho;
            Angle = angle;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="angle"></param>
        /// <param name="name"></param>
        public TOfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle)
        {
            TallyType = TallyType.TOfRhoAndAngle;
            Name = TallyType.TOfRhoAndAngle.ToString();
            Rho = rho;
            Angle = angle;
        }

        /// <summary>
        /// Default constructor uses default rho and angle bins
        /// </summary>
        public TOfRhoAndAngleDetectorInput()
            : this(new DoubleRange(0.0, 10, 101), new DoubleRange(0.0, Math.PI / 2, 2), TallyType.TOfRhoAndAngle.ToString()) 
        {
        }

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
