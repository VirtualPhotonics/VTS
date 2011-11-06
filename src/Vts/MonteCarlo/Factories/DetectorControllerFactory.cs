using System;
using System.Collections.Generic;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.Factories
{
    public static class DetectorControllerFactory
    {
        public static IDetectorController GetDetectorController(VirtualBoundaryType virtualBoundaryType, IEnumerable<IDetector> detectors, ITissue tissue)
        {
            switch (virtualBoundaryType)
            {
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.DiffuseTransmittance:
                case VirtualBoundaryType.SpecularReflectance:
                case VirtualBoundaryType.SurfaceRadiance:
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return new DetectorController(detectors);
                case VirtualBoundaryType.GenericVolumeBoundary:
                    return new HistoryDetectorController(detectors, tissue);
                default:
                    throw new ArgumentOutOfRangeException("virtualBoundaryType");
            }
        }
    }
}