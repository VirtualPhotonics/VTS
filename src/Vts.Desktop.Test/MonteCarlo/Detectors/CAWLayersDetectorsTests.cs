using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests execute a continuous absorption weighting (CAW)
    /// MC simulation with 100 photons and verify
    /// that the tally results match the linux results given the same seed
    /// mersenne twister STANDARD_TEST.  The linux results assumes photon passes
    /// through specular and deweights photon by specular.  This test starts photon 
    /// inside tissue and then multiplies result by specular deweighting to match 
    /// linux results.
    /// </summary>
    [TestFixture]
    public class CAWLayersDetectorsTests
    {
        private SimulationOutput _outputOneLayerTissue;
        private SimulationOutput _outputTwoLayerTissue;
        private double _layerThickness = 2.0;
        private double _factor;

        /// <summary>
        /// ContinuousAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and a homogeneous
        /// two layer tissue (both layers have same optical properties), execute simulations
        /// and verify results agree with linux results given the same seed
        /// mersenne twister STANDARD_TEST.  The linux results assumes photon passes
        /// through specular and deweights photon by specular.  This test starts photon 
        /// inside tissue and then multiplies result by specular deweighting to match.
        /// NOTE: currently two region executes same photon biography except for pauses
        /// at layer interface, BUT CAW results have greater variance.  Why? CKH to look into.
        /// </summary>
        [TestFixtureSetUp]
        public void execute_Monte_Carlo()
        {
            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                    0, // rng seed = same as linux (0)
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                    PhaseFunctionType.HenyeyGreenstein,
                    new List<DatabaseType>() { }, // databases to be written
                    false, // track statistics
                    0.0, // RR threshold -> 0 = no RR performed
                    0);
             var source = new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1); // start inside tissue
            var detectors = 
                new List<IDetectorInput>
                {
                    //new ATotalDetectorInput() ckh 11/6/11 comment out for now with new Abs.Wt.Method rule
                    // CAW not coded for volume tallies yet
                    new RDiffuseDetectorInput() {TallySecondMoment = true},                    
                    new ROfAngleDetectorInput() {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2)},
                    new ROfRhoAndAngleDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2)},
                    new ROfRhoDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 101), TallySecondMoment = true },
                    new ROfRhoAndTimeDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), Time = new DoubleRange(0.0, 1.0, 101)},                   
                    new ROfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 101), Y = new DoubleRange(-10.0, 10.0, 101) },
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 101), Omega = new DoubleRange(0.05, 1.0, 20)}, // DJC - edited to reflect frequency sampling points (not bins)
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new TOfRhoDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101)},
                    new TOfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new ReflectedTimeOfRhoAndSubregionHistDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Time=new DoubleRange(0.0, 1.0, 101)},
                };
            // one tissue layer
            var inputOneLayerTissue = new SimulationInput(
                 100,
                 "Output",
                 simulationOptions,
                 source,
                 new MultiLayerTissueInput(
                     new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                 ),
                 detectors);              
           _outputOneLayerTissue = new MonteCarloSimulation(inputOneLayerTissue).Run();
            // two tissue layers with same optical properties
           var inputTwoLayerTissue = new SimulationInput(
                    100,
                    "Output",
                    simulationOptions,
                    source,
                    new MultiLayerTissueInput(
                        new ITissueRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, _layerThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(_layerThickness, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectors);

            _outputTwoLayerTissue = new MonteCarloSimulation(inputTwoLayerTissue).Run();

           _factor = 1.0 - Optics.Specular(
                           inputOneLayerTissue.TissueInput.Regions[0].RegionOP.N,
                           inputOneLayerTissue.TissueInput.Regions[1].RegionOP.N);
        }

        // validation values obtained from linux run using above input and seeded the same
        // Diffuse Reflectance
        [Test]
        public void validate_CAW_RDiffuse()
        {
            var sdOneLayerTissue = ErrorCalculation.StandardDeviation(
                _outputOneLayerTissue.Input.N, _outputOneLayerTissue.Rd, _outputOneLayerTissue.Rd2);
            var sdTwoLayerTissue = ErrorCalculation.StandardDeviation(
                _outputTwoLayerTissue.Input.N, _outputTwoLayerTissue.Rd, _outputTwoLayerTissue.Rd2);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rd * _factor - 0.572710099), 0.000000001);
            // figure out best check of two below 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rd * _factor - 0.572710099), 1 * sdOneLayerTissue);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rd * _factor - 0.572710099), 0.000000001);
        }
        // Reflection R(rho)
        [Test]
        public void validate_CAW_ROfRho()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r[0] * _factor - 0.922411018), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_r[0] * _factor - 0.922411018), 0.00001);
        }
        // Reflection R(rho) 2nd moment, linux value output in printf statement
        [Test]
        public void validate_CAW_ROfRho_second_moment()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r2[0] * _factor * _factor - 28.36225), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_r2[0] * _factor * _factor - 28.36225), 0.00001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_CAW_ROfAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_a[0] * _factor - 0.0820635109), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_a[0] * _factor - 0.0820635109), 0.0005);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_CAW_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_ra[0, 0] * _factor - 0.132172083), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_ra[0, 0] * _factor - 0.132172083), 0.0000000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_CAW_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rt[0, 0] * _factor - 92.2411018), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rt[0, 0] * _factor - 92.2411018), 0.0000001);
        }
        // Reflection R(rho,omega)
        [Test]
        public void validate_CAW_ROfRhoAndOmega()
        {
            // todo: warning - this validation data from Linux is actually for Omega = 0.025GHz
            // (see here: http://virtualphotonics.codeplex.com/discussions/278250)
            Assert.Less(Complex.Abs(
                 _outputOneLayerTissue.R_rw[0, 0] * _factor - (0.9224103 - Complex.ImaginaryOne * 0.0008737114)), 0.000001);
            Assert.Less(Complex.Abs(
                    _outputTwoLayerTissue.R_rw[0, 0] * _factor - (0.9224103 - Complex.ImaginaryOne * 0.0008737114)), 0.000001);
        }
        // Total Absorption : wait on this test until CAW worked out for ATotal
        //[Test]
        //public void validate_CAW_ATotal()
        //{
        //    Assert.Less(Math.Abs(_outputOneLayerTissue.Atot * _factor - 0.37402175), 0.002);
        //    Assert.Less(Math.Abs(_outputTwoLayerTissue.Atot * _factor - 0.37402175), 0.002);
        //}
        // Absorption A(rho,z) not coded yet for CAW

        // Diffuse Transmittance
        [Test]
        public void validate_CAW_TDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Td * _factor - 0.0232993770), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Td * _factor - 0.0232993770), 0.000000001);
        }
        // Transmittance T(rho)
        [Test]
        public void validate_CAW_TOfRho()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r[54] * _factor - 0.00167241353), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_r[54] * _factor - 0.00167241353), 0.00000000001);
        }
        // Transmittance T(angle)
        [Test]
        public void validate_CAW_TOfAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_a[0] * _factor - 0.00333856288), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_a[0] * _factor - 0.00333856288), 0.00000000001);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void validate_CAW_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_ra[54, 0] * _factor - 0.000239639787), 0.000000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_ra[54, 0] * _factor - 0.000239639787), 0.000000000001);
        }
        //// Fluence Flu(rho,z) not coded yet for CAW

        // Reflectance R(x,y)
        [Test]
        public void validate_CAW_ROfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy[0, 14] * _factor - 0.00060744), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xy[0, 14] * _factor - 0.00060744), 0.00000001);
        }

        // sanity checks
        //[Test] // wait on this until CAW volume tallies worked out
        //public void validate_CAW_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        //{
        //    // no specular because photons started inside tissue
        //    Assert.Less(Math.Abs(_outputOneLayerTissue.Rd + _outputOneLayerTissue.Atot + _outputOneLayerTissue.Td - 1), 0.00000000001);
        //}

        // ReflectedTimeOfRhoAndSubregionHist : this is validated using initial run results since no supporting linux code 
        [Test]
        public void validate_CAW_ReflectedTimeOfRhoAndSubregionHist()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefTime_rs_hist[0, 1, 0] - 0.9487656), 0.0000001);
        }
    }
}
