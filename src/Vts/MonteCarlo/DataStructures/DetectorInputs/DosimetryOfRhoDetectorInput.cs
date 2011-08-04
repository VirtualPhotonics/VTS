using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r)
    /// </summary>
    public class RadianceOfRhoDetectorInput : IDetectorInput
    {
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
        /// <param name="rho"></param>
        /// <param name="name"></param>
        public RadianceOfRhoDetectorInput(double zDepth, DoubleRange rho) 
            : this (zDepth, rho, TallyType.RadianceOfRho.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public RadianceOfRhoDetectorInput() 
            : this(5.5, new DoubleRange(0.0, 10, 101), TallyType.RadianceOfRho.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public double ZDepth { get; set; }
    }
}
