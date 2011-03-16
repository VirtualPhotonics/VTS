using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for Flu(r,z)
    /// </summary>
    public class FluenceOfRhoAndZDetectorInput : IDetectorInput
    {
        public FluenceOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z)
        {
            TallyType = TallyType.FluenceOfRhoAndZ;
            Rho = rho;
            Z = z;
        }

        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public FluenceOfRhoAndZDetectorInput()
            : this(
            new DoubleRange(0.0, 10.0, 101), //rho
            new DoubleRange(0.0, 10.0, 101)) // z
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Z { get; set; }
    }
}
