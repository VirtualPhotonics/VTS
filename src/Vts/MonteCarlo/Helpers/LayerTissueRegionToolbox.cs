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
        /// <param name="photonPos">position of photon exit</param>
        /// <param name="photonDir">direction of photon exit</param>
        /// <param name="zPlane">z-plane above tissue in air, must be negative value</param>
        /// <returns>position on plane extended to</returns>
        public static Position RayExtendToInfiniteZPlane(Position photonPos, Direction photonDir, double zPlane)
        {
            var denominator = -photonDir.Uz; // direction dot normal (0, 0, -1)
            if (denominator < 1e-14) // check if almost parallel
                return null;

            var t = -zPlane / denominator;
            return new Position(photonPos.X + photonDir.Ux * t, photonPos.Y + photonDir.Uy * t, photonPos.Z + photonDir.Uz * t);
        }

        /// <summary>
        /// This extends photon ray from position and angle of exit from tissue into air
        /// up to slanted plane
        /// https://en.wikipedia.org/wiki/Line%E2%80%93plane_intersection
        /// </summary>
        /// <param name="photonPos">position of photon exit</param>
        /// <param name="photonDir">direction of photon exit</param>
        /// <param name="planePos">position on the detector plane</param>
        /// <param name="detectorNormal">normal Direction of detector</param>
        /// <returns>position on plane extended to</returns>
        public static Position RayExtendToInfiniteSlantedPlane(Position photonPos, Direction photonDir, 
            Position planePos, Direction detectorNormal)
        {
            //compute denominator
            var denominator = detectorNormal.Ux * photonDir.Ux + detectorNormal.Uy * photonDir.Uy + detectorNormal.Uz * photonDir.Uz; 
            if (denominator < 1e-14) // check if almost parallel to the plane
                return null;

            //compute numerator
            var dx = planePos.X - photonPos.X;
            var dy = planePos.Y - photonPos.Y;
            var dz = planePos.Z - photonPos.Z;
            var d = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            var diffDir = new Direction(dx / d, dy / d, dz / d);
            var numerator = detectorNormal.Ux * diffDir.Ux + detectorNormal.Uy * diffDir.Uy + detectorNormal.Uz * diffDir.Uz; 
            if (numerator < 1e-14) // check if almost parallel to the plane
                return null;

            var t = numerator / denominator;
            return new Position(photonPos.X + photonDir.Ux * t, photonPos.Y + photonDir.Uy * t, photonPos.Z + photonDir.Uz * t);
        }
    }
}
