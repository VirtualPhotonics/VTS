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
            Assert.AreEqual(0.0, _infiniteCylinderTissueRegion.Center.X);
            Assert.AreEqual(0.0, _infiniteCylinderTissueRegion.Center.Y);
            Assert.AreEqual(3.0, _infiniteCylinderTissueRegion.Center.Z);
            Assert.AreEqual(1.0, _infiniteCylinderTissueRegion.Radius);
            Assert.AreEqual(0.01, _infiniteCylinderTissueRegion.RegionOP.Mua);
            Assert.AreEqual(1.0,_infiniteCylinderTissueRegion.RegionOP.Musp);
            Assert.AreEqual(0.8, _infiniteCylinderTissueRegion.RegionOP.G);
            Assert.AreEqual(1.4, _infiniteCylinderTissueRegion.RegionOP.N);
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
            double distanceToBoundary;
            var result = _infiniteCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            photon.S = 0.5; // definitely don't intersect
            result = _infiniteCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(Double.PositiveInfinity, distanceToBoundary);
            photon.S = 1.0; // ends right at boundary => both out and no intersection
            result = _infiniteCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(Double.PositiveInfinity, distanceToBoundary);
        }

        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector.  Should be outward directed normal.
        /// </summary>
        [Test]
        public void Verify_SurfaceNormal_method_returns_correct_result()
        {
            var result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(0, 0, 2.0)); // top of cyl
            Assert.AreEqual(result.Ux, 0);
            Assert.AreEqual(result.Uy, 0);
            Assert.AreEqual(result.Uz, -1);
            result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(0, 0, 4.0)); // bottom of cyl
            Assert.AreEqual(result.Ux, 0);
            Assert.AreEqual(result.Uy, 0);
            Assert.AreEqual(result.Uz, 1);
            // select a more random location on the surface of the cylinder
            const double x = 0.3; // pick any x value and determine z on cylinder
            var z = Math.Sqrt(_infiniteCylinderTissueRegion.Radius * _infiniteCylinderTissueRegion.Radius - x * x);
            const double y = 1.11; // pick any y value
            result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(x, y, z)); 
            Assert.IsTrue(Math.Abs(result.Ux - 0.145072) < 1e-6);
            Assert.AreEqual(result.Uy, 0); // surface normal on infinite cylinder should *always* have Uz=0
            Assert.IsTrue(Math.Abs(result.Uz + 0.989421) < 1e-6);
        }

    }
}
