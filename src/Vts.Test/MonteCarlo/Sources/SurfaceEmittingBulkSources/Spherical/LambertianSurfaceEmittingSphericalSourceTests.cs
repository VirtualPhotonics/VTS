using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for LambertianSurfaceEmittingSphericalSource
    /// </summary>
    [TestFixture]
    public class LambertianSurfaceEmittingSphericalSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LambertianSurfaceEmittingSphericalSourceInput();
            Assert.That(si, Is.Not.Null);
            // check full definition
            si = new LambertianSurfaceEmittingSphericalSourceInput(
                    1.0,
                    SourceDefaults.DefaultPosition.Clone(),
                    0
            );
            Assert.That(si, Is.Not.Null);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.Not.Null);
        }
        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_off_surface_of_sphere()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var radius = 1.0;
            var translationFromOrigin = new Position(2, 3, 4);

            var ps = new LambertianSurfaceEmittingSphericalSource(
                radius,
                translationFromOrigin,
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (var i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                var centerX = translationFromOrigin.X;
                var centerY = translationFromOrigin.Y;
                var centerZ = translationFromOrigin.Z;
                // make sure initial x-y-z position relative to center is on surface
                var positionOnSurface = new Position(photon.DP.Position.X - centerX,
                    photon.DP.Position.Y - centerY, photon.DP.Position.Z - centerZ);
                Assert.IsTrue(Math.Abs(
                    radius - Math.Sqrt(positionOnSurface.X * positionOnSurface.X +
                                       positionOnSurface.Y * positionOnSurface.Y +
                                       positionOnSurface.Z * positionOnSurface.Z)) < 0.00001);
                // determine outward normal from photon position
                var outwardNormalAtPosition = new Direction(positionOnSurface.X,
                    positionOnSurface.Y, positionOnSurface.Z);
                // make sure outward normal has theta in [0, pi/2]
                Assert.IsTrue(Math.Abs(outwardNormalAtPosition.Uz) >= 0 &&
                              Math.Abs(outwardNormalAtPosition.Uz) <= Math.PI / 2);
            }
        }
    }
}
