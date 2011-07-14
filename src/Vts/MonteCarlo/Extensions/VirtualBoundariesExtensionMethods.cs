using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods used to determine if surface or volume virtual boundary.
    /// </summary>
    public static class VirtualBoundariesExtensionMethods
    {
        public static bool IsSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.DiffuseTransmittance:
                case VirtualBoundaryType.SpecularReflectance:
                case VirtualBoundaryType.pMCDiffuseReflectance:
                case VirtualBoundaryType.Dosimetry:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsVolumeVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.GenericVolumeBoundary:
                    return true;
                default:
                    return false;
            }
        }
    }
}
