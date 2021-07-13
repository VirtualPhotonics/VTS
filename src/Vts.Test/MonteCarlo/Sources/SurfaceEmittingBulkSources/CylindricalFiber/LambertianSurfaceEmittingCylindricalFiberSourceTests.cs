using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for LambertianSurfaceEmittingCylindricalFiberSource
    /// </summary>
    [TestFixture]
    public class LambertianSurfaceEmittingCylindricalFiberSourceTests
    {
        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_off_surface_of_fiber()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); 
            var fiberRadius = 1.0;
            var fiberHeight = 4;
            var curvedSurfaceEfficiency = 0.5;
            var bottomSurfaceEfficiency = 1.0;
            var principalAxis = new Direction(0, 0, 1);
            var translationFromOrigin = new Position(0, 0, 2);

            var ps = new LambertianSurfaceEmittingCylindricalFiberSource(
                fiberRadius,
                fiberHeight,
                curvedSurfaceEfficiency,
                bottomSurfaceEfficiency,
                principalAxis,
                translationFromOrigin,
                0)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial x-y position is on surface if not on bottom
            if (Math.Abs(photon.DP.Position.Z - 4) > 1e-10 ) // on sides
            {
                Assert.IsTrue(Math.Abs(fiberRadius -
                                       Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X +
                                                 photon.DP.Position.Y * photon.DP.Position.Y)) < 0.00001);
            }
            else // on bottom, make sure pointed down
            {
                Assert.IsTrue(photon.DP.Direction.Uz > 0);
            }
        }
    }
}
