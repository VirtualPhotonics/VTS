using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines a contract for TissueInput.
    /// </summary>
    public interface IInclusionInput
    {
        /// <summary>
        /// Type of tissue
        /// </summary>
        TissueType TissueType { get; }

        /// <summary>
        /// List of tissue regions comprising tissue.
        /// </summary>
        IInclusionRegion[] Regions { get; }
    }
}
