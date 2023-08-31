using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for MultiLayerWithSurfaceFiberTissue
    /// </summary>
    [TestFixture]
    public class MultiLayerWithSurfaceFiberTissueTests
    {
        private MultiLayerWithSurfaceFiberTissue _tissue;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _tissue = new MultiLayerWithSurfaceFiberTissue(new SurfaceFiberTissueRegion(
                new Position(0, 0, 0), 1.0, new OpticalProperties(0, 1, 0.8, 1.4)),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                });
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            var index = _tissue.GetRegionIndex(new Position(2, 0, 0)); // outside surface fiber
            Assert.AreEqual(1, index);
            index = _tissue.GetRegionIndex(new Position(0.5, 0, 0)); // inside surface fiber
            Assert.AreEqual(3, index);
            index = _tissue.GetRegionIndex(new Position(0, 0, 1.0)); // below surface fiber
            Assert.AreEqual(1, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex  return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            var photon = new Photon( // in fiber pointed into it within NA
                new Position(0, 0, 0.0),
                new Direction(0.0, 0, -1.0),
                1,
                _tissue,
                1,
                new Random());
            var index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(3, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex for tissueWithEllipsoid return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_correct_when_photon_bottom_slab()
        {
            // on bottom of slab pointed out
            var photon = new Photon( // have to reinitialize photon so that _onBoundary is set to false
                new Position(0, 0, 100.0),
                new Direction(0.0, 0, 1.0),
                1,
                _tissue,
                1,
                new Random());
            var index = _tissue.GetNeighborRegionIndex(photon);
            Assert.AreEqual(2, index);
        }


        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon( // on top of ellipsoid pointed into it
                new Position(0, 0, 1.0),
                new Direction(0.0, 0, 1.0),
                1,
                _tissue,
                1,
                new Random());
            var cosTheta = _tissue.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1,cosTheta);
        }
        /// <summary>
        /// verify exception thrown when GetNeighborIndex is called and the photon
        /// Direction Uz=0 (parallel with any layer)
        /// </summary>
        [Test]
        public void Verify_exception_when_calling_GetNeighborIndex_with_Uz_equal_0()
        {
            var photon = new Photon(
                new Position(0, 0, 0),
                new Direction(0, 0, 0), // specify Uz=0.0
                1,
                _tissue,
                1,
                null);
            Assert.Throws<ArgumentException>(() => _tissue.GetNeighborRegionIndex(photon));
        }

    }
}
