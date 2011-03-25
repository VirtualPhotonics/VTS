using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for DetectorInput classes.
    /// </summary>
    public interface IDetectorInput
    {
       TallyType TallyType { get; set; }
       string Name { get; set; }
    }
}
