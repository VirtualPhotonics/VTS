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
    public class AOfXAndYAndZLoaderTests
    {
        private AOfXAndYAndZDetector _aOfXAndYAndZDetector;
        private FluorescenceEmissionAOfXAndYAndZSource _fluorEmissionAOfXAndYAndZSourceCdf,
            _fluorEmissionAOfXAndYAndZSourceUnif;
        private AOfXAndYAndZLoader _xyzLoaderCdf, _xyzLoaderUnif;

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
            "AOfXAndYAndZ",
            "AOfXAndYAndZ.txt",
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
        /// set up A(x,y,z) detectors to test that AOfXAndYAndZLoader and
        /// initialized all arrays correctly.
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

            // AOfXAndYAndZ in resource is defined by:
            // single layer tissue with embedded infinite cylinder center=(0,0,1) radius=1
            // detector x=[-2 2] 2 bins, y=[-10 10] 2 bin, z=[0 3] 3 bins
            _aOfXAndYAndZDetector = (dynamic)DetectorIO.ReadDetectorFromFileInResources(
                "AOfXAndYAndZ", "Resources/sourcetest/", assemblyName);
            // overwrite statistical data in Mean with deterministic values to test
            var count = 1;
            for (var i = 0; i < _aOfXAndYAndZDetector.X.Count - 1; i++)
            {
                for (var j = 0; j < _aOfXAndYAndZDetector.Y.Count - 1; j++)
                {
                    for (var k = 0; k < _aOfXAndYAndZDetector.Z.Count - 1; k++)
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
            _fluorEmissionAOfXAndYAndZSourceCdf = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3, SourcePositionSamplingType.CDF);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorEmissionAOfXAndYAndZSourceCdf.Loader = new AOfXAndYAndZLoader(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3);
            _xyzLoaderCdf = _fluorEmissionAOfXAndYAndZSourceCdf.Loader;
            _xyzLoaderCdf.InitializeFluorescentRegionArrays();
            
            // following setup is used to test FluorescenceEmissionSource Unif sampling method
            _fluorEmissionAOfXAndYAndZSourceUnif = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3, SourcePositionSamplingType.Uniform);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorEmissionAOfXAndYAndZSourceUnif.Loader = new AOfXAndYAndZLoader(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3);
            _xyzLoaderUnif = _fluorEmissionAOfXAndYAndZSourceCdf.Loader;
            _xyzLoaderCdf.InitializeFluorescentRegionArrays();
        }

        /// <summary>
        /// Validate AOfXAndYAndZLoader method InitializeFluorescentRegionArrays creates arrays
        /// MapOfXAndYAndZ = has 1's in voxels of fluorescent tissue region
        /// PDFOfXAndYAndZ = AOfXAndYAndZ in locations where MapOfXAndYAndZ has 1, then normalized to PDF by total
        /// CDPOfXAndYAndZ = CDF of PDFOfXAndYAndZ
        /// </summary>
        [Test]
        public void Validate_AOfXAndYAndZLoader_method_InitializeFluorescentRegionArrays()
        {
            var loader = _xyzLoaderCdf;
            // check that arrays are of correct dimension
            // note that what gets loaded as fluorescent source has dimensions one less than absorbed energy map
            Assert.AreEqual(loader.MapOfXAndYAndZ.GetLength(0), loader.X.Count - 3);
            Assert.AreEqual(loader.MapOfXAndYAndZ.GetLength(1), loader.Y.Count - 3);
            Assert.AreEqual(loader.MapOfXAndYAndZ.GetLength(2), loader.Z.Count - 2);
            Assert.AreEqual(loader.PDFOfXAndYAndZ.GetLength(0), loader.X.Count - 3);
            Assert.AreEqual(loader.PDFOfXAndYAndZ.GetLength(1), loader.Y.Count - 3);
            Assert.AreEqual(loader.PDFOfXAndYAndZ.GetLength(2), loader.Z.Count - 2);
            Assert.AreEqual(loader.CDFOfXAndYAndZ.GetLength(0), loader.X.Count - 3);
            Assert.AreEqual(loader.CDFOfXAndYAndZ.GetLength(1), loader.Y.Count - 3);
            Assert.AreEqual(loader.CDFOfXAndYAndZ.GetLength(2), loader.Z.Count - 2);
            // check that Map is only 1 in region of infinite cylinder
            Assert.AreEqual(0, loader.MapOfXAndYAndZ[0, 0, 0]);
            Assert.AreEqual(0, loader.MapOfXAndYAndZ[0, 0, 1]);
            Assert.AreEqual(0, loader.MapOfXAndYAndZ[0, 1, 0]);
            Assert.AreEqual(0, loader.MapOfXAndYAndZ[0, 1, 1]);
            Assert.AreEqual(1, loader.MapOfXAndYAndZ[1, 0, 0]);
            Assert.AreEqual(1, loader.MapOfXAndYAndZ[1, 0, 1]);
            Assert.AreEqual(1, loader.MapOfXAndYAndZ[1, 1, 0]);
            Assert.AreEqual(1, loader.MapOfXAndYAndZ[1, 1, 1]);
            // check that PDF is correct
            Assert.AreEqual(0, loader.PDFOfXAndYAndZ[0, 0, 0]);
            Assert.AreEqual(0, loader.PDFOfXAndYAndZ[0, 0, 1]);
            Assert.AreEqual(0, loader.PDFOfXAndYAndZ[0, 1, 0]);
            Assert.AreEqual(0, loader.PDFOfXAndYAndZ[0, 1, 1]);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[1, 0, 0] - 0.216666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[1, 0, 1] - 0.233333) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[1, 1, 0] - 0.266666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.PDFOfXAndYAndZ[1, 1, 1] - 0.283333) < 1e-6);
            // check that CDF is only !=0 in region of infinite cylinder and increasing with z,y,x order
            Assert.AreEqual(0, loader.CDFOfXAndYAndZ[0, 0, 0]);
            Assert.AreEqual(0, loader.CDFOfXAndYAndZ[0, 0, 1]);
            Assert.AreEqual(0, loader.CDFOfXAndYAndZ[0, 1, 0]);
            Assert.AreEqual(0, loader.CDFOfXAndYAndZ[0, 1, 1]);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[1, 0, 0] - 0.216666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[1, 0, 1] - 0.45) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[1, 1, 0] - 0.716666) < 1e-6);
            Assert.IsTrue(Math.Abs(loader.CDFOfXAndYAndZ[1, 1, 1] - 1.0) < 1e-6);
        }
    }
}