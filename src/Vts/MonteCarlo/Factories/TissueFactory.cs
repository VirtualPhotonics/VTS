using System;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Instantiates appropriate ITissue given ITissueInput.
    /// </summary>
    public static class TissueFactory
    {
        public static ITissue GetTissue(ITissueInput ti, AbsorptionWeightingType awt)
        {
            ITissue t = null;
            if (ti is MultiLayerTissueInput)
            {
                t = new MultiLayerTissue((MultiLayerTissueInput)ti, awt);
            }
            //if (ti is SingleEllipsoidTissueInput)
            //{
            //    return new SingleEllipsoidTissue();
            //}
            if (t == null)
                throw new ArgumentException(
                    "Problem generating ITissue instance. Check that TissueInput, ti, has a matching ITissue definition.");

            return t;
        }
    }
}
