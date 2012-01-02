using System;
using System.Collections.Generic;
using Vts.MonteCarlo.Controllers;
using System.Collections;
using System.Linq;

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
                    return new HistoryDetectorController((from d in detectors where d is IHistoryDetector select (IHistoryDetector)d).ToList(), tissue);
                default:
                    throw new ArgumentOutOfRangeException("virtualBoundaryType"); 
            }
        }
    }
}