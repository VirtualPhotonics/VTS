using System;
using System.Collections.Generic;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Interfaces;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate Detector given IDetectorInput and ITissue
    /// </summary>
    public static class DetectorControllerFactory
    {
        public static IDetectorController GetStandardDetectorController(IList<IDetectorInput> inputs, ITissue tissue)
        {
            IDetectorController controller = null;
            controller = new DetectorController(inputs, tissue);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }


        public static IDetectorController GetpMCDetectorController(IList<IpMCDetectorInput> inputs, ITissue tissue)
        {
            IDetectorController controller = null;
            controller = new pMCDetectorController(inputs, tissue);

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }
    }
}
