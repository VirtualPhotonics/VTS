using System;
using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// DetectorInput for total absorption
    /// </summary>
    public class ATotalDetectorInput : IDetectorInput
    {
        public ATotalDetectorInput(String name)
        {
            TallyType = TallyType.ATotal;
            Name = name;
        }
        public ATotalDetectorInput() : this(TallyType.ATotal.ToString()) { }

        public TallyType TallyType { get; set; }
        public String Name { get; set; }
    }
}
