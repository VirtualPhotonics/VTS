using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for BoundingVoxelTissue and BoundingVoxelTissue
    /// </summary>
    [TestFixture]
    public class BoundingVoxelTissueTests
    {
        private BoundedTissue _oneLayerTissueBoundedByVoxel, _twoLayerTissueBoundedByVoxel;
        /// <summary>
        /// Validate general constructor of Tissue for a one layer and two layer tissue voxel
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _oneLayerTissueBoundedByVoxel = new BoundedTissue(new CaplessVoxelTissueRegion(
                new DoubleRange(-1, 1, 2), // x range
                new DoubleRange(-1, 1, 2), // y range
                new DoubleRange(0, 100.0, 2),  // z range spans tissue
                new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
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
            _twoLayerTissueBoundedByVoxel = new BoundedTissue(new CaplessVoxelTissueRegion(
                new DoubleRange(-1, 1, 2), // x range
                new DoubleRange(-1, 1, 2), // y range
                new DoubleRange(0, 100.0, 2),  // z range spans tissue
                new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
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
            var index = _oneLayerTissueBoundedByVoxel.GetRegionIndex(new Position(10, 0, 0)); // outside voxel
            Assert.That(index, Is.EqualTo(3));
            index = _oneLayerTissueBoundedByVoxel.GetRegionIndex(new Position(0, 0, 2.5)); // inside voxel
            Assert.That(index, Is.EqualTo(1));
            index = _oneLayerTissueBoundedByVoxel.GetRegionIndex(new Position(0, 0, 0)); // on voxel is considered in
            Assert.That(index, Is.EqualTo(1));
            // two layer results
            index = _twoLayerTissueBoundedByVoxel.GetRegionIndex(new Position(10, 0, 0)); // outside voxel
            Assert.That(index, Is.EqualTo(4));
            index = _twoLayerTissueBoundedByVoxel.GetRegionIndex(new Position(0, 0, 2.5)); // inside voxel
            Assert.That(index, Is.EqualTo(2));
            index = _twoLayerTissueBoundedByVoxel.GetRegionIndex(new Position(0, 0, 0)); // on voxel is considered in
            Assert.That(index, Is.EqualTo(1));
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            var photon = new Photon( // on side of voxel pointed into it
                new Position(-1, 0, 1),
                new Direction(1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByVoxel,
                3,
                new Random());
            var index = _oneLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon); 
            Assert.That(index, Is.EqualTo(1));
            photon = new Photon( // on side of voxel pointed out of it
                new Position(-1, 0, 1),
                new Direction(-1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByVoxel,
                1,
                new Random());
            index = _oneLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(3));
            // two layer results
            photon = new Photon( // on side of voxel pointed into LAYER 1
                new Position(-1, 0, 0.5),  
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxel,
                4,
                new Random());
            index = _twoLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(1));
            photon = new Photon( // on side of voxel in LAYER 1 pointed out of it
                new Position(-1, 0, 0.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxel,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(4));
            photon = new Photon( // on side of voxel pointed into LAYER 2
                new Position(-1, 0, 1.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxel,
                4,
                new Random());
            index = _twoLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(2));
            photon = new Photon( // on side of voxel in LAYER 2 pointed out of it
                new Position(-1, 0, 1.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxel,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxel.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(4));
        }

        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon( // on top of tissue pointed into it
                new Position(0, 0, 0.0),
                new Direction(0.0, 0, 1.0),
                1,
                _twoLayerTissueBoundedByVoxel,
                1,
                new Random());
            var cosTheta = _twoLayerTissueBoundedByVoxel.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            // put on side of cylinder pointing in
            photon.DP.Position = new Position(-1.0, 0.0, 5.0);
            photon.DP.Direction = new Direction(1.0, 0.0, 0.0);
            cosTheta = _twoLayerTissueBoundedByVoxel.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
        }


    }
}
