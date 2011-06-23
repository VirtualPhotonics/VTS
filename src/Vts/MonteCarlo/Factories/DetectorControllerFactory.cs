using System;
using System.Collections.Generic;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate Detector given IDetectorInput and ITissue
    /// </summary>
    public static class DetectorControllerFactory
    {
        public static DetectorController GetStandardDetectorController(IList<IDetector> detectors)
        {
            DetectorController controller = null;
            //controller = new DetectorController(inputs, tissue, tallySecondMoment);
            controller = new DetectorController(detectors);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

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
