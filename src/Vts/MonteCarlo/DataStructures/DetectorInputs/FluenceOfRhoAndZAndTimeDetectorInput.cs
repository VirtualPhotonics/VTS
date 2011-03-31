using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z,t)
    /// </summary>
    public class FluenceOfRhoAndZAndTimeDetectorInput : IDetectorInput
    {
        public FluenceOfRhoAndZAndTimeDetectorInput(
            DoubleRange rho, DoubleRange z, DoubleRange time, String name)
        {
            TallyType = TallyType.FluenceOfRhoAndZAndTime;
            Name = name;
            Rho = rho;
            Z = z;
            Time = time;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="z"></param>
        /// <param name="time"></param>
        public FluenceOfRhoAndZAndTimeDetectorInput(
            DoubleRange rho, DoubleRange z, DoubleRange time) 
            : this (rho, z, time, TallyType.FluenceOfRhoAndZAndTime.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public FluenceOfRhoAndZAndTimeDetectorInput() 
            : this(
                new DoubleRange(0.0, 10.0, 101), // rho
                new DoubleRange(0.0, 10.0, 101), // z
                new DoubleRange(0.0, 1, 101), // time
                TallyType.FluenceOfRhoAndZAndTime.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
        public DoubleRange Rho { get; set; }
        public DoubleRange Z { get; set; }
        public DoubleRange Time { get; set; }
    }
}
