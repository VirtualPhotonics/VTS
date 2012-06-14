using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for specular reflectance
    /// </summary>
    public class RSpecularDetectorInput : IDetectorInput
    {
        /// <summary>
        /// constructor for specular reflectance detector input
        /// </summary>
        /// <param name="name">detector name</param>
        public RSpecularDetectorInput(String name)
        {
            TallyType = "RSpecular";
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        public RSpecularDetectorInput() : this ("RSpecular") {}

        /// <summary>
        /// detector tally name
        /// </summary>
        public string TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be different if user specifies
        /// </summary>
        public string Name { get; set; }
    }
}
