using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for CustomSurfaceEmittingSphericalSource
    /// </summary>
    [TestFixture]
    public class CustomSurfaceEmittingSphericalSourceTests
    {
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
            var polarRange = new DoubleRange(0, Math.PI, 2);
            var azimuthalRange = new DoubleRange(0, 2 * Math.PI, 2);
            var translationFromOrigin = new Position(2, 3, 4);
            var newDirectionOfPrincipalAxis = new Direction(0, 0, 1);

            var ps = new CustomSurfaceEmittingSphericalSource(
                radius,
                polarRange,
                azimuthalRange,
                newDirectionOfPrincipalAxis,
                translationFromOrigin,
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (int i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                var centerX = translationFromOrigin.X;
                var centerY = translationFromOrigin.Y;
                var centerZ = translationFromOrigin.Z;
                // make sure initial x-y-z position is on surface
                Assert.IsTrue(Math.Abs(
                    radius - Math.Sqrt((photon.DP.Position.X - centerX) * (photon.DP.Position.X - centerX) +
                                       (photon.DP.Position.Y - centerY) * (photon.DP.Position.Y - centerY) +
                                       (photon.DP.Position.Z - centerZ) * (photon.DP.Position.Z - centerZ))) < 0.00001);

            }
        }

        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_off_partial_surface_of_sphere()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var radius = 1.0;
            var polarRange = new DoubleRange(0, Math.PI / 2, 2);
            var azimuthalRange = new DoubleRange(0, 2 * Math.PI, 2);
            var translationFromOrigin = new Position(2, 3, 4);
            var newDirectionOfPrincipalAxis = new Direction(0, 0, 1);

            var ps = new CustomSurfaceEmittingSphericalSource(
                radius,
                polarRange,
                azimuthalRange,
                newDirectionOfPrincipalAxis,
                translationFromOrigin,
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (int i = 0; i < 10; i++)
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
                // make sure initial x-y-z position is only in lower hemisphere
                Assert.IsTrue(positionOnSurface.Z >= 0.0);
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
