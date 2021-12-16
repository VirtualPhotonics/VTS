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
                new Position(0, 0, 3), 1.0, 
                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                "HenyeyGreensteinKey1");
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void validate_infiniteCylinder_properties()
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
        public void verify_RayIntersectBoundary_method_returns_correct_result()
        {
            Photon photon = new Photon();
            photon.DP.Position = new Position(0, 0, 3);
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
