using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Fluorescence Emission Sources
    /// </summary>
    [TestFixture]
    public class FluorescenceEmissionSourcesTests
    {
        private AOfXAndYAndZDetector _aOfXAndYAndZDetector;
        private FluorescenceEmissionAOfXAndYAndZSource _fluorescenceEmissionAOfXAndYAndZSource;
        private AOfXAndYAndZLoader _loader;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "sourcetest",
        };
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "input.txt",
            "AOfXAndYAndZ",
            "AOfXAndYAndZ.txt"
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }
        /// <summary>
        /// set up A(x,y,z) detector to test that AOfXAndYAndZLoader initialized all arrays correctly.
        /// Note about this unit test: a) could not just run MC simulation and read results because MC sim
        /// puts results in Vts.MCCL/bin/Debug an not accessible by this unit test, b) goal was to test
        /// if source reads data correctly, to do this needed to have correct files in resources then
        /// write them locally so that they could be read by this unit test.
        /// </summary>
        [OneTimeSetUp]
        public void setup()
        {
            // set up MC simulation that generated the absorbed energy detector results in embedded resources
            //var input = new SimulationInput(
            //100,
            //"test",
            //new SimulationOptions(
            //    0, // random number generator seed, -1=random seed, 0=fixed seed
            //    RandomNumberGeneratorType.MersenneTwister,
            //    AbsorptionWeightingType.Discrete,
            //    PhaseFunctionType.HenyeyGreenstein,
            //    new List<DatabaseType>() { }, // databases to be written
            //    false, // track statistics
            //    0.0, // RR threshold -> no RR performed
            //    0),
            //    new DirectionalPointSourceInput(
            //        new Position(0.0, 0.0, 0.0),
            //        new Direction(0.0, 0.0, 1.0),
            //        0), // 0=start in air, 1=start in tissue
            //    new SingleInfiniteCylinderTissueInput(
            //        new InfiniteCylinderTissueRegion(
            //            new Position(0, 0, 1),
            //            1.0,
            //            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
            //        ),
            //        new ITissueRegion[]
            //        {
            //            new LayerTissueRegion(
            //                new DoubleRange(double.NegativeInfinity, 0.0),
            //                new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
            //            new LayerTissueRegion(
            //                new DoubleRange(0.0, 100.0),
            //                new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
            //            new LayerTissueRegion(
            //                new DoubleRange(100.0, double.PositiveInfinity),
            //                new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
            //        }
            //    ),
            //    new List<IDetectorInput>()
            //    {
            //        new AOfXAndYAndZDetectorInput(){
            //            X =new DoubleRange(-2, 2, 5),
            //            Y =new DoubleRange(-10, 10, 2),
            //            Z =new DoubleRange(0, 3, 4)}
            //    }
            //);
            //var output = new MonteCarloSimulation(input).Run();  //NOTE: this puts output in Vts.MCCL/bin/Debug
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            _aOfXAndYAndZDetector = (dynamic)DetectorIO.ReadDetectorFromFileInResources(
                "AOfXAndYAndZ", "Resources/sourcetest/", assemblyName);
            // overwrite statistical data in Mean with deterministic values to test
            int count = 1;
            for (int i = 0; i < _aOfXAndYAndZDetector.X.Count - 1; i++)
            {
                for (int j = 0; j < _aOfXAndYAndZDetector.Y.Count - 1; j++)
                {
                    for (int k = 0; k < _aOfXAndYAndZDetector.Z.Count - 1; k++)
                    {
                        _aOfXAndYAndZDetector.Mean[i, j, k] = count; // make all nonzero and unique
                        ++count;
                    }
                }
            }
            DetectorIO.WriteDetectorToFile(_aOfXAndYAndZDetector, "sourcetest");  

            FileIO.CopyFileFromResources(
                "Resources/sourcetest/input.txt", "sourcetest/input.txt", assemblyName);
            // following setup is used to test FluorescenceEmissionSource
            _fluorescenceEmissionAOfXAndYAndZSource = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "input.txt", 3);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorescenceEmissionAOfXAndYAndZSource.Loader = new AOfXAndYAndZLoader(
                "sourcetest", "input.txt", 3);
            _loader = _fluorescenceEmissionAOfXAndYAndZSource.Loader;
            
            _loader.InitializeFluorescentRegionArrays();
        }

        /// <summary>
        /// Validate AOfXandYAndZLoader method InitializeFluorscentRegionArrays creates arrays
        /// MapOfXAndYAndZ = has 1's in voxels of fluorescent tissue region
        /// PDFOfXAndYAndZ = AOfXAndYAndZ in locations where MapOfXAndYAndZ has 1, then normalized to PDF by total
        /// CDPOfXAndYAndZ = CDF of PDFOfXAndYAndZ
        /// </summary>
        [Test]
        public void validate_AOfXAndYAndZLoader_method_InitializeFluorscentRegionArrays()
        {
            // check that arrays are of correct dimension
            Assert.AreEqual(_loader.MapOfXAndYAndZ.GetLength(0), _loader.X.Count - 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ.GetLength(1), _loader.Y.Count - 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ.GetLength(2), _loader.Z.Count - 1);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ.GetLength(0), _loader.X.Count - 1);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ.GetLength(1), _loader.Y.Count - 1);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ.GetLength(2), _loader.Z.Count - 1);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ.GetLength(0), _loader.X.Count - 1);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ.GetLength(1), _loader.Y.Count - 1);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ.GetLength(2), _loader.Z.Count - 1);
            // check that Map is only 1 in region of infinite cylinder
            Assert.AreEqual(_loader.MapOfXAndYAndZ[0, 0, 0], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[1, 0, 0], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[2, 0, 0], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[0, 0, 1], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[1, 0, 1], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[2, 0, 1], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[0, 0, 2], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[1, 0, 2], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 2], 0);
            // check that PDF is correct
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[0, 0, 0], 0);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[1, 0, 0] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[2, 0, 0] - 0.291666) < 1e-6);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[0, 0, 1], 0);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[1, 0, 1] - 0.208333) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[2, 0, 1] - 0.333333) < 1e-6);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[0, 0, 2], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[1, 0, 2], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[3, 0, 2], 0);
            // check that CDF is only !=0 in region of infinite cylinder and increasing with z,y,x order
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[0, 0, 0], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[0, 0, 1], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[0, 0, 2], 0);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[1, 0, 0] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[1, 0, 1] - 0.375) < 1e-6);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[1, 0, 2], 0);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[2, 0, 0] - 0.666666) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[2, 0, 1] - 1) < 1e-6);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 2], 0);
        }
        /// <summary>
        /// Test to make sure GetFinalPosition produces correct distribution of sources
        /// </summary>
        [Test]
        public void verify_that_GetFinalPosition_samples_from_CDFOfXAndYAndZ_correctly()
        {
            var countArray = new int[_aOfXAndYAndZDetector.X.Count - 1,
                _aOfXAndYAndZDetector.Y.Count - 1, _aOfXAndYAndZDetector.Z.Count - 1];
            ITissueInput _tissueInput = new SingleInfiniteCylinderTissueInput();
            IDictionary<string, IPhaseFunction> regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            Random _rng = new Random();
            for (int i = 0; i < _tissueInput.Regions.Length; i++)
            {
                regionPhaseFunctions.Add(_tissueInput.Regions[i].PhaseFunctionKey,
                    PhaseFunctionFactory.GetPhaseFunction(_tissueInput.Regions[i],
                        _tissueInput, _rng));
            }
            ITissue _tissue = _tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                regionPhaseFunctions, 0);
            _tissue.Regions[3] = _loader.FluorescentTissueRegion;
            for (int i = 0; i < 100; i++)
            {
                var photon = _fluorescenceEmissionAOfXAndYAndZSource.GetNextPhoton(_tissue);
                // verify that photons start within range of midpoints of voxels
                Assert.IsTrue((photon.DP.Position.X >= -0.5) && (photon.DP.Position.X <= 0.5));
                Assert.AreEqual(photon.DP.Position.Y, 0);
                Assert.IsTrue((photon.DP.Position.Z >= 0.5) && (photon.DP.Position.Z <= 1.5));
                int ix = (int)(photon.DP.Position.X + 0.5) + 1;
                int iz =(int)(Math.Floor(photon.DP.Position.Z));
                countArray[ix, 0, iz] += 1;
            }
            // check that countArray is only 1 in region of infinite cylinder
            Assert.AreEqual(countArray[0, 0, 0], 0);
            Assert.AreEqual(countArray[1, 0, 0], 17);
            Assert.AreEqual(countArray[2, 0, 0], 31);
            Assert.AreEqual(countArray[3, 0, 0], 0);
            Assert.AreEqual(countArray[0, 0, 1], 0);
            Assert.AreEqual(countArray[1, 0, 1], 16);
            Assert.AreEqual(countArray[2, 0, 1], 36);
            Assert.AreEqual(countArray[3, 0, 1], 0);
            Assert.AreEqual(countArray[0, 0, 2], 0);
            Assert.AreEqual(countArray[1, 0, 2], 0);
            Assert.AreEqual(countArray[2, 0, 2], 0);
            Assert.AreEqual(countArray[3, 0, 2], 0);
            // Note: with the unit test AOfXAndYAndZ defined array the PDF is:
            // PDF[1,0,0]=16.6
            // PDF[2,0,0]=29.2
            // PDF[1,0,1]=20.8
            // PDF[2,0,1]=33.3 -> so not bad agreement
        }

    }
}