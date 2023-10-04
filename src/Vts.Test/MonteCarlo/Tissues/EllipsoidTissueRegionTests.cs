using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Tissues
{
    /// <summary>
    /// Unit tests for EllipsoidTissueRegion
    /// </summary>
    [TestFixture]
    public class EllipsoidTissueRegionTests
    {
        private EllipsoidTissueRegion _ellipsoidTissueRegion;
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [OneTimeSetUp]
        public void Create_instance_of_class()
        {
            _ellipsoidTissueRegion = new EllipsoidTissueRegion(
                new Position(0, 0, 3), 1.0, 1.0, 2.0, new OpticalProperties(0.01, 1.0, 0.8, 1.4));
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_ellipsoid_properties()
        {
            Assert.AreEqual(0.0, _ellipsoidTissueRegion.Center.X);
            Assert.AreEqual(0.0, _ellipsoidTissueRegion.Center.Y);
            Assert.AreEqual(3.0,_ellipsoidTissueRegion.Center.Z );
            Assert.AreEqual(1.0, _ellipsoidTissueRegion.Dx);
            Assert.AreEqual(1.0, _ellipsoidTissueRegion.Dy);
            Assert.AreEqual(2.0, _ellipsoidTissueRegion.Dz);
        }
        /// <summary>
        /// Validate method OnBoundary return correct Boolean.
        /// Currently OnBoundary of an inclusion region isn't called by any code ckh 3/5/19.
        /// </summary>
        [Test]
        public void Verify_OnBoundary_method_returns_correct_result()
        {
            // OnBoundary returns false if *exactly* on boundary
            var result = _ellipsoidTissueRegion.OnBoundary(new Position(0, 0, 1.0));
            Assert.IsFalse(result);
            // but returns true if outside ellipsoid which doesn't make sense but it is how code is written
            // and all unit tests (linux included) are based on this wrong return
            result = _ellipsoidTissueRegion.OnBoundary(new Position(0, 0, 0.5));
            Assert.IsTrue(result);
            result = _ellipsoidTissueRegion.OnBoundary(new Position(0, 0, 2.0));
            Assert.IsFalse(result);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct Boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _ellipsoidTissueRegion.ContainsPosition(new Position(0, 0, 3.0)); // inside
            Assert.IsTrue(result);
            result = _ellipsoidTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // on boundary
            Assert.IsTrue(result);
        }
        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector
        /// </summary>
        [Test]
        public void Verify_SurfaceNormal_method_returns_correct_result()
        {
            var result = _ellipsoidTissueRegion.SurfaceNormal(new Position(0, 0, 1.0));
            Assert.AreEqual(0, result.Ux);
            Assert.AreEqual(0, result.Uy);
            Assert.AreEqual(-1, result.Uz);
            result = _ellipsoidTissueRegion.SurfaceNormal(new Position(0, 0, 5.0));
            Assert.AreEqual(0, result.Ux);
            Assert.AreEqual(0, result.Uy);
            Assert.AreEqual(1, result.Uz);
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
                    Position = new Position(-2, 0, 3),
                    Direction = new Direction(1, 0, 0)
                },
                S = 10.0 // definitely intersect 
            };
            double distanceToBoundary;
            var result = _ellipsoidTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(true, result);
            Assert.AreEqual(1.0, distanceToBoundary);
            photon.S = 0.5; // definitely don't intersect
            result = _ellipsoidTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(Double.PositiveInfinity, distanceToBoundary);
            photon.S = 1.0; // ends right at boundary => both out and no intersection
            result = _ellipsoidTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.AreEqual(false, result);
            Assert.AreEqual(Double.PositiveInfinity, distanceToBoundary);
        }
    }
}
