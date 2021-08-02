using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.TissueInputs
{
    [TestFixture]
    public class BoundingCylinderTissueInputValidationTests
    {
        /// <summary>
        /// Test to check that underlying MultiLayerTissue is good
        /// </summary>
        [Test]
        public void validate_underlying_multilayer_tissue_definition()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingCylinderTissueInput(
                    new CaplessCylinderTissueRegion(
                        new Position(0, 0, 1),
                        0.0,
                        10.0,
                        new OpticalProperties()),
                    // define layer tissues that are incorrect
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 10.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>() { }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        /// <summary>
        /// Test to check that bounding cylinder height is height of tissue layer
        /// </summary>
        [Test]
        public void validate_bounding_cylinder_height()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingCylinderTissueInput(
                    // set cylinder height to 1 and tissue layer height to 20
                    new CaplessCylinderTissueRegion(
                        new Position(0, 0, 1), 
                        0.0, 
                        1.0, 
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
                new BoundingCylinderTissueInput(
                    new CaplessCylinderTissueRegion(
                        new Position(0, 0, 3),
                        1.0,
                        20.0,
                        new OpticalProperties(0.01, 1.0, 0.9, 1.3)), 
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
        /// Test to check that bounding cylinder refractive index matches refractive index of surrounding layer
        /// </summary>
        [Test]
        public void validate_bounding_cylinder_refractive_index_matches_that_of_layers()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                // set n of cylinder to be 1.3 different from the tissue
                new BoundingCylinderTissueInput(
                    new CaplessCylinderTissueRegion(
                        new Position(0, 0, 3), 
                        1.0, 
                        20.0, 
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
