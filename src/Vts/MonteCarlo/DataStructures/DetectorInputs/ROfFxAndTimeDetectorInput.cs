using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for R(r,t)
    /// </summary>
    public class ROfFxAndTimeDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for reflectance as a function of rho and time detector input
        /// </summary>
        /// <param name="fx">fx sampling points</param>
        /// <param name="time">time binning</param>
        /// <param name="name">detector name</param>
        public ROfFxAndTimeDetectorInput(DoubleRange fx, DoubleRange time, String name)
        {
            TallyType = TallyType.ROfRhoAndTime;
            Name = name;
            Fx = fx;
            Time = time;
        }

        /// <summary>
        /// constructor that uses TallyType for name
        /// </summary>
        /// <param name="fx">fx sampling points</param>
        /// <param name="time">time binning</param>
        public ROfFxAndTimeDetectorInput(DoubleRange fx, DoubleRange time) 
            : this (fx, time, TallyType.ROfRhoAndTime.ToString()) {}

        /// <summary>
        /// Default constructor uses default rho and time bins
        /// </summary>
        public ROfFxAndTimeDetectorInput()
            : this (new DoubleRange(0.0, 0.2, 21), 
                    new DoubleRange(0.0, 1, 101), // time (ns=1000ps)
                    TallyType.ROfFxAndTime.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Fx sampling points (not binned)
        /// </summary>
        public DoubleRange Fx { get; set; }
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
    }
}
