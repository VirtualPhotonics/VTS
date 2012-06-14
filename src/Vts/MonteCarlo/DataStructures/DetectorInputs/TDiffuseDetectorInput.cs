using System;
using System.Collections.Generic;
using Vts.Common;

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
            TallyType = "TDiffuse";
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        public TDiffuseDetectorInput() : this ("TDiffuse") {}

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be different if user specifies
        /// </summary>
        public string Name { get; set; }
    }
}
