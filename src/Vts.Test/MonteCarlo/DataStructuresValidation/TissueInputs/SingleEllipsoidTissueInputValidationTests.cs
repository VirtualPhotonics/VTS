using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.TissueInputs
{
    [TestFixture]
    public class SingleEllipsoidTissueInputValidationTests
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
                new SingleEllipsoidTissueInput(
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 1), 
                        0.0, 
                        1.0, 
                        1.0, 
                        new OpticalProperties(),
                        "HenyeyGreensteinKey4"),
                    // define layer tissues that are incorrect
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey3")
                    }
                ),
                new List<IDetectorInput>() { }
            );
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
                new SingleEllipsoidTissueInput(
                    // set ellipsoid axis to 0.0
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 1), 
                        0.0, 1.0, 1.0, 
                        new OpticalProperties(), 
                        "HenyeyGreensteinKey4"), 
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey3")
                    }
                ),
                new List<IDetectorInput>(){ }
            );
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
                new SingleEllipsoidTissueInput(
                    new EllipsoidTissueRegion(), 
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey2")
                    }
                ), 
                new List<IDetectorInput>(){ }
            );
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
                new SingleEllipsoidTissueInput(
                    new EllipsoidTissueRegion(new Position(0,0,0), 1.0, 1.0, 1.0, new OpticalProperties(),"" ), 
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            ""),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            ""),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "")
                    }
                ), 
                new List<IDetectorInput>() { }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
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
                new SingleEllipsoidTissueInput(
                    new EllipsoidTissueRegion(
                        new Position(0, 0, 3), 1.0, 1.0, 1.0, 
                        new OpticalProperties(0.01, 1.0, 0.9, 1.3), 
                        "HenyeyGreensteinKey4"),
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey1"),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                            "HenyeyGreensteinKey2"),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                            "HenyeyGreensteinKey3")
                    }
                ),
                new List<IDetectorInput>() { }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }
    }
}
