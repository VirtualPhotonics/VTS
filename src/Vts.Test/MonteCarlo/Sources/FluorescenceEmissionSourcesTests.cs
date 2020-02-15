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
using Vts.MonteCarlo.Helpers;

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
        private AOfXAndYAndZLoader _xyzLoaderCDF, _xyzLoaderUnif;

        private AOfRhoAndZDetector _aOfRhoAndZDetector;
        private FluorescenceEmissionAOfRhoAndZSource _fluorEmissionAOfRhoAndZSourceCDF,
            _fluorEmissionAOfRhoAndZSourceUnif;
        private AOfRhoAndZLoader _rhozLoaderCDF, _rhozLoaderUnif;

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
            "AOfRhoAndZ",
            "AOfRhoAndZ.txt"
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

            _xyzLoaderCDF.InitializeFluorescentRegionArrays();

            // AOfRhoAndZ in resource is defined by:
            // single layer tissue with embedded infinite cylinder center=(0,0,1) radius=4
            // detector rho=[0 4] 4 bins, z=[0 2] 2 bins
            _aOfRhoAndZDetector = (dynamic)DetectorIO.ReadDetectorFromFileInResources(
                "AOfRhoAndZ", "Resources/sourcetest/", assemblyName);
            // overwrite statistical data in Mean with deterministic values to test
            count = 1;
            for (int i = 0; i < _aOfRhoAndZDetector.Rho.Count - 1; i++)
            {
                for (int k = 0; k < _aOfRhoAndZDetector.Z.Count - 1; k++)
                {
                    _aOfRhoAndZDetector.Mean[i, k] = count; // make all nonzero and unique
                    ++count;
                }                
            }
            DetectorIO.WriteDetectorToFile(_aOfRhoAndZDetector, "sourcetest");

            FileIO.CopyFileFromResources(
                "Resources/sourcetest/inputAOfRhoAndZ.txt", "sourcetest/inputAOfRhoAndZ.txt", assemblyName);

            // following setup is used to test FluorescenceEmissionSource CDF sampling method
            _fluorEmissionAOfRhoAndZSourceCDF = new FluorescenceEmissionAOfRhoAndZSource(
                "sourcetest", "inputAOfRhoAndZ.txt", 3, SourcePositionSamplingType.CDF);
            // empty infileFolder will initialize AOfRhoAndZLoader with no AOfRhoAndZ read
            _fluorEmissionAOfRhoAndZSourceCDF.Loader = new AOfRhoAndZLoader(
                "sourcetest", "inputAOfRhoAndZ.txt", 3);
            _rhozLoaderCDF = _fluorEmissionAOfRhoAndZSourceCDF.Loader;
            _rhozLoaderCDF.InitializeFluorescentRegionArrays();

            // following setup is used to test FluorescenceEmissionSource Unif sampling method
            _fluorEmissionAOfRhoAndZSourceUnif = new FluorescenceEmissionAOfRhoAndZSource(
                "sourcetest", "inputAOfRhoAndZ.txt", 3, SourcePositionSamplingType.Uniform);
            // empty infileFolder will initialize AOfRhoAndZLoader with no AOfRhoAndZ read
            _fluorEmissionAOfRhoAndZSourceUnif.Loader = new AOfRhoAndZLoader(
                "sourcetest", "inputAOfRhoAndZ.txt", 3);
            _rhozLoaderUnif = _fluorEmissionAOfRhoAndZSourceCDF.Loader;

            _rhozLoaderCDF.InitializeFluorescentRegionArrays();
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
            var loader = _xyzLoaderCDF;
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
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein, 0);
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
        /// <summary>
        /// Validate AOfRhoAndZLoader method InitializeFluorscentRegionArrays creates arrays
        /// MapOfRhoAndZ = has 1's in voxels of fluorescent tissue region
        /// PDFOfRhoAndZ = AOfRhoAndZ in locations where MapOfRhoAndZ has 1, then normalized to PDF by total
        /// CDPOfRhoAndZ = CDF of PDFOfRhoAndZ
        /// </summary>
        [Test]
        public void validate_AOfRhoAndZLoader_method_InitializeFluorscentRegionArrays()
        {
            var loader = _rhozLoaderCDF;
            // check that arrays are of correct dimension
            Assert.AreEqual(loader.MapOfRhoAndZ.GetLength(0), loader.Rho.Count - 1);
            Assert.AreEqual(loader.MapOfRhoAndZ.GetLength(1), loader.Z.Count - 1);
            Assert.AreEqual(loader.PDFOfRhoAndZ.GetLength(0), loader.Rho.Count - 1);
            Assert.AreEqual(loader.PDFOfRhoAndZ.GetLength(1), loader.Z.Count - 1);
            Assert.AreEqual(loader.CDFOfRhoAndZ.GetLength(0), loader.Rho.Count - 1);
            Assert.AreEqual(loader.CDFOfRhoAndZ.GetLength(1), loader.Z.Count - 1);
            // check that Map is 1 in region of AOfRhoAndZ
            Assert.AreEqual(loader.MapOfRhoAndZ[0, 0], 1);
            Assert.AreEqual(loader.MapOfRhoAndZ[1, 0], 1);
            Assert.AreEqual(loader.MapOfRhoAndZ[2, 0], 1);
            Assert.AreEqual(loader.MapOfRhoAndZ[3, 0], 1);
            Assert.AreEqual(loader.MapOfRhoAndZ[0, 1], 1);
            Assert.AreEqual(loader.MapOfRhoAndZ[1, 1], 1);
            Assert.AreEqual(loader.MapOfRhoAndZ[2, 1], 1);
            Assert.AreEqual(loader.MapOfRhoAndZ[3, 1], 1);
            // check that PDF is correct
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[0, 0] - 0.027777) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[0, 1] - 0.055555) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[1, 0] - 0.083333) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[1, 1] - 0.111111) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[2, 0] - 0.138888) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[2, 1] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[3, 0] - 0.194444) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfRhoAndZ[3, 1] - 0.222222) < 1e-6);
            // check that CDF is only !=0 in region of infinite cylinder and increasing with z,y,x order
            Assert.IsTrue(Math.Abs(loader.CDFOfRhoAndZ[0, 0] - 0.027777) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfRhoAndZ[0, 1] - 0.083333) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfRhoAndZ[1, 0] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfRhoAndZ[1, 1] - 0.277777) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfRhoAndZ[2, 0] - 0.416666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfRhoAndZ[2, 1] - 0.583333) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfRhoAndZ[3, 0] - 0.777777) < 1e-6);
            Assert.AreEqual(loader.CDFOfRhoAndZ[3, 1], 1);
        }
        /// <summary>
        /// Test to make sure GetFinalPosition produces correct distribution of sources and weights
        /// for CDF sampling
        /// </summary>
        [Test]
        public void verify_that_GetFinalPositionAndWeight_samples_from_CDFOfRhoAndZ_correctly()
        {
            var countArray = new int[_aOfRhoAndZDetector.Rho.Count - 1,
                _aOfRhoAndZDetector.Z.Count - 1];
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein, 0);
            tissue.Regions[1] = _rhozLoaderCDF.FluorescentTissueRegion;
            for (int i = 0; i < 100; i++)
            {
                var photon = _fluorEmissionAOfRhoAndZSourceCDF.GetNextPhoton(tissue);
                // verify that photons start within range of midpoints of voxels in bounding cylinder
                var rho = Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X + 
                    photon.DP.Position.Y * photon.DP.Position.Y);
                Assert.IsTrue(rho <= 3.5);
                Assert.IsTrue((photon.DP.Position.Z >= 0.5) && (photon.DP.Position.Z <= 1.5));
                Assert.IsTrue(Math.Abs(photon.DP.Weight - 1.0) < 1e-6);
                int irho = (int)(Math.Floor(rho));
                int iz = (int)(Math.Floor(photon.DP.Position.Z));
                countArray[irho, iz] += 1;
            }
            // check that countArray is > 1 in region of AOfRhoAndZ
            Assert.AreEqual(countArray[0, 0], 2);
            Assert.AreEqual(countArray[0, 1], 4);
            Assert.AreEqual(countArray[1, 0], 11);
            Assert.AreEqual(countArray[1, 1], 7);
            Assert.AreEqual(countArray[2, 0], 12);
            Assert.AreEqual(countArray[2, 1], 19);
            Assert.AreEqual(countArray[3, 0], 22);
            Assert.AreEqual(countArray[3, 1], 23);
        }
        /// <summary>
        /// Test to make sure GetFinalPosition produces correct distribution of sources and weights
        /// for CDF sampling
        /// </summary>
        [Test]
        public void verify_that_GetFinalPositionAndWeight_samples_from_UnifOfRhoAndZ_sampling_correctly()
        {
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein, 0);
            tissue.Regions[1] = _xyzLoaderUnif.FluorescentTissueRegion;
            for (int i = 0; i < 100; i++)
            {
                var photon = _fluorEmissionAOfRhoAndZSourceUnif.GetNextPhoton(tissue);
                // verify that photons start within range of midpoints of voxels in infinite cylinder
                Assert.IsTrue(photon.DP.Position.X <= 3.5);
                Assert.AreEqual(photon.DP.Position.Y, 0);
                Assert.IsTrue((photon.DP.Position.Z >= 0.5) && (photon.DP.Position.Z <= 1.5));
                // verify sampling is proceeding in coded sequence
                // detector center=(0,0,1) radius=4, rho=[0 4] 4 bins, z=[0 2] 2 bins
                var ir = DetectorBinning.WhichBin(DetectorBinning.GetRho(
                    photon.DP.Position.X, photon.DP.Position.Y),
                    _rhozLoaderUnif.Rho.Count - 1, _rhozLoaderUnif.Rho.Delta, _rhozLoaderUnif.Rho.Start);
                var iz = DetectorBinning.WhichBin(photon.DP.Position.Z, _rhozLoaderUnif.Z.Count - 1, _rhozLoaderUnif.Z.Delta, _rhozLoaderUnif.Z.Start);
                var rhozNorm = (_rhozLoaderUnif.Rho.Start + (ir + 0.5) * _rhozLoaderUnif.Rho.Delta) * 2.0 * Math.PI * _rhozLoaderUnif.Rho.Delta * _rhozLoaderUnif.Z.Delta; 
                // verify weight at location is equal to AOfRhoAndZ note: setup with single y bin
                // expected: Map [ 1 1 1 1; 1 1 1 1] row major
                // expected: A   [ 1 3 5 7; 2 4 6 8] row major
                Assert.IsTrue(Math.Abs(photon.DP.Weight -
                                       _rhozLoaderUnif.AOfRhoAndZ[ir, iz] * rhozNorm) < 1e-6);
            }
        }
    }
}