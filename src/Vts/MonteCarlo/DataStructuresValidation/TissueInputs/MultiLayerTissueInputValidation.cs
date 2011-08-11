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
    /// This verifies that the layers do not overlap.  It assumes that the layers are
    /// adjacent and defined in order. Public because SimulationInputValidation calls it.
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
            var layers = input.Regions;
            for (int i = 0; i < layers.Count() - 1; i++)
            {
                var thisLayer = (LayerRegion)layers[i];
                var nextLayer = (LayerRegion)layers[i + 1];
                if (thisLayer.ZRange.Stop != nextLayer.ZRange.Start)
                {
                    return new ValidationResult(
                        false,
                        "MultiLayerTissueInput: each layer must start where the previous layer stopped",
                        "Error occured between layer " + i + " and layer " + (i+1));
                }
            }

            return new ValidationResult(
                true,
                "MultiLayerTissueInput: each layer must start where the previous layer stopped");
        }
    }
}
