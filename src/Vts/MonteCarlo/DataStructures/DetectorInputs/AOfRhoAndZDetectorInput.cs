using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Absorption(r,z)
    /// </summary>
    public class AOfRhoAndZDetectorInput : IDetectorInput
    {
        public AOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z, String name)
        {
            TallyType = TallyType.AOfRhoAndZ;
            Name = name;
            Rho = rho;
            Z = z;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="z"></param>
        /// <param name="name"></param>
        public AOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z)
        {
            TallyType = TallyType.AOfRhoAndZ;
            Name = TallyType.AOfRhoAndZ.ToString();
            Rho = rho;
            Z = z;
        }
        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public AOfRhoAndZDetectorInput()
            : this(
            new DoubleRange(0.0, 10.0, 101), //rho
            new DoubleRange(0.0, 10.0, 101), // z
            TallyType.AOfRhoAndZ.ToString()) 
        {
        }

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Z { get; set; }
    }
}
