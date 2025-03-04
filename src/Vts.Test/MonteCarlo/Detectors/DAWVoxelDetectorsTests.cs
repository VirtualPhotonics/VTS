using System;
using System.Collections.Generic;
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
    /// same seed using mersenne twister STANDARD_TEST.  The tests then run a simulation
    /// through a homogeneous two layer tissue (both layers have the same optical properties)
    /// and verify that the detector tallies are the same.  This tests whether the pseudo-
    /// collision pausing at the layer interface does not change the results.
    /// </summary>
    [TestFixture]
    public class DAWVoxelDetectorsTests
    {
        private SimulationOutput _outputOneRegionTissue;
        private SimulationOutput _outputTwoRegionTissue;
        private SimulationInput _inputOneRegionTissue;
        private SimulationInput _inputTwoRegionTissue;
        private double _factor;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "file.txt", // file that captures screen output of MC simulation
        };
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
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
        [OneTimeSetUp]
        public void execute_Monte_Carlo()
        {
            // delete previously generated files
            clear_folders_and_files();

            // instantiate common classes
            var simulationOptions = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete,
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
                    new ROfXAndYDetectorInput() {X=new DoubleRange(-200.0, 200.0, 401),Y=new DoubleRange(-200.0, 200.0, 401)}, 
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput() {Angle=new DoubleRange(0.0, Math.PI / 2, 2)},
                    new ATotalDetectorInput(),
                };
            var ti = new MultiLayerTissueInput(
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey1"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 20.0),
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey3")
                }
                );
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());

            _inputOneRegionTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                ti,
                detectors);             
            _outputOneRegionTissue = new MonteCarloSimulation(_inputOneRegionTissue).Run();

            var ti2 = new SingleVoxelTissueInput(
                new VoxelTissueRegion(
                    new DoubleRange(-5, 5),
                    new DoubleRange(-5, 5),
                    new DoubleRange(1e-9, 5), // smallest Z.Start with tests passing is 1e-9 
                    new OpticalProperties(0.01, 1.0, 0.8, 1.4), //debug with g=1
                    "HenyeyGreensteinKey1"
                    ),
                new LayerTissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey2"),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 20.0), // debug with thin slab d=2
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                        "HenyeyGreensteinKey3"), // debug with g=1
                    new LayerTissueRegion(
                        new DoubleRange(20.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0),
                        "HenyeyGreensteinKey4")
                }
                );
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunctionInput());
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunctionInput());
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunctionInput());
            ti2.RegionPhaseFunctionInputs.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunctionInput());
    
            _inputTwoRegionTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                ti2,
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
        public void validate_DAW_voxel_RDiffuse()
        {
            Assert.That(Math.Abs(_outputOneRegionTissue.Rd * _factor - 0.565017749), Is.LessThan(0.000000001));
            Assert.That(Math.Abs(_outputTwoRegionTissue.Rd * _factor - 0.565017749), Is.LessThan(0.000000001));
        }
        // Reflection R(angle)
        [Test]
        public void validate_DAW_voxel_ROfAngle()
        {
            Assert.That(Math.Abs(_outputOneRegionTissue.R_a[0] * _factor - 0.0809612757), Is.LessThan(0.0000000001));
            Assert.That(Math.Abs(_outputTwoRegionTissue.R_a[0] * _factor - 0.0809612757), Is.LessThan(0.0000000001));
        }
        // Diffuse Transmittance
        [Test]
        public void validate_DAW_voxel_TDiffuse()
        {
            Assert.That(Math.Abs(_outputOneRegionTissue.Td * _factor - 0.0228405921), Is.LessThan(0.000000001));
            Assert.That(Math.Abs(_outputTwoRegionTissue.Td * _factor - 0.0228405921), Is.LessThan(0.000000001));
        }
        // Transmittance Time(angle)
        [Test]
        public void validate_DAW_voxel_TOfAngle()
        {
            Assert.That(Math.Abs(_outputOneRegionTissue.T_a[0] * _factor - 0.00327282369), Is.LessThan(0.00000000001));
            Assert.That(Math.Abs(_outputTwoRegionTissue.T_a[0] * _factor - 0.00327282369), Is.LessThan(0.00000000001));
        }
        // Reflectance R(x,y)
        [Test]
        public void validate_DAW_voxel_ROfXAndY()
        {
            Assert.That(Math.Abs(_outputOneRegionTissue.R_xy[198, 201] * _factor - 0.00825301), Is.LessThan(0.00000001));
            Assert.That(Math.Abs(_outputTwoRegionTissue.R_xy[198, 201] * _factor - 0.00825301), Is.LessThan(0.00000001));
        }
        // Total Absorption
        [Test]
        public void validate_DAW_voxel_ATotal()
        {
            Assert.That(Math.Abs(_outputOneRegionTissue.Atot * _factor - 0.384363881), Is.LessThan(0.000000001));
            Assert.That(Math.Abs(_outputTwoRegionTissue.Atot * _factor - 0.384363881), Is.LessThan(0.000000001));
        }
       
        // sanity checks
        [Test]
        public void validate_DAW_voxel_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // no specular because photons started inside tissue
            Assert.That(Math.Abs(_outputTwoRegionTissue.Rd + _outputTwoRegionTissue.Atot + 
                                 _outputTwoRegionTissue.Td - 1), Is.LessThan(0.00000000001));
        }
    }
}
