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
    /// These tests execute an Analog MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST
    /// </summary>
    [TestFixture]
    public class AnalogTallyActionsTests
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
                    AbsorptionWeightingType.Analog, 
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
                    AbsorptionWeightingType.Analog
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
                            TallyType.TOfRhoAndAngle,
                            TallyType.FluenceOfRhoAndZ,
                            //TallyType.FluenceOfRhoAndZAndTime,
                            TallyType.AOfRhoAndZ,
                            TallyType.ATotal
                        },
                    new DoubleRange(0.0, 10, 101), // rho (mm)
                    new DoubleRange(0.0, 10, 101),  // z
                    new DoubleRange(0.0, Math.PI / 2, 2), // angle
                    new DoubleRange(0.0, 1, 101), // time (ns=1000ps)
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-200.0, 200.0, 401), // x
                    new DoubleRange(-200.0, 200.0, 401) // y
                ));
            _output = new MonteCarloSimulation(input).Run();
        }

        // validation values obtained from linux run using above input and seeded 
        // the same for:
        // Diffuse Reflectance
        [Test]
        public void validate_Analog_RDiffuse()
        {
            Assert.Less(Math.Abs(_output.Rd - 0.670833333), 0.000000001);
        }
        // Diffuse Reflectance
        [Test]
        public void validate_Analog_RTotal()
        {
            Assert.Less(Math.Abs(_output.Rtot - 0.698611111), 0.000000001);
        }
        // Reflection R(rho)
        [Test]
        public void validate_Analog_ROfRho()
        {
            Assert.Less(Math.Abs(_output.R_r[0] - 0.928403835), 0.000000001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_Analog_ROfAngle()
        {
            Assert.Less(Math.Abs(_output.R_a[0] - 0.0961235688), 0.0000000001);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_Analog_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.R_ra[0, 1] - 0.133030792), 0.000000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_Analog_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_output.R_rt[2, 1] - 6.18935890), 0.00000001);
        }
        // Reflection R(rho,omega)
        [Test]
        public void validate_Analog_ROfRhoAndOmega()
        {
            Assert.Less(Complex.Abs(
                _output.R_rw[0, 0] - (0.9284030 - Complex.ImaginaryOne * 0.0007940711)), 0.000001);
        }
        // Total Absorption
        [Test]
        public void validate_Analog_ATotal()
        {
            Assert.Less(Math.Abs(_output.Atot - 0.000562763362), 0.000000000001);
        }
        // Absorption A(rho,z)
        [Test]
        public void validate_Analog_AOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_output.A_rz[0, 6] - 0.00617700489), 0.00000000001);
        }
        // Diffuse Transmittance
        [Test]
        public void validate_Analog_TDiffuse()
        {
            Assert.Less(Math.Abs(_output.Td - 0.0194444444), 0.0000000001);
        }
        // Transmittance T(rho)
        [Test]
        public void validate_Analog_TOfRho()
        {
            Assert.Less(Math.Abs(_output.T_r[46] - 0.00332761231), 0.00000000001);
        }
        // Transmittance T(angle)
        [Test]
        public void validate_Analog_TOfAngle()
        {
            Assert.Less(Math.Abs(_output.T_a[0] - 0.00278619040), 0.00000000001);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void validate_Analog_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.T_ra[46, 0] - 0.000476812876), 0.000000000001);
        }
        // Fluence Flu(rho,z)
        [Test]
        public void validate_Analog_FluenceOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_output.Flu_rz[0, 6] - 0.617700489), 0.000000001);
        }
        //[Test]
        //public void validate_Analog_FluenceOfRhoAndZAndTime()
        //{
        //    Assert.Less(Math.Abs(_output.Flu_rzt[0, 6, 0] - 0.617700489), 0.000000001);
        //}
        // Reflectance R(x,y)
        [Test]
        public void validate_Analog_ROfXAndY()
        {
            Assert.Less(Math.Abs(_output.R_xy[198, 201] - 0.0097222222), 0.0000000001);
        }
    }
}
