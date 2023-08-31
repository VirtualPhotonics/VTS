using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the bounding cylinder is the same height as the tissue,
    /// and that the refractive index of the tissue layer and ellipsoid match.
    /// </summary>
    public static class BoundingCylinderTissueInputValidation
    {
        /// <summary>
        /// Main validation method for BoundingCylinderTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((BoundingCylinderTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var boundingCylinder = (CaplessCylinderTissueRegion)((BoundingCylinderTissueInput)input).CylinderRegion;
            var tempResult = ValidateGeometry(layers, boundingCylinder);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            return tempResult;
        }
        /// <summary>
        /// Method to validate that the geometry of tissue layers and bounding cylinder agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="boundingCylinder">CylinderTissueRegion</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, 
            CaplessCylinderTissueRegion boundingCylinder)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);

            if (!tempResult.IsValid) return tempResult;

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());
            
            // check that there is at least one layer of tissue 
            var layerTissueRegions = tissueLayers.ToList();
            if (!layerTissueRegions.Any())
            {
                tempResult = new ValidationResult(
                    false,
                    "BoundingCylinderTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "BoundingCylinderTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }

            if (!tempResult.IsValid) return tempResult;

            var layersHeight = layerTissueRegions.Sum(layer => layer.ZRange.Delta);

            if (boundingCylinder.Height != layersHeight)
            {
                tempResult = new ValidationResult(
                    false,
                    "BoundingCylinderTissueInput: bounding cylinder must have same height as tissue",
                    "BoundingCylinderTissueInput: make sure cylinder Height = depth of tissue");
            }

            if (!tempResult.IsValid) return tempResult;

            return new ValidationResult(
                true,
                "BoundingCylinderTissueInput: geometry and refractive index settings validated");
        }
    }
}
