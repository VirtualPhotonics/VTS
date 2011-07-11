using System;
using System.Numerics;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;

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
    public class CAWDetectorsTests
    {
        private Output _outputOneLayerTissue;
        private Output _outputTwoLayerTissue;
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
                    0,
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Continuous,
                    PhaseFunctionType.HenyeyGreenstein,
                    null, // databases to be written
                    true, // tally 2nd moment
                    false, // track statistics
                    0);
             var source = new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    1); // start inside tissue
            var detectors = new List<IDetectorInput>()
                {
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10.0, 101)),
                    new ROfRhoAndAngleDetectorInput(
                        new DoubleRange(0.0, 10.0, 101),
                        new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoAndTimeDetectorInput(
                        new DoubleRange(0.0, 10.0, 101),
                        new DoubleRange(0.0, 1.0, 101)),
                    new ROfXAndYDetectorInput(
                        new DoubleRange(-200.0, 200.0, 401), // x
                        new DoubleRange(-200.0, 200.0, 401)), // y,
                    new ROfRhoAndOmegaDetectorInput(
                        new DoubleRange(0.0, 10.0, 101),
                        new DoubleRange(0.0, 1000.0, 21)),     
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new TOfRhoDetectorInput(new DoubleRange(0.0, 10.0, 101)),
                    new TOfRhoAndAngleDetectorInput(
                        new DoubleRange(0.0, 10.0, 101),
                        new DoubleRange(0.0, Math.PI / 2, 2))
                };
            // one tissue layer
            var inputOneLayerTissue = new SimulationInput(
                 100,
                 "Output",
                 simulationOptions,
                 source,
                 new MultiLayerTissueInput(
                     new List<ITissueRegion>
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
                        new List<ITissueRegion>
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
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rd * _factor - 0.573738839), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rd * _factor - 0.573738839), 0.003);
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
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_a[0] * _factor - 0.0822109189), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_a[0] * _factor - 0.0822109189), 0.0005);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_CAW_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_ra[0, 0] * _factor - 0.132172083), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_ra[0, 0] * _factor - 0.132172083), 0.000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_CAW_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rt[0, 0] * _factor - 92.2411018), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rt[0, 0] * _factor - 92.2411018), 0.0001);
        }
        // Reflection R(rho,omega)
        [Test]
        public void validate_CAW_ROfRhoAndOmega()
        {
           Assert.Less(Complex.Abs(
                _outputOneLayerTissue.R_rw[0, 0] * _factor - (0.9224103 - Complex.ImaginaryOne * 0.0008737114)), 0.000001);
           Assert.Less(Complex.Abs(
                   _outputTwoLayerTissue.R_rw[0, 0] * _factor - (0.9224103 - Complex.ImaginaryOne * 0.0008737114)), 0.0001);
        }
        // Total Absorption not coded yet for CAW
        // Absorption A(rho,z) not coded yet for CAW

        // Diffuse Transmittance
        [Test]
        public void validate_CAW_TDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Td * _factor - 0.0233366015), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Td * _factor - 0.0233366015), 0.0002);
        }
        // Transmittance T(rho)
        [Test]
        public void validate_CAW_TOfRho()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r[54] * _factor - 0.00167241353), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_r[54] * _factor - 0.00167241353), 0.000003);
        }
        // Transmittance T(angle)
        [Test]
        public void validate_CAW_TOfAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_a[0] * _factor - 0.00334389677), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_a[0] * _factor - 0.00334389677), 0.00002);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void validate_CAW_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_ra[54, 0] * _factor - 0.000239639787), 0.000000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_ra[54, 0] * _factor - 0.000239639787), 0.0000003);
        }
        // Fluence Flu(rho,z) not coded yet for CAW

        // Reflectance R(x,y)
        [Test]
        public void validate_CAW_ROfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy[198, 201] * _factor - 0.00827581), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xy[198, 201] * _factor - 0.00827581), 0.00001);
        }
    }
}
