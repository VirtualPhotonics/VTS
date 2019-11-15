using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
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

        private FluorescenceEmissionAOfXAndYAndZSource _fluorEmissionAOfXAndYAndZSourceCDF,
            _fluorEmissionAOfXAndYAndZSourceUnif;
        private AOfXAndYAndZLoader _loaderCDF, _loaderUnif;

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
        /// write them locally so that they could be r-2ead by this unit test.
        /// </summary>
        [OneTimeSetUp]
        public void setup()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            // AOfXAnYAndZ in resource is defined by:
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
                "Resources/sourcetest/input.txt", "sourcetest/input.txt", assemblyName);
            
            // following setup is used to test FluorescenceEmissionSource CDF sampling method
            _fluorEmissionAOfXAndYAndZSourceCDF = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "input.txt", 3, SourcePositionSamplingType.CDF);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorEmissionAOfXAndYAndZSourceCDF.Loader = new AOfXAndYAndZLoader(
                "sourcetest", "input.txt", 3);
            _loaderCDF = _fluorEmissionAOfXAndYAndZSourceCDF.Loader;
            _loaderCDF.InitializeFluorescentRegionArrays();
            
            // following setup is used to test FluorescenceEmissionSource Unif sampling method
            _fluorEmissionAOfXAndYAndZSourceUnif = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "input.txt", 3, SourcePositionSamplingType.Uniform);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorEmissionAOfXAndYAndZSourceUnif.Loader = new AOfXAndYAndZLoader(
                "sourcetest", "input.txt", 3);
            _loaderUnif = _fluorEmissionAOfXAndYAndZSourceCDF.Loader;

            _loaderCDF.InitializeFluorescentRegionArrays();
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
            var loader = _loaderCDF;
            // check that arrays are of correct dimension
            Assert.AreEqual(loader.MapOfXAndYAndZ.GetLength(0), loader.X.Count - 1);
            Assert.AreEqual(loader.MapOfXAndYAndZ.GetLength(1), loader.Y.Count - 1);
            Assert.AreEqual(loader.MapOfXAndYAndZ.GetLength(2), loader.Z.Count - 1);
            Assert.AreEqual(loader.PDFOfXAndYAndZ.GetLength(0), loader.X.Count - 1);
            Assert.AreEqual(loader.PDFOfXAndYAndZ.GetLength(1), loader.Y.Count - 1);
            Assert.AreEqual(loader.PDFOfXAndYAndZ.GetLength(2), loader.Z.Count - 1);
            Assert.AreEqual(loader.CDFOfXAndYAndZ.GetLength(0), loader.X.Count - 1);
            Assert.AreEqual(loader.CDFOfXAndYAndZ.GetLength(1), loader.Y.Count - 1);
            Assert.AreEqual(loader.CDFOfXAndYAndZ.GetLength(2), loader.Z.Count - 1);
            // check that Map is only 1 in region of infinite cylinder
            Assert.AreEqual(loader.MapOfXAndYAndZ[0, 0, 0], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[1, 0, 0], 1);
            Assert.AreEqual(loader.MapOfXAndYAndZ[2, 0, 0], 1);
            Assert.AreEqual(loader.MapOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[0, 0, 1], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[1, 0, 1], 1);
            Assert.AreEqual(loader.MapOfXAndYAndZ[2, 0, 1], 1);
            Assert.AreEqual(loader.MapOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[0, 0, 2], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[1, 0, 2], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[3, 0, 2], 0);
            // check that PDF is correct
            Assert.AreEqual(loader.PDFOfXAndYAndZ[0, 0, 0], 0);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[1, 0, 0] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[2, 0, 0] - 0.291666) < 1e-6);
            Assert.AreEqual(loader.PDFOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(loader.PDFOfXAndYAndZ[0, 0, 1], 0);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[1, 0, 1] - 0.208333) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[2, 0, 1] - 0.333333) < 1e-6);
            Assert.AreEqual(loader.PDFOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(loader.PDFOfXAndYAndZ[0, 0, 2], 0);
            Assert.AreEqual(loader.PDFOfXAndYAndZ[1, 0, 2], 0);
            Assert.AreEqual(loader.PDFOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(loader.PDFOfXAndYAndZ[3, 0, 2], 0);
            // check that CDF is only !=0 in region of infinite cylinder and increasing with z,y,x order
            Assert.AreEqual(loader.CDFOfXAndYAndZ[0, 0, 0], 0);
            Assert.AreEqual(loader.CDFOfXAndYAndZ[0, 0, 1], 0);
            Assert.AreEqual(loader.CDFOfXAndYAndZ[0, 0, 2], 0);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[1, 0, 0] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[1, 0, 1] - 0.375) < 1e-6);
            Assert.AreEqual(loader.CDFOfXAndYAndZ[1, 0, 2], 0);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[2, 0, 0] - 0.666666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[2, 0, 1] - 1) < 1e-6);
            Assert.AreEqual(loader.CDFOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(loader.CDFOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(loader.CDFOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(loader.MapOfXAndYAndZ[3, 0, 2], 0);
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
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein, 0);
            tissue.Regions[3] = _loaderCDF.FluorescentTissueRegion;
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
        public void verify_that_GetFinalPositionAndWeight_samples_from_Uniform_sampling_correctly()
        {
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein, 0);
            tissue.Regions[3] = _loaderUnif.FluorescentTissueRegion;
            var xyzNorm = _loaderUnif.X.Delta * _loaderUnif.Y.Delta * _loaderUnif.Z.Delta;
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
                                       _loaderUnif.AOfXAndYAndZ[ix, 0, iz] * xyzNorm) < 1e-6);
            }
        }

    }
}