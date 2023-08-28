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
    /// These tests execute simulation through tissue with inclusion of different refractive index.
    /// </summary>
    [TestFixture]
    public class InclusionWithRefractiveIndexMismatchTests
    {
        private SimulationInput _inputOneRegionTissue;
        private SimulationInput _inputTwoRegionMatchedTissue;
        private SimulationInput _inputTwoRegionMismatchedTissue;
        private SimulationOutput _outputOneRegionTissue;
        private SimulationOutput _outputTwoRegionMatchedTissue;
        private SimulationOutput _outputTwoRegionMismatchedTissue;
        private double _factor;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> _listOfTestGeneratedFiles = new List<string>()
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
        /// SingleInclusionTissue with InfiniteCylinderTissueRegion
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
            _inputOneRegionTissue = new SimulationInput(
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
            _outputOneRegionTissue = new MonteCarloSimulation(_inputOneRegionTissue).Run();

            _inputTwoRegionMatchedTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                new SingleInfiniteCylinderTissueInput(
                     new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 2), // center
                        1, // radius
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4) 
                    ), 
                    new ITissueRegion[]
                    { 
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0), // debug with thin slab d=2
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),// debug with g=1
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectors);
            _outputTwoRegionMatchedTissue = new MonteCarloSimulation(_inputTwoRegionMatchedTissue).Run();

            _inputTwoRegionMismatchedTissue = new SimulationInput(
                100,
                "",
                simulationOptions,
                source,
                new SingleInfiniteCylinderTissueInput(
                    new InfiniteCylinderTissueRegion(
                        new Position(0, 0, 2), // center
                        1, // radius
                        new OpticalProperties(0.01, 1.0, 0.8, 1.5) // mismatched n=1.5
                    ),
                    new ITissueRegion[]
                    {
                        new LayerTissueRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerTissueRegion(
                            new DoubleRange(0.0, 20.0), // debug with thin slab d=2
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),// debug with g=1
                        new LayerTissueRegion(
                            new DoubleRange(20.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                detectors);
            _outputTwoRegionMismatchedTissue = new MonteCarloSimulation(_inputTwoRegionMismatchedTissue).Run();

            _factor = 1.0 - Optics.Specular(
                            _inputOneRegionTissue.TissueInput.Regions[0].RegionOP.N,
                            _inputOneRegionTissue.TissueInput.Regions[1].RegionOP.N);
        }

        // Validation values obtained from linux run using above input and seeded the same for homogeneous
        // and two region matched.  Prior results to validate mismatched results.

        // Diffuse Reflectance
        [Test]
        public void Validate_DAW_voxel_RDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Rd * _factor - 0.565017749), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMatchedTissue.Rd * _factor - 0.565017749), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMismatchedTissue.Rd - 0.585135), 0.000001);
        }
        // Reflection R(angle)
        [Test]
        public void Validate_DAW_voxel_ROfAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMatchedTissue.R_a[0] * _factor - 0.0809612757), 0.0000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMismatchedTissue.R_a[0] - 0.083843), 0.000001);
        }
        // Diffuse Transmittance
        [Test]
        public void Validate_DAW_voxel_TDiffuse()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Td * _factor - 0.0228405921), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMatchedTissue.Td * _factor - 0.0228405921), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMismatchedTissue.Td - 0.028486), 0.000001);
        }
        // Transmittance Time(angle)
        [Test]
        public void Validate_DAW_voxel_TOfAngle()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMatchedTissue.T_a[0] * _factor - 0.00327282369), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMismatchedTissue.T_a[0] - 0.004081), 0.000001);
        }
        // Reflectance R(x,y)
        [Test]
        public void Validate_DAW_voxel_ROfXAndY()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.R_xy[198, 201] * _factor - 0.00825301), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoRegionMatchedTissue.R_xy[198, 201] * _factor - 0.00825301), 0.00000001);
            Assert.Less(Math.Abs(_outputTwoRegionMismatchedTissue.R_xy[198, 201] - 0.00), 0.000001);
        }
        // Total Absorption
        [Test]
        public void Validate_DAW_voxel_ATotal()
        {
            Assert.Less(Math.Abs(_outputOneRegionTissue.Atot * _factor - 0.384363881), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMatchedTissue.Atot * _factor - 0.384363881), 0.000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMismatchedTissue.Atot - 0.386378), 0.000001);
        }
       
        // sanity checks
        [Test]
        public void Validate_DAW_voxel_RDiffuse_plus_ATotal_plus_TDiffuse_equals_one()
        {
            // no specular because photons started inside tissue
            Assert.Less(Math.Abs(_outputOneRegionTissue.Rd + 
                                 _outputOneRegionTissue.Atot + 
                                 _outputOneRegionTissue.Td - 1), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMatchedTissue.Rd + 
                                 _outputTwoRegionMatchedTissue.Atot + 
                                 _outputTwoRegionMatchedTissue.Td - 1), 0.00000000001);
            Assert.Less(Math.Abs(_outputTwoRegionMismatchedTissue.Rd + 
                                 _outputTwoRegionMismatchedTissue.Atot + 
                                 _outputTwoRegionMismatchedTissue.Td - 1), 0.00000000001);
        }
    }
}
