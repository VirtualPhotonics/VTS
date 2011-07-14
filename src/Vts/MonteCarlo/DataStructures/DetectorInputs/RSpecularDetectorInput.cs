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
        public RSpecularDetectorInput(String name)
        {
            TallyType = TallyType.RSpecular;
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        /// <param name="name"></param>
        public RSpecularDetectorInput() : this (TallyType.RSpecular.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
    }
}
