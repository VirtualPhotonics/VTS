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
        public TDiffuseDetectorInput(String name)
        {
            TallyType = TallyType.TDiffuse;
            Name = name;
        }
        /// <summary>
        /// default constructor uses TallyType as name
        /// </summary>
        /// <param name="name"></param>
        public TDiffuseDetectorInput()
        {
            TallyType = TallyType.TDiffuse;
            Name = TallyType.TDiffuse.ToString();
        }

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
    }
}
