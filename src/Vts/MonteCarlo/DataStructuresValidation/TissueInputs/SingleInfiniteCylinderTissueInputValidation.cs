using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the infinite cylinder is entirely contained within tissue layer,
    /// and that at least one tissue layer is defined.
    /// </summary>
    public static class SingleInfiniteCylinderTissueInputValidation
    {
        /// <summary>
        /// Main validation method for SingleInfiniteCylinderTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((SingleInfiniteCylinderTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var cylinder = ((SingleInfiniteCylinderTissueInput)input).InfiniteCylinderRegion;
            var tempResult = ValidateGeometry(layers, (InfiniteCylinderTissueRegion)cylinder);
            return tempResult;
        }

        /// <summary>
        /// Method to validate that the geometry of tissue layers and ellipsoid agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="infiniteCylinder">InfiniteCylinderTissueRegion</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, InfiniteCylinderTissueRegion infiniteCylinder)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);
            if (!tempResult.IsValid) return tempResult; 

            if (infiniteCylinder.Radius == 0.0)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleInfiniteCylinderTissueInput: infinite cylinder has radius equal to 0",
                    "SingleInfiniteCylinderTissueInput: make sure infinite cylinder radius is > 0");
            }
            if (!tempResult.IsValid) return tempResult; 

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());

            // check that there is at least one layer of tissue 
            var layerTissueRegions = tissueLayers as LayerTissueRegion[] ?? tissueLayers.ToArray();
            if (!layerTissueRegions.Any())
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleInfiniteCylinderTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "SingleInfiniteCylinderTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }
            if (!tempResult.IsValid) return tempResult; 

            // check that cylinder is contained within a tissue layer 
            var cylinderRadius = infiniteCylinder.Radius;
            var correctlyContainedInLayer = layerTissueRegions.Any(
                layer =>
                    layer.ContainsPosition(infiniteCylinder.Center) &&
                    infiniteCylinder.Center.Z + cylinderRadius <= layer.ZRange.Stop &&
                    infiniteCylinder.Center.Z - cylinderRadius >= layer.ZRange.Start
                );

            if (!correctlyContainedInLayer)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleInfiniteCylinderTissueInput: infinite cylinders must be entirely contained within a tissue layer",
                    "Resize radii and or Center so that infinite cylinders entirely within tissue layer");
            }

            if (!tempResult.IsValid) return tempResult; 

            return new ValidationResult(
                true,
                "SingleInfiniteCylinderTissueInput: geometry and refractive index settings validated");
        }
    }
}
