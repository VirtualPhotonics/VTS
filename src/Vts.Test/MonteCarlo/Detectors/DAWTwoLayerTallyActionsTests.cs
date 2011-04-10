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
    /// These tests execute a discrete absorption weighting (DAW) in 2layer
    /// MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST
    /// NOTE that these results DO NOT match 1 layer results because the crossing
    /// at the internal interface adds random number calls
    /// </summary>
    [TestFixture]
    public class DAWTwoLayerTallyActionsTests
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
                    AbsorptionWeightingType.Discrete, 
                    PhaseFunctionType.HenyeyGreenstein,
                    DatabaseType.NoDatabaseGeneration, 
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
                    new AOfRhoAndZDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 10, 101)),
                    new ATotalDetectorInput(),
                    new FluenceOfRhoAndZDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 10, 101)),
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
        public void validate_DAW_two_layer_RDiffuse()
        {
            Assert.Less(Math.Abs(_output.Rd - 0.570818117), 0.000000001);
        }
        // Diffuse Reflectance
        //[Test]
        //public void validate_DAW_two_layer_RTotal()
        //{
        //    Assert.Less(Math.Abs(_output.Rtot - 0.598595895), 0.000000001);
        //}
        // Reflection R(rho)
        [Test]
        public void validate_DAW_two_layer_ROfRho()
        {
            Assert.Less(Math.Abs(_output.R_r[1] - 0.102334844), 0.000000001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_DAW_two_layer_ROfAngle()
        {
            Assert.Less(Math.Abs(_output.R_a[0] - 0.0817924093), 0.0000000001);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_DAW_two_layer_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.R_ra[1, 0] - 0.0146635384), 0.0000000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_DAW_two_layer_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_output.R_rt[1, 0] - 10.2334844), 0.0000001);
        }
        // Reflection R(rho,omega)
        //public void validate_DAW_two_layer_ROfRhoAndOmega()
        //{
        //    Assert.Less(Complex.Abs(
        //        _output.R_rw[0, 0] - (0.6152383 - Complex.ImaginaryOne * 0.0002368336)), 0.000001);
        //}
        // Total Absorption
        [Test]
        public void validate_DAW_two_layer_ATotal()
        {
            Assert.Less(Math.Abs(_output.Atot - 0.379223950), 0.000000001);
        }
        // Absorption A(rho,z)
        [Test]
        public void validate_DAW_two_layer_AOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_output.A_rz[0, 0] - 0.308653049), 0.00000001);
        }
        // Diffuse Transmittance
        [Test]
        public void validate_DAW_two_layer_TDiffuse()
        {
            Assert.Less(Math.Abs(_output.Td - 0.0221801550), 0.000000001);
        }
        // Transmittance T(rho)
        [Test]
        public void validate_DAW_two_layer_TOfRho()
        {
            Assert.Less(Math.Abs(_output.T_r[54] - 0.000153880454), 0.00000000001);
        }
        // Transmittance T(angle)
        [Test]
        public void validate_DAW_two_layer_TOfAngle()
        {
            Assert.Less(Math.Abs(_output.T_a[0] - 0.00317818980), 0.00000000001);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void validate_DAW_two_layer_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.T_ra[54,0] - 0.0000220494982), 0.000000000001);
        }
        // Fluence Flu(rho,z)
        [Test]
        public void validate_DAW_two_layer_FluenceOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_output.Flu_rz[0, 0] - 30.8653049), 0.0000001);
        }
        // Reflectance R(x,y)
        [Test]
        public void validate_DAW_two_layer_ROfXAndY()
        {
            Assert.Less(Math.Abs(_output.R_xy[198, 201] - 0.00932274), 0.00000001);
        }
    }
}
