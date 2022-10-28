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
using Vts.MonteCarlo.PhaseFunctions;
using Vts.MonteCarlo.Rng;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Fluorescence Emission Sources: FluorescenceEmissionAOfRhoAndZSource
    /// </summary>
    [TestFixture]
    public class FluorescenceEmissionAOfRhoAndZSourceTests
    {
        private AOfRhoAndZDetector _aOfRhoAndZDetector;
        private FluorescenceEmissionAOfRhoAndZSource _fluorEmissionAOfRhoAndZSourceCdf,
            _fluorEmissionAOfRhoAndZSourceUnif, _fluorEmissionAOfRhoAndZSourceOther;
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
        /// set up A(x,y,z) and A(rho,z) detectors to test that AOfXAndYAndZLoader and
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

            _rhozLoaderCdf.InitializeFluorescentRegionArrays();

            // following setup is used to test FluorescenceEmissionSource other sampling method
            // to test switch default exception by setting enum SourcePositionSamplingType outside range
            _fluorEmissionAOfRhoAndZSourceOther = new FluorescenceEmissionAOfRhoAndZSource(
                "sourcetest", "inputAOfRhoAndZ.txt", 3, (SourcePositionSamplingType)3);

        }
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input()
        {
            // check default constructor
            var si = new FluorescenceEmissionAOfRhoAndZSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new FluorescenceEmissionAOfRhoAndZSourceInput(
                        "sourcetest", 
                        "inputAOfRhoAndZ.txt", 
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
        public void verify_that_GetFinalPositionAndWeight_samples_from_CDFOfRhoAndZ_correctly()
        {
            var countArray = new int[_aOfRhoAndZDetector.Rho.Count - 1,
                _aOfRhoAndZDetector.Z.Count - 1];
            var rng = new MathNet.Numerics.Random.MersenneTwister(0);
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            var regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            regionPhaseFunctions.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunction(0.8, rng));
            var tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                regionPhaseFunctions, 0);
            tissue.Regions[1] = _rhozLoaderCdf.FluorescentTissueRegion;
            for (var i = 0; i < 100; i++)
            {
                var photon = _fluorEmissionAOfRhoAndZSourceCdf.GetNextPhoton(tissue);
                // verify that photons start within range of midpoints of voxels in bounding cylinder
                var rho = Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X + 
                    photon.DP.Position.Y * photon.DP.Position.Y);
                Assert.IsTrue(rho <= 3.5);
                Assert.IsTrue((photon.DP.Position.Z >= 0.5) && (photon.DP.Position.Z <= 1.5));
                Assert.IsTrue(Math.Abs(photon.DP.Weight - 1.0) < 1e-6);
                var irho = (int)(Math.Floor(rho));
                var iz = (int)(Math.Floor(photon.DP.Position.Z));
                countArray[irho, iz] += 1;
            }
            // check that countArray is > 1 in region of AOfRhoAndZ
            Assert.AreEqual(2, countArray[0, 0]);
            Assert.AreEqual(4, countArray[0, 1]);
            Assert.AreEqual(11, countArray[1, 0]);
            Assert.AreEqual(7, countArray[1, 1]);
            Assert.AreEqual(12, countArray[2, 0]);
            Assert.AreEqual(19, countArray[2, 1]);
            Assert.AreEqual(22, countArray[3, 0]);
            Assert.AreEqual(23, countArray[3, 1]);
        }
        /// <summary>
        /// test switch statement in GetFinalPositionAndWeight method for setting other
        /// than Uniform or CDF and verify exception is thrown
        /// </summary>
        [Test]
        public void Verify_that_samplingMethod_not_set_to_Uniform_or_CDF_throws_exception()
        {
            var rng = new SerializableMersenneTwister();
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            var regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            regionPhaseFunctions.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunction(0.8, rng));
            ITissue tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                regionPhaseFunctions, 0);
            Assert.Throws<ArgumentOutOfRangeException>(
                () => _fluorEmissionAOfRhoAndZSourceOther.GetNextPhoton(tissue));
        }
    }
}