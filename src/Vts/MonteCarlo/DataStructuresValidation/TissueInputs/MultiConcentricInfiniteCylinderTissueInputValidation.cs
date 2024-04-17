using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the infinite cylinders are entirely contained within tissue layer,
    /// that at least one tissue layer is defined.
    /// </summary>
    public static class MultiConcentricInfiniteCylinderTissueInputValidation
    {
        /// <summary>
        /// Main validation method for MultiConcentricInfiniteCylinderTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((MultiConcentricInfiniteCylinderTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var cylinders = ((MultiConcentricInfiniteCylinderTissueInput)input).InfiniteCylinderRegions.Select(
                region => (InfiniteCylinderTissueRegion)region).ToArray();
            var tempResult = ValidateGeometry(layers, cylinders);
            return tempResult;
        }

        /// <summary>
        /// Method to validate that the geometry of tissue layers and ellipsoid agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="infiniteCylinders">List of InfiniteCylinderTissueRegion</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, IList<InfiniteCylinderTissueRegion> infiniteCylinders)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);
            if (!tempResult.IsValid){ return tempResult; }

            if (infiniteCylinders.Any(region => region.Radius == 0.0))
            {
                tempResult = new ValidationResult(
                    false,
                    "MultiConcentricInfiniteCylinderTissueInput: one infinite cylinder has radius equal to 0",
                    "MultiConcentricInfiniteCylinderTissueInput: make sure infinite cylinder radii are > 0");
            }
            if (!tempResult.IsValid) { return tempResult; }

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());

            // check that there is at least one layer of tissue 
            var layerTissueRegions = tissueLayers as LayerTissueRegion[] ?? tissueLayers.ToArray();
            if (!layerTissueRegions.Any())
            {
                tempResult = new ValidationResult(
                    false,
                    "MultiConcentricInfiniteCylinderTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "MultiConcentricInfiniteCylinderTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }
            if (!tempResult.IsValid) { return tempResult; }

            // check that infinite cylinders all have same Center
            var theCenter = infiniteCylinders[0].Center;

            foreach (var cylinder in infiniteCylinders.Skip(1))
            {
                if (cylinder.Center != theCenter)
                {
                    tempResult = new ValidationResult(
                    false,
                    "MultiConcentricInfiniteCylinderTissueInput: infinite cylinders are not concentric",
                    "MultiConcentricInfiniteCylinderTissueInput: set Center of each to be the same");
                }
            }
            if (!tempResult.IsValid) { return tempResult; }

            // check that cylinder with largest radius contained within a tissue layer then all will be
            var largestRadius = infiniteCylinders.Select(region => region.Radius).Max();
            var correctlyContainedInLayer = layerTissueRegions.Any(
                layer =>
                    layer.ContainsPosition(theCenter) &&
                    theCenter.Z + largestRadius <= layer.ZRange.Stop &&
                    theCenter.Z - largestRadius >= layer.ZRange.Start
                );

            if (!correctlyContainedInLayer)
            {
                tempResult = new ValidationResult(
                    false,
                    "MultiConcentricInfiniteCylinderTissueInput: infinite cylinders must be entirely contained within a tissue layer",
                    "Resize radii and or Center so that infinite cylinders entirely within tissue layer");
            }

            if (!tempResult.IsValid) { return tempResult; }

            return new ValidationResult(
                true,
                "MultiConcentricInfiniteCylinderTissueInput: geometry and refractive index settings validated");
        }
    }
}
