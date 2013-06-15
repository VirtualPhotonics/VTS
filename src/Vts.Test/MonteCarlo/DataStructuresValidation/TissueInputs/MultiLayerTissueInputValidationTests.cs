using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhaseFunctionInputs;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.DataStructuresValidation.TissueInputs
{
    [TestFixture]
    public class MultiLayerTissueInputValidationTests
    {
        /// <summary>
        /// Test to check that layers that overlap.
        /// </summary>
        [Test]
        public void validate_layers_do_not_overlap()
        {
            MultiLayerTissueInput ti = new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreesteinKey1"),
                        new LayerRegion(
                            new DoubleRange(0.0, 50.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreesteinKey1"),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreesteinKey1")
                    }
                );
            if (!ti.RegionPhaseFunctionInputs.ContainsKey("HenyeyGreesteinKey1"))
                ti.RegionPhaseFunctionInputs.Add("HenyeyGreesteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                ti,
                new List<IDetectorInput>(){ }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        /// <summary>
        /// Test to check that 0 thickness layers not defined.
        /// </summary>
        [Test]
        public void validate_layers_do_not_have_0_thickness()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        new HenyeyGreensteinPhaseFunctionInput()),
                        new LayerRegion(
                            new DoubleRange(0.0, 0.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        new HenyeyGreensteinPhaseFunctionInput()),
                        new LayerRegion(
                            new DoubleRange(0.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        new HenyeyGreensteinPhaseFunctionInput())
                    }
                ),
                new List<IDetectorInput>(){ }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
        /// <summary>
        /// Test to check that top and bottom layers are defined as air
        /// </summary>
        [Test]
        public void validate_top_and_bottom_layers_are_air()
        {
            var input = new SimulationInput(
                10,
                "",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4))
                    }
                ),
                new List<IDetectorInput>() { }
            );
            var result = SimulationInputValidation.ValidateInput(input);
            Assert.IsFalse(result.IsValid);
        }
    }
}
