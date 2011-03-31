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
        public ROfAngleDetectorInput(DoubleRange angle, String name)
        {
            TallyType = TallyType.ROfAngle;
            Name = name;
            Angle = angle;
        }
        /// <summary>
        /// constructor that uses TallyType as name
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="name"></param>
        public ROfAngleDetectorInput(DoubleRange angle) 
            : this (angle, TallyType.ROfAngle.ToString()) {}

        /// <summary>
        /// Default constructor uses default angle bins
        /// </summary>
        public ROfAngleDetectorInput() 
            : this (new DoubleRange(0.0, Math.PI / 2, 2), 
                    TallyType.ROfAngle.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
