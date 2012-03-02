using System.Collections.Generic;
using Vts.MonteCarlo;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for TissueInput.
    /// </summary>
    public interface ITissueInput
    {
        /// <summary>
        /// Type of tissue
        /// </summary>
        TissueType TissueType { get; }

        /// <summary>
        /// List of tissue regions comprising tissue.
        /// </summary>
        ITissueRegion[] Regions { get; }
    }
}
