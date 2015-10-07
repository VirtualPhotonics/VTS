using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.TissueInputs
{
    [TestFixture]
    public class SingleVoxelTissueInputValidationTests
    {
        /// <summary>
        /// Test to check that voxel has non-zero axis definitions.
        /// </summary>
        [Test]
        public void validate_voxel_has_nonzero_dimensions()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new SingleVoxelTissueInput(
                    // set ellipsoid axis to 0.0
                    new VoxelTissueRegion(new DoubleRange(0, 0),
                        new DoubleRange(-10, 10), 
                        new DoubleRange(1, 5),
                        new OpticalProperties()), 
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>(){ }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        /// <summary>
        /// Test to check that at least one tissue layer is defined
        /// </summary>
        [Test]
        public void validate_at_least_one_tissue_layer_defined()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new SingleVoxelTissueInput(
                    new VoxelTissueRegion(), 
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ), 
                new List<IDetectorInput>(){ }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        /// <summary>
        /// Test to check that voxel is entirely contained within tissue layer
        /// </summary>
        [Test]
        public void validate_voxel_is_within_tissue_layer()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new SingleVoxelTissueInput(
                    new VoxelTissueRegion(new DoubleRange(-5, 5), 
                        new DoubleRange(-5, 5),
                        new DoubleRange(0, 10), // eheck that voxel not at surface
                        new OpticalProperties()), 
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ), 
                new List<IDetectorInput>() { }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        /// <summary>
        /// Test to check that voxel refractive index matches refractive index of surrounding layer
        /// </summary>
        [Test]
        public void validate_voxel_refractive_index_matches_that_of_surrounding_layer()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new SingleVoxelTissueInput(
                    new VoxelTissueRegion(new DoubleRange(-5, 5),  
                        new DoubleRange(-5, 5),
                        new DoubleRange(1, 2),
                        new OpticalProperties(0.01, 1.0, 0.9, 1.3)),
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>() { }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
    }
}
