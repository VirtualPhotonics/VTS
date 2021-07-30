using System;
using MathNet.Numerics.Random;
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
        /// test source input
        /// </summary>
        [Test]
        public void validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new CircularAngledFromPointSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new CircularAngledFromPointSourceInput(
                1.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0
            );
            Assert.IsNotNull(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsNotNull(source);
        }
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
