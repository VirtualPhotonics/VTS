using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for Rdiffuse
    /// </summary>
    public class RDiffuseDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for diffuse reflectance detector input
        /// </summary>
        /// <param name="name">detector name</param>
        public RDiffuseDetectorInput(string name)
        {
            TallyType = "RDiffuse";
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        public RDiffuseDetectorInput() : this ("RDiffuse") {}

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can different if user specifies it
        /// </summary>
        public string Name { get; set; }
    }
}
