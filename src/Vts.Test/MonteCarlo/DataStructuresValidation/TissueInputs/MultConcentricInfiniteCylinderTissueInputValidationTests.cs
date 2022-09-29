using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.TissueInputs
{
    [TestFixture]
    public class MultiConcentricInfiniteCylinderTissueInputValidationTests
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
                new MultiConcentricInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion[]
                    {
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 10),
                        2.0,
                        new OpticalProperties()),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 10),
                        1.0,
                        new OpticalProperties()),
                    },
                    // define layer tissues that are incorrect
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
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
        /// Test to check that infinite cylinders have non-zero axis definitions.
        /// </summary>
        [Test]
        public void validate_infinite_cylinders_have_nonzero_radii()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiConcentricInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion[]
                    {
                        // set one infinite cylinder radius to 0.0
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 0.0, new OpticalProperties()),
                                                new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 1.0, new OpticalProperties()),
                    },
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
                new MultiConcentricInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion[]
                    {
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 2.0, new OpticalProperties()),
                                                new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 1.0, new OpticalProperties()),
                    },
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
        /// Test to check that infinite cylinders have same center
        /// </summary>
        [Test]
        public void validate_infinite_cylinders_have_same_center()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiConcentricInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion[]
                    {
                        // set different Centers
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 5), 2.0, new OpticalProperties()),
                                                new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 1.0, new OpticalProperties()),
                    },
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 10.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(10.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ), 
                new List<IDetectorInput>() { }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        /// <summary>
        /// Test to check that infinite cylinder is entirely contained within tissue layer
        /// </summary>
        [Test]
        public void validate_ellipsoid_is_within_tissue_layer()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiConcentricInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion[]
                    {
                        // set one infinite cylinder radius to 0.0
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 15.0, new OpticalProperties()),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 1.0, new OpticalProperties()),
                    }, new ITissueRegion[]
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
        /// Test to check that ellipsoid refractive index matches refractive index of surrounding layer
        /// </summary>
        [Test]
        public void validate_ellipsoid_refractive_index_matches_that_of_surrounding_layer()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiConcentricInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion[]
                    {
                        // set refractive index to not match
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 0.0, 
                            new OpticalProperties(1.0, 1.0, 0.8, 1.6)),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1), 1.0, new OpticalProperties()),
                    },
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
