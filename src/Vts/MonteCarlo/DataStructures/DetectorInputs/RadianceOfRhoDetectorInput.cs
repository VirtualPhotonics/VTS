using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for surface detector Radiance(rho)
    /// </summary>
    public class RadianceOfRhoDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for surface radiance as a function of rho detector input.  
        /// Surface defined as a plane with constant z.
        /// </summary>
        /// <param name="zDepth">plane definition z value</param>
        /// <param name="rho">rho binning</param>
        /// <param name="name">detector name</param>
        public RadianceOfRhoDetectorInput(double zDepth, DoubleRange rho, String name)
        {
            TallyType = TallyType.RadianceOfRho;
            Name = name;
            Rho = rho;
            ZDepth = zDepth;
        }
        /// <summary>
        /// constructor uses TallyType as name
        /// </summary>
        /// <param name="zDepth">defines z plane tally surface</param>
        /// <param name="rho">rho binning on z plane surface</param>
        public RadianceOfRhoDetectorInput(double zDepth, DoubleRange rho) 
            : this (zDepth, rho, TallyType.RadianceOfRho.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public RadianceOfRhoDetectorInput() 
            : this(5.5, new DoubleRange(0.0, 10, 101), TallyType.RadianceOfRho.ToString()) {}

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
        /// z constant specifying plane of surface radiance
        /// </summary>
        public double ZDepth { get; set; }
    }
}
