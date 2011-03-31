using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r)
    /// </summary>
    public class ROfRhoDetectorInput : IDetectorInput
    {
        public ROfRhoDetectorInput(DoubleRange rho, String name)
        {
            TallyType = TallyType.ROfRho;
            Name = name;
            Rho = rho;
        }
        /// <summary>
        /// constructor uses TallyType as name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="name"></param>
        public ROfRhoDetectorInput(DoubleRange rho) 
            : this (rho, TallyType.ROfRho.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public ROfRhoDetectorInput() 
            : this(new DoubleRange(0.0, 10, 101), TallyType.ROfRho.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
    }
}
