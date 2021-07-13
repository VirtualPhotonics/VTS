using System;
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
                0)
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
    }
}
