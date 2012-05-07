using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z)
    /// </summary>
    public class ReflectedTimeOfRhoAndSubregionHistDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of rho and z detector input
        /// </summary>
        /// <param name="rho">rho binning</param> 
        /// <param name="mtBins">momentum transfer binning</param>
        /// <param name="name">detector name</param>
        public ReflectedTimeOfRhoAndSubregionHistDetectorInput(DoubleRange rho, DoubleRange time, String name)
        {
            TallyType = TallyType.ReflectedTimeOfRhoAndSubregionHist;
            Name = name;
            Rho = rho;
            Time = time;
        }
        /// <summary>
        /// constructor that uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="time">time binning</param>
        public ReflectedTimeOfRhoAndSubregionHistDetectorInput(DoubleRange rho, DoubleRange time) 
            : this (rho, time, TallyType.ReflectedTimeOfRhoAndSubregionHist.ToString()) { }

        /// <summary>
        /// Default constructor uses default rho and mt bins
        /// </summary>
        public ReflectedTimeOfRhoAndSubregionHistDetectorInput()
            : this(
                new DoubleRange(0.0, 10.0, 101), //rho
                new DoubleRange(0.0, 1.0, 101), // time bins
                TallyType.ReflectedTimeOfRhoAndSubregionHist.ToString()) {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; } 
        /// <summary>
        /// time binning
        /// </summary>
        public DoubleRange Time { get; set; }
    }
}
