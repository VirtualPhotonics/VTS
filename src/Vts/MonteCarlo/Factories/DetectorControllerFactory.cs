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
                    if (detectors == null)
                    {
                        return GetStandardSurfaceDetectorController(null);
                    }
                    return GetStandardSurfaceDetectorController(detectors.Select(d => (ISurfaceDetector)d).ToList());
                case VirtualBoundaryType.GenericVolumeBoundary:
                    if (detectors == null)
                    {
                        return GetStandardSurfaceDetectorController(null);
                    }
                    return GetStandardVolumeDetectorController(detectors.Select(d => (IVolumeDetector)d).ToList());

            }
        }

        private static SurfaceDetectorController GetStandardSurfaceDetectorController(IList<ISurfaceDetector> detectors)
        {
            SurfaceDetectorController controller = null;
            if (detectors == null)
            {
                return controller;
            }
            controller = new SurfaceDetectorController(detectors);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating surface detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }

        private static VolumeDetectorController GetStandardVolumeDetectorController(IList<IVolumeDetector> detectors)
        {
            VolumeDetectorController controller = null;
            if (detectors == null)
            {
                return controller;
            }
            controller = new VolumeDetectorController(detectors);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating volume detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }

        // pMC methods
        public static IDetectorController GetpMCDetectorController(
            VirtualBoundaryType virtualBoundaryType, IList<IDetector> detectors,
            ITissue tissue, bool tallySecondMoment)
        {
            switch (virtualBoundaryType)
            {
                default:
                case VirtualBoundaryType.pMCDiffuseReflectance:
                    return GetpMCSurfaceDetectorController(
                        (from d in detectors select d as IpMCSurfaceDetector).ToList(),
                        tissue, tallySecondMoment);
            }
        }

        private static pMCSurfaceDetectorController GetpMCSurfaceDetectorController(
            IList<IpMCSurfaceDetector> detectors, ITissue tissue, bool tallySecondMoment)
        {
            pMCSurfaceDetectorController controller = null;
            controller = new pMCSurfaceDetectorController(detectors, tissue, tallySecondMoment);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating pMC detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }
    }
}
