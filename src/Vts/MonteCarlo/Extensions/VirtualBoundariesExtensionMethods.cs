namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods used to determine type of virtual boundary.
    /// </summary>
    public static class VirtualBoundariesExtensionMethods
    {
        /// <summary>
        /// Method to determine whether VB is Surface VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if surface VB, false if not </returns>
        public static bool IsSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.DiffuseTransmittance:
                case VirtualBoundaryType.SpecularReflectance:
                case VirtualBoundaryType.pMCDiffuseReflectance:
                case VirtualBoundaryType.SurfaceRadiance:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if VB is volume VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if volume VB, false if not</returns>
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
        /// <summary>
        /// Method to determine if VB is surface reflectance VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true of surface reflectance VB, false if not</returns>
        public static bool IsReflectanceSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if transmittance surface VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if transmittance surface VB, false if not</returns>
        public static bool IsTransmittanceSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseTransmittance:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if specular surface VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns></returns>
        public static bool IsSpecularSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.SpecularReflectance:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if internal surface VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type </param>
        /// <returns>true if internal surface VB, false if not</returns>
        public static bool IsInternalSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.SurfaceRadiance:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if generic volume VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if generic volume VB, false if not</returns>
        public static bool IsGenericVolumeVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.GenericVolumeBoundary:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if perturbation Monte Carlo (pMC) VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if pMC VB, false if not</returns>
        public static bool IspMCVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return true;
                default:
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.DiffuseTransmittance:
                case VirtualBoundaryType.SpecularReflectance:
                case VirtualBoundaryType.SurfaceRadiance:
                    return false;
            }
        }
    }
}
