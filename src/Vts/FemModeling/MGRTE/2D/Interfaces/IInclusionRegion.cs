using Vts.Common;

namespace Vts.FemModeling.MGRTE._2D
{
    /// <summary>
    /// Defines contract for InclusionRegion classes.
    /// </summary>
    public interface IInclusionRegion 
    {
        /// <summary>
        /// Optical properties of tissue region.
        /// </summary>
        OpticalProperties RegionOP { get; }

        /// <summary>
        /// Radius of a inclusion
        /// </summary>
        Position Location { get; }

        /// <summary>
        /// Radius of a inclusion
        /// </summary>
        double Radius { get; }
    }
}