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
    /// Unit tests for Fluorescence Emission Sources: AOfRhoAndZLoader
    /// </summary>
    [TestFixture]
    public class AOfRhoAndZLoaderTests
    {
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
        /// set up A(rho,z) detectors to test that 
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

            // AOfRhoAndZ in resource is defined by:
            // single layer tissue with embedded infinite cylinder center=(0,0,1) radius=4
            // detector rho=[0 4] 4 bins, z=[0 2] 2 bins
            _aOfRhoAndZDetector = (dynamic)DetectorIO.ReadDetectorFromFileInResources(
                "AOfRhoAndZ", "Resources/sourcetest/", assemblyName);
            // overwrite statistical data in Mean with deterministic values to test
            int count = 1;
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
            _rhozLoaderUnif.InitializeFluorescentRegionArrays();
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

    }
}