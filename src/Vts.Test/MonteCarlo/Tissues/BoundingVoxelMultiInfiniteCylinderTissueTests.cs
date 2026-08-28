using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for BoundingVoxelMultiInfiniteCylinderTissue 
    /// </summary>
    [TestFixture]
    public class BoundingVoxelMultiInfiniteCylinderTissueTests
    {
        private BoundedMultiInclusionTissue _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder, 
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder;
        /// <summary>
        /// Validate general constructor of Tissue for a one layer and two layer tissue voxel
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder = 
                new BoundedMultiInclusionTissue(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-2, 2, 2), // x range
                        new DoubleRange(-2, 2, 2), // y range
                        new DoubleRange(0, 10.0, 2),  // z range spans tissue
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)), 
                    [
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1.5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                        ),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                            ],
                    [
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]);
            _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder = 
                new BoundedMultiInclusionTissue(
                    new CaplessVoxelTissueRegion(
                        new DoubleRange(-2, 2, 2), // x range
                        new DoubleRange(-2, 2, 2), // y range
                        new DoubleRange(0, 100.0, 2),  // z range spans tissue
                        new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                    [
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 1.5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4)
                        ),
                        new InfiniteCylinderTissueRegion(
                            new Position(0, 0, 5),
                            1.0,
                            new OpticalProperties(0.05, 1.0, 0.8, 1.4))
                    ],
                [
                    new LayerTissueRegion(
                        new DoubleRange(double.NegativeInfinity, 0.0),
                        new OpticalProperties( 0.0, 1e-10, 1.0, 1.0)),
                    new LayerTissueRegion(
                        new DoubleRange(0.0, 3.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(3.0, 100.0),
                        new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                    new LayerTissueRegion(
                        new DoubleRange(100.0, double.PositiveInfinity),
                        new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                ]);
        }

        /// <summary>
        /// Validate method GetRegionIndex return correct Boolean.
        /// Order of tissue region indices: layers, bounding region, inclusions.
        /// </summary>
        [Test]
        public void Verify_GetRegionIndex_method_returns_correct_result()
        {
            // one layer results indices: air(0)-tissue(1)-air(2)-voxel(3)-top cylinder(4)-bot cylinder(5)
            var index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 1.5)); // 1st layer 1st cylinder
            Assert.That(index, Is.EqualTo(4));
            index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 5)); // 1st layer 2nd cylinder
            Assert.That(index, Is.EqualTo(5));
            index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 0)); // on voxel considered in
            Assert.That(index, Is.EqualTo(1));
            // two layer results indices: air(0)-top layer(1)-bot layer(2)-air(3)-voxel(4)-top cylinder(5)-bot cylinder(6)
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 1.5)); // 1st layer cylinder
            Assert.That(index, Is.EqualTo(5));
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 5)); // 2nd layer cylinder
            Assert.That(index, Is.EqualTo(6));
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(10, 0, 0)); // outside voxel
            Assert.That(index, Is.EqualTo(4));
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 2.5)); // inside voxel top layer 1st cylinder
            Assert.That(index, Is.EqualTo(5));
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetRegionIndex(new Position(0, 0, 0)); // on voxel is considered in
            Assert.That(index, Is.EqualTo(1));
        }

        /// <summary>
        /// Validate method GetNeighborRegionIndex return correct Boolean
        /// </summary>
        [Test]
        public void Verify_GetNeighborRegionIndex_method_returns_correct_result()
        {
            var photon = new Photon( // on side of voxel pointed into it
                new Position(-2, 0, 1),
                new Direction(1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                3,
                new Random());
            var index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon); 
            Assert.That(index, Is.EqualTo(1));
            photon = new Photon( // on side of voxel pointed out of it
                new Position(-2, 0, 1),
                new Direction(-1.0, 0, 0),
                1.0,
                _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _oneLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(3));
            // check two layer results
            photon = new Photon( // on side of voxel pointed into LAYER 1
                new Position(2, 0, 0.5),  
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                4,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(1));
            photon = new Photon( // on side of voxel in LAYER 1 pointed out of it
                new Position(2, 0, 0.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(4));
            photon = new Photon( // on side of voxel pointed into LAYER 2
                new Position(-2, 0, 3.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                4,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(2));
            photon = new Photon( // on side of voxel in LAYER 2 pointed out of it
                new Position(-2, 0, 3.5),
                new Direction(-1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(4));
            // check inclusions in two layer tissue
            photon = new Photon( // on side of top inclusion layer 1, pointing into it
                new Position(-1, 0, 1.5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(5));
            photon = new Photon( // on side of bottom inclusion layer 2, pointing into it
                new Position(-1, 0, 5),
                new Direction(1.0, 0, 0),
                1.0,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            index = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetNeighborRegionIndex(photon);
            Assert.That(index, Is.EqualTo(6));
        }

        /// <summary>
        /// Validate method GetAngleRelativeToBoundaryNormal return correct Boolean.
        /// Boundaries are considered to be top and bottom of tissue and bounding.
        /// Note: Math.Abs taken in method to ensure that the angle is always positive,
        /// so Assert check is always positive.
        /// </summary>
        [Test]
        public void Verify_GetAngleRelativeToBoundaryNormal_method_returns_correct_result()
        {
            var photon = new Photon( // on top of tissue pointed into it
                new Position(0, 0, 0.0),
                new Direction(0.0, 0, 1.0),
                1,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                1,
                new Random());
            var cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            photon = new Photon( // on bounding voxel pointed into it
                new Position(-2, 0, 1.0),
                new Direction(-1.0, 0, 0.0),
                1,
                _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder,
                2,
                new Random());
            cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
            // put on side of bottom infinite cylinder pointing in
            photon.DP.Position = new Position(-1.0, 0.0, 5.0);
            photon.DP.Direction = new Direction(1.0, 0.0, 0.0);
            photon.CurrentRegionIndex = 2;
            cosTheta = _twoLayerTissueBoundedByVoxelMultiInfiniteCylinder.GetAngleRelativeToBoundaryNormal(photon);
            Assert.That(cosTheta, Is.EqualTo(1));
        }


    }
}
