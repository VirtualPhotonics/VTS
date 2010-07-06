using System.Collections.Generic;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines a contract for TissueInput.
    /// </summary>
    public interface ITissueInput
    {
        IList<ITissueRegion> Regions { get; }
    }
}
