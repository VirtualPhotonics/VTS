using System;
using System.IO;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests execute an Analog MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST.  The linux results assumes photon passes
    /// through specular and deweights photon by specular.  This test starts photon 
    /// inside tissue and then multiplies result by specular deweighting to match 
    /// linux results.
    /// </summary>
    [TestFixture]
    public class AnalogOneLayerDetectorsTests
    {
        private SimulationOutput _output;
        private SimulationInput _input;
        private double _factor;
        private SimulationStatistics _simulationStatistics;

        /// <summary>
        /// Setup input to the MC, SimulationInput, and execute MC
        /// </summary>
        [TestFixtureSetUp]
        public void execute_Monte_Carlo()
        {
            // make sure statistic file generated from previous tests are deleted
            if (File.Exists("statistics.txt"))
            {
                File.Delete("statistics.txt");
            }
           _input = new SimulationInput(
                100,
                "Output",
                new SimulationOptions(
                    0, 
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Analog, 
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    true, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1 // start off inside tissue 
                ),
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput() {TallySecondMoment = true},
                    new ROfAngleDetectorInput() {Angle=new DoubleRange(Math.PI / 2 , Math.PI, 2)},
                    new ROfRhoDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101), TallySecondMoment = true},
                    new ROfRhoAndAngleDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101),Angle=new DoubleRange(Math.PI / 2 , Math.PI, 2)},                        
                    new ROfRhoAndTimeDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Time=new DoubleRange(0.0, 1.0, 101)},
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-10.0, 10.0, 101), Y= new DoubleRange(-10.0, 10.0, 101)},
                    new ROfRhoAndOmegaDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Omega=new DoubleRange(0.05, 1.0, 20)},  // frequency sampling points (not bins)
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() { Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101)},
                    new TOfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101), Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new AOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Z=new DoubleRange(0.0, 10.0, 101)},
                    new ATotalDetectorInput(),
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101), Z=new DoubleRange(0.0, 10.0, 101)},                    
                    new FluenceOfRhoAndZAndTimeDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), 
                        Z=new DoubleRange(0.0, 10.0, 101),
                        Time=new DoubleRange(0.0, 1.0, 11)
                    },
                    new RadianceOfRhoAndZAndAngleDetectorInput()
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), 
                        Z=new DoubleRange(0.0, 10.0, 101),
                        Angle=new DoubleRange(-Math.PI / 2, Math.PI / 2, 5)
                    }
                }
            );

            _output = new MonteCarloSimulation(_input).Run();

            _simulationStatistics = SimulationStatistics.FromFile(_input.OutputName + "/statistics.txt");

            _factor = 1.0 - Optics.Specular(
                            _input.TissueInput.Regions[0].RegionOP.N,
                            _input.TissueInput.Regions[1].RegionOP.N);
        }
   
        // validation values obtained from linux run using above input and seeded 
        // the same for:
        //// Diffuse Reflectance
        [Test]
        public void validate_Analog_RDiffuse()
        {
            Assert.Less(Math.Abs(_output.Rd * _factor - 0.670833333), 0.000000001);
            //var sd = ErrorCalculation.StandardDeviation(_output.Input.N, _output.Rd, _output.Rd2);
            // for analog 1st and 2nd moment should be equal (since weight tallied is 1)
            Assert.AreEqual(_output.Rd, _output.Rd2);
        }
        // Reflection R(rho)
        [Test]
        public void validate_Analog_ROfRho()
        {
            Assert.Less(Math.Abs(_output.R_r[0] * _factor - 0.928403835), 0.000000001);
        }
        // Reflection R(rho) 2nd moment, linux value output in printf statement
        [Test]
        public void validate_Analog_ROfRho_second_moment()
        {
            Assert.Less(Math.Abs(_output.R_r2[0] * _factor * _factor - 28.73112), 0.00001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_Analog_ROfAngle()
        {
            Assert.Less(Math.Abs(_output.R_a[0] * _factor - 0.0961235688), 0.0000000001);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_Analog_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.R_ra[0, 0] * _factor - 0.133030792), 0.000000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_Analog_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_output.R_rt[2, 1] * _factor - 6.18935890), 0.00000001);
        }
        // Reflection R(rho,omega)
        [Test]
        public void validate_Analog_ROfRhoAndOmega()
        {
            // todo: warning - this validation data from Linux is actually for Omega = 0.025GHz
            // (see here: http://virtualphotonics.codeplex.com/discussions/278250)
            Assert.Less(Complex.Abs(
                _output.R_rw[0, 0] * _factor - (0.9284030 - Complex.ImaginaryOne * 0.0007940711)), 0.000001);
        }
        // Diffuse Transmittance
        [Test]
        public void validate_Analog_TDiffuse()
        {
            Assert.Less(Math.Abs(_output.Td * _factor - 0.0194444444), 0.0000000001);
        }
        // Transmittance Time(rho)
        [Test]
        public void validate_Analog_TOfRho()
        {
            Assert.Less(Math.Abs(_output.T_r[46] * _factor - 0.00332761231), 0.00000000001);
        }
        // Transmittance Time(angle)
        [Test]
        public void validate_Analog_TOfAngle()
        {
            Assert.Less(Math.Abs(_output.T_a[0] * _factor - 0.00278619040), 0.00000000001);
        }
        // Transmittance Time(rho,angle)
        [Test]
        public void validate_Analog_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_output.T_ra[46, 0] * _factor - 0.000476812876), 0.000000000001);
        }
        // Total Absorption
        [Test]
        public void validate_Analog_ATotal()
        {
            Assert.Less(Math.Abs(_output.Atot * _factor - 0.281944444), 0.000000001);
        }
        // Absorption A(rho,z)
        [Test]
        public void validate_Analog_AOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_output.A_rz[0, 6] * _factor - 3.09467945), 0.00000001);
        }
        // Fluence Flu(rho,z)
        [Test]
        public void validate_Analog_FluenceOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_output.Flu_rz[0, 6] * _factor - 309.467945), 0.000001);
        }
        // Volume Radiance Rad(rho,z,angle)
        // Verify integral over angle of Radiance equals Fluence
        [Test]
        public void validate_Analog_RadianceOfRhoAndZAndAngle()
        {
            // undo angle bin normalization
            var angle = ((RadianceOfRhoAndZAndAngleDetectorInput)_input.DetectorInputs.
                Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First()).Angle;
            var norm = 2 * Math.PI * angle.Delta;
            var integral = 0.0;
            for (int ia = 0; ia < angle.Count - 1; ia++)
            {
                integral += _output.Rad_rza[0, 6, ia] * Math.Sin((ia + 0.5) * angle.Delta);
            }
            Assert.Less(Math.Abs(integral * norm - _output.Flu_rz[0, 6]), 0.000000000001);
        }
        // Fluence(rho,z,t)
        [Test]
        public void validate_Analog_FluenceOfRhoAndZAndTime()
        {
            Assert.Less(Math.Abs(_output.Flu_rzt[0, 6, 0]*_factor - 3094.67945), 0.00001);
        }

        // Reflectance R(x,y)
        [Test]
        public void validate_Analog_ROfXAndY()
        {
            Assert.Less(Math.Abs(_output.R_xy[0, 22] * _factor - 0.24305556), 0.00000001);
        }
        // sanity checks
        [Test]
        public void validate_Analog_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // no specular because photons started inside tissue
            Assert.Less(Math.Abs(_output.Rd + _output.Atot + _output.Td - 1), 0.00000000001);
        }
        // statistics
        [Test]
        public void validate_Analog_Statistics()
        {
            Assert.IsTrue(_simulationStatistics.NumberOfPhotonsOutTopOfTissue == 69);
            Assert.IsTrue(_simulationStatistics.NumberOfPhotonsOutBottomOfTissue == 2);
            Assert.IsTrue(_simulationStatistics.NumberOfPhotonsAbsorbed == 29);
        }
    }
}
