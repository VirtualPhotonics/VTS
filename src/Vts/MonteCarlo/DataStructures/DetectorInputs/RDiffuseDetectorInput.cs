using System;

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
        public RDiffuseDetectorInput(String name)
        {
            TallyType = TallyType.RDiffuse;
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        public RDiffuseDetectorInput() : this (TallyType.RDiffuse.ToString()) {}

        /// <summary>
        /// detector tally identifier
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can different if user specifies it
        /// </summary>
        public String Name { get; set; }
    }
}
