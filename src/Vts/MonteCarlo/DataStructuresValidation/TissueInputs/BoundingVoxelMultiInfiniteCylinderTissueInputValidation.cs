using System;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the BoundingVoxelMultiInfiniteCylinder has same height as the tissue
    /// </summary>
    public static class BoundingVoxelMultiInfiniteCylinderTissueInputValidation
    {
        /// <summary>
        /// Main validation method for BoundingVoxelMultiInfiniteCylindersTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>An instance of the ValidationResult class</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((BoundingVoxelMultiInfiniteCylinderTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var boundingVoxel = (CaplessVoxelTissueRegion)((BoundingVoxelMultiInfiniteCylinderTissueInput)input).VoxelRegion;
            var cylinders = (IList<InfiniteCylinderTissueRegion>)((BoundingVoxelMultiInfiniteCylinderTissueInput)input).InclusionRegions.Select(
                region => (InfiniteCylinderTissueRegion)region).ToArray();
            var tempResult = ValidateGeometry(layers, boundingVoxel, cylinders);
            return tempResult;
        }

        /// <summary>
        /// Method to validate that the geometry of tissue layers and BoundingVoxelMultiInfiniteCylinders agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="boundingVoxel">Voxel that bounds tissue</param>
        /// <param name="cylinders">Cylinders within one or more layers</param>
        /// <returns>An instance of the ValidationResult class</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, 
            CaplessVoxelTissueRegion boundingVoxel, IList<InfiniteCylinderTissueRegion> cylinders)
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
                    "BoundingVoxelMultiInfiniteCylindersTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "BoundingVoxelMultiInfiniteCylindersTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }

            if (!tempResult.IsValid) return tempResult;

            var layersHeight = layerTissueRegions.Sum(layer => layer.ZRange.Delta);

            if (Math.Abs(boundingVoxel.Z.Delta - layersHeight) > 1e-3)
            {
                tempResult = new ValidationResult(
                    false,
                    "BoundingVoxelMultiInfiniteCylindersTissueInput: bounding voxel must have same height as tissue",
                    "BoundingVoxelMultiInfiniteCylindersTissueInput: make sure bounding voxel Z (Stop-Start) = depth of tissue");
            }

            if (!tempResult.IsValid) return tempResult;

            return new ValidationResult(
                true,
                "BoundingVoxelMultiInfiniteCylindersTissueInput: geometry and refractive index settings validated");
        }
    }
}
