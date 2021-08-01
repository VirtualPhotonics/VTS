using System;
using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Factories;
using Moq;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for TissueFactory
    /// </summary>
    [TestFixture]
    public class TissueFactoryTests
    {
        /// <summary>
        /// Simulate basic usage of TissueFactory
        /// </summary>
        [Test]
        public void Demonstrate_GetTissue()
        {
            ITissueInput tissueInput = new MultiLayerTissueInput
                {
                    TissueType =  "MultiLayer",
                };
            var tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Analog,
                PhaseFunctionType.HenyeyGreenstein,
                0.0);

            Assert.NotNull(tissue);
        }        
        /// <summary>
        /// Simulate erroneous invocation
        /// </summary>
        [Ignore("need to ask Lisa for help")]
        [Test]
        public void Demonstrate_GetTissue_returns_null_on_faulty_tissue_input()
        {
            var tissueInputMock = new Mock<MultiLayerTissueInput>();
            tissueInputMock.Setup(x => x.CreateTissue(
                AbsorptionWeightingType.Analog,
                PhaseFunctionType.Bidirectional,
                0.0)).Returns((ITissue)null);

            var tissue = TissueFactory.GetTissue(
                tissueInputMock.Object,
            AbsorptionWeightingType.Analog,
            PhaseFunctionType.Bidirectional,
            0.0);

            Assert.IsNull(tissue);
        }
    }
}
