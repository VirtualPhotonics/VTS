using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.PostProcessorInputs
{
    [TestFixture]
    public class PostProcessorInputValidationTests
    {
        /// <summary>
        /// Test to check that post-processor perturbed OPs are not negative
        /// </summary>
        [Test]
        public void validate_tissue_optical_properties_are_non_negative()
        {
            var tissueInput = new MultiLayerTissueInput(
                    new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(-1.0, 1.0, 0.8, 1.4)), // make mua negative
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
            var input = new PostProcessorInput(
                new List<IDetectorInput>()
                {
                    new pMCROfRhoDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 100.0),
                        // set perturbed ops to reference ops
                        PerturbedOps = new List<OpticalProperties>() 
                        { 
                            tissueInput.Regions[0].RegionOP,
                            tissueInput.Regions[1].RegionOP,
                            tissueInput.Regions[2].RegionOP
                        },
                        PerturbedRegionsIndices = new List<int>() { 1 }
                    }  
                },
                "","",""
            );
            var result = PostProcessorInputValidation.ValidateInput(input,"");
            Assert.IsFalse(result.IsValid);
        }
    }
}
