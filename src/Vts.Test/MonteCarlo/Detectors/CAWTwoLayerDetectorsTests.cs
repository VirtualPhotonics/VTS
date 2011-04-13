using System;
using System.Numerics;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests execute a continuous absorption weighting (CAW) in 2 layers
    /// each layer having the same optical properties in a
    /// MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST
    /// NOTE that these results DO NOT match 1 layer results because the crossing
    /// at the internal interface adds random number calls
    /// </summary>
    [TestFixture]
    public class CAWTwoLayerDetectorsTests
    {
        private Output _output;

        /// <summary>
        /// Setup input to the MC, SimulationInput, and execute MC
        /// </summary>
        [TestFixtureSetUp]
        public void execute_Monte_Carlo()
        {
           var input = new SimulationInput(
                100,
                "Output",
                new SimulationOptions(
                    0, 
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous, 
                    PhaseFunctionType.HenyeyGreenstein,
                    null, 
                    0),
                new CustomPointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)
                ),
                new MultiLayerTissueInput(
                    new List<ITissueRegion>
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
                new List<IDetectorInput>()
                {
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    new ROfRhoAndAngleDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoAndTimeDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 1, 101)),
                    new ROfXAndYDetectorInput(
                        new DoubleRange(-200.0, 200.0, 401), // x
                        new DoubleRange(-200.0, 200.0, 401)), // y,
                    new ROfRhoAndOmegaDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 1000, 21)),
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new TOfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    new TOfRhoAndAngleDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, Math.PI / 2, 2))
                });
            _output = new MonteCarloSimulation(input).Run();
        }

        // validation values obtained from linux run using above input and 
        // seeded the same for:
        // Diffuse Reflectance
        [Test]
        public void validate_CAW_two_layer_RDiffuse()
        {
            Assert.Less(Math.Abs(_output.Rd - 0.599832000), 0.000000001);
        }
        // Diffuse Reflectance
        //[Test]
        //public void validate_CAW_two_layer_RTotal()
        //{
        //    Assert.Less(Math.Abs(_output.Rtot - 0.627609778), 0.000000001);
        //}
        // Reflection R(rho)
        [Test]
        public void validate_CAW_two_layer_ROfRho()
        {
            Assert.Less(Math.Abs(_output.R_r[3] - 0.0160632923), 0.000000001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_CAW_two_layer_ROfAngle()
        {
            Assert.Less(Math.Abs(_output.R_a[0] - 0.085949802), 0.000000001);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_CAW_two_layer_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.R_ra[3, 0] - 0.00230170582), 0.0000000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_CAW_two_layer_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_output.R_rt[4, 0] - 10.1840199), 0.000001);
        }
        // Reflection R(rho,omega)
        //[Test]
        //public void validate_CAW_two_layer_ROfRhoAndOmega()
        //{
        //   Assert.Less(Complex.Abs(
        //        _output.R_rw[0, 0] - (0.9224103 - Complex.ImaginaryOne * 0.0008737114)), 0.000001);
        //}
        // Total Absorption not coded yet for CAW
        // Absorption A(rho,z) not coded yet for CAW

        // Diffuse Transmittance
        [Test]
        public void validate_CAW_two_layer_TDiffuse()
        {
            Assert.Less(Math.Abs(_output.Td - 0.0158211135), 0.000000001);
        }
        // Transmittance T(rho)
        [Test]
        public void validate_CAW_two_layer_TOfRho()
        {
            Assert.Less(Math.Abs(_output.T_r[50] - 0.000825294502), 0.000000000001);
        }
        // Transmittance T(angle)
        [Test]
        public void validate_CAW_two_layer_TOfAngle()
        {
            Assert.Less(Math.Abs(_output.T_a[0] - 0.00226700407), 0.00000000001);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void validate_CAW_two_layer_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.T_ra[50,0] - 0.00011825627), 0.00000000001);
        }
        // Fluence Flu(rho,z) not coded yet for CAW

        // Reflectance R(x,y)
        [Test]
        public void validate_CAW_two_layer_ROfXAndY()
        {
            Assert.Less(Math.Abs(_output.R_xy[198, 201] - 0.00870566), 0.00000001);
        }
    }
}
