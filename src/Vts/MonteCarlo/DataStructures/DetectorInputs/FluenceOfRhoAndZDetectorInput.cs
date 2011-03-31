using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z)
    /// </summary>
    public class FluenceOfRhoAndZDetectorInput : IDetectorInput
    {
        public FluenceOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z, String name)
        {
            TallyType = TallyType.FluenceOfRhoAndZ;
            Name = name;
            Rho = rho;
            Z = z;
        }
        /// <summary>
        /// constructor that uses TallyType for name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="z"></param>
        public FluenceOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z) 
            : this (rho, z, TallyType.FluenceOfRhoAndZ.ToString()) { }

        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public FluenceOfRhoAndZDetectorInput()
            : this(
                new DoubleRange(0.0, 10.0, 101), //rho
                new DoubleRange(0.0, 10.0, 101), // z
                TallyType.FluenceOfRhoAndZ.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Z { get; set; }
    }
}
