using Vts.MonteCarlo;

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
        InclusionType InclusionType { get; }

        /// <summary>
        /// List of tissue regions comprising tissue.
        /// </summary>
        ITissueRegion[] Regions { get; }
    }
}
