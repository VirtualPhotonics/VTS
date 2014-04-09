using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Time(angle)
    /// </summary>
    public class TOfAngleDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for transmittance as a function of angle detector input
        /// </summary>
        /// <param name="angle">angle binning</param>
        /// <param name="name">detector name</param>
        public TOfAngleDetectorInput(DoubleRange angle, String name)
        {
            TallyType = TallyType.TOfAngle;
            Name = name;
            Angle = angle;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="angle">angle binning</param>
        public TOfAngleDetectorInput(DoubleRange angle) 
            : this (angle, TallyType.TOfAngle.ToString()) {}

        /// <summary>
        /// Default constructor uses default angle bins
        /// </summary>
        public TOfAngleDetectorInput() 
            : this (new DoubleRange(0.0, Math.PI / 2, 2), 
                    TallyType.TOfAngle.ToString())  {}

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
    }
}
