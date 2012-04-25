using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Flu(r,z)
    /// </summary>
    public class ReflectedMTOfRhoAndSubRegionHistDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for fluence as a function of rho and z detector input
        /// </summary>
        /// <param name="rho">rho binning</param> 
        /// <param name="mtBins">momentum transfer binning</param>
        /// <param name="name">detector name</param>
        public ReflectedMTOfRhoAndSubRegionHistDetectorInput(DoubleRange rho, DoubleRange mtBins, String name)
        {
            TallyType = TallyType.ReflectedMTOfRhoAndSubRegionHist;
            Name = name;
            Rho = rho;
            MTBins = mtBins;
        }
        /// <summary>
        /// constructor that uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="mtBins">momentum transfer binning</param>
        public ReflectedMTOfRhoAndSubRegionHistDetectorInput(DoubleRange rho, DoubleRange mtBins) 
            : this (rho, mtBins, TallyType.ReflectedMTOfRhoAndSubRegionHist.ToString()) { }

        /// <summary>
        /// Default constructor uses default rho and mt bins
        /// </summary>
        public ReflectedMTOfRhoAndSubRegionHistDetectorInput()
            : this(
                new DoubleRange(0.0, 10.0, 101), //rho
                new DoubleRange(0.0, 300.0, 101), // mt bins
                TallyType.ReflectedMTOfRhoAndSubRegionHist.ToString()) {}

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
        /// momentum transfer binning
        /// </summary>
        public DoubleRange MTBins { get; set; }
    }
}
