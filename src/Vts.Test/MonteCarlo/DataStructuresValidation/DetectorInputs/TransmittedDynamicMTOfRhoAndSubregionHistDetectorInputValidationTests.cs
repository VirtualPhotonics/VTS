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
    public class TransmittedDynamicMTOfRhoAndSubregionHistDetectorInputValidationTests
    {
        /// <summary>
        /// Check that blood volume fraction count specification matches tissue region count
        /// </summary>
        [Test]
        public void validate_blood_volume_fraction_list_length_matches_tissue_region()
        {
            var tissueInput = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 2.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(2.0, 20.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                }
            );
            var detectorInput = new List<IDetectorInput>()
            {
                new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput()
                {
                    Rho = new DoubleRange(0.0, 10.0, 2),
                    Z = new DoubleRange(0, 10.0, 2),
                    BloodVolumeFraction = new List<double>() {0, 0.5, 0.4, 0},
                    MTBins = new DoubleRange(0, 500, 6)
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
            // test case where blood volume list count does not match number tissue regions
            detectorInput = new List<IDetectorInput>()
            {
                new TransmittedDynamicMTOfRhoAndSubregionHistDetectorInput()
                {
                    Rho = new DoubleRange(-10.0, 10.0, 2),
                    Z = new DoubleRange(0, 10.0, 2),
                    BloodVolumeFraction = new List<double>() {0, 0.4, 0},
                    MTBins = new DoubleRange(0, 500, 6)
                }
            };
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
