using System.Collections.Generic;
using Vts.Common;

namespace Vts.MonteCarlo.Tissues
{
    /// <summary>
    /// Inherits from MultiLayerTissue and implements commonly used 
    /// MultiLayerTissue (e.g. Homogeneous tissue layer with air above and
    /// below).
    /// </summary>
    public class MultiLayerTissueProvider : MultiLayerTissue
    {
        // todo: still needed?
        public static MultiLayerTissue HomogeneousTissueWithAirAboveAndBelowProvider()
        {
            // intersection points and optical properties
            return new MultiLayerTissue(
                new List<ITissueRegion>
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                    new LayerRegion(
                        new DoubleRange(0.0, 10.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerRegion(
                        new DoubleRange(10.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                },
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein);
        }
    }
}
