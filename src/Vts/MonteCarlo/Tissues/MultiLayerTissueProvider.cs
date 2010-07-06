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
        public static MultiLayerTissue HomogeneousTissueWithAirAboveAndBelowProvider()
        {
            // intersection points and optical properties
            return new MultiLayerTissue(
                new MultiLayerTissueInput(new List<LayerRegion>
                { 
                    new LayerRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0, 2),
                        new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                        AbsorptionWeightingType.Discrete),
                    new LayerRegion(
                        new DoubleRange(0.0, 10.0, 2),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        AbsorptionWeightingType.Discrete),
                    new LayerRegion(
                        new DoubleRange(10.0, double.PositiveInfinity, 2),
                        new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                        AbsorptionWeightingType.Discrete)
                }));
        }
    }
}
