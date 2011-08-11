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
    /// This verifies that the ellipsoid is entirely contained within tissue layer,
    /// that only one tissue layer is defined, and that the refractive index of the
    /// tissue layer and ellipsoid match.
    /// </summary>
    public class SingleEllipsoidTissueInputValidation
    {
        /// <summary>
        /// Main validation method for SingleEllipsoidTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>ValidationResult</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = (List<LayerRegion>)((SingleEllipsoidTissueInput)input).LayerRegions;
            var ellipsoid = (EllipsoidRegion)((SingleEllipsoidTissueInput)input).EllipsoidRegion;
            ValidationResult tempResult;
            tempResult = ValidateGeometry(layers, ellipsoid);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            tempResult = ValidateRefractiveIndexMatch(layers, ellipsoid);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            return tempResult;
        }
        /// <summary>
        /// Method to validate that the geometry of tissue layers and ellipsoid agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerRegion</param>
        /// <param name="ellipsoid">EllipsoidRegion</param>
        /// <returns>ValidationResult</returns>
        private static ValidationResult ValidateGeometry(List<LayerRegion> layers, EllipsoidRegion ellipsoid)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateInput((ITissueInput)layers);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            // check that there is only one layer of tissue
            if (layers.Count > 3)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleEllipsoidTissueInput: tissue layer is assumed to be single layer with air layer above and below",
                    "SingleEllipsoidTissueInput: redefine tissue definition to be single layer of tissue");
            }
            // check that ellipsoid contained within tissue layer
            LayerRegion tissueLayer = (LayerRegion)layers[1];
            if (2 * ellipsoid.Dz > tissueLayer.ZRange.Delta)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleEllipsoidTissueInput: ellipsoid must be entirely contained within tissue layer",
                    "Resize Dz of Ellipsoid dimension so that 2*Dz<layer[1] depth");
            }
            return new ValidationResult(
                true,
                "SingleEllipsoidTissueInput: geometry and refractive index settings validated");
        }
        /// <summary>
        /// Method to verify refractive index of tissue layer and ellipsoid match.
        /// Code does not yet include reflecting/refracting off ellipsoid surface.
        /// </summary>
        /// <param name="layers">list of LayerRegion</param>
        /// <param name="ellipsiod"EllipsoidRegion></param>
        /// <returns>ValidationResult</returns>
        private static ValidationResult ValidateRefractiveIndexMatch(
            List<LayerRegion> layers, EllipsoidRegion ellipsiod)
        {
            if (layers[1].RegionOP.N != ellipsiod.RegionOP.N)
            {
                return new ValidationResult(
                    false,
                    "SingleEllipsoidTissueInput: refractive index of tissue layer must match that of ellipsoid",
                    "Change N of ellipsoid to match tissue layer N");
            }
            return new ValidationResult(
                true,
                "SingleEllipsoidTissueInput: refractive index of tissue and ellipsoid match");
        }
    }
}
