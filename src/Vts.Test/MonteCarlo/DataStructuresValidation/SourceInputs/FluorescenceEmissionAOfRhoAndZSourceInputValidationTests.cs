using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.SourceInputs
{
    [TestFixture]
    public class FluorescenceEmissionAOfRhoAndZInputValidationTests
    {
        /// <summary>
        /// Test to check that validation result is correct when specifying
        /// Uniform sampling
        /// </summary>
        [Test]
        public void Validate_Uniform_sampling_specification_returns_correct_validation()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new FluorescenceEmissionAOfRhoAndZSourceInput
                {
                    SamplingMethod = SourcePositionSamplingType.Uniform
                },
                new MultiLayerTissueInput(),
                new List<IDetectorInput>
                {
                    new ROfRhoDetectorInput()
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid);
            Assert.IsTrue(result.ValidationRule != null);
        }
    }
}