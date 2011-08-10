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

        /// <summary>
        /// Checks to see if the specified position is within the tissue region
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <returns>True if the specified position is within the tissue region, false otherwise</returns>
        bool ContainsPosition(Position p);
    }
}