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
            Assert.That(_caplessVoxelTissueRegion.X.Start, Is.EqualTo(-1));
            Assert.That(_caplessVoxelTissueRegion.X.Stop, Is.EqualTo(1));
            Assert.That(_caplessVoxelTissueRegion.Y.Start, Is.EqualTo(-1));
            Assert.That(_caplessVoxelTissueRegion.Y.Stop, Is.EqualTo(1));
            Assert.That(_caplessVoxelTissueRegion.Z.Start, Is.EqualTo(1).Within(1));
            Assert.That(_caplessVoxelTissueRegion.Z.Stop, Is.EqualTo(3).Within(3));
            Assert.That(_caplessVoxelTissueRegion.Center.X, Is.EqualTo(0.0));
            Assert.That(_caplessVoxelTissueRegion.Center.Y, Is.EqualTo(0.0));
            Assert.That(_caplessVoxelTissueRegion.Center.Z, Is.EqualTo(2.0));
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
            Assert.That(result, Is.False);
            result = _caplessVoxelTissueRegion.OnBoundary(new Position(0, 0, 0.5)); // outside
            Assert.That(result, Is.False);
            result = _caplessVoxelTissueRegion.OnBoundary(new Position(0, 0, 2.0)); // inside
            Assert.That(result, Is.False);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _caplessVoxelTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // inside
            Assert.That(result, Is.True);
            result = _caplessVoxelTissueRegion.ContainsPosition(new Position(0, 0, 1.0)); // on boundary
            Assert.That(result, Is.True);
        }
        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector.
        /// Only valid for sides of voxel
        /// </summary>
        [Test]
        public void Verify_SurfaceNormal_method_returns_correct_result()
        {
            var result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0, 0, 1.0)); // top
            Assert.That(result, Is.Null);
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0, 0, 3.0));  //bottom
            Assert.That(result, Is.Null);
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(1.0, 0, 2.0)); // right side
            Assert.That(result.Ux, Is.EqualTo(1));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(0));
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(-1.0, 0, 2.0));  //left side
            Assert.That(result.Ux, Is.EqualTo(-1));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(0));
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0.0, -1.0, 2.0)); // back side
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(-1));
            Assert.That(result.Uz, Is.EqualTo(0));
            result = _caplessVoxelTissueRegion.SurfaceNormal(new Position(0.0, 1.0, 2.0));  //front side
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(1));
            Assert.That(result.Uz, Is.EqualTo(0));
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
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            photon.S = 0.5; // definitely don't intersect
            result = _caplessVoxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.False);
            Assert.That(distanceToBoundary, Is.EqualTo(double.PositiveInfinity));
            photon.S = 1.0; // ends right at boundary => intersection
            result = _caplessVoxelTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
        }
    }
}
