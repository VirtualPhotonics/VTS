using System;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Helper methods for  layer tissue region methods.  For example, ray intersect infinite plane
    /// at z=constant
    /// </summary>
    public abstract class LayerTissueRegionToolbox 
    {
        /// <summary>
        /// This extends photon ray from position and angle of exit from tissue into air
        /// up to plane specified by zPlane
        /// </summary>
        /// <param name="p">position of photon exit</param>
        /// <param name="d">direction of photon exit</param>
        /// <param name="zPlane">z-plane above tissue in air, must be negative value</param>
        /// <returns>position on plane extended to</returns>
        public static Position RayExtendToInfinitePlane(Position p, Direction d, double zPlane)
        {
            var denominator = -d.Uz; // direction dot normal (0, 0, -1)
            if (denominator < 1e-14) // check if almost parallel
                return null;

            var t = -zPlane / denominator;
            return new Position(p.X + d.Ux * t, p.Y + d.Uy * t, p.Z + d.Uz * t);
        }
    }
}
