using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for T(angle)
    /// </summary>
    public class TOfAngleDetectorInput : IDetectorInput
    {
        public TOfAngleDetectorInput(DoubleRange angle, String name)
        {
            TallyType = TallyType.TOfAngle;
            Name = name;
            Angle = angle;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="name"></param>
        public TOfAngleDetectorInput(DoubleRange angle) 
            : this (angle, TallyType.TOfAngle.ToString()) {}

        /// <summary>
        /// Default constructor uses default angle bins
        /// </summary>
        public TOfAngleDetectorInput() 
            : this (new DoubleRange(0.0, Math.PI / 2, 2), 
                    TallyType.TOfAngle.ToString())  {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
