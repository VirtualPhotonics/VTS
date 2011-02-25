using System;
using System.Numerics;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.TallyActions
{
    /// <summary>
    /// These tests execute a continuous absorption weighting (CAW)
    /// MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST
    /// </summary>
    [TestFixture]
    public class CAWTallyActionsTests
    {
        Output _output;

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
                    false, 
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
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0))
                    },
                    AbsorptionWeightingType.Continuous
                ),
                new DetectorInput(
                    new List<TallyType>()
                        {
                            TallyType.RDiffuse,
                            TallyType.ROfAngle,
                            TallyType.ROfRho,
                            TallyType.ROfRhoAndAngle,
                            TallyType.ROfRhoAndTime,
                            TallyType.ROfXAndY,
                            TallyType.ROfRhoAndOmega,
                            TallyType.TDiffuse,
                            TallyType.TOfAngle,
                            TallyType.TOfRho,
                            TallyType.TOfRhoAndAngle
                        },
                    new DoubleRange(0.0, 10, 101), // rho
                    new DoubleRange(0.0, 10, 101),  // z
                    new DoubleRange(0.0, Math.PI / 2, 2), // angle
                    new DoubleRange(0.0, 1, 101), // time
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-200.0, 200.0, 401), // x
                    new DoubleRange(-200.0, 200.0, 401) // y
                ) );
            _output = new MonteCarloSimulation(input).Run();
        }

        // validation values obtained from linux run using above input and 
        // seeded the same for:
        // Diffuse Reflectance
        [Test]
        public void validate_CAW_RDiffuse()
        {
            Assert.Less(Math.Abs(_output.Rd - 0.573738839), 0.000000001);
        }
        // Diffuse Reflectance
        [Test]
        public void validate_CAW_RTotal()
        {
            Assert.Less(Math.Abs(_output.Rtot - 0.601516617), 0.000000001);
        }
        // Reflection R(rho)
        [Test]
        public void validate_CAW_ROfRho()
        {
            Assert.Less(Math.Abs(_output.R_r[0] - 0.922411018), 0.000000001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_CAW_ROfAngle()
        {
            Assert.Less(Math.Abs(_output.R_a[0] - 0.0822109189), 0.0000000001);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_CAW_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.R_ra[0, 0] - 0.132172083), 0.0000000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_CAW_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_output.R_rt[0, 0] - 92.2411018), 0.0000001);
        }
        // Reflection R(rho,omega)
        [Test]
        public void validate_CAW_ROfRhoAndOmega()
        {
           Assert.Less(Complex.Abs(
                _output.R_rw[0, 0] - (0.9224103 - Complex.ImaginaryOne * 0.0008737114)), 0.000001);
        }
        // Total Absorption not coded yet for CAW
        // Absorption A(rho,z) not coded yet for CAW

        // Diffuse Transmittance
        [Test]
        public void validate_CAW_TDiffuse()
        {
            Assert.Less(Math.Abs(_output.Td - 0.0233366015), 0.000000001);
        }
        // Transmittance T(rho)
        [Test]
        public void validate_CAW_TOfRho()
        {
            Assert.Less(Math.Abs(_output.T_r[54] - 0.00167241353), 0.00000000001);
        }
        // Transmittance T(angle)
        [Test]
        public void validate_CAW_TOfAngle()
        {
            Assert.Less(Math.Abs(_output.T_a[0] - 0.00334389677), 0.00000000001);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void validate_CAW_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.T_ra[54,0] - 0.000239639787), 0.000000000001);
        }
        // Fluence Flu(rho,z) not coded yet for CAW

        // Reflectance R(x,y)
        [Test]
        public void validate_CAW_ROfXAndY()
        {
            Assert.Less(Math.Abs(_output.R_xy[198, 201] - 0.00827581), 0.00000001);
        }
    }
}
