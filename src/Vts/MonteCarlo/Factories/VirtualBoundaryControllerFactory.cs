using System;
using System.Collections.Generic;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate virtual boundaries given a list of IDetector and ITissue
    /// </summary>
    public static class VirtualBoundaryControllerFactory
    {
        public static VirtualBoundaryController GetVirtualBoundaryController(
            IList<IDetector> detectors, ITissue tissue)
        {
            var controller = new VirtualBoundaryController(
                VirtualBoundaryFactory.GetVirtualBoundaries(detectors, tissue));

            if (controller == null)
                throw new ArgumentException(
                    "Problem generating virtual boundary controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

            return controller;
        }

        // not sure about how to handle pMC yet, don't need VBs in postprocessing
        //public static pMCDetectorController GetpMCDetectorController(IList<IpMCDetectorInput> inputs, ITissue tissue, bool tallySecondMoment)
        //{
        //    pMCDetectorController controller = null;
        //    controller = new pMCDetectorController(inputs, tissue, tallySecondMoment);

        //    if (controller == null)
        //        throw new ArgumentException(
        //            "Problem generating detector controller. Check that each XXXDetectorInput has a matching XXXDetector definition.");

        //    return controller;
        //}
    }
}
