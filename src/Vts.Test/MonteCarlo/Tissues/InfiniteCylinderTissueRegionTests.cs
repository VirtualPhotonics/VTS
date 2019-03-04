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
        public void create_instance_of_class()
        {
            _infiniteCylinderTissueRegion = new InfiniteCylinderTissueRegion(
                new Position(0, 0, 3), 1.0, new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void validate_infiniteCylinder_properties()
        {
            Assert.AreEqual(_infiniteCylinderTissueRegion.Center.X, 0.0);
            Assert.AreEqual(_infiniteCylinderTissueRegion.Center.Y, 0.0);
            Assert.AreEqual(_infiniteCylinderTissueRegion.Center.Z, 3.0);
        }
        ///// <summary>
        ///// Validate method OnBoundary return correct boolean
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
        //    Direction result = _infiniteCylinderTissueRegion.SurfaceNormal(new Position(0, 0, 2.0));
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
            Photon photon = new Photon();
            photon.DP.Position = new Position(-2, 0, 3);
            photon.DP.Direction = new Direction(1, 0, 0);
            photon.S = 2.0; // definitely intersect
            double distanceToBoundary;
            bool result = _infiniteCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
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
    }
}
