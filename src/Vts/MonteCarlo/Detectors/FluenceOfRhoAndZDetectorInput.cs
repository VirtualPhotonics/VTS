using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    public class FluenceOfRhoAndZDetectorInput : IDetectorInput
    {
        public FluenceOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z)
        {
            TallyType = TallyType.FluenceOfRhoAndZ;
            Rho = rho;
            Z = z;
        }

        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public FluenceOfRhoAndZDetectorInput()
            : this(
            new DoubleRange(0.0, 10.0, 101),
            new DoubleRange(0.0, 10.0, 11))
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Z { get; set; }
    }
}
