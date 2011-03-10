using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    public class ROfRhoAndTDetectorInput : IDetectorInput
    {
        public ROfRhoAndTDetectorInput(DoubleRange rho, DoubleRange time)
        {
            TallyType = TallyType.ROfRho;
            Rho = rho;
            Time = time;
        }

        /// <summary>
        /// Default constructor tallies all tallies
        /// </summary>
        public ROfRhoAndTDetectorInput()
            : this(new DoubleRange(0.0, 10, 101), new DoubleRange(0.0, 4.0, 801)) // todo: check time defaults
        {
        }

        public TallyType TallyType { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Time { get; set; }
    }
}
