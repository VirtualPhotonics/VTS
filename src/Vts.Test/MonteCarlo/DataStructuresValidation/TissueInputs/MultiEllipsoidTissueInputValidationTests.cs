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
    public class MultiEllipsoidTissueInputValidationTests
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
                new MultiEllipsoidTissueInput(
                    [
                        new EllipsoidTissueRegion(
                        new Position(0, 0, 1), 
                        0.5, 
                        0.5, 
                        0.5, 
                        new OpticalProperties()),
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 2),
                        0.5,
                        0.5,
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
        /// Test to check that ellipsoid has non-zero axis definitions.
        /// </summary>
        [Test]
        public void validate_ellipsoid_has_nonzero_semiaxes()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiEllipsoidTissueInput(
                    [
                        new EllipsoidTissueRegion(),
                        // set ellipsoid axis to 0.0
                        new EllipsoidTissueRegion(
                            new Position(0, 0, 1), 
                            0.0, // bad
                            1.0, 
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
                new MultiEllipsoidTissueInput(
                    [
                        new EllipsoidTissueRegion(),
                        new EllipsoidTissueRegion()
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
        /// Test to check that ellipsoid is entirely contained within tissue layer
        /// </summary>
        [Test]
        public void validate_ellipsoid_is_within_tissue_layer()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiEllipsoidTissueInput(
                    [
                        // first ellipsoid intersection layer boundary at z=0  
                        new EllipsoidTissueRegion(
                            new Position(0,0,0), 
                            1.0, 
                            1.0, 
                            1.0, 
                            new OpticalProperties()
                        ),
                        new EllipsoidTissueRegion( // this ellipsoid definition is fine
                            new Position(0,0,10),
                            1.0,
                            1.0,
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
        /// Test to check that ellipsoid refractive index mismatch refractive index of
        /// surrounding layer passes
        /// </summary>
        [Test]
        public void validate_ellipsoid_refractive_index_mismatches_that_of_surrounding_layer_is_okay()
        {
            var input = new SimulationInput(
                100,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiEllipsoidTissueInput(
                    [
                        new EllipsoidTissueRegion(
                            new Position(0, 0, 2), 
                            1.0, 
                            1.0, 
                            1.0,
                            new OpticalProperties(0.01, 1.0, 0.9, 1.3)),
                        new EllipsoidTissueRegion(
                            new Position(0, 0, 3), 
                            1.0, 
                            1.0, 
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
    }
}
