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
    /// Unit tests for Surface Emitting Sources: CircularAngledFromPointSource
    /// </summary>
    [TestFixture]
    public class CircularAngledFromPointSourceTests
    {
        /// <summary>
        /// This test different from others in that it is validated by geometrically
        /// determined results
        /// </summary>
        [Test]
        public void validate_CircularAngledFromPoint_source()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var _radius = 1.0;
            var _pointLocation = new Position(0, 0, -1); // put directly above
            var _translationFromOrigin = new Position(0, 0, 0);  

            var ps = new CircularAngledFromPointSource(_radius, profile, _pointLocation, _translationFromOrigin)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(photon.DP.Position.Z, 0.0);
            // make sure initial position is inside radius
            Assert.IsTrue(Math.Sqrt(
                (photon.DP.Position.X - _translationFromOrigin.X) *
                (photon.DP.Position.X - _translationFromOrigin.X) +
                (photon.DP.Position.Y - _translationFromOrigin.Y) *
                (photon.DP.Position.Y - _translationFromOrigin.Y)) <= _radius);
            // make sure angle is less than 45 degrees
            Assert.IsTrue(photon.DP.Direction.Uz >= 1 / Math.Sqrt(2));
        }

    }
}
