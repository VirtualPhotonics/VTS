using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies the structure of a MultiLayerTissue
    /// </summary>
    public class MultiLayerTissueInputValidation
    {
        /// <summary>
        /// Method to validate that the tissue layers are contiguous and don't overlap
        /// </summary>
        /// <param name="input">tissue input in SimulationInput</param>
        /// <returns></returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = input.Regions.Select(region => (LayerRegion)region).ToArray();

            // other stuff could go here...
            var tempResult = ValidateLayers(layers);
            if (tempResult.IsValid)
            {
                tempResult = ValidateTopAndBottomLayersAreAir(layers);
            }
            return tempResult;
        }
        /// <summary>
        /// This verifies that the top and bottom layers are air.  The photon propagation algorithm in
        /// Photon class assumes that these layers are air and that the photon exits the domain
        /// after crossing into these layers and is no longer propagated.
        /// </summary>
        /// <param name="layers">list of LayerRegion</param>
        /// <returns></returns>
        public static ValidationResult ValidateTopAndBottomLayersAreAir(IList<LayerRegion> layers)
        {
            // test if first and last layer are not air layers 
            if (!layers[0].IsAir() || !layers[layers.Count - 1].IsAir())
            {
                return new ValidationResult(
                    false,
                    "MultiLayerTissueInput: top and bottom layer must be air",
                    "Make last layer very thick if need semi-infinite media assumption"); 
            }
            return new ValidationResult(
                true,
                "MultiLayerTissueInput: top and bottom layer have been defined as air");
        }

        /// <summary>
        /// This verifies that the layers do not overlap.  It assumes that the layers are
        /// adjacent and defined in order. Public because SimulationInputValidation calls it.
        /// </summary>
        /// <param name="layers">list of LayerRegion</param>
        /// <returns></returns>
        public static ValidationResult ValidateLayers(IList<LayerRegion> layers )
        {
            for (int i = 0; i < layers.Count - 1; i++)
            {
                var thisLayer = layers[i];
                var nextLayer = layers[i + 1];
                if (thisLayer.ZRange.Stop != nextLayer.ZRange.Start)
                {
                    return new ValidationResult(
                        false,
                        "MultiLayerTissueInput: each layer must start where the previous layer stopped",
                        "Error occured between layer " + i + " and layer " + (i + 1));
                }
                if (thisLayer.ZRange.Start == thisLayer.ZRange.Stop)
                {
                    return new ValidationResult(
                        false,
                        "MultiLayerTissueInput: a layer with 0 thickness has been defined",
                        "Check that the ZRange.Start does not equal ZRange.Stop for all layers");
                }
            }

            return new ValidationResult(
                true,
                "MultiLayerTissueInput: each layer must start where the previous layer stopped");
        }
    }
}
