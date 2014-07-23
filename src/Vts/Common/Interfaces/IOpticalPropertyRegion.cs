using Vts.Common;

namespace Vts
{
    public interface IOpticalPropertyRegion
    {
        /// <summary>
        /// Optical properties of tissue region.
        /// </summary>
        OpticalProperties RegionOP { get; }
    }
}
