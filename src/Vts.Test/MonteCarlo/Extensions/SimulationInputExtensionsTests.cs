using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SimulationInputExtensionsTests
    {
        private SimulationInput _input;
        [OneTimeSetUp]
        public void initialize_simulation_input()
        {
            var ti = new SingleEllipsoidTissueInput(
                new EllipsoidTissueRegion(
                    new Position(0,0,1),
                    0.5,
                    0.5,
                    0.5,
                    new OpticalProperties(0.05, 1.0, 0.8, 1.4),
                    "HenyeyGreensteinKey5"),
                // create input with two tissue layers
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 1.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(1.0, 20.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"),
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                        "HenyeyGreensteinKey4")
                });

            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey5", new HenyeyGreensteinPhaseFunctionInput());
            // create input with two tissue layers
            _input = new SimulationInput(
                10,
                "Output",
                new SimulationOptions(),
                new DirectionalPointSourceInput(),
                ti,
                new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput(), 
                }
            );
        }
        /// <summary>
        /// test command line parameter nphot
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_nphot_correctly()
        {
            var inputWithChange = _input.WithValue("nphot", 10000);
            Assert.AreEqual(10000,inputWithChange.N);
        }
        /// <summary>
        /// test command line parameter mua1
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_mua1_correctly()
        { 
            var inputWithChange = _input.WithValue(InputParameterType.Mua1.ToString(), 99.0);
            Assert.AreEqual(99.0,inputWithChange.TissueInput.Regions[1].RegionOP.Mua);
        }
        /// <summary>
        /// test command line parameter mus1
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_mus1_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.Mus1.ToString(), 99.0);
            Assert.AreEqual(99.0,inputWithChange.TissueInput.Regions[1].RegionOP.Mus);
        }
        /// <summary>
        /// test command line parameter n1
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_n1_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.N1.ToString(), 99.0);
            Assert.AreEqual(99.0,inputWithChange.TissueInput.Regions[1].RegionOP.N);
        }
        /// <summary>
        /// test command line parameter g1
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_g1_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.G1.ToString(), 99.0);
            Assert.AreEqual(99.0,inputWithChange.TissueInput.Regions[1].RegionOP.G);
        }
        /// <summary>
        /// test command line parameter mua2
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_mua2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.Mua2.ToString(), 99.0);
            Assert.AreEqual(99.0,inputWithChange.TissueInput.Regions[2].RegionOP.Mua);
        }
        /// <summary>
        /// test command line parameter mus2
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_mus2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.Mus2.ToString(), 99.0);
            Assert.AreEqual(99.0, inputWithChange.TissueInput.Regions[2].RegionOP.Mus);
        }
        /// <summary>
        /// test command line parameter n2
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_n2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.N2.ToString(), 99.0);
            Assert.AreEqual(99.0,inputWithChange.TissueInput.Regions[2].RegionOP.N);
        }
        /// <summary>
        /// test command line parameter g2
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_g2_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.G2.ToString(), 99.0);
            Assert.AreEqual(99.0,inputWithChange.TissueInput.Regions[2].RegionOP.G);
        }
        /// <summary>
        /// test command line parameter d1
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_d1_correctly()
        {
            // if change top layer to 99, second layer should be same thickness but start at 99
            var originalRegions = _input.TissueInput.Regions;

            var inputWithChange = _input.WithValue(InputParameterType.D1.ToString(), 99.0);

            var regionsWithChange = inputWithChange.TissueInput.Regions;

            Assert.AreEqual(0.0,((LayerTissueRegion)regionsWithChange[1]).ZRange.Start);
            Assert.AreEqual(99.0,((LayerTissueRegion)regionsWithChange[1]).ZRange.Stop);
            Assert.AreEqual(99.0, ((LayerTissueRegion)regionsWithChange[2]).ZRange.Start);
            Assert.AreEqual(((LayerTissueRegion)originalRegions[2]).ZRange.Delta,((LayerTissueRegion)regionsWithChange[2]).ZRange.Delta);
        }

        /// <summary>
        /// test command line parameter xsourceposition, ysourceposition
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_source_xy_correctly()
        {
            var inputWithChange = _input.WithValue(InputParameterType.XSourcePosition.ToString(), 1.0);
            Assert.AreEqual(1.0, ((DirectionalPointSourceInput)inputWithChange.SourceInput).PointLocation.X);
            inputWithChange = _input.WithValue(InputParameterType.YSourcePosition.ToString(), 2.0);
            Assert.AreEqual(2.0, ((DirectionalPointSourceInput)inputWithChange.SourceInput).PointLocation.Y);
        }
        /// <summary>
        /// test command line parameter x,y,z inclusionposition
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_inclusion_xyz_position_correctly()
        {
            // original position at (0, 0, 1)
            var inputWithChange = _input.WithValue(InputParameterType.XInclusionPosition.ToString(), 1.0);
            Assert.AreEqual(1.0, ((SingleEllipsoidTissueInput)inputWithChange.TissueInput).EllipsoidRegion.Center.X);
            inputWithChange = _input.WithValue(InputParameterType.YInclusionPosition.ToString(), 2.0);
            Assert.AreEqual(2.0, ((SingleEllipsoidTissueInput)inputWithChange.TissueInput).EllipsoidRegion.Center.Y);
            inputWithChange = _input.WithValue(InputParameterType.ZInclusionPosition.ToString(), 3.0);
            Assert.AreEqual(3.0, ((SingleEllipsoidTissueInput)inputWithChange.TissueInput).EllipsoidRegion.Center.Z);
        }
        /// <summary>
        /// test command line parameter x,y,z inclusionradius
        /// </summary>
        [Test]
        public void Verify_WithValue_method_modifies_inclusion_xyz_radius_correctly()
        {
            // original radii (0.5, 0.5, 0.5)
            var inputWithChange = _input.WithValue(InputParameterType.XInclusionRadius.ToString(), 1.0);
            Assert.AreEqual(1.0, ((EllipsoidTissueRegion)((SingleEllipsoidTissueInput) inputWithChange.TissueInput).EllipsoidRegion).Dx);
            inputWithChange = _input.WithValue(InputParameterType.YInclusionRadius.ToString(), 2.0);
            Assert.AreEqual(2.0, ((EllipsoidTissueRegion)((SingleEllipsoidTissueInput)inputWithChange.TissueInput).EllipsoidRegion).Dy);
            inputWithChange = _input.WithValue(InputParameterType.ZInclusionRadius.ToString(), 3.0);
            Assert.AreEqual(3.0, ((EllipsoidTissueRegion)((SingleEllipsoidTissueInput)inputWithChange.TissueInput).EllipsoidRegion).Dz);
        }
    }
}

