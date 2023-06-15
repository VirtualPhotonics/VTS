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
    /// Unit tests for Fluorescence Emission Sources: FluorescenceEmissionAOfXAndYAndZSource
    /// </summary>
    [TestFixture]
    public class FluorescenceEmissionAOfXAndYAndZSourceTests
    {
        private AOfXAndYAndZDetector _aOfXAndYAndZDetector;
        private FluorescenceEmissionAOfXAndYAndZSource _fluorEmissionAOfXAndYAndZSourceCdf,
            _fluorEmissionAOfXAndYAndZSourceUnif, _fluorEmissionAOfXAndYAndZSourceOther;
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

            // AOfXAndYAndZ in resource is defined by:
            // single layer tissue with embedded infinite cylinder center=(0,0,1) radius=1
            // detector x=[-2 2] 4 bins, y=[-10 10] 4 bins, z=[0 3] 3 bins
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
            _xyzLoaderUnif.InitializeFluorescentRegionArrays();

            // following setup is used to test FluorescenceEmissionSource other sampling method
            // to test switch default exception by setting enum SourcePositionSamplingType outside range
            _fluorEmissionAOfXAndYAndZSourceOther = new FluorescenceEmissionAOfXAndYAndZSource(
                "sourcetest", "inputAOfXAndYAndZ.txt", 3, (SourcePositionSamplingType)3);

        }
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input()
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
            var rng = new MathNet.Numerics.Random.MersenneTwister(0);
            // note need to omit "edge" bins from fluorescence generation
            var countArray = new int[_aOfXAndYAndZDetector.X.Count - 3,
                _aOfXAndYAndZDetector.Y.Count - 3, _aOfXAndYAndZDetector.Z.Count - 2];
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            var regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            regionPhaseFunctions.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunction(0.8, rng));
            var tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                regionPhaseFunctions, 0);
            tissue.Regions[3] = _xyzLoaderCdf.FluorescentTissueRegion;
            for (int i = 0; i < 100; i++)
            {
                var photon = _fluorEmissionAOfXAndYAndZSourceCdf.GetNextPhoton(tissue);
                // verify that photons start within range of midpoints of voxels in infinite cylinder
                Assert.IsTrue(photon.DP.Position.X >= -0.5 && photon.DP.Position.X <= 0.5);
                Assert.IsTrue(photon.DP.Position.Y >= -7.5 && photon.DP.Position.X <= 0.75);
                Assert.IsTrue(photon.DP.Position.Z >= 0.5 && photon.DP.Position.Z <= 1.5);
                Assert.IsTrue(Math.Abs(photon.DP.Weight - _xyzLoaderCdf.TotalAbsorbedEnergy) < 1e-6);
                var ix = (int)(photon.DP.Position.X + 0.5) + 1;
                var iz =(int)Math.Floor(photon.DP.Position.Z);
                countArray[ix, 0, iz] += 1;
            }
            // check that countArray is only 1 in region of infinite cylinder
            Assert.AreEqual(0,countArray[0, 0, 0]);
            Assert.AreEqual(0,countArray[0, 0, 1]);
            Assert.AreEqual(0,countArray[0, 1, 0]);
            Assert.AreEqual(0,countArray[0, 1, 1]);
            Assert.AreEqual(51,countArray[1, 0, 0]);
            Assert.AreEqual(49,countArray[1, 0, 1]);
            Assert.AreEqual(0, countArray[1, 1, 0]);
            Assert.AreEqual(0, countArray[1, 1, 1]);
        }
        /// <summary>
        /// Test to make sure GetFinalPosition produces correct distribution of sources and weights
        /// for CDF sampling
        /// </summary>
        [Test]
        public void Verify_that_GetFinalPositionAndWeight_samples_from_UnifOfXAndYAndZ_sampling_correctly()
        {
            var rng = new MathNet.Numerics.Random.MersenneTwister(0);
            ITissueInput tissueInput = new SingleInfiniteCylinderTissueInput();
            var regionPhaseFunctions = new Dictionary<string, IPhaseFunction>();
            regionPhaseFunctions.Add("HenyeyGreensteinKey1", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey2", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey3", new HenyeyGreensteinPhaseFunction(0.8, rng));
            regionPhaseFunctions.Add("HenyeyGreensteinKey4", new HenyeyGreensteinPhaseFunction(0.8, rng));
            var tissue = tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                regionPhaseFunctions, 0);
            tissue.Regions[3] = _xyzLoaderUnif.FluorescentTissueRegion;
            var xyzNorm = _xyzLoaderUnif.X.Delta * _xyzLoaderUnif.Y.Delta * _xyzLoaderUnif.Z.Delta;
            for (var i = 0; i < 100; i++)
            {
                var photon = _fluorEmissionAOfXAndYAndZSourceUnif.GetNextPhoton(tissue);
                // verify that photons start within range of midpoints of voxels in infinite cylinder
                Assert.IsTrue(photon.DP.Position.X >= -0.5 && photon.DP.Position.X <= 0.5);
                Assert.IsTrue(photon.DP.Position.Y >= -7.5 && photon.DP.Position.X <= 7.5);
                Assert.IsTrue(photon.DP.Position.Z >= 0.5 && photon.DP.Position.Z <= 2.5);
                // verify sampling is proceeding in coded sequence
                // detector x=[-2 2] 4 bins, y=[-10 10] 4 bins, z=[0 3] 3 bins
                var ix = (int)((photon.DP.Position.X + 1.0)/_xyzLoaderUnif.X.Delta) + 1;
                var iy = (int)((photon.DP.Position.Y + 10.0)/_xyzLoaderUnif.Y.Delta);
                var iz = (int)(Math.Floor(photon.DP.Position.Z)/_xyzLoaderUnif.Z.Delta);
                // verify weight at location is equal to AOfXAndYAndZ 
                Assert.IsTrue(Math.Abs(photon.DP.Weight - 
                                       _xyzLoaderUnif.AOfXAndYAndZ[ix, iy, iz] * xyzNorm) < 1e-6);
            }
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
            Assert.Throws< ArgumentOutOfRangeException>(
                () => _fluorEmissionAOfXAndYAndZSourceOther.GetNextPhoton(tissue));
        }
    }
}