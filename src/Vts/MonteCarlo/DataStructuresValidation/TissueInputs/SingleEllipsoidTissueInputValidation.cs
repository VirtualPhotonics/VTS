using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the ellipsoid is entirely contained within tissue layer,
    /// that only one tissue layer is defined, and that the refractive index of the
    /// tissue layer and ellipsoid match.
    /// </summary>
    public static class SingleEllipsoidTissueInputValidation
    {
        /// <summary>
        /// Main validation method for SingleEllipsoidTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((SingleEllipsoidTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var ellipsoid = (EllipsoidTissueRegion)((SingleEllipsoidTissueInput)input).EllipsoidRegion;
            var tempResult = ValidateGeometry(layers, ellipsoid);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            tempResult = ValidateRefractiveIndexMatch(layers, ellipsoid);

            return tempResult;
        }
        /// <summary>
        /// Method to validate that the geometry of tissue layers and ellipsoid agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="ellipsoid">EllipsoidTissueRegion</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, EllipsoidTissueRegion ellipsoid)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);

            if (!tempResult.IsValid) return tempResult;

            if (ellipsoid.Dx == 0 || ellipsoid.Dy == 0 || ellipsoid.Dz == 0)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleEllipsoidTissueInput: ellipsoid has a radial axis equal to 0",
                    "SingleEllipsoidTissueInput: make sure Dx, Dy, Dz are > 0");
            }

            if (!tempResult.IsValid) return tempResult;

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());

            // check that there is at least one layer of tissue 
            var layerTissueRegions = tissueLayers.ToList();
            if (!layerTissueRegions.Any())
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleEllipsoidTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "SingleEllipsoidTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }

            if (!tempResult.IsValid) return tempResult;

            // check that ellipsoid contained within a tissue layer
            var correctlyContainedInLayer = layerTissueRegions.Any(
                layer =>
                    layer.ContainsPosition(ellipsoid.Center) &&
                    ellipsoid.Center.Z + ellipsoid.Dz <= layer.ZRange.Stop &&
                    ellipsoid.Center.Z - ellipsoid.Dz >= layer.ZRange.Start
                );

            if (!correctlyContainedInLayer)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleEllipsoidTissueInput: ellipsoid must be entirely contained within a tissue layer",
                    "Resize Dz of Ellipsoid dimension so that 2*Dz<layer[1] depth");
            }

            if (!tempResult.IsValid) return tempResult;

            return new ValidationResult(
                true,
                "SingleEllipsoidTissueInput: geometry and refractive index settings validated");
        }

        /// <summary>
        /// Method to verify refractive index of tissue layer and ellipsoid match.
        /// Code does not yet include reflecting/refracting off ellipsoid surface.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="ellipsoid">EllipsoidTissueRegion></param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateRefractiveIndexMatch(
            IList<LayerTissueRegion> layers, EllipsoidTissueRegion ellipsoid)
        {
            var containingLayerIndex = -1;
            for (var i = 0; i < layers.Count - 1; i++)
            {
                if (layers[i].ContainsPosition(ellipsoid.Center) &&
                    ellipsoid.Center.Z + ellipsoid.Dz <= layers[i].ZRange.Stop &&
                    ellipsoid.Center.Z - ellipsoid.Dz >= layers[i].ZRange.Start)
                {
                    containingLayerIndex = i;
                }
            }
            if (containingLayerIndex != -1 && layers[containingLayerIndex].RegionOP.N != ellipsoid.RegionOP.N)
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
