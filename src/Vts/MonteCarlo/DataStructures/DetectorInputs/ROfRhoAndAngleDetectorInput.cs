using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,angle)
    /// </summary>
    public class ROfRhoAndAngleDetectorInput : IDetectorInput
    { 
        /// <summary>
        /// constructor for reflectance as a function of rho and angle detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="angle">angle binning</param>
        /// <param name="name">detector name</param>
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
        /// <param name="rho">rho binning</param>
        /// <param name="angle">angle binning</param>
        public ROfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle) 
            : this (rho, angle, TallyType.ROfRhoAndAngle.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho and angle bins
        /// </summary>
        public ROfRhoAndAngleDetectorInput() 
            : this (new DoubleRange(0.0, 10, 101), 
                  new DoubleRange(0.0, Math.PI / 2, 2), 
                  TallyType.ROfRhoAndAngle.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
    }
}
