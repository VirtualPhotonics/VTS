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
        /// <summary>
        /// constructor for reflectance as a function of angle detector input
        /// </summary>
        /// <param name="angle">angle binning</param>
        /// <param name="name">detector name</param>
        public ROfAngleDetectorInput(DoubleRange angle, String name)
        {
            TallyType = "ROfAngle";
            Name = name;
            Angle = angle;
        }
        /// <summary>
        /// constructor that uses TallyType as name
        /// </summary>
        /// <param name="angle">angle binning</param>
        public ROfAngleDetectorInput(DoubleRange angle) 
            : this (angle, "ROfAngle") {}

        /// <summary>
        /// Default constructor uses default angle bins
        /// </summary>
        public ROfAngleDetectorInput() 
            : this (new DoubleRange(0.0, Math.PI / 2, 2), 
                    "ROfAngle") {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
    }
}
