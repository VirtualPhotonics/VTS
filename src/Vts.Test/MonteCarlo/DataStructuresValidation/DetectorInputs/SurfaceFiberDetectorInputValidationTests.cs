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
    public class SurfaceFiberDetectorInputValidationTests
    {
        /// <summary>
        /// Test to check that detector defined on surface of tissue
        /// </summary>
        [Test]
        public void validate_code_checks_that_detector_defined_on_tissue_surface()
        {
            var tissueInput = new MultiLayerTissueInput();
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                new List<IDetectorInput>()
                {
                    new SurfaceFiberDetectorInput()
                    {
                        Center = new Position(0, 0, 10)
                    }  
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.False);
        }
    }
}
