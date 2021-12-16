using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.DetectorInputs
{
    [TestFixture]
    public class RecessedDetectorInputValidationTests
    {
        /// <summary>
        /// Need to specify ZPlane as 0 or negative in the infile
        /// </summary>
        [Test]
        public void Validate_ZPlane_is_specified_correctly()
        {
            var tissueInput = new MultiLayerTissueInput();
            var detectorInput = new List<IDetectorInput>()
            {
                new ROfXAndYRecessedDetectorInput()
                {
                    ZPlane = 1.0 // make positive in error
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
            Assert.IsFalse(result.IsValid);
            // redefine ZPlane to be negative to pass validation
            ((ROfXAndYRecessedDetectorInput) detectorInput.First()).ZPlane = -1.0;
            input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                detectorInput
            );
            result = SimulationInputValidation.ValidateInput(input);
            Assert.IsTrue(result.IsValid);
        }
    }
}
