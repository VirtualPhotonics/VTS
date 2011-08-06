using Vts.Common;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Defines contract for TissueRegion classes.
    /// </summary>
    public interface ITissueRegion 
    {
        /// <summary>
        /// Optical properties of tissue region.
        /// </summary>
        OpticalProperties RegionOP { get; }
    }
}