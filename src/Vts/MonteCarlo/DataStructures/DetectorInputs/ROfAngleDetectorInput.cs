using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(angle)
    /// </summary>
    public class ROfAngleDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of angle detector input
        /// </summary>
        /// <param name="angle">angle binning</param>
        /// <param name="name">detector name</param>
        public ROfAngleDetectorInput(DoubleRange angle, String name)
        {
            TallyType = TallyType.ROfAngle;
            Name = name;
            Angle = angle;
        }
        /// <summary>
        /// constructor that uses TallyType as name
        /// </summary>
        /// <param name="angle">angle binning</param>
        public ROfAngleDetectorInput(DoubleRange angle) 
            : this (angle, TallyType.ROfAngle.ToString()) {}

        /// <summary>
        /// Default constructor uses default angle bins
        /// </summary>
        public ROfAngleDetectorInput() 
            : this (new DoubleRange(0.0, Math.PI / 2, 2), 
                    TallyType.ROfAngle.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
    }
}
