using Vts.Common;

namespace Vts
{
    /// <summary>
    /// interface for region optical properties
    /// </summary>
    public interface IOpticalPropertyRegion
    {
        /// <summary>
        /// Optical properties of tissue region.
        /// </summary>
        OpticalProperties RegionOP { get; }
    }
}
