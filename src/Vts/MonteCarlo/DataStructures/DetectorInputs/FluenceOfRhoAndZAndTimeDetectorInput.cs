using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z,t)
    /// </summary>
    public class FluenceOfRhoAndZAndTimeDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of rho, z and time detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="time">time binning</param>
        /// <param name="name">detector name</param>
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
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="time">time binning</param>
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

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
    }
}
