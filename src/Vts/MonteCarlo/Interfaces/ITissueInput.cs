using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for TissueInput.
    /// </summary>
    public interface ITissueInput
    {
        /// <summary>
        /// List of tissue regions comprising tissue.
        /// </summary>
        IList<ITissueRegion> Regions { get; }
    }
}
