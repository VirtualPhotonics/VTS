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
       // /// <summary>
       // /// method to determine if photon ray (or track) will intersect layer
       ///// </summary>
       // /// <param name="p1">ray starting position</param>
       // /// <param name="p2">ray ending position</param>
       // /// <param name="zPlane">plane definition z=zLocation</param>
       // /// <param name="distanceToBoundary">return: distance to boundary, infinity if no intersection</param>
       // /// <returns>boolean: true if intersection, false if on boundary of cylinder</returns>
       // public static bool RayIntersectInfinitePlane(Position p1, Position p2, double zPlane, 
       //     out double distanceToBoundary)
       // {
       //     var dist = Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) +
       //         (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z));
       //     var Ux = (p2.X - p1.X) / dist;
       //     var Uy = (p2.Y - p1.Y) / dist;
       //     var Uz = (p2.Z - p1.Z) / dist;
       //     // check if one of points on plane
       //     if (Math.Abs(p1.Z - zPlane) < 1e-10) 
       //     {
       //         distanceToBoundary = 0.0;
       //         return true;
       //     }
       //     else
       //     {
       //         if (Math.Abs(p2.Z - zPlane) < 1e-10)
       //         {
       //             distanceToBoundary = dist;
       //             return true;
       //         }
       //     }
       //     // if passes going up or down
       //     if (((p1.Z > zPlane) && (p2.Z < zPlane)) || ((p1.Z < zPlane) && (p2.Z > zPlane))) 
       //     {
       //         distanceToBoundary = (zPlane - p1.Z) / Uz;
       //         return true;
       //     }
       //     distanceToBoundary = 0.0;
       //     return false;
       // }  

        /// <summary>
        /// This extends photon ray from position and angle of exit from tissue into air
        /// up to plane specified by zPlane
        /// </summary>
        /// <param name="p">position of photon exit</param>
        /// <param name="d">direction of photon exit</param>
        /// <param name="zPlane">z-plane above tissue in air, must be negative value</param>
        /// <returns></returns>
        public static Position RayExtendToInfinitePlane(Position p, Direction d, double zPlane)
        {
            var denom = -d.Uz; // direction dot normal (0, 0, -1)
            if (denom < 1e-14) // check if almost parallel
                return null;

            var t = -zPlane / denom;
            return new Position( p.X + d.Ux * t, p.Y + d.Uy * t, p.Z + d.Uz * t);
        }
    }
}
