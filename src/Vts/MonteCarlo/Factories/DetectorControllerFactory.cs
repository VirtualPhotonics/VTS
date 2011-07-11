using System;
using System.Linq;
using System.Collections.Generic;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate DetectorController given VirtualBoundaryGroup
    /// </summary>
    public static class DetectorControllerFactory
    {
        public static IDetectorController GetDetectorController(
            VirtualBoundaryType virtualBoundaryType, IList<IDetector> detectors)
        {
            switch (virtualBoundaryType)
            {
                default:
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.DiffuseTransmittance:
                case VirtualBoundaryType.Dosimetry:
                    return GetStandardSurfaceDetectorController((from d in detectors select d as ISurfaceDetector).ToList());
                case VirtualBoundaryType.GenericVolumeBoundary:
                    return GetStandardVolumeDetectorController((from d in detectors select d as IVolumeDetector).ToList());

            }
        }
        public static SurfaceDetectorController GetStandardSurfaceDetectorController(IList<ISurfaceDetector> detectors)
        {
            SurfaceDetectorController controller = null;
            controller = new SurfaceDetectorController(detectors);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating surface detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }
        public static VolumeDetectorController GetStandardVolumeDetectorController(IList<IVolumeDetector> detectors)
        {
            VolumeDetectorController controller = null;
            controller = new VolumeDetectorController(detectors);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating volume detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }

        public static pMCDetectorController GetpMCDetectorController(IList<IpMCDetectorInput> inputs, ITissue tissue, bool tallySecondMoment)
        {
            pMCDetectorController controller = null;
            controller = new pMCDetectorController(inputs, tissue, tallySecondMoment);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }
    }
}
