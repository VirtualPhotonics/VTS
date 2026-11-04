using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.TissueInputs
{
    [TestFixture]
    public class BoundingVoxelMultiInfiniteCylinderTissueInputValidationTests
    {        
        /// <summary>
        /// Test to check that underlying BoundingVoxelMultiLayerTissue is good
        /// </summary>
        [Test]
        public void validate_underlying_BoundingVoxelMultilayer_tissue_definition()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingVoxelMultiInfiniteCylinderTissueInput(
                    new CaplessVoxelTissueRegion(
                    new DoubleRange(-10.0, 10),
                    new DoubleRange(-10.0, 10),
                    new DoubleRange(0.0, 10),
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 1), 
                        0.5, 
                        new OpticalProperties()),
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 2),
                        0.5,
                        new OpticalProperties())
                    ],
                    // define layer tissues that are incorrect
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                ),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput()
                });
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }

        /// <summary>
        /// Test to check that InfiniteCylinder has non-zero axis definitions.
        /// </summary>
        [Test]
        public void validate_InfiniteCylinder_has_nonzero_semiaxes()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingVoxelMultiInfiniteCylinderTissueInput(
                        new CaplessVoxelTissueRegion(      
                            new DoubleRange(-10.0, 10),
                            new DoubleRange(-10.0, 10),
                            new DoubleRange(0.0, 0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)), 
                        new ITissueRegion[] {
                            new InfiniteCylinderTissueRegion(
                                new Position(0, 0, 5), 1,
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                            new InfiniteCylinderTissueRegion(
                                new Position(0, 0, 15), 1,
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4))
                        },
                        [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput()
                });
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
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
                new BoundingVoxelMultiInfiniteCylinderTissueInput(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(0.0, 0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        new InfiniteCylinderTissueRegion(),
                        new InfiniteCylinderTissueRegion()
                    ],
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                ),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput()
                });
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }

        /// <summary>
        /// Test to check that InfiniteCylinder is entirely contained within tissue layer
        /// </summary>
        [Test]
        public void validate_InfiniteCylinder_is_within_tissue_layer()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingVoxelMultiInfiniteCylinderTissueInput(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(0.0, 0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        // first InfiniteCylinder intersection layer boundary at z=0  
                        new InfiniteCylinderTissueRegion(
                            new Position(0,0,0), 
                            1.0, 
                            new OpticalProperties()
                        ),
                        new InfiniteCylinderTissueRegion( // this InfiniteCylinder definition is fine
                            new Position(0,0,10),
                            1.0,
                            new OpticalProperties())
                    ],
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                ),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput()
                });
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }

        /// <summary>
        /// Test to check that InfiniteCylinder refractive index mismatch refractive index of
        /// surrounding layer passes
        /// </summary>
        [Test]
        public void validate_InfiniteCylinder_refractive_index_mismatches_that_of_surrounding_layer_is_okay()
        {
            var input = new SimulationInput(
                100,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingVoxelMultiInfiniteCylinderTissueInput(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(0.0, 20),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 2), 
                            1.0, 
                            new OpticalProperties(0.01, 1.0, 0.9, 1.3)),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 5), 
                            1.0, 
                            new OpticalProperties(0.01, 1.0, 0.9, 1.3))
                    ],
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                ),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput()
                });
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.True);
        }

        /// <summary>
        /// Test to check that InfiniteCylinders are not concentric 
        /// </summary>
        [Test]
        public void validate_InfiniteCylinders_are_not_concentric()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingVoxelMultiInfiniteCylinderTissueInput(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(0.0, 0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        // concentric cylinder definition
                        new InfiniteCylinderTissueRegion(
                            new Position(0,0,10),
                            2.0,
                            new OpticalProperties()
                        ),
                        new InfiniteCylinderTissueRegion( 
                            new Position(0,0,10),
                            1.0,
                            new OpticalProperties())
                    ],
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                ),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput()
                });
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }

        /// <summary>
        /// Test to check that InfiniteCylinders do not overlap
        /// </summary>
        [Test]
        public void validate_InfiniteCylinders_do_not_overlap()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new BoundingVoxelMultiInfiniteCylinderTissueInput(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(-10.0, 10),
                        new DoubleRange(0.0, 0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        // overlapping cylinders
                        new InfiniteCylinderTissueRegion(
                            new Position(0,0,9),
                            2.0,
                            new OpticalProperties()
                        ),
                        new InfiniteCylinderTissueRegion(
                            new Position(0,0,10),
                            1.0,
                            new OpticalProperties())
                    ],
                    [
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    ]
                ),
                new List<IDetectorInput>
                {
                    new AOfXAndYAndZDetectorInput()
                });
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }

    }
}
