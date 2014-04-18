using System;

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
            TallyType = TallyType.RSpecular;
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        public RSpecularDetectorInput() : this (TallyType.RSpecular.ToString()) {}

        /// <summary>
        /// detector tally name
        /// </summary>
        public TallyType TallyType { get; set; }
        /// <summary>
        /// detector name, default uses TallyType, but can be different if user specifies
        /// </summary>
        public String Name { get; set; }
    }
}
