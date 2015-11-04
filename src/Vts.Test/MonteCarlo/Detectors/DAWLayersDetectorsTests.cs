using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
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
    public class DAWLayersDetectorsTests
    {
        private SimulationOutput _outputOneLayerTissue;
        private SimulationOutput _outputTwoLayerTissue;
        private SimulationInput _inputOneLayerTissue;
        private SimulationInput _inputTwoLayerTissue;
        private double _layerThickness = 1.0; // tissue is homogeneous (both layer opt. props same)
        private double _dosimetryDepth = 2.0;
        private double _factor;

        /// <summary>
        /// DiscreteAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and a homogeneous
        /// two layer tissue (both layers have same optical properties), execute simulations
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
            var detectors =  new List<IDetectorInput>
                {
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput() {Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2)},
                    new ROfRhoDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), TallySecondMoment = true},
                    new ROfRhoAndAngleDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), Angle = new DoubleRange(Math.PI / 2, Math.PI, 2)},
                    new ROfRhoAndTimeDetectorInput() {Rho = new DoubleRange(0.0, 10.0, 101), Time = new DoubleRange(0.0, 1.0, 101), TallySecondMoment = true },
                    new ROfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 101), Y = new DoubleRange(-10.0, 10.0, 101) },
                    new ROfRhoAndOmegaDetectorInput() { Rho = new DoubleRange(0.0, 10.0, 101), Omega = new DoubleRange(0.05, 1.0, 20)}, // DJC - edited to reflect frequency sampling points (not bins)
                    new ROfFxDetectorInput() {Fx = new DoubleRange(0.0, 0.5, 51)},
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new TOfRhoDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101)},
                    new TOfRhoAndAngleDetectorInput(){Rho=new DoubleRange(0.0, 10.0, 101), Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new TOfXAndYDetectorInput() { X = new DoubleRange(-10.0, 10.0, 101), Y = new DoubleRange(-10.0, 10.0, 101) },
                    new AOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Z=new DoubleRange(0.0, 10.0, 101)},
                    new AOfXAndYAndZDetectorInput()
                    {
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Z =  new DoubleRange(0.0, 10.0, 101),
                        TallySecondMoment = true
                    },
                    new ATotalDetectorInput() { TallySecondMoment = true },
                    new FluenceOfRhoAndZDetectorInput() {Rho=new DoubleRange(0.0, 10.0, 101),Z=new DoubleRange(0.0, 10.0, 101)},   
                    new FluenceOfXAndYAndZDetectorInput()
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        TallySecondMoment = true
                    },
                    new FluenceOfXAndYAndZAndOmegaDetectorInput()
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        Omega = new DoubleRange(0.05, 1.0, 20)
                    },
                    new RadianceOfRhoAtZDetectorInput() {ZDepth=_dosimetryDepth, Rho= new DoubleRange(0.0, 10.0, 101)},
                    new RadianceOfRhoAndZAndAngleDetectorInput(){Rho= new DoubleRange(0.0, 10.0, 101),Z=new DoubleRange(0.0, 10.0, 101),Angle=new DoubleRange(0, Math.PI, 5)},
                    new RadianceOfFxAndZAndAngleDetectorInput(){Fx=new DoubleRange(0.0, 0.5, 51),Z=new DoubleRange(0.0, 10, 101),Angle=new DoubleRange(0, Math.PI, 5)},
                    new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput()
                    {
                        X=new DoubleRange(-10.0, 10.0, 11), 
                        Y=new DoubleRange(-10.0, 10.0, 11),
                        Z=new DoubleRange(0.0, 10.0, 11),
                        Theta=new DoubleRange(0.0, Math.PI, 5), // theta (polar angle)
                        Phi=new DoubleRange(-Math.PI, Math.PI, 5), // phi (azimuthal angle)
                    },
                    new ReflectedMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins MAKE SURE AGREES with ROfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)   
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput() 
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins MAKE SURE AGREES with TOfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)   
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput() 
                    {
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput() 
                    {    
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11)   
                    }
                };
            _inputOneLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
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
                detectors);             
            _outputOneLayerTissue = new MonteCarloSimulation(_inputOneLayerTissue).Run();

            _inputTwoLayerTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                new MultiLayerTissueInput(
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, _layerThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(_layerThickness, 20.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectors);
            _outputTwoLayerTissue = new MonteCarloSimulation(_inputTwoLayerTissue).Run();

            _factor = 1.0 - Optics.Specular(
                            _inputOneLayerTissue.TissueInput.Regions[0].RegionOP.N,
                            _inputOneLayerTissue.TissueInput.Regions[1].RegionOP.N);
        }

        // validation values obtained from linux run using above input and 
        // seeded the same for:
        // Diffuse Reflectance
        [Test]
        public void validate_DAW_RDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rd * _factor - 0.565017749), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rd * _factor - 0.565017749), 0.000000001);
        }
        // Reflection R(rho)
        [Test]
        public void validate_DAW_ROfRho()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r[0] * _factor - 0.615238307), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_r[0] * _factor - 0.615238307), 0.000000001);
        }
        // Reflection R(rho) 2nd moment, linux value output in printf statement
        [Test]
        public void validate_DAW_ROfRho_second_moment()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r2[0] * _factor * _factor - 18.92598), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_r2[0] * _factor * _factor - 18.92598), 0.00001);
        }
        // Reflection R(angle)
        [Test]
        public void validate_DAW_ROfAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
        }
        // Reflection R(rho,angle)
        [Test]
        public void validate_DAW_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_ra[0, 0] * _factor - 0.0881573691), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_ra[0, 0] * _factor - 0.0881573691), 0.0000000001);
        }
        // Reflection R(rho,time), 2nd moment validated with prior test
        [Test]
        public void validate_DAW_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rt2[0, 0] - 200229.1), 0.1);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rt2[0, 0] - 200229.1), 0.1);

        }
        // Reflection R(rho,omega)
        [Test]
        public void validate_DAW_ROfRhoAndOmega()
        {
            // todo: warning - this validation data from Linux is actually for Omega = 0.025GHz
            // (see here: http://virtualphotonics.codeplex.com/discussions/278250)

            Assert.Less(Complex.Abs(
                _outputOneLayerTissue.R_rw[0, 0] * _factor - (0.6152383 - Complex.ImaginaryOne * 0.0002368336)), 0.000001);
            Assert.Less(Complex.Abs(
                _outputTwoLayerTissue.R_rw[0, 0] * _factor - (0.6152383 - Complex.ImaginaryOne * 0.0002368336)), 0.000001);
        }
        // Reflection R(fx) validated with prior test
        [Test]
        public void validate_DAW_ROfFx()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fx[1].Real - 0.557019), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fx[1].Imaginary - 0.050931), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fx[1].Real - 0.557019), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fx[1].Imaginary - 0.050931), 0.000001);
        }
        // Diffuse Transmittance
        [Test]
        public void validate_DAW_TDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Td * _factor - 0.0228405921), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Td * _factor - 0.0228405921), 0.000000001);
        }
        // Transmittance Time(rho)
        [Test]
        public void validate_DAW_TOfRho()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r[54] * _factor - 0.00169219067), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_r[54] * _factor - 0.00169219067), 0.00000000001);
        }
        // Transmittance T(angle)
        [Test]
        public void validate_DAW_TOfAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void validate_DAW_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_ra[54, 0] * _factor - 0.000242473649), 0.000000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_ra[54, 0] * _factor - 0.000242473649), 0.000000000001);
        }
        // Transmittance T(x,y): validation is not with linux code, but with prior execution of test so no "factor"
        [Test]
        public void validate_DAW_TOfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xy[0, 0] - 0.0067603), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xy[0, 0] - 0.0067603), 0.0000001);
        }
        //// Verify integral over rho,angle of T(rho,angle) equals TDiffuse
        [Test]
        public void validate_DAW_integral_of_TOfRhoAndAngle_equals_TDiffuse()
        {
            // undo angle bin normalization
            var angle = ((TOfRhoAndAngleDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "TOfRhoAndAngle").First()).Angle;
            var rho = ((TOfRhoAndAngleDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "TOfRhoAndAngle").First()).Rho;
            var norm = 2 * Math.PI * rho.Delta * 2 * Math.PI * angle.Delta;
            var integral = 0.0;
            for (int ir = 0; ir < rho.Count - 1; ir++)
            {
                for (int ia = 0; ia < angle.Count - 1; ia++)
                {
                    integral += _outputOneLayerTissue.T_ra[ir, ia] * (rho.Start + (ir + 0.5) * rho.Delta) * Math.Sin((ia + 0.5) * angle.Delta);
                }
            }
            Assert.Less(Math.Abs(integral * norm - _outputOneLayerTissue.Td), 0.000000000001);
        }
        // Reflectance R(x,y)
        [Test]
        public void validate_DAW_ROfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy[0, 0] * _factor - 0.01828126), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xy[0, 0] * _factor - 0.01828126), 0.00000001);
        }
        // Total Absorption, 2nd moment validated with prior test
        [Test]
        public void validate_DAW_ATotal()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Atot * _factor - 0.384363881), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Atot * _factor - 0.384363881), 0.000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Atot2 - 0.00052388), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Atot2 - 0.00052388), 0.00000001);
        }
        // Absorption A(rho,z)
        [Test]
        public void validate_DAW_AOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.A_rz[0, 0] * _factor - 0.39494647), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.A_rz[0, 0] * _factor - 0.39494647), 0.00000001);
        }
        // Absorption A(x,y,z) 2nd moment validation based on prior run
        [Test]
        public void validate_DAW_AOfXAndYAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.A_xyz[0, 0, 0] * _factor - 0.0003656252), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.A_xyz[0, 0, 0] * _factor - 0.0003656252), 0.000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.A_xyz2[0, 0, 0] - 0.00001414), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.A_xyz2[0, 0, 0] - 0.00001414), 0.00000001);
        }
        // Fluence Flu(rho,z)
        [Test]
        public void validate_DAW_FluenceOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rz[0, 0] * _factor - 39.4946472), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rz[0, 0] * _factor - 39.4946472), 0.0000001);
        }
        // Fluence Flu(x,y,z), 1st and 2nd moment validated with prior test
        [Test]
        public void validate_DAW_FluenceOfXAndYAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyz[0, 0, 0] - 0.0016990), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyz[0, 0, 0] - 0.0016990), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyz2[0, 0, 0] - 0.0001815), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyz2[0, 0, 0] - 0.0001815), 0.0000001);
        }
        // Fluence Flu(x,y,z,omega), 1st moment validated with prior test
        [Test]
        public void validate_DAW_FluenceOfXAndYAndZAndOmega()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzo[0, 0, 0, 10].Real + 0.0002956), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzo[0, 0, 0, 10].Imaginary + 0.0009234), 0.0000001);
        }
        // Volume Radiance Rad(rho,z,angle)
        // Verify integral over angle of Radiance equals Fluence
        [Test]
        public void validate_DAW_RadianceOfRhoAndZAndAngle()
        {
            // undo angle bin normalization
            var angle = ((RadianceOfRhoAndZAndAngleDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "RadianceOfRhoAndZAndAngle").First()).Angle;
            var norm = 2 * Math.PI * angle.Delta;
            var integral = 0.0;
            for (int ia = 0; ia < angle.Count - 1; ia++)
            {
                integral += _outputOneLayerTissue.Rad_rza[0, 6, ia] * Math.Sin((ia + 0.5) * angle.Delta);
            }
            Assert.Less(Math.Abs(integral * norm - _outputOneLayerTissue.Flu_rz[0, 6]), 0.000000000001);
        }

        // Volume Radiance Rad(x,y,z,theta,phi)
        // Verify integral over angle of Radiance equals Fluence
        [Test]
        public void validate_DAW_RadianceOfXAndYAndZAndThetaAndPhi()
        {
            // undo angle bin normalization
            var theta = ((RadianceOfXAndYAndZAndThetaAndPhiDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").First()).Theta;
            var phi = ((RadianceOfXAndYAndZAndThetaAndPhiDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi").First()).Phi;
            var norm = theta.Delta * phi.Delta;
            var integral = 0.0;
            for (int it = 0; it < theta.Count - 1; it++)
            {
                for (int ip = 0; ip < phi.Count - 1; ip++)
                    integral += _outputOneLayerTissue.Rad_xyztp[0, 0, 0, it, ip] * Math.Sin((it + 0.5) * theta.Delta);
            }
            Assert.Less(Math.Abs(integral * norm - _outputOneLayerTissue.Flu_xyz[0, 0, 0]), 0.000000000001);
        }
        // Radiance(rho) at depth Z - not sure this detector is defined correctly yet
        [Test]
        public void validate_DAW_RadianceOfRhoAtZ()
        {
            //need radiance detector to compare results, for now make sure both simulations give same results
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rad_r[1] - _outputTwoLayerTissue.Rad_r[1]), 0.0000001);
        }
        // sanity checks
        [Test]
        public void validate_DAW_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // no specular because photons started inside tissue
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rd + _outputOneLayerTissue.Atot + _outputOneLayerTissue.Td - 1), 0.00000000001);
        }
        // Reflected Momentum Transfer of Rho and SubRegion
        [Test]
        public void validate_DAW_ReflectedMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_rmt[0, 0] - 0.632816), 0.000001);
            // make sure mean integral over MT equals R(rho) results
            var mtbins = ((ReflectedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefMT_rmt[0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r[0] - integral), 0.000001);
            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var rhobins = ((ReflectedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First()).Rho;
            var fracMTbins =
                ((ReflectedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                    Where(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist").First()).FractionalMTBins;
            var numsubregions = _inputOneLayerTissue.TissueInput.Regions.Length;
            for (int i = 0; i < rhobins.Count - 1; i++)
            {
                for (int j = 0; j < mtbins.Count - 1; j++)
                {
                    for (int k = 0; k < numsubregions; k++)
                    {
                        integral = 0.0;
                        for (int m = 0; m < fracMTbins.Count + 1; m++)
                        {
                            integral += _outputOneLayerTissue.RefMT_rmt_frac[i, j, k, m];
                        }
                        Assert.Less(Math.Abs(integral - _outputOneLayerTissue.RefMT_rmt[i, j]), 0.001);
                    }
                }
            }
            // validate a few fractional values - note third index = 0,2 is air and should have
            // contributions only to fourth index=0
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_rmt_frac[0, 0, 0, 0] - 0.632), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_rmt_frac[0, 0, 1, 11] - 0.632), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_rmt_frac[0, 0, 2, 0] - 0.632), 0.001);
            // validate 2 layer tissue results - complementary fracs should be the same in
            // the two layers, i.e. if region 1 has =1 weight, then region 2 should have =0 same weight 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefMT_rmt_frac[0, 0, 1, 11] - 0.632), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefMT_rmt_frac[0, 0, 2, 0] - 0.632), 0.001);
        }
        // Transmitted Momentum Transfer of Rho and SubRegion
        [Test]
        public void validate_DAW_TransmittedMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransMT_rmt[54, 5] - 0.0017405), 0.0000001);
            // make sure mean integral over MT equals T(rho) results
            var mtbins = ((TransmittedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransMT_rmt[54, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r[54] - integral), 0.000001);
        }
        // Reflected Momentum Transfer of X, Y and SubRegion
        [Test]
        public void validate_DAW_ReflectedMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_xymt[0, 0, 28] - 0.018803), 0.000001); 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefMT_xymt2[0, 0, 28] - 0.035357), 0.000001);     
            // make sure mean integral over MT equals R(rho) results
            var mtbins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefMT_xymt[0, 0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy[0, 0] - integral), 0.000001);
            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var xbins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First()).X;
            var ybins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First()).Y;
            var fracMTbins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
               Where(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist").First()).FractionalMTBins;
            var numsubregions = _inputOneLayerTissue.TissueInput.Regions.Length;
            for (int l = 0; l < xbins.Count - 1; l++)
            {
                for (int i = 0; i < ybins.Count - 1; i++)
                {
                    for (int j = 0; j < mtbins.Count - 1; j++)
                    {
                        for (int k = 0; k < numsubregions; k++)
                        {
                            integral = 0.0;
                            for (int m = 0; m < fracMTbins.Count + 1; m++)
                            {
                                integral += _outputOneLayerTissue.RefMT_xymt_frac[l, i, j, k, m];
                            }
                            Assert.Less(Math.Abs(integral - _outputOneLayerTissue.RefMT_xymt[l, i, j]), 0.001);
                        }
                    }
                }
            }
            // validate a few fractional values - note third index = 0,2 is air and should have
            // contributions only to fourth index=0
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_xymt_frac[0, 0, 28, 0, 0] - 0.019), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_xymt_frac[0, 0, 28, 1, 11] - 0.019), 0.001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_xymt_frac[0, 0, 28, 2, 0] - 0.019), 0.001);
            // validate 2 layer tissue results - complementary fracs should be the same in
            // the two layers, i.e. if region 1 has (0,0.1] weight, then region 2 should have (0.9,1] same weight 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefMT_xymt_frac[0, 0, 28, 1, 1] - 0.019), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefMT_xymt_frac[0, 0, 28, 2, 10] - 0.019), 0.001);
        }
        // Transmitted Momentum Transfer of X, Y and SubRegion
        [Test]
        public void validate_DAW_TransmittedMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransMT_xymt[0, 0, 33] - 0.006760), 0.000001);
            // make sure mean integral over MT equals T(rho) results
            var mtbins = ((TransmittedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.
                Where(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist").First()).MTBins;
            var integral = 0.0;
            for (int i = 0; i < mtbins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransMT_xymt[0, 0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xy[0, 0] - integral), 0.000001);
        }
    }
}
