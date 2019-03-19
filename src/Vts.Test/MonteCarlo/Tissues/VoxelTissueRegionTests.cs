using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for VoxelTissueRegion
    /// </summary>
    [TestFixture]
    public class VoxelTissueRegionTests
    {
        private VoxelTissueRegion _voxelTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _voxelTissueRegion = new VoxelTissueRegion(
                new DoubleRange(-1, 1, 2), // x range
                new DoubleRange(-1, 1, 2), // y range
                new DoubleRange(1, 3, 2),  // z range
                new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void validate_Voxel_properties()
        {
            Assert.AreEqual(_voxelTissueRegion.X.Start, -1);
            Assert.AreEqual(_voxelTissueRegion.X.Stop, 1);
            Assert.AreEqual(_voxelTissueRegion.Y.Start, -1);
            Assert.AreEqual(_voxelTissueRegion.Y.Stop, 1);           
            Assert.AreEqual(_voxelTissueRegion.Z.Start, 1);
            Assert.AreEqual(_voxelTissueRegion.Z.Stop, 3);
            Assert.AreEqual(_voxelTissueRegion.Center.X, 0.0);
            Assert.AreEqual(_voxelTissueRegion.Center.Y, 0.0);
            Assert.AreEqual(_voxelTissueRegion.Center.Z, 2.0);
        }
        /// <summary>
        /// Validate method OnBoundary return correct boolean.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        [Test]
        public void verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns true if *exactly* on boundary
            bool result = _voxelTissueRegion.OnBoundary(new Position(0, 0, 1.0)); // on boundary
            Assert.IsTrue(result);
            result = _voxelTissueRegion.OnBoundary(new Position(0, 0, 0.5)); // outside
            Assert.IsFalse(result);
            result = _voxelTissueRegion.OnBoundary(new Position(0, 0, 2.0)); // inside
            Assert.IsFalse(result);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void verify_ContainsPosition_method_returns_correct_result()
        {
            bool result = _voxelTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // inside
            Assert.IsTrue(result);
            result = _voxelTissueRegion.ContainsPosition(new Position(0, 0, 1.0)); // on boundary
            Assert.IsTrue(result);
        }
        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector
        /// </summary>
        [Test]
        public void verify_SurfaceNormal_method_returns_correct_result()
        {
            Direction result = _voxelTissueRegion.SurfaceNormal(new Position(0, 0, 1.0)); // top
            Assert.AreEqual(result.Ux, 0);
            Assert.AreEqual(result.Uy, 0);
            Assert.AreEqual(result.Uz, -1);
            result = _voxelTissueRegion.SurfaceNormal(new Position(0, 0, 3.0));  //bottom
            Assert.AreEqual(result.Ux, 0);
            Assert.AreEqual(result.Uy, 0);
            Assert.AreEqual(result.Uz, 1);
            result = _voxelTissueRegion.SurfaceNormal(new Position(1.0, 0, 2.0)); // right side
            Assert.AreEqual(result.Ux, 1);
            Assert.AreEqual(result.Uy, 0);
            Assert.AreEqual(result.Uz, 0);
            result = _voxelTissueRegion.SurfaceNormal(new Position(-1.0, 0, 2.0));  //left side
            Assert.AreEqual(result.Ux, -1);
            Assert.AreEqual(result.Uy, 0);
            Assert.AreEqual(result.Uz, 0);
            result = _voxelTissueRegion.SurfaceNormal(new Position(0.0, -1.0, 2.0)); // back side
            Assert.AreEqual(result.Ux, 0);
            Assert.AreEqual(result.Uy, -1);
            Assert.AreEqual(result.Uz, 0);
            result = _voxelTissueRegion.SurfaceNormal(new Position(0.0, 1.0, 2.0));  //front side
            Assert.AreEqual(result.Ux, 0);
            Assert.AreEqual(result.Uy, 1);
            Assert.AreEqual(result.Uz, 0);
        }
        /// <summary>
        /// Validate method RayIntersectBoundary return correct result
        /// </summary>
        [Test]
        public void verify_RayIntersectBoundary_method_returns_correct_result()
        {
            Photon photon = new Photon();
            photon.DP.Position = new Position(-2, 0, 2);
            photon.DP.Direction = new Direction(1, 0, 0);
            photon.S = 10.0; // definitely intersect 
            double distanceToBoundary;
            bool result = _voxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            photon.S = 0.5; // definitely don't intersect
            result = _voxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(Double.PositiveInfinity, distanceToBoundary);
            photon.S = 1.0; // ends right at boundary => intersection
            result = _voxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
        }
    }
}
