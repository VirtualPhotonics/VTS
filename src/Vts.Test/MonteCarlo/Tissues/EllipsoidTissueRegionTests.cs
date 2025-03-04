﻿using System;
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
                new Position(0, 0, 3), 1.0, 1.0, 2.0, 
                new OpticalProperties(0.01, 1.0, 0.8, 1.4), 
                "HenyeyGreensteinKey1");
        }
        /// <summary>
        /// Validate general constructor of TissueRegion
        /// </summary>
        [Test]
        public void Validate_ellipsoid_properties()
        {
            Assert.That(_ellipsoidTissueRegion.Center.X, Is.EqualTo(0.0));
            Assert.That(_ellipsoidTissueRegion.Center.Y, Is.EqualTo(0.0));
            Assert.That(_ellipsoidTissueRegion.Center.Z , Is.EqualTo(3.0));
            Assert.That(_ellipsoidTissueRegion.Dx, Is.EqualTo(1.0));
            Assert.That(_ellipsoidTissueRegion.Dy, Is.EqualTo(1.0));
            Assert.That(_ellipsoidTissueRegion.Dz, Is.EqualTo(2.0));
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
            Assert.That(result, Is.False);
            // but returns true if outside ellipsoid which doesn't make sense but it is how code is written
            // and all unit tests (linux included) are based on this wrong return
            result = _ellipsoidTissueRegion.OnBoundary(new Position(0, 0, 0.5));
            Assert.That(result, Is.True);
            result = _ellipsoidTissueRegion.OnBoundary(new Position(0, 0, 2.0));
            Assert.That(result, Is.False);
        }
        /// <summary>
        /// Validate method ContainsPositions return correct Boolean. ContainsPosition is true if inside
        /// or *on* boundary.
        /// </summary>
        [Test]
        public void Verify_ContainsPosition_method_returns_correct_result()
        {
            var result = _ellipsoidTissueRegion.ContainsPosition(new Position(0, 0, 3.0)); // inside
            Assert.That(result, Is.True);
            result = _ellipsoidTissueRegion.ContainsPosition(new Position(0, 0, 2.0)); // on boundary
            Assert.That(result, Is.True);
        }
        /// <summary>
        /// Validate method SurfaceNormal return correct normal vector
        /// </summary>
        [Test]
        public void Verify_SurfaceNormal_method_returns_correct_result()
        {
            var result = _ellipsoidTissueRegion.SurfaceNormal(new Position(0, 0, 1.0));
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(-1));
            result = _ellipsoidTissueRegion.SurfaceNormal(new Position(0, 0, 5.0));
            Assert.That(result.Ux, Is.EqualTo(0));
            Assert.That(result.Uy, Is.EqualTo(0));
            Assert.That(result.Uz, Is.EqualTo(1));
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
            Assert.That(result, Is.EqualTo(true));
            Assert.That(distanceToBoundary, Is.EqualTo(1.0));
            photon.S = 0.5; // definitely don't intersect
            result = _ellipsoidTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.EqualTo(false));
            Assert.That(distanceToBoundary, Is.EqualTo(Double.PositiveInfinity));
            photon.S = 1.0; // ends right at boundary => both out and no intersection
            result = _ellipsoidTissueRegion.RayIntersectBoundary(photon, out distanceToBoundary);
            Assert.That(result, Is.EqualTo(false));
            Assert.That(distanceToBoundary, Is.EqualTo(Double.PositiveInfinity));
        }
    }
}
