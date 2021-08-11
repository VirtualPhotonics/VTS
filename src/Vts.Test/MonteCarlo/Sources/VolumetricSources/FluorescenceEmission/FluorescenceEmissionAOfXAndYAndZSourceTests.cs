using System;
using System.Collections.Generic;
using System.Reflection;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.PhaseFunctions;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Fluorescence Emission Sources: FluorescenceEmissionAOfXAndYAndZSource
    /// </summary>
    [TestFixture]
    public class FluorescenceEmissionAOfXAndYAndZSourceTests
    {
        private AOfXAndYAndZDetector _aOfXAndYAndZDetector;
        private FluorescenceEmissionAOfXAndYAndZSource _fluorEmissionAOfXAndYAndZSourceCDF,
            _fluorEmissionAOfXAndYAndZSourceUnif;
        private AOfXAndYAndZLoader _xyzLoaderCDF, _xyzLoaderUnif;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "sourcetest",
        };
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "inputAOfXAndYAndZ.txt",
            "AOfXAndYAndZ",
            "AOfXAndYAndZ.txt",
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
        /// set up A(x,y,z) and A(rho,z) detectors to test that AOfXAndYAndZLoader and
        /// AOfRhoAndZLoader initialized all arrays correctly.
        /// Note about this unit test: a) could not just run MC simulation and read 
        /// results because MC sim puts results in Vts.MCCL/bin/Debug an not accessible 
        /// by this unit test, b) goal was to test if source reads data correctly, 
        /// to do this needed to have correct files in resources then
        /// write them locally so that they could be read by this unit test.
        /// </summary>
        [OneTimeSetUp]
        public void setup()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;

            // AOfXAndYAndZ in resource is defined by:
            // single layer tissue with embedded infinite cylinder center=(0,0,1) radius=1
            // detector x=[-2 2] 4 bins, y=[-10 10] 1 bin, z=[0 3] 3 bins
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
                "Resources/sourcetest/inputAOfXAndYAndZ.txt", "sourcetest/inputAOfXAndYAndZ.txt", assemblyName);
            
            // following setup is used to test FluorescenceEmissionSource CDF sampling method
            _fluorEmissionAOfXAndYAndZSourceCDF = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3, SourcePositionSamplingType.CDF);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorEmissionAOfXAndYAndZSourceCDF.Loader = new AOfXAndYAndZLoader(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3);
            _xyzLoaderCDF = _fluorEmissionAOfXAndYAndZSourceCDF.Loader;
            _xyzLoaderCDF.InitializeFluorescentRegionArrays();
            
            // following setup is used to test FluorescenceEmissionSource Unif sampling method
            _fluorEmissionAOfXAndYAndZSourceUnif = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3, SourcePositionSamplingType.Uniform);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorEmissionAOfXAndYAndZSourceUnif.Loader = new AOfXAndYAndZLoader(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3);
            _xyzLoaderUnif = _fluorEmissionAOfXAndYAndZSourceCDF.Loader;
            _xyzLoaderUnif.InitializeFluorescentRegionArrays();
        }
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void validate_source_input()
        {
            // check default constructor
            var si = new FluorescenceEmissionAOfXAndYAndZSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new FluorescenceEmissionAOfXAndYAndZSourceInput(
                "sourcetest",
                "inputAOfXAndYAndZ.txt",
                0,
                SourcePositionSamplingType.CDF
            );
            Assert.IsNotNull(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsNotNull(source);
        }
        /// <summary>
        /// Test to make sure GetFinalPosition produces correct distribution of sources and weights
        /// for CDF sampling
        /// </summary>
        [Test]
        public void verify_that_GetFinalPositionAndWeight_samples_from_CDFOfXAndYAndZ_correctly()
        {
            var countArray = new int[_aOfXAndYAndZDetector.X.Count - 1,
                _aOfXAndYAndZDetector.Y.Count - 1, _aOfXAndYAndZDetector.Z.Count - 1];
            var rng = new MathNet.Numerics.Random.MersenneTwister(0);
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            var regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            regionPhaseFunctions.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunction(0.8, rng));
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                regionPhaseFunctions, 0);
            tissue.Regions[3] = _xyzLoaderCDF.FluorescentTissueRegion;
            for (int i = 0; i < 100; i++)
            {
                var photon = _fluorEmissionAOfXAndYAndZSourceCDF.GetNextPhoton(tissue);
                // verify that photons start within range of midpoints of voxels in infinite cylinder
                Assert.IsTrue((photon.DP.Position.X >= -0.5) && (photon.DP.Position.X <= 0.5));
                Assert.AreEqual(photon.DP.Position.Y, 0);
                Assert.IsTrue((photon.DP.Position.Z >= 0.5) && (photon.DP.Position.Z <= 1.5));
                Assert.IsTrue(Math.Abs(photon.DP.Weight - 1.0) < 1e-6);
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
        /// <summary>
        /// Test to make sure GetFinalPosition produces correct distribution of sources and weights
        /// for CDF sampling
        /// </summary>
        [Test]
        public void verify_that_GetFinalPositionAndWeight_samples_from_UnifOfXAndYAndZ_sampling_correctly()
        {
            var rng = new MathNet.Numerics.Random.MersenneTwister(0);
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            var regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            regionPhaseFunctions.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunction(0.8, rng));
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                regionPhaseFunctions, 0);
            tissue.Regions[3] = _xyzLoaderUnif.FluorescentTissueRegion;
            var xyzNorm = _xyzLoaderUnif.X.Delta * _xyzLoaderUnif.Y.Delta * _xyzLoaderUnif.Z.Delta;
            int ix, iz;
            for (int i = 0; i < 100; i++)
            {
                var photon = _fluorEmissionAOfXAndYAndZSourceUnif.GetNextPhoton(tissue);
                // verify that photons start within range of midpoints of voxels in infinite cylinder
                Assert.IsTrue((photon.DP.Position.X >= -0.5) && (photon.DP.Position.X <= 0.5));
                Assert.AreEqual(photon.DP.Position.Y, 0);
                Assert.IsTrue((photon.DP.Position.Z >= 0.5) && (photon.DP.Position.Z <= 1.5));
                // verify sampling is proceeding in coded sequence
                // detector x=[-2 2] 4 bins, y=[-10 10] 1 bin, z=[0 3] 3 bins
                ix = (int) (photon.DP.Position.X + 0.5) + 1;
                iz = (int)(Math.Floor(photon.DP.Position.Z));
                // verify weight at location is equal to AOfXAndYAndZ note: setup with single y bin
                // expected: Map [ 0 1 1 0; 0 1 1 0; 0 0 0 0] row major
                // expected: A   [ 1 4 7 10; 2 5 8 11; 3 6 9 12] row major
                Assert.IsTrue(Math.Abs(photon.DP.Weight - 
                                       _xyzLoaderUnif.AOfXAndYAndZ[ix, 0, iz] * xyzNorm) < 1e-6);
            }
        }
    }
}