using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for DetectorInput classes.
    /// </summary>
    public interface IDetectorInput
    {
        List<TallyType> TallyTypeList { get; set; }
        //List<ITally> TallyList { get; set; }
        DoubleRange Rho { get; set; }
        DoubleRange Angle { get; set; }
        DoubleRange Time { get; set; }
        DoubleRange Omega { get; set; }
        DoubleRange X { get; set; }
        DoubleRange Y { get; set; }
        DoubleRange Z { get; set; }
    }
}
