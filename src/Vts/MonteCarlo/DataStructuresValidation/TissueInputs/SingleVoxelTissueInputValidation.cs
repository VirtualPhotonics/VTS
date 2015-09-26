using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// This verifies that the voxel is entirely contained within tissue layer,
    /// that only one tissue layer is defined, and that the refractive index of the
    /// tissue layer and voxel match.
    /// </summary>
    public class SingleVoxelTissueInputValidation
    {
        /// <summary>
        /// Main validation method for SingleVoxelTissueInput.
        /// </summary>
        /// <param name="input">tissue input defined in SimulationInput</param>
        /// <returns>ValidationResult</returns>
        public static ValidationResult ValidateInput(ITissueInput input)
        {
            var layers = ((SingleVoxelTissueInput)input).LayerRegions.Select(region => (LayerTissueRegion)region).ToArray();
            var voxel = (VoxelTissueRegion)((SingleVoxelTissueInput)input).VoxelRegion;
            ValidationResult tempResult;
            tempResult = ValidateGeometry(layers, voxel);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            tempResult = ValidateRefractiveIndexMatch(layers, voxel);
            if (!tempResult.IsValid)
            {
                return tempResult;
            }
            return tempResult;
        }
        /// <summary>
        /// Method to validate that the geometry of tissue layers and Voxel agree with capabilities
        /// of code.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="Voxel">VoxelTissueRegion</param>
        /// <returns>ValidationResult</returns>
        private static ValidationResult ValidateGeometry(IList<LayerTissueRegion> layers, VoxelTissueRegion Voxel)
        {            
            // check that layer definition is valid
            var tempResult = MultiLayerTissueInputValidation.ValidateLayers(layers);

            if (!tempResult.IsValid){ return tempResult; }

            if ((Voxel.X.Stop - Voxel.X.Start <= 0) || ((Voxel.Y.Stop - Voxel.Y.Start) <= 0) || ((Voxel.Z.Stop - Voxel.Z.Start) <= 0))
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleVoxelTissueInput: Voxel has 0 dimension along at least one side",
                    "SingleVoxelTissueInput: make sure X.Stop (Y.Stop, Z.Stop) is > X.Start (Y.Stop, Z.Stop)");
            }

            if (!tempResult.IsValid) { return tempResult; }

            // test for air layers and eliminate from list
            var tissueLayers = layers.Where(layer => !layer.IsAir());

            // check that there is at least one layer of tissue 
            if (!tissueLayers.Any())
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleVoxelTissueInput: tissue layer is assumed to be at least a single layer with air layer above and below",
                    "SingleVoxelTissueInput: redefine tissue definition to contain at least a single layer of tissue");
            }

            if (!tempResult.IsValid) { return tempResult; }

            // check that Voxel contained within a tissue layer
            bool correctlyContainedInLayer = tissueLayers.Any(
                layer =>
                    Voxel.Z.Start > layer.ZRange.Start &&
                    Voxel.Z.Stop < layer.ZRange.Stop
                );

            if (!correctlyContainedInLayer)
            {
                tempResult = new ValidationResult(
                    false,
                    "SingleVoxelTissueInput: Voxel must be entirely contained within a tissue layer",
                    "Redefine Voxel.Z.Start and Voxel.Z.Stop to be entirely within layer.ZRange.Start and Stop");
            }

            if (!tempResult.IsValid) { return tempResult; }

            return new ValidationResult(
                true,
                "SingleVoxelTissueInput: geometry and refractive index settings validated");
        }

        /// <summary>
        /// Method to verify refractive index of tissue layer and Voxel match.
        /// Code does not yet include reflecting/refracting off Voxel surface.
        /// </summary>
        /// <param name="layers">list of LayerTissueRegion</param>
        /// <param name="Voxel">VoxelTissueRegion></param>
        /// <returns>ValidationResult</returns>
        private static ValidationResult ValidateRefractiveIndexMatch(
            IList<LayerTissueRegion> layers, VoxelTissueRegion Voxel)
        {
            int containingLayerIndex = -1;
            for (int i = 0; i < layers.Count - 1; i++)
            {
                if (Voxel.Z.Start > layers[i].ZRange.Start &&
                    Voxel.Z.Stop < layers[i].ZRange.Stop)
                {
                    containingLayerIndex = i;
                }
            }
            if ((containingLayerIndex != -1) && (layers[containingLayerIndex].RegionOP.N != Voxel.RegionOP.N))
            {
                return new ValidationResult(
                    false,
                    "SingleVoxelTissueInput: refractive index of tissue layer must match that of voxel",
                    "Change N of voxel to match tissue layer N");
            }
            return new ValidationResult(
                true,
                "SingleVoxelTissueInput: refractive index of tissue and voxel match");
        }
    }
}
