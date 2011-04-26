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
        public static DetectorController GetStandardDetectorController(IList<IDetectorInput> inputs, ITissue tissue)
        {
            DetectorController controller = null;
            controller = new DetectorController(inputs, tissue);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }


        public static pMCDetectorController GetpMCDetectorController(IList<IpMCDetectorInput> inputs, ITissue tissue)
        {
            pMCDetectorController controller = null;
            controller = new pMCDetectorController(inputs, tissue);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }
    }
}
