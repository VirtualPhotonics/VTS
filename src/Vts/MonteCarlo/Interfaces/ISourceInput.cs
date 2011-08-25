
namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for SourceInput classes.
    /// </summary>
    public interface ISourceInput
    {
        /// <summary>
        /// Type of source
        /// </summary>
        SourceType SourceType { get; set; }

        /// <summary>
        /// Index of region (according to Tissue definition) where photon first starts.
        /// </summary>
        int InitialTissueRegionIndex { get; set; }
    }
}
