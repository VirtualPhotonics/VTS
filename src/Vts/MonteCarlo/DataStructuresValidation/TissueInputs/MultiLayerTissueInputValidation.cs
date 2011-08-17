using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.DataStructuresValidation;

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

            return ValidateLayers(layers);
        }

        /// <summary>
        /// This verifies that the layers do not overlap.  It assumes that the layers are
        /// adjacent and defined in order. Public because SimulationInputValidation calls it.
        /// </summary>
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
