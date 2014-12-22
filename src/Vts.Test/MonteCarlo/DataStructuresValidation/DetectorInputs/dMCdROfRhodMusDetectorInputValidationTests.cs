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
    public class dMCdROfRhodMusDetectorInputValidationTests
    {
        /// <summary>
        /// Test to check that layers that overlap.
        /// </summary>
        [Test]
        public void validate_only_one_perturbed_region_index_specified()
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
                    new dMCdROfRhodMusDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 100.0),
                        // set perturbed ops to reference ops
                        PerturbedOps = new List<OpticalProperties>() 
                        { 
                            tissueInput.Regions[0].RegionOP,
                            tissueInput.Regions[1].RegionOP,
                            tissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices = new List<int>() { 1, 2 }
                    }
                }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
    }
}
