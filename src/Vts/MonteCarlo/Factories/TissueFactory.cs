using System;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate ITissue given ITissueInput.
    /// </summary>
    public static class TissueFactory
    {
        // todo: revisit to make signatures here and in Tissue/TissueInput class signatures strongly typed
        public static ITissue GetTissue(ITissueInput ti, AbsorptionWeightingType awt, PhaseFunctionType pft)
        {
            ITissue t = null;
            if (ti is MultiLayerTissueInput)
            {
                var multiLayerTissueInput = (MultiLayerTissueInput) ti;
                t = new MultiLayerTissue(multiLayerTissueInput.Regions, awt, pft);
            }
            if (ti is SingleEllipsoidTissueInput)
            {
                var singleEllipsoidTissueInput = (SingleEllipsoidTissueInput)ti;
                return new SingleInclusionTissue(
                    singleEllipsoidTissueInput.EllipsoidRegion,
                    singleEllipsoidTissueInput.LayerRegions,
                    awt,
                    pft);
            }
            if (t == null)
                throw new ArgumentException(
                    "Problem generating ITissue instance. Check that TissueInput, ti, has a matching ITissue definition.");

            return t;
        }
    }
}
