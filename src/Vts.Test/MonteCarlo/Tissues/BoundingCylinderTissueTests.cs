using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for BoundingCylinderTissue and BoundingCylinderTissue
    /// </summary>
    [TestFixture]
    public class BoundingCylinderTissueTests
    {
        private BoundedTissue _oneLayerTissueBoundedByCylinder, _twoLayerTissueBoundedByCylinder;
        /// <summary>
        /// Validate general constructor of Tissue for a one layer and two layer tissue cylinder
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _oneLayerTissueBoundedByCylinder = new BoundedTissue(new CaplessCylinderTissueRegion(
                new Position(0, 0, 50), 1.0, 100.0, new OpticalProperties()),
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
            _twoLayerTissueBoundedByCylinder = new BoundedTissue(new CaplessCylinderTissueRegion(
                    new Position(0, 0, 50), 1.0, 100.0, new OpticalProperties()),
                new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 1.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(1.0, 100.0),
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
            var index = _oneLayerTissueBoundedByCylinder.GetRegionIndex(new Position(10, 0, 0)); // outside cylinder
            Assert.AreEqual(3, index);
            index = _oneLayerTissueBoundedByCylinder.GetRegionIndex(new Position(0, 0, 2.5)); // inside cylinder
            Assert.AreEqual(1, index);
            index = _oneLayerTissueBoundedByCylinder.GetRegionIndex(new Position(0, 0, 0)); // on cylinder is considered in
            Assert.AreEqual(1, index);
            // two layer results
            index = _twoLayerTissueBoundedByCylinder.GetRegionIndex(new Position(10, 0, 0)); // outside cylinder
            Assert.AreEqual(4, index);
            index = _twoLayerTissueBoundedByCylinder.GetRegionIndex(new Position(0, 0, 2.5)); // inside cylinder
            Assert.AreEqual(2, index);
            index = _twoLayerTissueBoundedByCylinder.GetRegionIndex(new Position(0, 0, 0)); // on cylinder is considered in
            Assert.AreEqual(1, index);
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            var photon = new Photon( // on side of cylinder pointed into it
                new Position(-1, 0, 1),
                new Direction(1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByCylinder,
                3,
                new Random());
            var index = _oneLayerTissueBoundedByCylinder.GetNeighborRegionIndex(photon);
            Assert.AreEqual(1, index);
            photon = new Photon( // on side of cylinder pointed out of it
                new Position(-1, 0, 1),
                new Direction(-1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByCylinder,
                1,
                new Random());
            index = _oneLayerTissueBoundedByCylinder.GetNeighborRegionIndex(photon);
            Assert.AreEqual(3, index);
            // two layer results
            photon = new Photon( // on side of cylinder pointed into LAYER 1
                new Position(-1, 0, 0.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByCylinder,
                4,
                new Random());
            index = _twoLayerTissueBoundedByCylinder.GetNeighborRegionIndex(photon);
            Assert.AreEqual(1, index);
            photon = new Photon( // on side of cylinder in LAYER 1 pointed out of it
                new Position(-1, 0, 0.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByCylinder.GetNeighborRegionIndex(photon);
            Assert.AreEqual(4, index);
            photon = new Photon( // on side of cylinder pointed into LAYER 2
                new Position(-1, 0, 1.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByCylinder,
                4,
                new Random());
            index = _twoLayerTissueBoundedByCylinder.GetNeighborRegionIndex(photon);
            Assert.AreEqual(2, index);
            photon = new Photon( // on side of cylinder in LAYER 2 pointed out of it
                new Position(-1, 0, 1.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByCylinder.GetNeighborRegionIndex(photon);
            Assert.AreEqual(4, index);
        }

        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon( // on top of tissue pointed into it
                new Position(0.0, 0.0, 0.0),
                new Direction(0.0, 0.0, 1.0),
                1,
                _twoLayerTissueBoundedByCylinder,
                1,
                new Random());
            var cosTheta = _twoLayerTissueBoundedByCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.AreEqual(1, cosTheta);
            // put on side of cylinder pointing in
            photon.DP.Position = new Position(1.0, 1.0, 5.0);
            photon.DP.Direction = new Direction(1.0, 0.0, 0.0);
            cosTheta = _twoLayerTissueBoundedByCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.IsTrue(Math.Abs(cosTheta - 1/Math.Sqrt(2)) < 1e-6);
        }

    }
}

