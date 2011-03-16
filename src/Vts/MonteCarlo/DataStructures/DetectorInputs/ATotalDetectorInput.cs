using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// DetectorInput for total absorption
    /// </summary>
    public class ATotalDetectorInput : IDetectorInput
    {
        public ATotalDetectorInput()
        {
            TallyType = TallyType.ATotal;
        }  

        public TallyType TallyType { get; set; }
    }
}
