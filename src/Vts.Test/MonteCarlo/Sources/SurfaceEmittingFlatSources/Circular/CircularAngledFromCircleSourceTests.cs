using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: CircularAngledFromCircleSource
    /// </summary>
    [TestFixture]
    public class CircularAngledFromCircleSourceTests
    {
        /// <summary>
        /// This test different from others in that it is validated by geometrically
        /// determined results
        /// </summary>
        [Test]
        public void validate_CircularAngledFromCircle_source()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var _profile = new FlatSourceProfile();
            var _radiusOnTissue = 10.0;
            var _radiusInAir = 0.0;
            var _circleInAirTranslationFromOrigin = new Position(0, 0, -10);

            var ps = new CircularAngledFromCircleSource(
                _radiusOnTissue, 
                _profile,
                _radiusInAir, 
                _circleInAirTranslationFromOrigin)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(photon.DP.Position.Z, 0.0);
            // make sure initial position is inside radius
            Assert.IsTrue(Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X +
                                    photon.DP.Position.Y * photon.DP.Position.Y) <= _radiusOnTissue);
            // make sure angle is less than 45 degrees
            Assert.IsTrue(photon.DP.Direction.Uz >= 1 / Math.Sqrt(2));
        }
    }
}
