﻿using System;
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
        /// set up A(x,y,z) detectors to test that AOfXAndYAndZLoader and
        /// initialized all arrays correctly.
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
    }
}