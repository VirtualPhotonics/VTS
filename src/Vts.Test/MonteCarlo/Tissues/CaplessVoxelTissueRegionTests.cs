using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for CaplessVoxelTissueRegion
    /// </summary>
    [TestFixture]
    public class CaplessVoxelTissueRegionTests
    {
        private CaplessVoxelTissueRegion _caplessVoxelTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _caplessVoxelTissueRegion = new CaplessVoxelTissueRegion(
                new DoubleRange(-1, 1, 2), // x range
                new DoubleRange(-1, 1, 2), // y range
                new DoubleRange(1, 3, 2),  // z range
                new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_CaplessVoxel_properties()
        {
            Assert.AreEqual(-1, _caplessVoxelTissueRegion.X.Start);
            Assert.AreEqual(1, _caplessVoxelTissueRegion.X.Stop);
            Assert.AreEqual(-1, _caplessVoxelTissueRegion.Y.Start);
            Assert.AreEqual(1, _caplessVoxelTissueRegion.Y.Stop);
            Assert.AreEqual(1, _caplessVoxelTissueRegion.Z.Start, 1);
            Assert.AreEqual(3, _caplessVoxelTissueRegion.Z.Stop, 3);
            Assert.AreEqual(0.0, _caplessVoxelTissueRegion.Center.X);
            Assert.AreEqual(0.0, _caplessVoxelTissueRegion.Center.Y);
            Assert.AreEqual(2.0, _caplessVoxelTissueRegion.Center.Z);
        }
        /// <summary>
        /// Validate method OnBoundary return correct boolean.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        [Test]
        public void Verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns true if *exactly* on boundary
            var result = _caplessVoxelTissueRegion.OnBoundary(new Position(0, 0, 1.0)); // on boundary
            Assert.IsFalse(result);
            result = _caplessVoxelTissueRegion.OnBoundary(new Position(0, 0, 0.5)); // outside
            Assert.IsFalse(result);
            result = _caplessVoxelTissueRegion.OnBoundary(new Position(0, 0, 2.0)); // inside
            Assert.IsFalse(result);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _caplessVoxelTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // inside
            Assert.IsTrue(result);
            result = _caplessVoxelTissueRegion.ContainsPosition(new Position(0, 0, 1.0)); // on boundary
            Assert.IsTrue(result);
        }
        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector.
        /// Only valid for sides of voxel
        /// </summary>
        [Test]
        public void Verify_SurfaceNormal_method_returns_correct_result()
        {
            var result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0, 0, 1.0)); // top
            Assert.AreEqual(null, result);
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0, 0, 3.0));  //bottom
            Assert.AreEqual(null, result);
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(1.0, 0, 2.0)); // right side
            Assert.AreEqual(1, result.Ux);
            Assert.AreEqual(0, result.Uy);
            Assert.AreEqual(0, result.Uz);
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(-1.0, 0, 2.0));  //left side
            Assert.AreEqual(-1, result.Ux);
            Assert.AreEqual(0, result.Uy);
            Assert.AreEqual(0, result.Uz);
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0.0, -1.0, 2.0)); // back side
            Assert.AreEqual(0, result.Ux);
            Assert.AreEqual(-1, result.Uy);
            Assert.AreEqual(0, result.Uz);
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0.0, 1.0, 2.0));  //front side
            Assert.AreEqual(0, result.Ux);
            Assert.AreEqual(1, result.Uy);
            Assert.AreEqual(0, result.Uz);
        }
        /// <summary>
        /// Validate method RayIntersectBoundary return correct result
        /// </summary>
        [Test]
        public void Verify_RayIntersectBoundary_method_returns_correct_result()
        {
            var photon = new Photon
            {
                DP =
                {
                    Position = new Position(-2, 0, 2),
                    Direction = new Direction(1, 0, 0)
                },
                S = 10.0 // definitely intersect 
            };
            double distanceToBoundary;
            var result = _caplessVoxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            photon.S = 0.5; // definitely don't intersect
            result = _caplessVoxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(double.PositiveInfinity, distanceToBoundary);
            photon.S = 1.0; // ends right at boundary => intersection
            result = _caplessVoxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
        }
    }
}
