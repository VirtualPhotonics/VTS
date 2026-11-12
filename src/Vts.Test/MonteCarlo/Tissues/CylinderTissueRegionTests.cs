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
        /// </summary>
        [Test]
        public void Verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns false if *exactly* on boundary (see method summary)
            var result = _cylinderTissueRegion.OnBoundary(new Position(0, 0, 1.0)); // on top cap boundary
            Assert.That(result, Is.False);
            result = _cylinderTissueRegion.OnBoundary(new Position(0, 0, 3.0)); // on bottom cap boundary
            Assert.That(result, Is.False);
            // OnBoundary returns false if definitely not on boundary
            result = _cylinderTissueRegion.OnBoundary(new Position(0, 0, 10.0)); // not on boundary
            Assert.That(result, Is.False);
            // on cylinder will return false too, since OnBoundary returns false if *exactly* on boundary
            result = _cylinderTissueRegion.OnBoundary(new Position(1.0/Math.Sqrt(2), 1.0/Math.Sqrt(2), 2.0)); 
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Validate method ContainsPositions return correct Boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            // ContainsPosition returns true if inside
            var result = _cylinderTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // inside
            Assert.That(result, Is.True);
            // ContainsPosition returns true if *exactly* on boundary
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
            // intersect side of cylinder tests
            // photon inside cylinder 
            var result = _cylinderTissueRegion.RayIntersectBoundary(photon, out var distanceToBoundary);
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
            // photon outside cylinder
            photon.DP.Position.X = 2;
            photon.DP.Direction.Ux = -1;
            photon.S = 2.0; // definitely intersect side
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
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
            // intersect both sides of cylinder
            photon.DP.Position = new Position(-2, 0, 2);
            photon.DP.Direction = new Direction(1, 0, 0);
            photon.S = 4.0; // make sure intersects both sides
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            // intersect cap of cylinder tests
            // intersect top cap from above top
            photon.DP.Position = new Position(0, 0, 0); 
            photon.DP.Direction = new Direction(0, 0, 1);  
            photon.S = 2.0; // make sure intersects top cap
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            // intersect top cap from below top
            photon.DP.Position = new Position(0, 0, 2);
            photon.DP.Direction = new Direction(0, 0, -1);
            photon.S = 2.0; // make sure intersects top cap
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            // intersect bottom cap from below bottom
            photon.DP.Position = new Position(0, 0, 4); 
            photon.DP.Direction = new Direction(0, 0, -1);
            photon.S = 2.0; // make sure intersects bottom cap
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.EqualTo(true));
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            // intersect both top and bottom caps
            photon.DP.Position = new Position(0, 0, 0); 
            photon.DP.Direction = new Direction(0, 0, 1);
            photon.S = 10.0; // make sure intersects both
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            // intersect corner tests
            // top left corner from below and outside
            photon.DP.Position = new Position(-1.5, 0, 2);
            photon.DP.Direction = new Direction(0.7071, 0, -0.7071);
            photon.S = 3.0; // make sure intersects corner, should hit cylinder before cap
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(Math.Abs(distanceToBoundary - 0.7071) < 1e-4, Is.True);
            // bottom right corner from above and outside
            photon.DP.Position = new Position(1.5, 0, 2);
            photon.DP.Direction = new Direction(-0.7071, 0, 0.7071);
            photon.S = 3.0; // make sure intersects corner, should hit cylinder before cap
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(Math.Abs(distanceToBoundary - 0.7071) < 1e-4, Is.True);
            // top right corner from above and outside
            photon.DP.Position = new Position(0.0, 0, 0.5);
            photon.DP.Direction = new Direction(0.7071, 0, 0.7071);
            photon.S = 3.0; // make sure intersects corner, should hit cap before cylinder
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(Math.Abs(distanceToBoundary - 0.7071) < 1e-4, Is.True);
            // bottom left corner from below and outside
            photon.DP.Position = new Position(0.0, 0, 3.5);
            photon.DP.Direction = new Direction(-0.7071, 0, -0.7071);
            photon.S = 3.0; // make sure intersects corner, should hit cap before cylinder
            result = _cylinderTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.True);
            Assert.That(Math.Abs(distanceToBoundary - 0.7071) < 1e-4, Is.True);
        }

        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector.  Should be outward directed normal.
        /// </summary>
        [Test]
        public void Verify_SurfaceNormal_method_returns_correct_result()
        {
            // select position on top of cylinder (x=0, y=0, z=1)
            var result = _cylinderTissueRegion.SurfaceNormal(new Position(0, 0, 1.0)); 
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(-1));
            // select position on bottom of cylinder (x=0, y=0, z=3)
            result = _cylinderTissueRegion.SurfaceNormal(new Position(0, 0, 3.0)); 
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(1));
            // select position on front of cylinder (x=0, y=1, z=2)
            var x = 0.0;
            var y = 1.0;
            var z = 2.0;
            result = _cylinderTissueRegion.SurfaceNormal(new Position(x, y, z));
            Assert.That(result.Ux, Is.EqualTo(0)); 
            Assert.That(Math.Abs(result.Uy - 1) < 1e-6, Is.True);
            Assert.That(result.Uz, Is.EqualTo(0));
            // select position on right side of cylinder (x=1, y=0, z=2)
            x = 1.0;
            y = 0.0;
            z = 2.0;
            result = _cylinderTissueRegion.SurfaceNormal(new Position(x, y, z));
            Assert.That(Math.Abs(result.Ux - 1) < 1e-6, Is.True);
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(0));
        }
    }
}
