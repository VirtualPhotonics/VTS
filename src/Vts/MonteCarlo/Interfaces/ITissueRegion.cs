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
        /// Center position
        /// </summary>
        Position Center { get; }

        /// <summary>
        /// Checks to see if the specified position is within the tissue region
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <returns>True if the specified position is within the tissue region, false otherwise</returns>
        bool ContainsPosition(Position p);

        /// <summary>
        /// Checks if the specified photon will intersect the region boundary
        /// </summary>
        /// <param name="p">Photon to check for intersection (including Position, Direction, and S)</param>
        /// <param name="distanceToBoundary">The distance to the next boundary</param>
        /// <returns>True if photon will intersect the region boundary, false otherwise</returns>
        /// <remarks>Use this method if the distance is required</remarks>
        bool RayIntersectBoundary(Photon p, out double distanceToBoundary);

        /// <summary>
        /// Checks if the specified photon will intersect the region boundary
        /// </summary>
        /// <param name="p">Photon to check for intersection (including Position, Direction, and S)</param>
        /// <returns>True if photon will intersect the region boundary, false otherwise</returns>
        /// <remarks>This method may be faster if the distance is not required</remarks>
        bool RayIntersectBoundary(Photon p);
    }
}