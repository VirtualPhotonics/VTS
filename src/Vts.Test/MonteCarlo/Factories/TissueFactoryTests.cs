using NSubstitute;
using NUnit.Framework;
using System;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Tissues;

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
        public void Demonstrate_GetTissue_successful_return()
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
        [Test]
        public void Demonstrate_GetTissue_returns_null_on_faulty_tissue_input()
        {
            var tissueInputMock = Substitute.For<ITissueInput>();
            tissueInputMock.CreateTissue(
                Arg.Any<AbsorptionWeightingType>(),
                Arg.Any<PhaseFunctionType>(),
                Arg.Any<double>()).Returns((ITissue)null);

            Assert.Throws<ArgumentException>(() =>
                TissueFactory.GetTissue(
                    tissueInputMock,
                    AbsorptionWeightingType.Analog,
                    PhaseFunctionType.Bidirectional,
                    0.0));
        }
    }
}
