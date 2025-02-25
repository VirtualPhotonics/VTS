using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for InfiniteCylinderTissueRegion
    /// </summary>
    [TestFixture]
    public class InfiniteCylinderTissueRegionTests
    {
        private InfiniteCylinderTissueRegion _infiniteCylinderTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _infiniteCylinderTissueRegion = new InfiniteCylinderTissueRegion(
                new Position(0, 0, 3), 1.0, new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_infiniteCylinder_properties()
        {
            Assert.That(_infiniteCylinderTissueRegion.Center.X, Is.EqualTo(0.0));
            Assert.That(_infiniteCylinderTissueRegion.Center.Y, Is.EqualTo(0.0));
            Assert.That(_infiniteCylinderTissueRegion.Center.Z, Is.EqualTo(3.0));
            Assert.That(_infiniteCylinderTissueRegion.Radius, Is.EqualTo(1.0));
            Assert.That(_infiniteCylinderTissueRegion.RegionOP.Mua, Is.EqualTo(0.01));
            Assert.That(_infiniteCylinderTissueRegion.RegionOP.Musp, Is.EqualTo(1.0));
            Assert.That(_infiniteCylinderTissueRegion.RegionOP.G, Is.EqualTo(0.8));
            Assert.That(_infiniteCylinderTissueRegion.RegionOP.N, Is.EqualTo(1.4));
        }

        /// <summary>
        /// Validate method ContainsPositions return correct Boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _infiniteCylinderTissueRegion.ContainsPosition(new Position(0, 0, 3.0)); // inside
            Assert.That(result, Is.True);
            result = _infiniteCylinderTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // on boundary
            Assert.That(result, Is.True);
        }
        /// <summary>
        /// Validate method OnBoundary return correct Boolean.
        /// </summary>
        [Test]
        public void Verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns false if *exactly* on boundary
            var result = _infiniteCylinderTissueRegion.OnBoundary(new Position(0, 0, 2.0));
            Assert.That(result, Is.False);
            // but returns true if outside infinite cylinder which doesn't make sense but it is how code is written
            // and all unit tests (linux included) are based on this wrong return
            result = _infiniteCylinderTissueRegion.OnBoundary(new Position(0, 0, 0.5));
            Assert.That(result, Is.True);
            result = _infiniteCylinderTissueRegion.OnBoundary(new Position(0, 0, 2.0));
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector.  Should be outward directed normal.
        /// </summary>
        [Test]
        public void Verify_SurfaceNormal_method_returns_correct_result()
        {
            var result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(0, 0, 2.0)); // top of cyl
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(-1));
            result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(0, 0, 4.0)); // bottom of cyl
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(1));
            // select a more random location on the surface of the cylinder
            const double x = 0.3; // pick any x value and determine z on cylinder
            var z = Math.Sqrt(_infiniteCylinderTissueRegion.Radius * _infiniteCylinderTissueRegion.Radius - x * x);
            const double y = 1.11; // pick any y value
            result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(x, y, z));
            Assert.That(Math.Abs(result.Ux - 0.145072) < 1e-6, Is.True);
            Assert.That(result.Uy, Is.EqualTo(0)); // surface normal on infinite cylinder should *always* have Uz=0
            Assert.That(Math.Abs(result.Uz + 0.989421) < 1e-6, Is.True);
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
                    Position = new Position(0, 0, 3),
                    Direction = new Direction(1, 0, 0)
                },
                S = 2.0 // definitely intersect
            };
            var result = _infiniteCylinderTissueRegion.RayIntersectBoundary(photon, out var distanceToBoundary);
            Assert.That(result, Is.EqualTo(true));
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            photon.S = 0.5; // definitely don't intersect
            result = _infiniteCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.EqualTo(false));
            Assert.That(distanceToBoundary, Is.EqualTo(double.PositiveInfinity));
            photon.S = 1.0; // ends right at boundary => both out and no intersection
            result = _infiniteCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.EqualTo(false));
            Assert.That(distanceToBoundary, Is.EqualTo(double.PositiveInfinity));
        }

    }
}
