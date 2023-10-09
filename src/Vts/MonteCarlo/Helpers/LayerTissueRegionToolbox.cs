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
    }
}
