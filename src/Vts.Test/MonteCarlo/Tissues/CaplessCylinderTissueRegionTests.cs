using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for CaplessCylinderTissueRegion
    /// </summary>
    [TestFixture]
    public class CaplessCylinderTissueRegionTests
    {
        private CaplessCylinderTissueRegion _caplessCylinderTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _caplessCylinderTissueRegion = new CaplessCylinderTissueRegion(
               new Position(0, 0, 2), // center
                1.0, 
                2.0, // z goes from z=1 to 3
                new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_caplessCylinder_properties()
        {
            Assert.AreEqual(0.0, _caplessCylinderTissueRegion.Center.X);
            Assert.AreEqual(0.0, _caplessCylinderTissueRegion.Center.Y);
            Assert.AreEqual(2.0, _caplessCylinderTissueRegion.Center.Z);
            Assert.AreEqual(1.0, _caplessCylinderTissueRegion.Radius);
            Assert.AreEqual(2.0, _caplessCylinderTissueRegion.Height);
            Assert.AreEqual(0.01, _caplessCylinderTissueRegion.RegionOP.Mua);
            Assert.AreEqual(1.0, _caplessCylinderTissueRegion.RegionOP.Musp);
            Assert.AreEqual(0.8, _caplessCylinderTissueRegion.RegionOP.G);
            Assert.AreEqual(1.4, _caplessCylinderTissueRegion.RegionOP.N);
        }

        /// <summary>
        /// Validate method OnBoundary return correct Boolean.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        [Test]
        public void Verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns true if *exactly* on boundary
            var result = _caplessCylinderTissueRegion.OnBoundary(new Position(0, 0, 1.0)); // on top cap boundary
            Assert.IsFalse(result);
            result = _caplessCylinderTissueRegion.OnBoundary(new Position(0, 0, 3.0)); // on bottom cap boundary -> so false
            Assert.IsFalse(result);
            result = _caplessCylinderTissueRegion.OnBoundary(new Position(0, 0, 10.0)); // not on boundary
            Assert.IsFalse(result);
            // on caplessCylinder
            result = _caplessCylinderTissueRegion.OnBoundary(new Position(1.0/Math.Sqrt(2), 1.0/Math.Sqrt(2), 2.0)); 
            Assert.IsTrue(result);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct Boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _caplessCylinderTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // inside
            Assert.IsTrue(result);
            result = _caplessCylinderTissueRegion.ContainsPosition(new Position(0, 0, 3.0)); // on boundary
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Validate method RayIntersectBoundary return correct result
        /// </summary>
        [Test]
        public void Verify_RayIntersectBoundary_method_returns_correct_result()
        {
            // test intersection with sides
            var photon = new Photon
            {
                DP =
                {
                    Position = new Position(0, 0, 2),
                    Direction = new Direction(1, 0, 0)
                },
                S = 2.0 // definitely intersect sides
            };
            double distanceToBoundary;
            var result = _caplessCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            photon.S = 0.5; // definitely don't intersect sides
            result = _caplessCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(double.PositiveInfinity, distanceToBoundary);
            photon.S = 1.0; // ends right at boundary => both out and no intersection
            result = _caplessCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(double.PositiveInfinity, distanceToBoundary);
            // intersect cap of caplessCylinder tests
            photon.DP.Position = new Position(0, 0, 0); // intersect top cap
            photon.DP.Direction = new Direction(0, 0, 1);  
            photon.S = 2.0; // make sure intersects top cap
            result = _caplessCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(double.PositiveInfinity, distanceToBoundary);
            photon.DP.Position = new Position(0, 0, 4); // intersect bottom cap
            photon.DP.Direction = new Direction(0, 0, -1);
            photon.S = 2.0; // make sure intersects top cap
            result = _caplessCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(double.PositiveInfinity, distanceToBoundary);
            photon.DP.Position = new Position(0, 0, 0); // intersect both
            photon.DP.Direction = new Direction(0, 0, 1);
            photon.S = 10.0; // make sure intersects both
            result = _caplessCylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(double.PositiveInfinity, distanceToBoundary);
        }
    }
}
