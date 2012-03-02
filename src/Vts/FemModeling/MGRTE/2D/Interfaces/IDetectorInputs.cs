using System.Collections.Generic;
using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for DetectorInput classes.
    /// </summary>
    public interface IDetectorInput
    {
        /// <summary>
        /// TallyType enum identifying type of detector
        /// </summary>
        TallyType TallyType { get; set; }
        /// <summary>
        /// Name of detector.  User can define or default is TallyType.ToString().
        /// </summary>
        string Name { get; set; }
    }
}
