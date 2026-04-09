using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.DetectorInputs
{
    [TestFixture]
    public class RadianceOfRhoAtZDetectorInputValidationTests
    {
        /// <summary>
        /// Check that ZDepth specification matches tissue layer definition
        /// </summary>
        [Test]
        public void validate_z_depth_matches_tissue_layer_definition()
        {
            const double layerThickness = 2.0;
            var tissueInput = new MultiLayerTissueInput(
                [
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, layerThickness),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(layerThickness, 20.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]
            );
            // check a good specification of ZDepth, i.e. at layerThickness
            var detectorInput = new List<IDetectorInput>
            {
                new RadianceOfRhoAtZDetectorInput
                {
                    Rho = new DoubleRange(0.0, 10.0, 2),
                    ZDepth = layerThickness
                }
            };
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                detectorInput
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.True);
            // test case where ZDepth not at tissue layer interface
            detectorInput =
            [
                new RadianceOfRhoAtZDetectorInput
                {
                    Rho = new DoubleRange(-10.0, 10.0, 2),
                    ZDepth = layerThickness + 0.5
                }
            ];
            input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                detectorInput
            );
            result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
            // test case where ZDepth specified at bottom of tissue
            detectorInput =
            [
                new RadianceOfRhoAtZDetectorInput
                {
                    Rho = new DoubleRange(-10.0, 10.0, 2),
                    ZDepth = 20
                }
            ];
            input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                detectorInput
            );
            result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }
    }
}
