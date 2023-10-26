using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
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
    /// same seed using Mersenne twister STANDARD_TEST.  The tests then run a simulation
    /// through a homogeneous two layer tissue (both layers have the same optical properties)
    /// and verify that the detector tallies are the same.  This tests whether the pseudo-
    /// collision pausing at the layer interface does not change the results.
    /// Second moment tests used linux for validation when available, otherwise used prior test run.
    /// </summary>
    [TestFixture]
    public class DAWLayersDetectorsTests
    {
        private SimulationOutput _outputOneLayerTissue;
        private SimulationOutput _outputTwoLayerTissue;
        private SimulationInput _inputOneLayerTissue;
        private SimulationInput _inputTwoLayerTissue;
        private const double LayerThickness = 1.0; // tissue is homogeneous (both layer opt. props same)
        private const double DosimetryDepth = 1.0;
        private double _factor;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "file.txt", // file that captures screen output of MC simulation
        };

        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// DiscreteAbsorptionWeighting detection.
        /// Setup input to the MC for a homogeneous one layer tissue and a homogeneous
        /// two layer tissue (both layers have same optical properties), execute simulations
        /// and verify results agree with linux results given the same seed
        /// Mersenne twister STANDARD_TEST.  The linux results assumes photon passes
        /// through specular and de-weights photon by specular.  This test starts photon 
        /// inside tissue and then multiplies result by specular de-weighting to match.
        /// NOTE: currently two region executes same photon biography except for pauses
        /// at layer interface.  Variance for DAW results not degraded.
        /// </summary>
        [OneTimeSetUp]
        public void Execute_Monte_Carlo()
        {
            // delete previously generated files
            Clear_folders_and_files();

            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein,
                new List<DatabaseType>(), // databases to be written
                false, // track statistics
                0.0, // RR threshold -> 0 = no RR performed
                0);
            var source = new DirectionalPointSourceInput(
                     new Position(0.0, 0.0, 0.0),
                     new Direction(0.0, 0.0, 1.0),
                     1); // start inside tissue
            var detectors = new List<IDetectorInput>
                {
                    new RSpecularDetectorInput { TallySecondMoment = true },  // this will be 0 since src inside
                    new RDiffuseDetectorInput { TallySecondMoment = true },
                    new ROfAngleDetectorInput
                    {
                        Angle = new DoubleRange(Math.PI / 2 , Math.PI, 2),
                        TallySecondMoment = true
                    },
                    new ROfRhoDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        TallySecondMoment = true
                    },
                    new ROfRhoRecessedDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        ZPlane = -1.0,
                        TallySecondMoment = true
                    },
                    new ROfRhoAndAngleDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        Angle = new DoubleRange(Math.PI / 2, Math.PI, 2),
                        TallySecondMoment = true
                    },
                    new ROfRhoAndTimeDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        Time = new DoubleRange(0.0, 1.0, 101),
                        TallySecondMoment = true
                    },
                    new ROfRhoAndMaxDepthDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        MaxDepth = new DoubleRange(0.0, 1.0, 51),
                        TallySecondMoment = true
                    },
                    new ROfRhoAndMaxDepthRecessedDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        MaxDepth = new DoubleRange(0.0, 1.0, 51),
                        ZPlane = -1.0,
                        TallySecondMoment = true
                    },
                    new ROfXAndYDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        TallySecondMoment = true
                    },
                    new ROfXAndYRecessedDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        ZPlane = -1.0,
                        TallySecondMoment = true
                    },
                    new ROfXAndYAndTimeDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Time = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new ROfXAndYAndTimeRecessedDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Time = new DoubleRange(0.0, 1.0, 11),
                        ZPlane = -1.0,
                        TallySecondMoment = true
                    },
                    new ROfXAndYAndTimeAndSubregionDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Time = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new ROfXAndYAndTimeAndSubregionRecessedDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Time = new DoubleRange(0.0, 1.0, 11),
                        ZPlane = -1.0,
                        TallySecondMoment = true
                    },
                    new ROfXAndYAndThetaAndPhiDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Theta = new DoubleRange(Math.PI / 2, Math.PI, 3),
                        Phi = new DoubleRange(-Math.PI, Math.PI, 5),
                        TallySecondMoment = true
                    },
                    new ROfXAndYAndMaxDepthDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        MaxDepth = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new ROfXAndYAndMaxDepthRecessedDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0, 10.0, 101), 
                        MaxDepth = new DoubleRange(0.0, 1.0, 11),
                        ZPlane = -1.0,
                        TallySecondMoment = true
                    },
                    new ROfRhoAndOmegaDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101), 
                        Omega = new DoubleRange(0.05, 1.0, 20), 
                        TallySecondMoment = true
                    }, // DJC - edited to reflect frequency sampling points (not bins)
                    new ROfFxDetectorInput
                    {
                        Fx = new DoubleRange(0.0, 0.5, 51), 
                        TallySecondMoment = true
                    },
                    new ROfFxAndTimeDetectorInput
                    {
                        Fx = new DoubleRange(0.0, 0.5, 51), 
                        Time = new DoubleRange(0.0, 1.0, 11), 
                        TallySecondMoment = true
                    },
                    new ROfFxAndAngleDetectorInput
                    {
                        Fx = new DoubleRange(0.0, 0.5, 51), 
                        Angle = new DoubleRange(Math.PI / 2, Math.PI, 5),
                        TallySecondMoment = true
                    },
                    new ROfFxAndMaxDepthDetectorInput
                    {
                        Fx = new DoubleRange(0.0, 0.5, 51),
                        MaxDepth = new DoubleRange(0.0, 10.0, 11),
                        TallySecondMoment = true
                    },
                    new TDiffuseDetectorInput { TallySecondMoment = true },
                    new TOfAngleDetectorInput
                    {
                        Angle=new DoubleRange(0.0, Math.PI / 2, 2),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
                    },
                    new TOfRhoDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
                    },
                    new TOfRhoAndAngleDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101),
                        Angle=new DoubleRange(0.0, Math.PI / 2, 2),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
                    },
                    new TOfXAndYDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
                    },
                    new TOfXAndYAndTimeAndSubregionDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101),
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Time = new DoubleRange(0.0, 1.0, 11),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
                    },
                    new TOfFxDetectorInput
                    {
                        Fx = new DoubleRange(0.0, 0.5, 51),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
                    },
                    new AOfRhoAndZDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101),
                        Z=new DoubleRange(0.0, 10.0, 101),
                        TallySecondMoment = true
                    },
                    new AOfXAndYAndZDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0, 10.0, 101),
                        Z =  new DoubleRange(0.0, 10.0, 101),
                        TallySecondMoment = true
                    },
                    new ATotalDetectorInput { TallySecondMoment = true },
                    new FluenceOfRhoAndZDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        Z = new DoubleRange(0.0, 10.0, 101),
                        TallySecondMoment = true
                    },
                    new FluenceOfRhoAndZAndTimeDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 11),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        Time = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new FluenceOfRhoAndZAndOmegaDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 11),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        Omega = new DoubleRange(0.05, 1.0, 20),
                        TallySecondMoment = true
                    },
                    new FluenceOfXAndYAndZDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        TallySecondMoment = true
                    },
                    new FluenceOfXAndYAndZAndOmegaDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 11), 
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        Omega = new DoubleRange(0.05, 1.0, 20),
                        TallySecondMoment = true
                    },
                    new FluenceOfXAndYAndZAndTimeDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 11),
                        Y = new DoubleRange(-10.0, 10.0, 11),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        Time = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new FluenceOfXAndYAndZAndStartingXAndYDetectorInput
                    {
                        StartingX = new DoubleRange(-10.0, 10.0, 3),
                        StartingY = new DoubleRange(-10.0, 10.0, 2),
                        X = new DoubleRange(-10.0, 10.0, 3),
                        Y = new DoubleRange(-10.0, 10.0, 3),
                        Z =  new DoubleRange(0.0, 10.0, 3),
                        TallySecondMoment = true
                    },
                    new FluenceOfFxAndZDetectorInput
                    {
                        Fx = new DoubleRange(0.0, 0.5, 51),
                        Z =  new DoubleRange(0.0, 10.0, 11),
                        TallySecondMoment = true
                    },
                    new RadianceOfRhoAtZDetectorInput
                    {
                        ZDepth = DosimetryDepth, 
                        Rho= new DoubleRange(0.0, 10.0, 101),
                        TallySecondMoment = true
                    },
                    new RadianceOfRhoAndZAndAngleDetectorInput
                    {
                        Rho = new DoubleRange(0.0, 10.0, 101),
                        Z = new DoubleRange(0.0, 10.0, 101),
                        Angle = new DoubleRange(0, Math.PI, 5),
                        TallySecondMoment = true
                    },
                    new RadianceOfFxAndZAndAngleDetectorInput
                    {
                        Fx = new DoubleRange(0.0, 0.5, 51),
                        Z = new DoubleRange(0.0, 10, 101),
                        Angle = new DoubleRange(0, Math.PI, 5),
                        TallySecondMoment = true
                    },
                    new RadianceOfXAndYAndZAndThetaAndPhiDetectorInput
                    {
                        X=new DoubleRange(-10.0, 10.0, 11), 
                        Y=new DoubleRange(-10.0, 10.0, 11),
                        Z=new DoubleRange(0.0, 10.0, 11),
                        Theta=new DoubleRange(0.0, Math.PI, 5), // theta (polar angle)
                        Phi=new DoubleRange(-Math.PI, Math.PI, 5), // phi (azimuthal angle)
                        TallySecondMoment = true
                    },
                    new ReflectedMTOfRhoAndSubregionHistDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins MAKE SURE AGREES with ROfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new TransmittedMTOfRhoAndSubregionHistDetectorInput
                    {
                        Rho=new DoubleRange(0.0, 10.0, 101), // rho bins MAKE SURE AGREES with TOfRho rho specification for unit test below
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
                    },
                    new ReflectedMTOfXAndYAndSubregionHistDetectorInput
                    {
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0, 0.0, 51),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        TallySecondMoment = true
                    },
                    new TransmittedMTOfXAndYAndSubregionHistDetectorInput
                    {    
                        X = new DoubleRange(-10.0, 10.0, 101), 
                        Y = new DoubleRange(-10.0,0.0, 51),
                        MTBins=new DoubleRange(0.0, 500.0, 51), // MT bins
                        FractionalMTBins = new DoubleRange(0.0, 1.0, 11),
                        FinalTissueRegionIndex = 2,
                        TallySecondMoment = true
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

            // modify transmittance detectors to have FinalTissueRegionIndex=3 for 2-layer tissue
            foreach (var detector in detectors.Where(detector => detector.TallyDetails.IsTransmittanceTally))
            {
                ((dynamic) detector).FinalTissueRegionIndex = 3;
            }

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
                            new DoubleRange(0.0, LayerThickness),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerTissueRegion(
                            new DoubleRange(LayerThickness, 20.0),
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
        // this tally was not in the linux code.  Specular will be 0 since source started inside tissue
        [Test]
        public void Validate_DAW_RSpecular()
        {
            Assert.AreEqual(0.0, _outputOneLayerTissue.Rspec);
            Assert.AreEqual(0.0, _outputTwoLayerTissue.Rspec);
            Assert.AreEqual(0.0, _outputOneLayerTissue.Rspec2);
            Assert.AreEqual(0.0, _outputTwoLayerTissue.Rspec2);
        }
        // validation values obtained from linux run using above input and seeded the same for:
        // Diffuse Reflectance
        [Test]
        public void Validate_DAW_RDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rd * _factor - 0.565017749), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rd * _factor - 0.565017749), 0.000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rd2 - 0.467357), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rd2 - 0.467357), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.Rd_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.Rd_TallyCount);
        }
        // Reflection R(rho)
        [Test]
        public void Validate_DAW_ROfRho()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r[0] * _factor - 0.615238307), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_r[0] * _factor - 0.615238307), 0.000000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_r_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_r_TallyCount);
        }
        // Reflection R(rho) 2nd moment, linux value output in printf statement
        [Test]
        public void Validate_DAW_ROfRho_second_moment()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r2[0] * _factor * _factor - 18.92598), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_r2[0] * _factor * _factor - 18.92598), 0.00001);
        }
        // Reflection R(rho) recessed, validated with prior test
        [Test]
        public void Validate_DAW_ROfRhoRecessed()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rr[2] - 0.116336), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rr[2] - 0.116336), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rr2[2] - 0.680291), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rr2[2] - 0.680291), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_rr_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_rr_TallyCount);
        }
        // Reflection R(angle)
        [Test]
        public void Validate_DAW_ROfAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_a2[0] - 0.009595), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_a2[0] - 0.009595), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_a_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_a_TallyCount);
        }
        // Reflection R(rho,angle)
        [Test]
        public void Validate_DAW_ROfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_ra[0, 0] * _factor - 0.0881573691), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_ra[0, 0] * _factor - 0.0881573691), 0.0000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_ra2[0, 0] - 0.411109), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_ra2[0, 0] - 0.411109), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_ra_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_ra_TallyCount);
        }
        // Reflection R(rho,time), 2nd moment validated with prior test
        [Test]
        public void Validate_DAW_ROfRhoAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rt[0, 0] * _factor - 61.5238307), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rt2[0, 0] - 200229.1), 0.1);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rt2[0, 0] - 200229.1), 0.1);
            Assert.AreEqual(89, _outputOneLayerTissue.R_rt_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_rt_TallyCount);

        }
        // Reflection R(rho,maxdepth), validated with integrated R(rho) results and prior test
        [Test]
        public void Validate_DAW_ROfRhoAndMaxDepth()
        {
            var maxDepth = ((ROfRhoAndMaxDepthDetectorInput)_inputOneLayerTissue.DetectorInputs
                .First(d => d.TallyType == "ROfRhoAndMaxDepth")).MaxDepth;
            var integralOneLayer = 0.0;
            var integralTwoLayer = 0.0;
            for (var i = 0; i < maxDepth.Count - 1; i++)
            {
                integralOneLayer += _outputOneLayerTissue.R_rmd[0, i];
                integralTwoLayer += _outputTwoLayerTissue.R_rmd[0, i];
            }
            Assert.Less(Math.Abs(integralOneLayer * _factor - 0.6152383), 0.0000001);  //R(rho) result
            Assert.Less(Math.Abs(integralTwoLayer * _factor - 0.6152383), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rmd[0, 4] - 0.315776), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rmd[0, 4] - 0.315776), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rmd2[0, 4] - 9.97145), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rmd2[0, 4] - 9.97145), 0.00001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_rmd_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_rmd_TallyCount);

        }
        // Reflection R(rho,maxdepth) recessed in air, validated with integrated R(rho) results and prior test
        [Test]
        public void Validate_DAW_ROfRhoAndMaxDepthRecessed()
        { 
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rmdr[2, 11] - 0.062402), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rmdr[2, 11] - 0.062402), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_rmdr2[2, 11] - 0.389408), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_rmdr2[2, 11] - 0.389408), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_rmd_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_rmd_TallyCount);

        }
        // Reflection R(rho,omega)
        [Test]
        public void Validate_DAW_ROfRhoAndOmega()
        {
            // warning - this validation data from Linux is actually for Omega = 0.025GHz
            // (see here: http://virtualphotonics.codeplex.com/discussions/278250)
            Assert.Less(Complex.Abs(
                _outputOneLayerTissue.R_rw[0, 0] * _factor - (0.6152383 - Complex.ImaginaryOne * 0.0002368336)), 0.000001);
            Assert.Less(Complex.Abs(
                _outputTwoLayerTissue.R_rw[0, 0] * _factor - (0.6152383 - Complex.ImaginaryOne * 0.0002368336)), 0.000001);
            Assert.Less(Complex.Abs(_outputOneLayerTissue.R_rw2[0, 0].Real - 20.022918), 0.000001);
            Assert.Less(Complex.Abs(_outputOneLayerTissue.R_rw2[0, 0].Imaginary - 0.0), 0.000001);
            Assert.Less(Complex.Abs(_outputTwoLayerTissue.R_rw2[0, 0].Real - 20.022918), 0.000001);
            Assert.Less(Complex.Abs(_outputTwoLayerTissue.R_rw2[0, 0].Imaginary - 0.0), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_rw_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_rw_TallyCount);
        }
        // Reflectance R(x,y)
        [Test]
        public void Validate_DAW_ROfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy[0, 0] * _factor - 0.01828126), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xy[0, 0] * _factor - 0.01828126), 0.00000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy2[0, 0] - 0.035357), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xy2[0, 0] - 0.035357), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xy_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_xy_TallyCount);
        }        
        // Reflectance R(x,y) recessed in air validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYRecessed()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xyr[0, 12] - 0.175180), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyr[0, 12] - 0.175180), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xyr2[0, 12] - 3.06881), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyr2[0, 12] - 3.06881), 0.00001);
            Assert.AreEqual(89,_outputOneLayerTissue.R_xyr_TallyCount);
            Assert.AreEqual(89,_outputTwoLayerTissue.R_xyr_TallyCount);
        }
        // Reflectance R(x,y,time) validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xyt[0, 0, 9] - 0.188035), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyt[0, 0, 9] - 0.188035), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xyt2[0, 0, 9] - 3.53574), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyt2[0, 0, 9] - 3.53574), 0.00001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xyt_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_xyt_TallyCount);
        }
        // Reflectance R(x,y,time) recessed in air validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYAndTimeRecessed()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xytr[0, 12, 1] - 1.75180), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytr[0, 12, 1] - 1.75180), 0.00001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xytr2[0, 12, 1] - 306.881), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytr2[0, 12, 1] - 306.881), 0.001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xytr_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_xytr_TallyCount);
        }
        // Reflectance R(x,y,time,subregion) validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYAndTimeAndSubregion()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xyts[0, 0, 9, 1] - 0.188035), 0.000001);
            // the following are in different time bins because binned based on time in region
            // not total time
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyts[0, 0, 0, 1] - 0.188035), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyts[0, 0, 9, 2] - 0.188035), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xyts2[0, 0, 9, 1] - 3.53574), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyts2[0, 0, 0, 1] - 3.53574), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyts2[0, 0, 9, 2] - 3.53574), 0.00001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xyt_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_xyt_TallyCount);
            // check that ROfXAndY array equals independent tally use R[0,0] results above test
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyts_xy[0, 0] - _outputOneLayerTissue.R_xy[0, 0]), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xyts_xy2[0, 0] - _outputOneLayerTissue.R_xy2[0, 0]), 0.000001);
        }
        // Reflectance R(x,y,time,subregion) recessed in air validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYAndTimeAndSubregionRecessed()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xytsr[0, 12, 1, 1] - 1.75180), 0.00001);
            // the following are in different time bins because binned based on time in region
            // not total time
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytsr[0, 12, 0, 1] - 1.75180), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytsr[0, 12, 1, 2] - 1.75180), 0.00001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xytsr2[0, 12, 1, 1] - 306.881), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytsr2[0, 12, 0, 1] - 306.881), 0.001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytsr2[0, 12, 1, 2] - 306.881), 0.001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xytsr_TallyCount);
            // check that ROfXAndY array equals independent tally use R[0,12] results above test
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytsr_xy[0, 12] - _outputOneLayerTissue.R_xyr[0, 12]), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytsr_xy2[0, 12] - _outputOneLayerTissue.R_xyr2[0, 12]), 0.000001);
        }
        // Reflectance R(x,y,theta,phi) validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYAndThetaAndPhi()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xytp[0, 0, 1, 2] - 0.039828), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytp[0, 0, 1, 2] - 0.039828), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xytp2[0, 0, 1, 2] - 0.158628), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xytp2[0, 0, 1, 2] - 0.158628), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xytp_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_xytp_TallyCount);
        }
        // Reflectance R(x,y,maxdepth) validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYAndMaxDepth()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xymd[0, 0, 9] - 0.018803), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xymd[0, 0, 9] - 0.018803), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xymd2[0, 0, 9] - 0.035357), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xymd2[0, 0, 9] - 0.035357), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xymd_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_xymd_TallyCount);
        }
        // Reflectance R(x,y,maxdepth) recessed in air validated with prior test
        [Test]
        public void Validate_DAW_ROfXAndYAndMaxRecessedDepth()
        { 
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xymdr[0, 12, 9] - 0.175180), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xymdr[0, 12, 9] - 0.175180), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xymdr2[0, 12, 9] - 3.06881), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_xymdr2[0, 12, 9] - 3.06881), 0.00001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_xymdr_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_xymdr_TallyCount);
        }
        // Reflection R(fx) validated with prior test
        [Test]
        public void Validate_DAW_ROfFx()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fx[1].Real - 0.557019), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fx[1].Imaginary - 0.050931), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fx[1].Real - 0.557019), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fx[1].Imaginary - 0.050931), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fx2[1].Real - 0.467357), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fx2[1].Imaginary - 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fx2[1].Real - 0.467357), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fx2[1].Imaginary - 0.0), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_fx_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_fx_TallyCount);
        }
        // Reflection R(fx, time) validated with prior test
        [Test]
        public void Validate_DAW_ROfFxAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxt[1,0].Real - 3.66252), 0.00001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxt[1,0].Imaginary - 0.209453), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxt[1,0].Real - 3.66252), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxt[1,0].Imaginary - 0.209453), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxt2[1, 0].Real - 34.4183), 0.0001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxt2[1, 0].Imaginary - 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxt2[1, 0].Real - 34.4183), 0.0001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxt2[1, 0].Imaginary - 0.0), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_fxt_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_fxt_TallyCount);
        }
        // Reflection R(fx, angle) validated with prior test
        [Test]
        public void Validate_DAW_ROfFxAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxa[1,0].Real - 0.016654), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxa[1,0].Imaginary - 0.003329), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxa[1,0].Real - 0.016654), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxa[1,0].Imaginary - 0.003329), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxa2[1, 0].Real - 0.005055), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxa2[1, 0].Imaginary - 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxa2[1, 0].Real - 0.005055), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxa2[1, 0].Imaginary - 0.0), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_fxa_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_fxa_TallyCount);
        }
        // Reflection R(fx, max depth) validated with prior test
        [Test]
        public void Validate_DAW_ROfFxAndMaxDepth()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxmd[1, 0].Real - 0.115924), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxmd[1, 0].Imaginary - 0.002295), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxmd[1, 0].Real - 0.115924), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxmd[1, 0].Imaginary - 0.002295), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxmd2[1, 0].Real - 0.112728), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_fxmd2[1, 0].Imaginary - 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxmd2[1, 0].Real - 0.112728), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.R_fxmd2[1, 0].Imaginary - 0.0), 0.000001);
            Assert.AreEqual(89, _outputOneLayerTissue.R_fxmd_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.R_fxmd_TallyCount);
        }
        // Diffuse Transmittance
        [Test]
        public void Validate_DAW_TDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Td * _factor - 0.0228405921), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Td * _factor - 0.0228405921), 0.000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Td2 - 0.008236), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Td2 - 0.008236), 0.000001);
            Assert.AreEqual(11, _outputOneLayerTissue.Td_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.Td_TallyCount);
        }
        // Transmittance Time(rho)
        [Test]
        public void Validate_DAW_TOfRho()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r[54] * _factor - 0.00169219067), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_r[54] * _factor - 0.00169219067), 0.00000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r2[54] - 0.000302), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_r2[54] - 0.000302), 0.000001);
            Assert.AreEqual(11, _outputOneLayerTissue.T_r_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.T_r_TallyCount);
        }
        // Transmittance T(angle)
        [Test]
        public void Validate_DAW_TOfAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_a2[0] - 0.000169), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_a2[0] - 0.000169), 0.000001);
            Assert.AreEqual(11, _outputOneLayerTissue.T_a_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.T_a_TallyCount);
        }
        // Transmittance T(rho,angle)
        [Test]
        public void Validate_DAW_TOfRhoAndAngle()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_ra[54, 0] * _factor - 0.000242473649), 0.000000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_ra[54, 0] * _factor - 0.000242473649), 0.000000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_ra2[54, 0] - 6.22010e-6), 0.00001e-6);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_ra2[54, 0] - 6.22010e-6), 0.00001e-6);
            Assert.AreEqual(11, _outputOneLayerTissue.T_ra_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.T_ra_TallyCount);
        }

        //// Verify integral over rho,angle of T(rho,angle) equals TDiffuse
        [Test]
        public void Validate_DAW_integral_of_TOfRhoAndAngle_equals_TDiffuse()
        {
            // undo angle bin normalization
            var angle = ((TOfRhoAndAngleDetectorInput)_inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "TOfRhoAndAngle")).Angle;
            var rho = ((TOfRhoAndAngleDetectorInput)_inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "TOfRhoAndAngle")).Rho;
            var norm = 2 * Math.PI * rho.Delta * 2 * Math.PI * angle.Delta;
            var integral = 0.0;
            for (var ir = 0; ir < rho.Count - 1; ir++)
            {
                for (var ia = 0; ia < angle.Count - 1; ia++)
                {
                    integral += _outputOneLayerTissue.T_ra[ir, ia] * (rho.Start + (ir + 0.5) * rho.Delta) * Math.Sin((ia + 0.5) * angle.Delta);
                }
            }
            Assert.Less(Math.Abs(integral * norm - _outputOneLayerTissue.Td), 0.000000000001);
        }
        // Transmittance T(x,y): validation is not with linux code, but with prior execution of test so no "factor"
        [Test]
        public void Validate_DAW_TOfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xy[0, 0] - 0.0067603), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xy[0, 0] - 0.0067603), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xy2[0, 0] - 0.004570), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xy2[0, 0] - 0.004570), 0.000001);
            Assert.AreEqual(11, _outputOneLayerTissue.T_xy_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.T_xy_TallyCount);
        }
        // Transmittance T(x,y,time,subregion) validated with prior test
        [Test]
        public void Validate_DAW_TOfXAndYAndTimeAndSubregion()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xyts[0, 0, 9, 1] - 0.067603), 0.000001);
            // the following are in different time bins because binned based on time in region
            // not total time
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xyts[0, 0, 0, 1] - 0.067603), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xyts[0, 0, 9, 2] - 0.067603), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xyts2[0, 0, 9, 1] - 0.457019), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xyts2[0, 0, 0, 1] - 0.457019), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xyts2[0, 0, 9, 2] - 0.457019), 0.0001);
            Assert.AreEqual(11, _outputOneLayerTissue.T_xyts_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.T_xyts_TallyCount);
            // check that ROfXAndY array equals independent tally use R[0,0] results above test
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xyts_xy[0, 0] - _outputOneLayerTissue.T_xy[0, 0]), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_xyts_xy2[0, 0] - _outputOneLayerTissue.T_xy2[0, 0]), 0.000001);
        }
        // Transmission T(fx) validated with prior test
        [Test]
        public void Validate_DAW_TOfFx()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_fx[1].Real - 0.019814), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_fx[1].Imaginary + 4.799287e-5), 0.000001e-5);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_fx[1].Real - 0.019814), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_fx[1].Imaginary + 4.799287e-5), 0.000001e-5);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_fx2[1].Real - 0.008236), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_fx2[1].Imaginary + 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_fx2[1].Real - 0.008236), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.T_fx2[1].Imaginary + 0.0), 0.000001);
            Assert.AreEqual(11, _outputOneLayerTissue.T_fx_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.T_fx_TallyCount);
        }

        // Total Absorption, 2nd moment validated with prior test
        [Test]
        public void Validate_DAW_ATotal()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Atot * _factor - 0.384363881), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Atot * _factor - 0.384363881), 0.000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Atot2 - 0.266285), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Atot2 - 0.266285), 0.000001);
            Assert.AreEqual(42334, _outputOneLayerTissue.Atot_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Atot_TallyCount);
        }
        // Absorption A(rho,z)
        [Test]
        public void Validate_DAW_AOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.A_rz[0, 0] * _factor - 0.39494647), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.A_rz[0, 0] * _factor - 0.39494647), 0.00000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.A_rz2[0, 0] - 0.498845), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.A_rz2[0, 0] - 0.498845), 0.000001);
            Assert.AreEqual(42334, _outputOneLayerTissue.A_rz_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.A_rz_TallyCount);
        }
        // Absorption A(x,y,z) 2nd moment validation based on prior run
        [Test]
        public void Validate_DAW_AOfXAndYAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.A_xyz[0, 0, 0] * _factor - 0.0003656252), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.A_xyz[0, 0, 0] * _factor - 0.0003656252), 0.000000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.A_xyz2[0, 0, 0] - 0.00001414), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.A_xyz2[0, 0, 0] - 0.00001414), 0.00000001);
            Assert.AreEqual(42334, _outputOneLayerTissue.A_xyz_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.A_xyz_TallyCount);
        }
        // Fluence Flu(rho,z)
        [Test]
        public void Validate_DAW_FluenceOfRhoAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rz[0, 0] * _factor - 39.4946472), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rz[0, 0] * _factor - 39.4946472), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rz2[0, 0] - 4988.45), 0.01);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rz2[0, 0] - 4988.45), 0.01);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_rz_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_rz_TallyCount);
        }
        // Fluence Flu(rho,z,t), 1st and 2nd moment validated with prior test
        [Test]
        public void Validate_DAW_FluenceOfRhoAndZAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rzt[0, 0, 0] - 5.52986), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rzt[0, 0, 0] - 5.52986), 0.00001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rzt2[0, 0, 0] - 42.7474), 0.0001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rzt2[0, 0, 0] - 42.7474), 0.0001);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_rzt_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_rzt_TallyCount);
        }
        // Fluence Flu(x,y,z), 1st and 2nd moment validated with prior test
        [Test]
        public void Validate_DAW_FluenceOfXAndYAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyz[0, 0, 0] - 0.0016990), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyz[0, 0, 0] - 0.0016990), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyz2[0, 0, 0] - 0.0001815), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyz2[0, 0, 0] - 0.0001815), 0.0000001);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_xyz_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_xyz_TallyCount);
        }
        // Fluence Flu(x,y,z,omega), 1st moment validated with prior test
        // Verify integral * mua over rho,z,omega equals ATotal
        [Test]
        public void Validate_DAW_FluenceOfXAndYAndZAndOmega()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzw[0, 0, 0, 10].Real + 0.0002956), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzw[0, 0, 0, 10].Imaginary + 0.0009234), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzw[0, 0, 0, 10].Real + 0.0002956), 0.0000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzw[0, 0, 0, 10].Imaginary + 0.0009234), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzw2[0, 0, 0, 10].Real - 0.000181), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzw2[0, 0, 0, 10].Imaginary - 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzw2[0, 0, 0, 10].Real - 0.000181), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzw2[0, 0, 0, 10].Imaginary - 0.0), 0.000001);
            // undo angle bin normalization
            var x = ((FluenceOfXAndYAndZAndOmegaDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndOmega")).X;
            var y = ((FluenceOfXAndYAndZAndOmegaDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndOmega")).Y;
            var z = ((FluenceOfXAndYAndZAndOmegaDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndOmega")).Z;
            var norm = x.Delta * y.Delta * z.Delta;
            var integral = 0.0;
            for (var ix = 0; ix < x.Count - 1; ix++)
            {
                for (var iy = 0; iy < y.Count - 1; iy++)
                {
                    for (var iz = 0; iz < z.Count - 1; iz++)
                    {
                        integral += _outputOneLayerTissue.Flu_xyzw[ix, iy, iz, 0].Magnitude * norm; // tally only DC
                    }
                }
            }
            var mua = _inputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua;
            Assert.Less(Math.Abs(integral * mua - _outputOneLayerTissue.Atot), 0.0006);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_xyzw_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_xyzw_TallyCount);
        }

        // Fluence Flu(x,y,z,time), 1st moment validated with prior test
        // Verify integral * mua over x,y,z,time equals ATotal
        [Test]
        public void Validate_DAW_FluenceOfXAndYAndZAndTime()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzt[0, 0, 0, 4] - 0.012811), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzt2[0, 0, 0, 4] - 0.016414), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzt[0, 0, 0, 4] - 0.012811), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzt2[0, 0, 0, 4] - 0.016414), 0.000001);
            // undo angle bin normalization
            var x = ((FluenceOfXAndYAndZAndTimeDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndTime")).X;
            var y = ((FluenceOfXAndYAndZAndTimeDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndTime")).Y;
            var z = ((FluenceOfXAndYAndZAndTimeDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndTime")).Z;
            var t = ((FluenceOfXAndYAndZAndTimeDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndTime")).Time;
            var norm = x.Delta * y.Delta * z.Delta * t.Delta;
            var integral = 0.0;
            for (var ix = 0; ix < x.Count - 1; ix++)
            {
                for (var iy = 0; iy < y.Count - 1; iy++)
                {
                    for (var iz = 0; iz < z.Count - 1; iz++)
                    {
                        for (var it = 0; it < t.Count - 1; it++)
                        {
                            integral += _outputOneLayerTissue.Flu_xyzt[ix, iy, iz, it] * norm;
                        } 
                    }
                }
            }
            var mua = _inputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua;
            Assert.Less(Math.Abs(integral * mua - _outputOneLayerTissue.Atot), 0.0006);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_xyzw_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_xyzw_TallyCount);
        }
        // Fluence Flu(x,y,z) and starting (x,y), 1st moment validated with prior test
        // Verify integral * mua over x,y,z starting (x,y) equals ATotal
        [Test]
        public void Validate_DAW_FluenceOfXAndYAndZAndStartingXAndY()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzxy[1, 0, 0, 0, 0] - 0.011210), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_xyzxy2[1, 0, 0, 0, 0] - 0.000363), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzxy[1, 0, 0, 0, 0] - 0.011210), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_xyzxy2[1, 0, 0, 0, 0] - 0.000363), 0.000001);
            Assert.AreEqual(100, _outputOneLayerTissue.Flu_xyzxy_xycount[1, 0]);
            // undo angle bin normalization
            var startingX = ((FluenceOfXAndYAndZAndStartingXAndYDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY")).StartingX;
            var startingY = ((FluenceOfXAndYAndZAndStartingXAndYDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY")).StartingY;
            var x = ((FluenceOfXAndYAndZAndStartingXAndYDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY")).X;
            var y = ((FluenceOfXAndYAndZAndStartingXAndYDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY")).Y;
            var z = ((FluenceOfXAndYAndZAndStartingXAndYDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfXAndYAndZAndStartingXAndY")).Z;
            var norm = x.Delta * y.Delta * z.Delta;
            var integral = 0.0;
            for (var isx = 0; isx < startingX.Count - 1; isx++)
            {
                for (var isy = 0; isy < startingY.Count - 1; isy++)
                {
                    for (var ix = 0; ix < x.Count - 1; ix++)
                    {
                        for (var iy = 0; iy < y.Count - 1; iy++)
                        {
                            for (var iz = 0; iz < z.Count - 1; iz++)
                            {
                                integral += _outputOneLayerTissue.Flu_xyzxy[isx, isy, ix, iy, iz] * norm;
                            }
                        }
                    }
                }
            }
            var mua = _inputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua;
            Assert.Less(Math.Abs(integral * mua - _outputOneLayerTissue.Atot), 0.0006);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_xyzw_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_xyzw_TallyCount);
        }
        // Fluence Flu(rho,z,omega), 1st moment validated with prior test
        // Verify integral * mua over rho,z,omega equals ATotal
        [Test]
        public void Validate_DAW_FluenceOfRhoAndZAndOmega()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rzw[0, 0, 0].Real - 0.553938), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rzw[0, 0, 0].Imaginary + 0.001667), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rzw[0, 0, 0].Real - 0.553938), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rzw[0, 0, 0].Imaginary + 0.001667), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rzw2[0, 0, 0].Real - 0.428065), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_rzw2[0, 0, 0].Imaginary + 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rzw2[0, 0, 0].Real - 0.428065), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_rzw2[0, 0, 0].Imaginary + 0.0), 0.000001);
            // undo angle bin normalization
            var rho = ((FluenceOfRhoAndZAndOmegaDetectorInput) _inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "FluenceOfRhoAndZAndOmega")).Rho;
            var z = ((FluenceOfRhoAndZAndOmegaDetectorInput) _inputOneLayerTissue.DetectorInputs.First(
                d => d.TallyType == "FluenceOfRhoAndZAndOmega")).Z;
            var normFactor = 2.0 * Math.PI * rho.Delta * z.Delta;
            var integral = 0.0;
            for (var ir = 0; ir < rho.Count - 1; ir++)
            {
                var norm = (rho.Start + (ir + 0.5) * rho.Delta) * normFactor;
                for (var iz = 0; iz < z.Count - 1; iz++)
                {
                    integral += _outputOneLayerTissue.Flu_rzw[ir, iz, 0].Magnitude * norm; // tally only DC
                }
            }

            var mua = _inputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua;
            Assert.Less(Math.Abs(integral * mua - _outputOneLayerTissue.Atot), 0.0008);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_rzw_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_rzw_TallyCount);
        }
        // Fluence Flu(fx,z), 1st moment validated with prior test
        // Verify integral * mua over z, with fx=0 equals ATotal
        [Test]
        public void Validate_DAW_FluenceOfFxAndZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_fxz[0, 0].Real - 5.768415), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_fxz[0, 0].Imaginary + 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_fxz[0, 0].Real - 5.768415), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_fxz[0, 0].Imaginary + 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_fxz2[0, 0].Real - 56.9632), 0.0001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Flu_fxz2[0, 0].Imaginary + 0.0), 0.000001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_fxz2[0, 0].Real - 56.9632), 0.0001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Flu_fxz2[0, 0].Imaginary + 0.0), 0.000001);
            // undo angle bin normalization
            var z = ((FluenceOfFxAndZDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "FluenceOfFxAndZ")).Z;
            var norm = z.Delta;
            var integral = 0.0;
            for (var iz = 0; iz < z.Count - 1; iz++)
            {
                integral += _outputOneLayerTissue.Flu_fxz[0, iz].Magnitude * norm; // tally only DC
            }
            
            var mua = _inputOneLayerTissue.TissueInput.Regions[1].RegionOP.Mua;
            Assert.Less(Math.Abs(integral * mua - _outputOneLayerTissue.Atot), 1e-6);
            Assert.AreEqual(42334, _outputOneLayerTissue.Flu_fxz_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Flu_fxz_TallyCount);
        }
        // Volume Radiance Rad(rho,z,angle)
        // Verify integral over angle of Radiance equals Fluence
        [Test]
        public void Validate_DAW_RadianceOfRhoAndZAndAngle()
        {
            // undo angle bin normalization
            var angle = ((RadianceOfRhoAndZAndAngleDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "RadianceOfRhoAndZAndAngle")).Angle;
            var norm = 2 * Math.PI * angle.Delta;
            var integral = 0.0;
            for (var ia = 0; ia < angle.Count - 1; ia++)
            {
                integral += _outputOneLayerTissue.Rad_rza[0, 6, ia] * Math.Sin((ia + 0.5) * angle.Delta);
            }
            Assert.Less(Math.Abs(integral * norm - _outputOneLayerTissue.Flu_rz[0, 6]), 0.000000000001);
            Assert.AreEqual(42334, _outputOneLayerTissue.Rad_rza_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Rad_rza_TallyCount);
        }

        // Volume Radiance Rad(x,y,z,theta,phi)
        // Verify integral over angle of Radiance equals Fluence
        [Test]
        public void Validate_DAW_RadianceOfXAndYAndZAndThetaAndPhi()
        {
            // undo angle bin normalization
            var theta = ((RadianceOfXAndYAndZAndThetaAndPhiDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi")).Theta;
            var phi = ((RadianceOfXAndYAndZAndThetaAndPhiDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "RadianceOfXAndYAndZAndThetaAndPhi")).Phi;
            var norm = theta.Delta * phi.Delta;
            var integral = 0.0;
            for (var it = 0; it < theta.Count - 1; it++)
            {
                for (var ip = 0; ip < phi.Count - 1; ip++)
                    integral += _outputOneLayerTissue.Rad_xyztp[0, 0, 0, it, ip] * Math.Sin((it + 0.5) * theta.Delta);
            }
            Assert.Less(Math.Abs(integral * norm - _outputOneLayerTissue.Flu_xyz[0, 0, 0]), 0.000000000001);
            Assert.AreEqual(42334, _outputOneLayerTissue.Rad_xyztp_TallyCount);
            Assert.AreEqual(42334, _outputTwoLayerTissue.Rad_xyztp_TallyCount);
        }
        // Radiance(rho) at depth Z - not sure this detector is defined correctly yet
        [Test]
        public void Validate_DAW_RadianceOfRhoAtZ()
        {
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rad_r[0] - 1.95161), 0.00001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rad_r[0] - 1.95161), 0.00001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rad_r2[0] - 63.5278), 0.0001);
            Assert.Less(Math.Abs(_outputTwoLayerTissue.Rad_r2[0] - 63.5278), 0.0001);
            //need radiance detector to compare results, for now make sure both simulations give same results
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rad_r[1] - _outputTwoLayerTissue.Rad_r[1]), 0.0000001);
            Assert.AreEqual(199, _outputOneLayerTissue.Rad_r_TallyCount);
            Assert.AreEqual(199, _outputTwoLayerTissue.Rad_r_TallyCount);
        }
        // sanity checks
        [Test]
        public void Validate_DAW_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // no specular because photons started inside tissue
            Assert.Less(Math.Abs(_outputOneLayerTissue.Rd + _outputOneLayerTissue.Atot + _outputOneLayerTissue.Td - 1), 0.00000000001);
        }
        // Reflected Momentum Transfer of Rho and SubRegion
        [Test]
        public void Validate_DAW_ReflectedMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_rmt[0, 0] - 0.632816), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_rmt2[0, 0] - 20.0229), 0.0001);
            // make sure mean integral over MT equals R(rho) results
            var mtBins = ((ReflectedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (var i = 0; i < mtBins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefMT_rmt[0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_r[0] - integral), 0.000001);
            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var rhoBins = ((ReflectedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist")).Rho;
            var fracMtBins =
                ((ReflectedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "ReflectedMTOfRhoAndSubregionHist")).FractionalMTBins;
            var numSubregions = _inputOneLayerTissue.TissueInput.Regions.Length;
            for (var i = 0; i < rhoBins.Count - 1; i++)
            {
                for (var j = 0; j < mtBins.Count - 1; j++)
                {
                    for (var k = 0; k < numSubregions; k++)
                    {
                        integral = 0.0;
                        for (var m = 0; m < fracMtBins.Count + 1; m++)
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
            Assert.AreEqual(89, _outputOneLayerTissue.RefMT_rmt_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.RefMT_rmt_TallyCount);
        }
        // Transmitted Momentum Transfer of Rho and SubRegion
        [Test]
        public void Validate_DAW_TransmittedMTOfRhoAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransMT_rmt[54, 5] - 0.0017405), 0.0000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransMT_rmt2[54, 5] - 0.000302), 0.000001);
            // make sure mean integral over MT equals T(rho) results
            var mtBins = ((TransmittedMTOfRhoAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "TransmittedMTOfRhoAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (var i = 0; i < mtBins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransMT_rmt[54, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_r[54] - integral), 0.000001);
            Assert.AreEqual(11, _outputOneLayerTissue.TransMT_rmt_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.TransMT_rmt_TallyCount);
        }
        // Reflected Momentum Transfer of X, Y and SubRegion
        [Test]
        public void Validate_DAW_ReflectedMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.RefMT_xymt[0, 0, 28] - 0.018803), 0.000001); 
            Assert.Less(Math.Abs(_outputTwoLayerTissue.RefMT_xymt2[0, 0, 28] - 0.035357), 0.000001);     
            // make sure mean integral over MT equals R(rho) results
            var mtBins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (var i = 0; i < mtBins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.RefMT_xymt[0, 0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.R_xy[0, 0] - integral), 0.000001);
            // verify that sum of FractionalMT for a particular region and dynamic or static summed over
            // other indices equals Mean(rho,mt)
            var xBins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist")).X;
            var yBins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist")).Y;
            var fracMtBins = ((ReflectedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "ReflectedMTOfXAndYAndSubregionHist")).FractionalMTBins;
            var numSubregions = _inputOneLayerTissue.TissueInput.Regions.Length;
            for (var l = 0; l < xBins.Count - 1; l++)
            {
                for (var i = 0; i < yBins.Count - 1; i++)
                {
                    for (var j = 0; j < mtBins.Count - 1; j++)
                    {
                        for (var k = 0; k < numSubregions; k++)
                        {
                            integral = 0.0;
                            for (var m = 0; m < fracMtBins.Count + 1; m++)
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
            Assert.AreEqual(89, _outputOneLayerTissue.RefMT_xymt_TallyCount);
            Assert.AreEqual(89, _outputTwoLayerTissue.RefMT_xymt_TallyCount);
        }
        // Transmitted Momentum Transfer of X, Y and SubRegion
        [Test]
        public void Validate_DAW_TransmittedMTOfXAndYAndSubregionHist()
        {
            // use initial results to verify any new changes to the code
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransMT_xymt[0, 0, 33] - 0.006760), 0.000001);
            Assert.Less(Math.Abs(_outputOneLayerTissue.TransMT_xymt2[0, 0, 33] - 0.004570), 0.000001);
            // make sure mean integral over MT equals T(rho) results
            var mtBins = ((TransmittedMTOfXAndYAndSubregionHistDetectorInput)_inputOneLayerTissue.DetectorInputs.First(d => d.TallyType == "TransmittedMTOfXAndYAndSubregionHist")).MTBins;
            var integral = 0.0;
            for (var i = 0; i < mtBins.Count - 1; i++)
            {
                integral += _outputOneLayerTissue.TransMT_xymt[0, 0, i];
            }
            Assert.Less(Math.Abs(_outputOneLayerTissue.T_xy[0, 0] - integral), 0.000001);
            Assert.AreEqual(11, _outputOneLayerTissue.TransMT_xymt_TallyCount);
            Assert.AreEqual(11, _outputTwoLayerTissue.TransMT_xymt_TallyCount);
        }
    }
}
