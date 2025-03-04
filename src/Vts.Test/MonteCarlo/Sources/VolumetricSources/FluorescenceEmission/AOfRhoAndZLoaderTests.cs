using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Sources;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Fluorescence Emission Sources: AOfRhoAndZLoader
    /// </summary>
    [TestFixture]
    public class AOfRhoAndZLoaderTests
    {
        private AOfRhoAndZDetector _aOfRhoAndZDetector;
        private FluorescenceEmissionAOfRhoAndZSource _fluorEmissionAOfRhoAndZSourceCdf,
            _fluorEmissionAOfRhoAndZSourceUnif;
        private AOfRhoAndZLoader _rhozLoaderCdf, _rhozLoaderUnif;

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> _listOfTestGeneratedFolders = new List<string>()
        {
            "sourcetest",
        };

        readonly List<string> _listOfTestGeneratedFiles = new List<string>()
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
        public void Clear_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
            foreach (var folder in _listOfTestGeneratedFolders)
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
        public void Setup()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;

            // AOfRhoAndZ in resource is defined by:
            // single layer tissue with embedded infinite cylinder center=(0,0,1) radius=4
            // detector rho=[0 4] 4 bins, z=[0 2] 2 bins
            _aOfRhoAndZDetector = (dynamic)DetectorIO.ReadDetectorFromFileInResources(
                "AOfRhoAndZ", "Resources/sourcetest/", assemblyName);
            // overwrite statistical data in Mean with deterministic values to test
            var count = 1;
            for (var i = 0; i < _aOfRhoAndZDetector.Rho.Count - 1; i++)
            {
                for (var k = 0; k < _aOfRhoAndZDetector.Z.Count - 1; k++)
                {
                    _aOfRhoAndZDetector.Mean[i, k] = count; // make all nonzero and unique
                    ++count;
                }                
            }
            DetectorIO.WriteDetectorToFile(_aOfRhoAndZDetector, "sourcetest");

            FileIO.CopyFileFromResources(
                "Resources/sourcetest/inputAOfRhoAndZ.txt", "sourcetest/inputAOfRhoAndZ.txt", assemblyName);

            // following setup is used to test FluorescenceEmissionSource CDF sampling method
            _fluorEmissionAOfRhoAndZSourceCdf = new FluorescenceEmissionAOfRhoAndZSource(
                "sourcetest", "inputAOfRhoAndZ.txt", 3, SourcePositionSamplingType.CDF);
            // empty infileFolder will initialize AOfRhoAndZLoader with no AOfRhoAndZ read
            _fluorEmissionAOfRhoAndZSourceCdf.Loader = new AOfRhoAndZLoader(
                "sourcetest", "inputAOfRhoAndZ.txt", 3);
            _rhozLoaderCdf = _fluorEmissionAOfRhoAndZSourceCdf.Loader;
            _rhozLoaderCdf.InitializeFluorescentRegionArrays();

            // following setup is used to test FluorescenceEmissionSource Unif sampling method
            _fluorEmissionAOfRhoAndZSourceUnif = new FluorescenceEmissionAOfRhoAndZSource(
                "sourcetest", "inputAOfRhoAndZ.txt", 3, SourcePositionSamplingType.Uniform);
            // empty infileFolder will initialize AOfRhoAndZLoader with no AOfRhoAndZ read
            _fluorEmissionAOfRhoAndZSourceUnif.Loader = new AOfRhoAndZLoader(
                "sourcetest", "inputAOfRhoAndZ.txt", 3);
            _rhozLoaderUnif = _fluorEmissionAOfRhoAndZSourceCdf.Loader;
            _rhozLoaderUnif.InitializeFluorescentRegionArrays();
        }

        /// <summary>
        /// Validate AOfRhoAndZLoader method InitializeFluorscentRegionArrays creates arrays
        /// MapOfRhoAndZ = has 1's in voxels of fluorescent tissue region
        /// PDFOfRhoAndZ = AOfRhoAndZ in locations where MapOfRhoAndZ has 1, then normalized to PDF by total
        /// CDPOfRhoAndZ = CDF of PDFOfRhoAndZ
        /// </summary>
        [Test]
        public void Validate_AOfRhoAndZLoader_method_InitializeFluorscentRegionArrays()
        {
            var loader = _rhozLoaderCdf;
            // check that arrays are of correct dimension (Count-2)
            // note that what gets loaded as fluorescent source has dimensions one less than absorbed energy map
            Assert.That(loader.Rho.Count - 2, Is.EqualTo(loader.MapOfRhoAndZ.GetLength(0)));
            Assert.That(loader.Z.Count - 2, Is.EqualTo(loader.MapOfRhoAndZ.GetLength(1)));
            Assert.That(loader.Rho.Count - 2, Is.EqualTo(loader.PDFOfRhoAndZ.GetLength(0)));
            Assert.That(loader.Z.Count - 2, Is.EqualTo(loader.PDFOfRhoAndZ.GetLength(1)));
            Assert.That(loader.Rho.Count - 2, Is.EqualTo(loader.CDFOfRhoAndZ.GetLength(0)));
            Assert.That(loader.Z.Count - 2, Is.EqualTo(loader.CDFOfRhoAndZ.GetLength(1)));
            // check that Map is 1 in region of AOfRhoAndZ
            Assert.That(loader.MapOfRhoAndZ[0, 0], Is.EqualTo(1));
            Assert.That(loader.MapOfRhoAndZ[1, 0], Is.EqualTo(1));
            Assert.That(loader.MapOfRhoAndZ[2, 0], Is.EqualTo(1));
            // check that PDF is correct
            Assert.That(Math.Abs(loader.PDFOfRhoAndZ[0, 0] - 0.111111) < 1e-6, Is.True);
            Assert.That(Math.Abs(loader.PDFOfRhoAndZ[1, 0] - 0.333333) < 1e-6, Is.True);
            Assert.That(Math.Abs(loader.PDFOfRhoAndZ[2, 0] - 0.555555) < 1e-6, Is.True);
            // check that CDF is only !=0 in region of infinite cylinder and increasing with z,y,x order
            Assert.That(Math.Abs(loader.CDFOfRhoAndZ[0, 0] - 0.111111) < 1e-6, Is.True);
            Assert.That(Math.Abs(loader.CDFOfRhoAndZ[1, 0] - 0.444444) < 1e-6, Is.True);
            Assert.That(Math.Abs(loader.CDFOfRhoAndZ[2, 0] - 1.0) < 1e-6, Is.True);
        }

    }
}