using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,t)
    /// </summary>
    public class ROfRhoAndTimeDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of rho and time detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="time">time binning</param>
        /// <param name="name">detector name</param>
        public ROfRhoAndTimeDetectorInput(DoubleRange rho, DoubleRange time, String name)
        {
            TallyType = TallyType.ROfRhoAndTime;
            Name = name;
            Rho = rho;
            Time = time;
        }

        /// <summary>
        /// constructor that uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="time">time binning</param>
        public ROfRhoAndTimeDetectorInput(DoubleRange rho, DoubleRange time) 
            : this (rho, time, TallyType.ROfRhoAndTime.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho and time bins
        /// </summary>
        public ROfRhoAndTimeDetectorInput()
            : this (new DoubleRange(0.0, 10, 101), 
                    new DoubleRange(0.0, 1, 101), // time (ns=1000ps)
                    TallyType.ROfRhoAndTime.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// rho binning
        /// </summary>
        public DoubleRange Rho { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
    }
}
