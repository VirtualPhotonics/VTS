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
        public TDiffuseDetectorInput()
        {
            TallyType = TallyType.TDiffuse;
        }

        public TallyType TallyType { get; set; }
    }
}
