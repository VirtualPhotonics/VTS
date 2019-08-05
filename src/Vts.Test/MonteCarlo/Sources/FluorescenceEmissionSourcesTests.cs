using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
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
        private FluorescenceEmissionAOfXAndYAndZSource _fluorescenceEmissionAOfXAndYAndZSource;
        private AOfXAndYAndZLoader _loader;

        /// <summary>
        /// set up A(x,y,z) detector to test source
        /// </summary>
        [OneTimeSetUp]
        public void setup()
        {
            // following setup is used to test AOfXAndYAndZLoader
            _aOfXAndYAndZDetector = new AOfXAndYAndZDetector();
            _aOfXAndYAndZDetector.X = new DoubleRange(-2, 2, 5);
            _aOfXAndYAndZDetector.Y = new DoubleRange(-10, 10, 2);
            _aOfXAndYAndZDetector.Z = new DoubleRange(0, 3, 4);
            _aOfXAndYAndZDetector.Mean = new double[_aOfXAndYAndZDetector.X.Count - 1,
                _aOfXAndYAndZDetector.Y.Count - 1, _aOfXAndYAndZDetector.Z.Count - 1];
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
            // following setup is used to test FluorescenceEmissionSource
            _fluorescenceEmissionAOfXAndYAndZSource = new FluorescenceEmissionAOfXAndYAndZSource(
                "", "", 3);
            // empty infileFolder will initialize AOfXAndYAndZLoader with no AOfXAndYAndZ read
            _fluorescenceEmissionAOfXAndYAndZSource.Loader = new AOfXAndYAndZLoader(
                "", "", 3);
            _loader = _fluorescenceEmissionAOfXAndYAndZSource.Loader;
            _loader.X = _aOfXAndYAndZDetector.X;
            _loader.Y = _aOfXAndYAndZDetector.Y;
            _loader.Z = _aOfXAndYAndZDetector.Z;
            _loader.AOfXAndYAndZ = _aOfXAndYAndZDetector.Mean;
            _loader.FluorescentTissueRegion = new InfiniteCylinderTissueRegion()
            {
                Center = new Position(0, 0, 1),
                Radius = 1,
                RegionOP = new OpticalProperties(0.01, 1, 0.8, 1.4)
            };
            // invoke method and check if arrays set up properly
            _loader.InitializeFluorescentRegionArrays();
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
            // check that arrays are of correct dimension
            Assert.AreEqual(_loader.MapOfXAndYAndZ.GetLength(0), _loader.X.Count - 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ.GetLength(1), _loader.Y.Count - 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ.GetLength(2), _loader.Z.Count - 1);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ.GetLength(0), _loader.X.Count - 1);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ.GetLength(1), _loader.Y.Count - 1);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ.GetLength(2), _loader.Z.Count - 1);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ.GetLength(0), _loader.X.Count - 1);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ.GetLength(1), _loader.Y.Count - 1);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ.GetLength(2), _loader.Z.Count - 1);
            // check that Map is only 1 in region of infinite cylinder
            Assert.AreEqual(_loader.MapOfXAndYAndZ[0, 0, 0], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[1, 0, 0], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[2, 0, 0], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[0, 0, 1], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[1, 0, 1], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[2, 0, 1], 1);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[0, 0, 2], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[1, 0, 2], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 2], 0);
            // check that PDF is correct
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[0, 0, 0], 0);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[1, 0, 0] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[2, 0, 0] - 0.291666) < 1e-6);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[0, 0, 1], 0);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[1, 0, 1] - 0.208333) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.PDFOfXAndYAndZ[2, 0, 1] - 0.333333) < 1e-6);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[0, 0, 2], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[1, 0, 2], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(_loader.PDFOfXAndYAndZ[3, 0, 2], 0);
            // check that CDF is only !=0 in region of infinite cylinder and increasing with z,y,x order
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[0, 0, 0], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[0, 0, 1], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[0, 0, 2], 0);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[1, 0, 0] - 0.166666) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[1, 0, 1] - 0.375) < 1e-6);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[1, 0, 2], 0);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[2, 0, 0] - 0.666666) < 1e-6);
            Assert.IsTrue(Math.Abs(_loader.CDFOfXAndYAndZ[2, 0, 1] - 1) < 1e-6);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[2, 0, 2], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[3, 0, 0], 0);
            Assert.AreEqual(_loader.CDFOfXAndYAndZ[3, 0, 1], 0);
            Assert.AreEqual(_loader.MapOfXAndYAndZ[3, 0, 2], 0);
        }
        /// <summary>
        /// Test to make sure GetFinalPosition produces correct distribution of sources
        /// </summary>
        [Test]
        public void verify_that_GetFinalPosition_samples_from_CDFOfXAndYAndZ_correctly()
        {
            var countArray = new int[_aOfXAndYAndZDetector.X.Count - 1,
                _aOfXAndYAndZDetector.Y.Count - 1, _aOfXAndYAndZDetector.Z.Count - 1];
            ITissueInput _tissueInput = new SingleInfiniteCylinderTissueInput();
            ITissue _tissue = _tissueInput.CreateTissue(AbsorptionWeightingType.Discrete,
                PhaseFunctionType.HenyeyGreenstein, 0);
            _tissue.Regions[3] = _loader.FluorescentTissueRegion;
            for (int i = 0; i < 100; i++)
            {
                var photon = _fluorescenceEmissionAOfXAndYAndZSource.GetNextPhoton(_tissue);
                // verify that photons start within range of midpoints of voxels
                Assert.IsTrue((photon.DP.Position.X >= -0.5) && (photon.DP.Position.X <= 0.5));
                Assert.AreEqual(photon.DP.Position.Y, 0);
                Assert.IsTrue((photon.DP.Position.Z >= 0.5) && (photon.DP.Position.Z <= 1.5));
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

    }
}