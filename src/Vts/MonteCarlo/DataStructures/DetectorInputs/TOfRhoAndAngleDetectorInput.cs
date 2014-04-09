using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,angle)
    /// </summary>
    public class TOfRhoAndAngleDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for transmittance as a functino of rho and angle detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="angle">angle binning</param>
        /// <param name="name">detector name</param>
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
        /// <param name="rho">rho binning</param>
        /// <param name="angle">angle binning</param>
        public TOfRhoAndAngleDetectorInput(DoubleRange rho, DoubleRange angle) 
            : this (rho, angle, TallyType.TOfRhoAndAngle.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho and angle bins
        /// </summary>
        public TOfRhoAndAngleDetectorInput() 
            : this (new DoubleRange(0.0, 10, 101), 
                    new DoubleRange(0.0, Math.PI / 2, 2), 
                    TallyType.TOfRhoAndAngle.ToString()) {}


        /// <summary>
        /// detector tally identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// detector angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
    }
}
