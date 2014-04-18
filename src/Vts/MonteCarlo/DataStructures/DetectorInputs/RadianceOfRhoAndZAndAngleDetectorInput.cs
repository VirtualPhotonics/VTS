using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for volume detector R(rho,z,angle)
    /// </summary>
    public class RadianceOfRhoAndZAndAngleDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for radiance that passes rho, z, angle, name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="angle">angle binning</param>
        /// <param name="name">detector name</param>
        public RadianceOfRhoAndZAndAngleDetectorInput(DoubleRange rho, DoubleRange z, DoubleRange angle, String name)
        {
            TallyType = TallyType.RadianceOfRhoAndZAndAngle;
            Name = name;
            Rho = rho;
            Z = z;
            Angle = angle;
        }
        /// <summary>
        /// Constructor for radiance(rho, z, angle) that doesn't pass name and uses TallyType.ToString() for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="angle">angle binning</param>
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

        /// <summary>
        /// detector identifier
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
        /// detector z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// detector angle binning
        /// </summary>
        public DoubleRange Angle { get; set; }
    }
}
