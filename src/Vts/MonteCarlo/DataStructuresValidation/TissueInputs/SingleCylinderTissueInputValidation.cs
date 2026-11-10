using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the cylinder with axis along z-axis is entirely contained within tissue layer,
    /// and that at least one tissue layer is defined.
    /// </summary>
    public static class SingleCylinderTissueInputValidation
    {
        /// <summary>
        /// Main validation method for SingleCylinderTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((SingleCylinderTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var cylinder = ((SingleCylinderTissueInput)input).CylinderRegion;
            var tempResult = ValidateGeometry(layers, (CylinderTissueRegion)cylinder);
            return tempResult;
        }

        /// <summary>
        /// Method to validate that the geometry of tissue layers and ellipsoid agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="Cylinder">CylinderTissueRegion</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, CylinderTissueRegion Cylinder)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);
            if (!tempResult.IsValid) return tempResult; 

            if (Cylinder.Radius == 0.0)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleCylinderTissueInput: cylinder has radius equal to 0",
                    "SingleCylinderTissueInput: make sure cylinder radius is > 0");
            }
            if (!tempResult.IsValid) return tempResult; 

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());

            // check that there is at least one layer of tissue 
            var layerTissueRegions = tissueLayers as LayerTissueRegion[] ?? tissueLayers.ToArray();
            if (layerTissueRegions.Length == 0)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleCylinderTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "SingleCylinderTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }
            if (!tempResult.IsValid) return tempResult; 

            // check that cylinder is contained within a tissue layer 
            var correctlyContainedInLayer = layerTissueRegions.Any(
                layer =>
                    layer.ContainsPosition(Cylinder.Center) &&
                    Cylinder.Center.Z + Cylinder.Height / 2 <= layer.ZRange.Stop &&
                    Cylinder.Center.Z - Cylinder.Height / 2 >= layer.ZRange.Start
                );

            if (!correctlyContainedInLayer)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleCylinderTissueInput: cylinder must be entirely contained within a tissue layer",
                    "Resize Height and or Center so that cylinder entirely within tissue layer");
            }

            if (!tempResult.IsValid) return tempResult; 

            return new ValidationResult(
                true,
                "SingleCylinderTissueInput: geometry and refractive index settings validated");
        }
    }
}
