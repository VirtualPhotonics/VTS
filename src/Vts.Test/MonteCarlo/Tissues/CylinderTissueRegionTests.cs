using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for CylinderTissueRegion
    /// </summary>
    [TestFixture]
    public class CylinderTissueRegionTests
    {
        private CylinderTissueRegion _cylinderTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void create_instance_of_class()
        {
            _cylinderTissueRegion = new CylinderTissueRegion(
                new Position(0, 0, 2), // center
                1.0, 
                2.0, // z goes from z=1 to 3
                new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void validate_cylinder_properties()
        {
            Assert.AreEqual(_cylinderTissueRegion.Center.X, 0.0);
            Assert.AreEqual(_cylinderTissueRegion.Center.Y, 0.0);
            Assert.AreEqual(_cylinderTissueRegion.Center.Z, 2.0);
            Assert.AreEqual(_cylinderTissueRegion.Radius, 1.0);
            Assert.AreEqual(_cylinderTissueRegion.Height, 2.0);
            Assert.AreEqual(_cylinderTissueRegion.RegionOP.Mua, 0.01);
            Assert.AreEqual(_cylinderTissueRegion.RegionOP.Musp, 1.0);
            Assert.AreEqual(_cylinderTissueRegion.RegionOP.G, 0.8);
            Assert.AreEqual(_cylinderTissueRegion.RegionOP.N, 1.4);
        }
        ///// <summary>
        ///// Validate method OnBoundary return correct boolean THIS METHOD MAY BE OBSOLETE
        ///// </summary>
        //[Test]
        //public void verify_OnBoundary_method_returns_correct_result()
        //{
        //    bool result = _infiniteCylinderTissueRegion.OnBoundary(new Position(0, 0, 2.0));
        //    Assert.IsTrue(result);
        //    result = _infiniteCylinderTissueRegion.OnBoundary(new Position(0, 0, 1.0));
        //    Assert.IsFalse(result);
        //}
        ///// <summary>
        ///// Validate method SurfaceNormal return correct normal vector
        ///// </summary>
        //[Test]
        //public void verify_SurfaceNormal_method_returns_correct_result()
        //{
        //    Direction result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(0, 0, 1.0));
        //    Assert.AreEqual(new Direction(0, 0, -1), result);
        //    result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(0, 0, 5.0));
        //    Assert.AreEqual(new Direction(0, 0, 1), result);
        //}
        /// <summary>
        /// Validate method RayIntersectBoundary return correct result
        /// </summary>
        [Test]
        public void verify_RayIntersectBoundary_method_returns_correct_result()
        {
            // test intersection with sides
            Photon photon = new Photon();
            photon.DP.Position = new Position(0, 0, 2);
            photon.DP.Direction = new Direction(1, 0, 0);
            photon.S = 2.0; // definitely intersect sides
            double distanceToBoundary;
            bool result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            photon.S = 0.5; // definitely don't intersect sides
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(Double.PositiveInfinity, distanceToBoundary);
            photon.S = 1.0; // ends right at boundary => both out and no intersection
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(Double.PositiveInfinity, distanceToBoundary);
            // test intersection with caps
            photon.DP.Direction = new Direction(0, 0, -1);
            photon.S = 2;
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            photon.DP.Direction = new Direction(0, 0, 1);
            photon.S = 2;
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            // test infinitely thin cylinder
            _cylinderTissueRegion.Height = 0;
            _cylinderTissueRegion.Center.Z = 0;
            photon.DP.Direction = new Direction(0, 0, -1);
            photon.S = 2;
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
        }
    }
}
