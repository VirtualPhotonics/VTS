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
    /// <param name="layers"></param>
    public class MultiLayerTissueInputValidation
    {
        public static ValidationResult ValidateInput(IList<ITissueRegion> layers)
        {
            var result = new ValidationResult();
            for (int i = 0; i < layers.Count() - 1; i++)
            {
                var thisLayer = (LayerRegion)layers[i];
                var nextLayer = (LayerRegion)layers[i + 1];
                if (thisLayer.ZRange.Stop != nextLayer.ZRange.Start)
                {
                    result.IsValid = false;
                    result.ErrorMessage =
                        "MultiLayerTissueInput: layers start/stop definition in error";
                    result.Remarks = "Make sure layer stop is next layer start";
                }
            }
            return result;
        }
    }
}
