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
            IVirtualBoundary vb = null;
            switch (vbType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                    vb = new DiffuseReflectanceVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.DiffuseReflectance.ToString());
                    break;
                case VirtualBoundaryType.DiffuseTransmittance:
                    vb = new DiffuseTransmittanceVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.DiffuseTransmittance.ToString());
                    break;
                case VirtualBoundaryType.SpecularReflectance:
                    // reflecting off first layer without transporting in medium
                    vb = new SpecularReflectanceVirtualBoundary(
                         tissue, detectorController, VirtualBoundaryType.SpecularReflectance.ToString());
                    break;
                case VirtualBoundaryType.Dosimetry:
                    vb = new RadianceVirtualBoundary(
                        detectorController, VirtualBoundaryType.Dosimetry.ToString());
                    break;
                case VirtualBoundaryType.GenericVolumeBoundary:
                    vb = new GenericVolumeVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.GenericVolumeBoundary.ToString());
                    break;
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    vb = new pMCDiffuseReflectanceVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.DiffuseReflectance.ToString());
                    break;
                case VirtualBoundaryType.pMCDiffuseTransmittance:
                    vb = new pMCDiffuseTransmittanceVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.DiffuseTransmittance.ToString());
                    break;
                case VirtualBoundaryType.BoundingVolume:
                    vb = new BoundingCylinderVirtualBoundary(
                        tissue, detectorController, VirtualBoundaryType.BoundingVolume.ToString());
                    break;

                default:
                    throw new ArgumentOutOfRangeException("Virtual boundary type not recognized: " + vbType);
            }
            return vb;
        }
    }
}
