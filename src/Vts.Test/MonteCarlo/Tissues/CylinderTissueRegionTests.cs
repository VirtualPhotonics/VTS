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
        public void Create_instance_of_class()
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
        public void Validate_cylinder_properties()
        {
            Assert.That(_cylinderTissueRegion.Center.X, Is.EqualTo(0.0));
            Assert.That(_cylinderTissueRegion.Center.Y, Is.EqualTo(0.0));
            Assert.That(_cylinderTissueRegion.Center.Z, Is.EqualTo(2.0));
            Assert.That(_cylinderTissueRegion.Radius, Is.EqualTo(1.0));
            Assert.That(_cylinderTissueRegion.Height, Is.EqualTo(2.0));
            Assert.That(_cylinderTissueRegion.RegionOP.Mua, Is.EqualTo(0.01));
            Assert.That(_cylinderTissueRegion.RegionOP.Musp, Is.EqualTo(1.0));
            Assert.That(_cylinderTissueRegion.RegionOP.G, Is.EqualTo(0.8));
            Assert.That(_cylinderTissueRegion.RegionOP.N, Is.EqualTo(1.4));
        }

        /// <summary>
        /// Validate method OnBoundary return correct Boolean.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        [Test]
        public void Verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns true if *exactly* on boundary
            var result = _cylinderTissueRegion.OnBoundary(new Position(0, 0, 1.0)); // on top cap boundary
            Assert.That(result, Is.True);
            result = _cylinderTissueRegion.OnBoundary(new Position(0, 0, 3.0)); // on bottom cap boundary
            Assert.That(result, Is.True);
            result = _cylinderTissueRegion.OnBoundary(new Position(0, 0, 10.0)); // not on boundary
            Assert.That(result, Is.False);
            // on cylinder
            result = _cylinderTissueRegion.OnBoundary(new Position(1.0/Math.Sqrt(2), 1.0/Math.Sqrt(2), 2.0)); 
            Assert.That(result, Is.True);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct Boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _cylinderTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // inside
            Assert.That(result, Is.True);
            result = _cylinderTissueRegion.ContainsPosition(new Position(0, 0, 3.0)); // on boundary
            Assert.That(result, Is.True);
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
            var result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            photon.S = 0.5; // definitely don't intersect sides
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.False);
            Assert.That(distanceToBoundary, Is.EqualTo(double.PositiveInfinity));
            photon.S = 1.0; // ends right at boundary => both out and no intersection
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.False);
            Assert.That(distanceToBoundary, Is.EqualTo(double.PositiveInfinity));
            // intersect cap of cylinder tests
            photon.DP.Position = new Position(0, 0, 0); // intersect top cap
            photon.DP.Direction = new Direction(0, 0, 1);  
            photon.S = 2.0; // make sure intersects top cap
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            photon.DP.Position = new Position(0, 0, 4); // intersect bottom cap
            photon.DP.Direction = new Direction(0, 0, -1);
            photon.S = 2.0; // make sure intersects top cap
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.EqualTo(true));
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            photon.DP.Position = new Position(0, 0, 0); // intersect both
            photon.DP.Direction = new Direction(0, 0, 1);
            photon.S = 10.0; // make sure intersects both
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
        }
    }
}
