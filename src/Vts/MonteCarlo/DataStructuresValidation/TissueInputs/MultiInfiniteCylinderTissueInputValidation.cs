using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the infinite cylinders are entirely contained within a tissue layer,
    /// that only one tissue layer is defined, and that the refractive index of the
    /// tissue layer and InfiniteCylinder match.
    /// </summary>
    public static class MultiInfiniteCylinderTissueInputValidation
    {
        /// <summary>
        /// Main validation method for MultiInfiniteCylinderTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((MultiInfiniteCylinderTissueInput)input).LayerRegions.
                Select(region => (LayerTissueRegion)region).ToArray();
            var infiniteCylinders = ((MultiInfiniteCylinderTissueInput)input).InfiniteCylinderRegions
                .Select(region => (InfiniteCylinderTissueRegion)region).ToArray();
            var tempResult = ValidateGeometry(layers, infiniteCylinders);
            return tempResult;
        }

        /// <summary>
        /// Method to validate that the geometry of tissue layers and InfiniteCylinder agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="infiniteCylinders">list of InfiniteCylinderTissueRegion</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers,
            IList<InfiniteCylinderTissueRegion> infiniteCylinders)
        {
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);

            if (!tempResult.IsValid) return tempResult;
            
            // check that cylinder radius is not 0
            foreach (var infiniteCylinder in infiniteCylinders)
            {
                if (infiniteCylinder.Radius <= 0)
                {
                    tempResult = new ValidationResult(
                        false,
                        "MultiInfiniteCylinderTissueInput: InfiniteCylinder has a radial axis equal to 0",
                        "MultiInfiniteCylinderTissueInput: make sure radius > 0");
                }
            }

            if (!tempResult.IsValid) return tempResult;

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());

            // check that there is at least one layer of tissue 
            var layerTissueRegions = tissueLayers.ToList();
            if (layerTissueRegions.Count == 0)
            {
                tempResult = new ValidationResult(
                    false,
                    "MultiInfiniteCylinderTissueInput: tissue layer is assumed to be at least a Multi layer with air layer above and below",
                    "MultiInfiniteCylinderTissueInput: redefine tissue definition to contain at least a Multi layer of tissue");
            }

            if (!tempResult.IsValid) return tempResult;

            // check that InfiniteCylinder contained within a tissue layer
            foreach (var infiniteCylinder in infiniteCylinders)
            {
                var correctlyContainedInLayer = layerTissueRegions.Any(layer =>
                    layer.ContainsPosition(infiniteCylinder.Center) &&
                    infiniteCylinder.Center.Z + infiniteCylinder.Radius <= layer.ZRange.Stop &&
                    infiniteCylinder.Center.Z - infiniteCylinder.Radius >= layer.ZRange.Start
                );
                if (!correctlyContainedInLayer)
                {
                    tempResult = new ValidationResult(
                        false,
                        "MultiInfiniteCylinderTissueInput: InfiniteCylinders must be entirely contained within a tissue layer",
                        "MultiInfiniteCylinderTissueInput: resize Radius of InfiniteCylinder dimension so that 2*Radius<thickness of layer of inclusion");

                }
            }

            if (!tempResult.IsValid) return tempResult;

            return new ValidationResult(
                true,
                "MultiInfiniteCylinderTissueInput: geometry settings validated");
        }

    }
}
