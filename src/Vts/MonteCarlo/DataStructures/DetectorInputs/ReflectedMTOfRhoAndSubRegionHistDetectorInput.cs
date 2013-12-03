using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z)
    /// </summary>
    public class ReflectedMTOfRhoAndSubregionHistDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of rho and z detector input
        /// </summary>
        /// <param name="rho">rho binning</param> 
        /// <param name="mtBins">momentum transfer binning</param>
        /// <param name="name">detector name</param>
        public ReflectedMTOfRhoAndSubregionHistDetectorInput(DoubleRange rho, DoubleRange mtBins, DoubleRange fractionalMTBins, String name)
        {
            TallyType = "ReflectedMTOfRhoAndSubregionHist";
            Name = name;
            Rho = rho;
            MTBins = mtBins;
            FractionalMTBins = fractionalMTBins;
        }
        /// <summary>
        /// constructor that uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="mtBins">momentum transfer binning</param>
        public ReflectedMTOfRhoAndSubregionHistDetectorInput(DoubleRange rho, DoubleRange mtBins, DoubleRange fractionalMTBins) 
            : this (rho, mtBins, fractionalMTBins, "ReflectedMTOfRhoAndSubregionHist") { }

        /// <summary>
        /// Default constructor uses default rho and mt bins
        /// </summary>
        public ReflectedMTOfRhoAndSubregionHistDetectorInput()
            : this(
                new DoubleRange(0.0, 10.0, 101), //rho
                new DoubleRange(0.0, 300.0, 101), // mt bins
                new DoubleRange(0.0, 1.0, 11), // fractional mt bins
                "ReflectedMTOfRhoAndSubregionHist") {}

        /// <summary>
        /// detector identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, defaults to TallyType.ToString() but can be user specified
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// detector rho binning
        /// </summary>
        public DoubleRange Rho { get; set; } 
        /// <summary>
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
        /// <summary>
        /// fractional momentum transfer binning
        /// </summary>
        public DoubleRange FractionalMTBins { get; set; }
    }
}
