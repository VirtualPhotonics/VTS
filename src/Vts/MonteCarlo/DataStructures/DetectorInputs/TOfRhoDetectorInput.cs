using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for T(r)
    /// </summary>
    public class TOfRhoDetectorInput : IDetectorInput
    {
        public TOfRhoDetectorInput(DoubleRange rho, String name)
        {
            TallyType = TallyType.TOfRho;
            Name = name;
            Rho = rho;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="name"></param>
        public TOfRhoDetectorInput(DoubleRange rho) 
            : this (rho, TallyType.TOfRho.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho bins
        /// </summary>
        public TOfRhoDetectorInput() 
            : this(new DoubleRange(0.0, 10, 101), TallyType.TOfRho.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
    }
}
