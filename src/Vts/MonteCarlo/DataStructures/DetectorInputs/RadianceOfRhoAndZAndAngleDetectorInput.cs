using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for volume detector R(rho,z,angle)
    /// </summary>
    public class RadianceOfRhoAndZAndAngleDetectorInput : IDetectorInput
    {
        public RadianceOfRhoAndZAndAngleDetectorInput(DoubleRange rho, DoubleRange z, DoubleRange angle, String name)
        {
            TallyType = TallyType.RadianceOfRhoAndZAndAngle;
            Name = name;
            Rho = rho;
            Z = z;
            Angle = angle;
        }
        /// <summary>
        /// constructor uses TallyType as name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="name"></param>
        public RadianceOfRhoAndZAndAngleDetectorInput(DoubleRange rho, DoubleRange z, DoubleRange angle) 
            : this (rho, z, angle, TallyType.RadianceOfRhoAndZAndAngle.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public RadianceOfRhoAndZAndAngleDetectorInput() 
            : this(new DoubleRange(0.0, 10, 101), 
                new DoubleRange(0.0, 10, 101),
                new DoubleRange(0.0, Math.PI / 2, 2),  
                TallyType.RadianceOfRhoAndZAndAngle.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Z { get; set; }
        public DoubleRange Angle { get; set; }
    }
}
