using System;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Absorption(r,z)
    /// </summary>
    public class AOfRhoAndZDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for absorbed energy as a function of rho and z detector input
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        /// <param name="name">detector name</param>
        public AOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z, String name)
        {
            TallyType = TallyType.AOfRhoAndZ;
            Name = name;
            Rho = rho;
            Z = z;
        }
        /// <summary>
        /// constructor uses TallyType for name
        /// </summary>
        /// <param name="rho">rho binning</param>
        /// <param name="z">z binning</param>
        public AOfRhoAndZDetectorInput(DoubleRange rho, DoubleRange z) 
            : this(rho, z, TallyType.AOfRhoAndZ.ToString()) { }

        /// <summary>
        /// Default constructor uses default rho and z bins
        /// </summary>
        public AOfRhoAndZDetectorInput() 
            : this(
                new DoubleRange(0.0, 10.0, 101), //rho
                new DoubleRange(0.0, 10.0, 101), // z
                TallyType.AOfRhoAndZ.ToString()) { }

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
        /// z binning
        /// </summary>
        public DoubleRange Z { get; set; }
    }
}
