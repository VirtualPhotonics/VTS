using System;
using Vts.MonteCarlo.VirtualBoundaries;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate virtual boundary.
    /// </summary>
    public class VirtualBoundaryFactory
    {
        /// <summary>
        /// method that gets appropriate VB 
        /// </summary>
        /// <param name="vbType">VirtualBoundaryType</param>
        /// <param name="tissue">ITissue</param>
        /// <param name="detectorController">IDetectorController</param>
        /// <returns>IVirtualBoundary</returns>
        public static IVirtualBoundary GetVirtualBoundary(
            VirtualBoundaryType vbType, ITissue tissue, IDetectorController detectorController)
        {
            IVirtualBoundary vb = vbType switch
            {
                VirtualBoundaryType.DiffuseReflectance => new DiffuseReflectanceVirtualBoundary(
                                        tissue, detectorController, VirtualBoundaryType.DiffuseReflectance.ToString()),
                VirtualBoundaryType.DiffuseTransmittance => new DiffuseTransmittanceVirtualBoundary(
                                        tissue, detectorController, VirtualBoundaryType.DiffuseTransmittance.ToString()),
                VirtualBoundaryType.SpecularReflectance => new SpecularReflectanceVirtualBoundary(
                                         tissue, detectorController, VirtualBoundaryType.SpecularReflectance.ToString()),// reflecting off first layer without transporting in medium
                VirtualBoundaryType.InternalSurface => new RadianceVirtualBoundary(
                                        detectorController, VirtualBoundaryType.InternalSurface.ToString()),
                VirtualBoundaryType.GenericVolumeBoundary => new GenericVolumeVirtualBoundary(
                                        tissue, detectorController, VirtualBoundaryType.GenericVolumeBoundary.ToString()),
                VirtualBoundaryType.pMCDiffuseReflectance => new pMCDiffuseReflectanceVirtualBoundary(
                                        tissue, detectorController, VirtualBoundaryType.DiffuseReflectance.ToString()),
                VirtualBoundaryType.pMCDiffuseTransmittance => new pMCDiffuseTransmittanceVirtualBoundary(
                                        tissue, detectorController, VirtualBoundaryType.DiffuseTransmittance.ToString()),
                VirtualBoundaryType.BoundingVolume => new LateralBoundingVirtualBoundary(
                                        tissue, detectorController, VirtualBoundaryType.BoundingVolume.ToString()),
                _ => throw new ArgumentOutOfRangeException("Virtual boundary type not recognized: " + vbType),
            };
            return vb;
        }
    }
}
