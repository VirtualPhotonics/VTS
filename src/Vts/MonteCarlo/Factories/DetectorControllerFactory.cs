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
        /// <summary>
        /// Static method to instantiate correct detector controller given a VirtualBoundaryType
        /// </summary>
        /// <param name="virtualBoundaryType">VirtualBoundaryType enum</param>
        /// <param name="detectors">list of detectors to include in controller</param>
        /// <returns></returns>
        public static IDetectorController GetDetectorController(
            VirtualBoundaryType virtualBoundaryType, IList<IDetector> detectors)
        {
            switch (virtualBoundaryType)
            {
                default:
                case VirtualBoundaryType.DiffuseReflectance:
                case VirtualBoundaryType.DiffuseTransmittance:
                case VirtualBoundaryType.SurfaceRadiance:
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
        /// <summary>
        /// This instantiates a standard surface detector controller.
        /// </summary>
        /// <param name="detectors"></param>
        /// <returns></returns>
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
        /// <summary>
        /// This instantiates a standard volume detector controller
        /// </summary>
        /// <param name="detectors">list of detectors to include in controller</param>
        /// <returns></returns>
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

        /// <summary>
        /// Static method to instantiate correct pMC detector controller given
        /// VirtualBoundaryType.
        /// </summary>
        /// <param name="virtualBoundaryType">VirtualBoundaryType enum</param>
        /// <param name="detectors">list of detectors to include in controller</param>
        /// <param name="tissue">ITissue needed to instantiate controller</param>
        /// <param name="tallySecondMoment">flag indicating whether to tally second moment</param>
        /// <returns></returns>
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
