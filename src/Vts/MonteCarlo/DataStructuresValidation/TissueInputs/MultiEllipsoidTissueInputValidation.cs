using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the ellipsoids are entirely contained within a tissue layer,
    /// that only one tissue layer is defined, and that the refractive index of the
    /// tissue layer and ellipsoid match.
    /// </summary>
    public static class MultiEllipsoidTissueInputValidation
    {
        /// <summary>
        /// Main validation method for MultiEllipsoidTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((MultiEllipsoidTissueInput)input).LayerRegions
                .Select(region => (LayerTissueRegion)region).ToArray();
            var ellipsoids = ((MultiEllipsoidTissueInput)input).EllipsoidRegions
                .Select(region => (EllipsoidTissueRegion)region).ToArray();
            var tempResult = ValidateGeometry(layers, ellipsoids);
            return tempResult;
        }

        /// <summary>
        /// Method to validate that the geometry of tissue layers and ellipsoid agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="ellipsoids">list of EllipsoidTissueRegion</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers,
            IList<EllipsoidTissueRegion> ellipsoids)
        {
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);

            if (!tempResult.IsValid) return tempResult;

            // check that ellipsoid semi-axes > 0
            foreach (var ellipsoid in ellipsoids)
            {
                if (ellipsoid.Dx <= 0 || ellipsoid.Dy <= 0 || ellipsoid.Dz <= 0)
                {
                    tempResult = new ValidationResult(
                        false,
                        "MultiEllipsoidTissueInput: ellipsoid has a radial axis equal to 0",
                        "MultiEllipsoidTissueInput: make sure Dx, Dy, Dz are > 0");
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
                    "MultiEllipsoidTissueInput: tissue layer is assumed to be at least a Multi layer with air layer above and below",
                    "MultiEllipsoidTissueInput: redefine tissue definition to contain at least a Multi layer of tissue");
            }

            if (!tempResult.IsValid) return tempResult;

            // check that each ellipsoid contained within a tissue layer
            foreach (var ellipsoid in ellipsoids)
            {
                var correctlyContainedInLayer = layerTissueRegions.Any(layer =>
                    layer.ContainsPosition(ellipsoid.Center) &&
                    ellipsoid.Center.Z + ellipsoid.Dz <= layer.ZRange.Stop &&
                    ellipsoid.Center.Z - ellipsoid.Dz >= layer.ZRange.Start
                );
                if (!correctlyContainedInLayer)
                {
                    tempResult = new ValidationResult(
                        false,
                        "MultiEllipsoidTissueInput: ellipsoids must be entirely contained within a tissue layer",
                        "MultiEllipsoidTissueInput: resize Dz of Ellipsoid dimension so that 2*Dz<thickness of layer of inclusion");

                }
            }

            if (!tempResult.IsValid) return tempResult;

            return new ValidationResult(
                true,
                "MultiEllipsoidTissueInput: geometry settings validated");
        }

    }
}
