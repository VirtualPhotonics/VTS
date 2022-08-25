
using System;

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
        string SourceType { get; set; }

        /// <summary>
        /// Index of region (according to Tissue definition) where photon first starts.
        /// </summary>
        int InitialTissueRegionIndex { get; set; }

        /// <summary>
        /// Required code to create a source based on the input values
        /// </summary>
        /// <param name="rng">random number generator Random</param>
        /// <returns>source class that implements ISource</returns>
        ISource CreateSource(Random rng = null);
    }
}
