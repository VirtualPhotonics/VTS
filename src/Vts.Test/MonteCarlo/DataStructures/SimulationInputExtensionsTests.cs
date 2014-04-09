using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SimulationInputExtensionsTests
    {
        private SimulationInput _input;
        [TestFixtureSetUp]
        public void initialize_simulation_input()
        {
            // create input with two tissue layers
            _input = new SimulationInput(
                10,
                "Output",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                new MultiLayerTissueInput(
                    new ITissueRegion[]
					{ 
						new LayerRegion(
							new DoubleRange(double.NegativeInfinity, 0.0),
							new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
						new LayerRegion(
							new DoubleRange(0.0, 1.0),
							new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
						new LayerRegion(
							new DoubleRange(1.0, 20.0),
							new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
						new LayerRegion(
							new DoubleRange(20.0, double.PositiveInfinity),
							new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
					}
                ),
                new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput(), 
                }     
            );
        }
        [Test]
        public void verify_WithValue_method_modifies_mua1_correctly()
        {      
            var inputWithChange = _input.WithValue(InputParameterType.Mua1.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[1].RegionOP.Mua, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_mus1_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.Mus1.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[1].RegionOP.Mus, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_n1_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.N1.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[1].RegionOP.N, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_g1_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.G1.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[1].RegionOP.G, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_mua2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.Mua2.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[2].RegionOP.Mua, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_mus2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.Mus2.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[2].RegionOP.Mus, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_n2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.N2.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[2].RegionOP.N, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_g2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.G2.ToString(), 99.0);
            Assert.AreEqual(inputWithChange.TissueInput.Regions[2].RegionOP.G, 99.0);
        }
        [Test]
        public void verify_WithValue_method_modifies_d1_correctly()
        {
            // if change top layer to 99, second layer should be same thickness but start at 99
            var originalRegions = _input.TissueInput.Regions;

            var inputWithChange = _input.WithValue(InputParameterType.D1.ToString(), 99.0);

            var regionsWithChange = inputWithChange.TissueInput.Regions;

            Assert.AreEqual(((LayerRegion)regionsWithChange[1]).ZRange.Start, 0.0);
            Assert.AreEqual(((LayerRegion)regionsWithChange[1]).ZRange.Stop, 99.0);
            Assert.AreEqual(((LayerRegion)regionsWithChange[2]).ZRange.Start, 99.0);
            Assert.AreEqual(((LayerRegion)regionsWithChange[2]).ZRange.Delta, ((LayerRegion)originalRegions[2]).ZRange.Delta);
        }
    }
}
