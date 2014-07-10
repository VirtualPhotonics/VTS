using System;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;

namespace Vts.Test.MonteCarlo.Detectors
{
    /// <summary>
    /// These tests execute a discrete absorption weighting (DAW) MC simulation with 
    /// 100 photons and verify that the tally results match the linux results given the 
    /// same seed using mersenne twister STANDARD_TEST.  The tests then run a simulation
    /// through a homogeneous two layer tissue (both layers have the same optical properties)
    /// and verify that the detector tallies are the same.  This tests whether the pseudo-
    /// collision pausing at the layer interface does not change the results.
    /// </summary>
    [TestFixture]
    public class DAWSphereDetectorsTests
    {
        private SimulationOutput _outputOneRegionTissue;
        private SimulationOutput _outputTwoRegionTissue;
        private SimulationInput _inputOneRegionTissue;
        private SimulationInput _inputTwoRegionTissue;
        private double _factor;

        /// <summary>
        /// DiscreteAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and a single
        /// ellipsoid tissue (both regions have same optical properties), execute simulations
        /// and verify results agree with linux results given the same seed
        /// mersenne twister STANDARD_TEST.  The linux results assumes photon passes
        /// through specular and deweights photon by specular.  This test starts photon 
        /// inside tissue and then multiplies result by specular deweighting to match.
        /// NOTE: currently two region executes same photon biography except for pauses
        /// at layer interface.  Variance for DAW results not degraded.
        /// </summary>
        [TestFixtureSetUp]
        public void execute_Monte_Carlo()
        {
            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
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
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput() { Angle=new DoubleRange(Math.PI / 2 , Math.PI, 2)},
                    new ROfRhoDetectorInput() { Rho=new DoubleRange(0.0, 10.0, 101), TallySecondMoment = true},
                    new ROfRhoAndAngleDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Angle=new DoubleRange(Math.PI / 2, Math.PI, 2)},
                    new ROfRhoAndTimeDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Time=new DoubleRange(0.0, 1.0, 101)},
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-200.0, 200.0, 401),Y=new DoubleRange(-200.0, 200.0, 401)}, 
                    new ROfRhoAndOmegaDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Omega=new DoubleRange(0.0, 1.0, 21)},
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101)},
                    new TOfRhoAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10.0, 101), Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new AOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Z=new DoubleRange(0.0, 10.0, 101)},
                    new ATotalDetectorInput(),
                    new FluenceOfRhoAndZDetectorInput(){Rho=new DoubleRange(0.0, 10.0, 101),Z=new DoubleRange(0.0, 10.0, 101)},
                    new RadianceOfRhoAndZAndAngleDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101),
                        Z=new DoubleRange(0.0, 10.0, 101),
                        Angle=new DoubleRange(-Math.PI / 2, Math.PI / 2, 5)
                    }
                };
            _inputOneRegionTissue = new SimulationInput(
                100,
                "",
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
            _outputOneRegionTissue = new MonteCarloSimulation(_inputOneRegionTissue).Run();

            _inputTwoRegionTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                new SingleEllipsoidTissueInput(
                     new EllipsoidRegion(
                        new Position(0, 0, 1),
                        0.5,
                        0.5,
                        0.5,
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4) //debug with g=1
                    ), 
                    new LayerRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 20.0), // debug with thin slab d=2
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),// debug with g=1
                        new LayerRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectors);
            _outputTwoRegionTissue = new MonteCarloSimulation(_inputTwoRegionTissue).Run();

            _factor = 1.0 - Optics.Specular(
                            _inputOneRegionTissue.TissueInput.Regions[0].RegionOP.N,
                            _inputOneRegionTissue.TissueInput.Regions[1].RegionOP.N);
        }

        // validation values obtained from linux run using above input and 
        // seeded the same for:
        // Diffuse Reflectance
        [Test]
        public void validate_DAW_sphere_RDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Rd * _factor - 0.565017749), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.Rd * _factor - 0.565017749), 0.000000001);
        }
        // Reflection R(rho)
        [Test]
        public void validate_DAW_sphere_ROfRho()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_r[0] * _factor - 0.615238307), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.R_r[0] * _factor - 0.615238307), 0.000000001);
        }
        // Reflection R(rho) 2nd moment, linux value output in printf statement
        [Test]
        public void validate_DAW_sphere_ROfRho_second_moment()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_r2[0] * _factor * _factor - 18.92598), 0.00001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.R_r2[0] * _factor * _factor - 18.92598), 0.00001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_DAW_sphere_ROfAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_DAW_sphere_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_ra[0, 0] * _factor - 0.0881573691), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.R_ra[0, 0] * _factor - 0.0881573691), 0.0000000001);
        }
        // Reflection R(rho,time)
        [Test]
        public void validate_DAW_sphere_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
        }
        // Reflection R(rho,omega)
        public void validate_DAW_sphere_ROfRhoAndOmega()
        {
            Assert.Less(Complex.Abs(
                _outputOneRegionTissue.R_rw[0, 0] * _factor - (0.6152383 - Complex.ImaginaryOne * 0.0002368336)), 0.000001);
            Assert.Less(Complex.Abs(
                _outputTwoRegionTissue.R_rw[0, 0] * _factor - (0.6152383 - Complex.ImaginaryOne * 0.0002368336)), 0.000001);
        }
        // Diffuse Transmittance
        [Test]
        public void validate_DAW_sphere_TDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Td * _factor - 0.0228405921), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.Td * _factor - 0.0228405921), 0.000000001);
        }
        // Transmittance Time(rho)
        [Test]
        public void validate_DAW_sphere_TOfRho()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.T_r[54] * _factor - 0.00169219067), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.T_r[54] * _factor - 0.00169219067), 0.00000000001);
        }
        // Transmittance Time(angle)
        [Test]
        public void validate_DAW_sphere_TOfAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
        }
        // Transmittance Time(rho,angle)
        [Test]
        public void validate_DAW_sphere_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.T_ra[54, 0] * _factor - 0.000242473649), 0.000000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.T_ra[54, 0] * _factor - 0.000242473649), 0.000000000001);
        }
        // Reflectance R(x,y)
        [Test]
        public void validate_DAW_sphere_ROfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_xy[198, 201] * _factor - 0.00825301), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.R_xy[198, 201] * _factor - 0.00825301), 0.00000001);
        }
        // Total Absorption
        [Test]
        public void validate_DAW_sphere_ATotal()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Atot * _factor - 0.384363881), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.Atot * _factor - 0.384363881), 0.000000001);
        }
        // Absorption A(rho,z)
        [Test]
        public void validate_DAW_sphere_AOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.A_rz[0, 0] * _factor - 0.39494647), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.A_rz[0, 0] * _factor - 0.39494647), 0.00000001);
        }
        // Fluence Flu(rho,z)
        [Test]
        public void validate_DAW_sphere_FluenceOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Flu_rz[0, 0] * _factor - 39.4946472), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoRegionTissue.Flu_rz[0, 0] * _factor - 39.4946472), 0.0000001);
        } 
        // Volume Radiance Rad(rho,z,angle)
        // Verify integral over angle of Radiance equals Fluence
        [Test]
        public void validate_DAW_sphere_RadianceOfRhoAndZAndAngle()
        {
            // undo angle bin normalization
            var angle = ((RadianceOfRhoAndZAndAngleDetectorInput)_inputOneRegionTissue.DetectorInputs.
                Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First()).Angle;
            var norm = 2 * Math.PI * angle.Delta;
            var integral = 0.0;
            for (int ia = 0; ia < angle.Count - 1; ia++)
            {
                integral += _outputOneRegionTissue.Rad_rza[0, 6, ia] * Math.Sin((ia + 0.5) * angle.Delta);
            }
            Assert.Less(Math.Abs(integral * norm - _outputOneRegionTissue.Flu_rz[0, 6]), 0.000000000001);
        }
        // Radiance(rho) - not sure this detector is defined correctly yet
        //[Test]
        //public void validate_DAW_sphere_RadianceOfRho()
        //{
        //    //need radiance detector to compare results, for now make sure both simulations give same results
        //    Assert.Less(Math.Abs(_outputOneRegionTissue.Rad_r[0] - _outputTwoRegionTissue.Rad_r[0]), 0.0000001);
        //}
        // sanity checks
        [Test]
        public void validate_DAW_sphere_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // no specular because photons started inside tissue
            Assert.Less(Math.Abs(_outputOneRegionTissue.Rd + _outputOneRegionTissue.Atot + _outputOneRegionTissue.Td - 1), 0.00000000001);
        }
    }
}
