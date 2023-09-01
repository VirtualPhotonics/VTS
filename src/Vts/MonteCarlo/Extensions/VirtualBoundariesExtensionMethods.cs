using System;

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
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.DiffuseReflectance => true,
                VirtualBoundaryType.DiffuseTransmittance => true,
                VirtualBoundaryType.SpecularReflectance => true,
                VirtualBoundaryType.pMCDiffuseReflectance => true,
                VirtualBoundaryType.pMCDiffuseTransmittance => true,
                VirtualBoundaryType.InternalSurface => true,
                VirtualBoundaryType.GenericVolumeBoundary => false,
                VirtualBoundaryType.BoundingVolume => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }
        /// <summary>
        /// Method to determine if VB is volume VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if volume VB, false if not</returns>
        public static bool IsVolumeVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.GenericVolumeBoundary => true,
                VirtualBoundaryType.BoundingVolume => true,
                VirtualBoundaryType.DiffuseReflectance => false,
                VirtualBoundaryType.DiffuseTransmittance => false,
                VirtualBoundaryType.SpecularReflectance => false,
                VirtualBoundaryType.InternalSurface => false,
                VirtualBoundaryType.pMCDiffuseReflectance => false,
                VirtualBoundaryType.pMCDiffuseTransmittance => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }
        /// <summary>
        /// Method to determine if VB is surface reflectance VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true of surface reflectance VB, false if not</returns>
        public static bool IsReflectanceSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.DiffuseReflectance => true,
                VirtualBoundaryType.pMCDiffuseReflectance => true,
                VirtualBoundaryType.DiffuseTransmittance => false,
                VirtualBoundaryType.SpecularReflectance => false,
                VirtualBoundaryType.GenericVolumeBoundary => false,
                VirtualBoundaryType.InternalSurface => false,
                VirtualBoundaryType.pMCDiffuseTransmittance => false,
                VirtualBoundaryType.BoundingVolume => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }
        /// <summary>
        /// Method to determine if transmittance surface VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if transmittance surface VB, false if not</returns>
        public static bool IsTransmittanceSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.DiffuseTransmittance => true,
                VirtualBoundaryType.pMCDiffuseTransmittance => true,
                VirtualBoundaryType.DiffuseReflectance => false,
                VirtualBoundaryType.SpecularReflectance => false,
                VirtualBoundaryType.GenericVolumeBoundary => false,
                VirtualBoundaryType.InternalSurface => false,
                VirtualBoundaryType.pMCDiffuseReflectance => false,
                VirtualBoundaryType.BoundingVolume => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }
        /// <summary>
        /// Method to determine if specular surface VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if transmittance surface VB, false if not</returns>
        public static bool IsSpecularSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.SpecularReflectance => true,
                VirtualBoundaryType.DiffuseReflectance => false,
                VirtualBoundaryType.DiffuseTransmittance => false,
                VirtualBoundaryType.GenericVolumeBoundary => false,
                VirtualBoundaryType.InternalSurface => false,
                VirtualBoundaryType.pMCDiffuseReflectance => false,
                VirtualBoundaryType.pMCDiffuseTransmittance => false,
                VirtualBoundaryType.BoundingVolume => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }
        /// <summary>
        /// Method to determine if internal surface (dosimetry) VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type </param>
        /// <returns>true if internal surface VB, false if not</returns>
        public static bool IsInternalSurfaceVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.InternalSurface => true,
                VirtualBoundaryType.DiffuseReflectance => false,
                VirtualBoundaryType.DiffuseTransmittance => false,
                VirtualBoundaryType.SpecularReflectance => false,
                VirtualBoundaryType.GenericVolumeBoundary => false,
                VirtualBoundaryType.pMCDiffuseReflectance => false,
                VirtualBoundaryType.pMCDiffuseTransmittance => false,
                VirtualBoundaryType.BoundingVolume => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }

        /// <summary>
        /// Method to determine if generic volume VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if generic volume VB, false if not</returns>
        public static bool IsGenericVolumeVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.GenericVolumeBoundary => true,
                VirtualBoundaryType.DiffuseReflectance => false,
                VirtualBoundaryType.DiffuseTransmittance => false,
                VirtualBoundaryType.SpecularReflectance => false,
                VirtualBoundaryType.InternalSurface => false,
                VirtualBoundaryType.pMCDiffuseReflectance => false,
                VirtualBoundaryType.pMCDiffuseTransmittance => false,
                VirtualBoundaryType.BoundingVolume => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }
        /// <summary>
        /// Method to determine if perturbation Monte Carlo (pMC) VB or not
        /// </summary>
        /// <param name="virtualBoundaryType">VB type</param>
        /// <returns>true if pMC VB, false if not</returns>
        public static bool IspMCVirtualBoundary(this VirtualBoundaryType virtualBoundaryType)
        {
            return virtualBoundaryType switch
            {
                VirtualBoundaryType.pMCDiffuseReflectance => true,
                VirtualBoundaryType.pMCDiffuseTransmittance => true,
                VirtualBoundaryType.GenericVolumeBoundary => false,
                VirtualBoundaryType.BoundingVolume => false,
                VirtualBoundaryType.DiffuseReflectance => false,
                VirtualBoundaryType.DiffuseTransmittance => false,
                VirtualBoundaryType.SpecularReflectance => false,
                VirtualBoundaryType.InternalSurface => false,
                _ => throw new ArgumentOutOfRangeException("Virtual Boundary type not recognized: " +
                                                           virtualBoundaryType)
            };
        }
    }
}
