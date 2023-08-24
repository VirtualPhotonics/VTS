using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using MathNet.Numerics.Random;
using Vts.Common;
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
            var tissueInput = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                });
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            tissueInput.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            IDictionary<string, IPhaseFunction> phaseFunctions = new Dictionary<string, IPhaseFunction>();
            for (int i = 0; i < tissueInput.Regions.Length; i++)
            {
                if (!phaseFunctions.ContainsKey(tissueInput.Regions[i].PhaseFunctionKey))
                {
                    phaseFunctions.Add(tissueInput.Regions[i].PhaseFunctionKey,
                        PhaseFunctionFactory.GetPhaseFunction(tissueInput.Regions[i],
                            tissueInput, new MersenneTwister(0)));
                }
            }
            var tissue = TissueFactory.GetTissue(
                tissueInput,
                AbsorptionWeightingType.Analog,
                phaseFunctions,
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
            var phaseFunctions = new Dictionary<string, IPhaseFunction>();
            tissueInputMock.CreateTissue(
                Arg.Any<AbsorptionWeightingType>(),
                phaseFunctions,
                Arg.Any<double>()).Returns((ITissue)null);

            Assert.Throws<ArgumentException>(() =>
                TissueFactory.GetTissue(
                    tissueInputMock,
                    AbsorptionWeightingType.Analog,
                    phaseFunctions,
                    0.0));
        }
    }
}
