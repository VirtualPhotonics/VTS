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
        public RDiffuseDetectorInput()
        {
            TallyType = TallyType.RDiffuse;
        }  

        public TallyType TallyType { get; set; }
    }
}
