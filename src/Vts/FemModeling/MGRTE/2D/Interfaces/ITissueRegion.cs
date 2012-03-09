using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D
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
        /// Checks to see if the specified position is within the tissue region.  Definition should be such
        /// that regions do not overlap, i.e. photon can only be in one region at a time.
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <returns>True if the specified position is within the tissue region, false otherwise</returns>
        bool ContainsPosition(Position p);

        /// <summary>
        /// Determines if photon on boundary of tissue region.  Some checks require adjustments for
        /// floating point error in precision of Position.
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <returns>True if specified position if on tissue region boundary, false otherwise</returns>
        bool OnBoundary(Position p);

        
    }
}