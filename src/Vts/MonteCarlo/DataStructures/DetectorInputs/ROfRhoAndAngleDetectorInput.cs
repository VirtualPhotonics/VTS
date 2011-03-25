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
        public ROfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle, String name)
        {
            TallyType = TallyType.ROfRhoAndAngle;
            Name = name;
            Rho = rho;
            Angle = angle;
        }
        /// <summary>
        /// constructor uses TallyType as name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="angle"></param>
        /// <param name="name"></param>
        public ROfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle)
        {
            TallyType = TallyType.ROfRhoAndAngle;
            Name = TallyType.ROfRhoAndAngle.ToString();
            Rho = rho;
            Angle = angle;
        }

        /// <summary>
        /// Default constructor uses default rho and angle bins
        /// </summary>
        public ROfRhoAndAngleDetectorInput()
            : this(new DoubleRange(0.0, 10, 101), new DoubleRange(0.0, Math.PI / 2, 2), TallyType.ROfRhoAndAngle.ToString()) 
        {
        }

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
