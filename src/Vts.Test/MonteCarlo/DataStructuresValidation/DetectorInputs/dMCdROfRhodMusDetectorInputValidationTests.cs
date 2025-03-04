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
    public class dMCdROfRhodMusDetectorInputValidationTests
    {
        /// <summary>
        /// Can only run dMC with one tissue region at the present.
        /// Check that detector input does not specify more than 1 region
        /// </summary>
        [Test]
        public void validate_only_one_perturbed_region_index_specified()
        {
            var tissueInput = new MultiLayerTissueInput();
            var detectorInput = new List<IDetectorInput>()
            {
                new dMCdROfRhodMusDetectorInput()
                {
                    Rho = new DoubleRange(0.0, 100.0),
                    // set perturbed ops to reference ops
                    PerturbedOps = new List<OpticalProperties>()
                    {
                        tissueInput.Regions[0].RegionOP,
                        tissueInput.Regions[1].RegionOP,
                        tissueInput.Regions[2].RegionOP
                    },
                    // set number of perturbed regions to 2
                    PerturbedRegionsIndices = new List<int>() {1, 2}
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
            Assert.That(result.IsValid, Is.False);
            // redefine detector number of perturbed region to pass validation
            ((dMCdROfRhodMusDetectorInput)detectorInput.First()).PerturbedRegionsIndices = new List<int> { 1 };
            input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                tissueInput,
                detectorInput
            );
            result = SimulationInputValidation.ValidateInput(input);
            Assert.That(result.IsValid, Is.True);
        }
    }
}
