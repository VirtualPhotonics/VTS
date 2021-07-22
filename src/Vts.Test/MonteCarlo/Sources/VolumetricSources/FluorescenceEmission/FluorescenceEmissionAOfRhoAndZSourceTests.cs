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
    /// Unit tests for Fluorescence Emission Sources: FluorescenceEmissionAOfRhoAndZSource
    /// </summary>
    [TestFixture]
    public class FluorescenceEmissionAOfRhoAndZSourceTests
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

            _rhozLoaderCDF.InitializeFluorescentRegionArrays();
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
    }
}