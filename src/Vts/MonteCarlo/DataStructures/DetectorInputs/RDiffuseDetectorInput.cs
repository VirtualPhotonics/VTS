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
        public RDiffuseDetectorInput(String name)
        {
            TallyType = TallyType.RDiffuse;
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        /// <param name="name"></param>
        public RDiffuseDetectorInput() : this (TallyType.RDiffuse.ToString()) {}

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
    }
}
