using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r)
    /// </summary>
    public class DosimetryOfRhoDetectorInput : IDetectorInput
    {
        public DosimetryOfRhoDetectorInput(double zDepth, DoubleRange rho, String name)
        {
            TallyType = TallyType.DosimetryOfRho;
            Name = name;
            Rho = rho;
            ZDepth = zDepth;
        }
        /// <summary>
        /// constructor uses TallyType as name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="name"></param>
        public DosimetryOfRhoDetectorInput(double zDepth, DoubleRange rho) 
            : this (zDepth, rho, TallyType.DosimetryOfRho.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public DosimetryOfRhoDetectorInput() 
            : this(5.5, new DoubleRange(0.0, 10, 101), TallyType.DosimetryOfRho.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public double ZDepth { get; set; }
    }
}
