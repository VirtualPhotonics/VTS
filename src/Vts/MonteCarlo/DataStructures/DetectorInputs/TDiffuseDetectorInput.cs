using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Tdiffuse
    /// </summary>
    public class TDiffuseDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for diffuse transmittance detector input
        /// </summary>
        /// <param name="name">detector name</param>
        public TDiffuseDetectorInput(String name)
        {
            TallyType = TallyType.TDiffuse;
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        public TDiffuseDetectorInput() : this (TallyType.TDiffuse.ToString()) {}

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be different if user specifies
        /// </summary>
        public String Name { get; set; }
    }
}
