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
    /// Unit tests for Surface Emitting Sources: CircularAngledFromCircleSource
    /// </summary>
    [TestFixture]
    public class CircularAngledFromCircleSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new CircularAngledFromCircleSourceInput();
            Assert.IsInstanceOf<CircularAngledFromCircleSourceInput>(si);
            // check full definition
            si = new CircularAngledFromCircleSourceInput(
                    10.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultPosition.Clone(),
                    1.0,
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    0
            );
            Assert.IsInstanceOf<CircularAngledFromCircleSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<CircularAngledFromCircleSource>(source);
        }
        /// <summary>
        /// This test different from others in that it is validated by geometrically
        /// determined results
        /// </summary>
        [Test]
        public void Validate_CircularAngledFromCircle_perpendicular_source_in_air()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var radiusOnTissue = 10.0;
            var translationFromOrigin = new Position(0, 0, 0);
            var radiusInAir = 0.0;
            var circleInAirTranslationFromOrigin = new Position(0, 0, -10);
            var circleInAirRotation = new Direction(0, 0, 1); // make perpendicular

            var ps = new CircularAngledFromCircleSource(
                radiusOnTissue, 
                profile,
                translationFromOrigin,
                radiusInAir, 
                circleInAirTranslationFromOrigin,
                circleInAirRotation)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(0.0, photon.DP.Position.Z);
            // make sure initial position is inside radius
            Assert.IsTrue(Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X +
                                    photon.DP.Position.Y * photon.DP.Position.Y) <= radiusOnTissue);
            // make sure angle is less than 45 degrees
            Assert.IsTrue(photon.DP.Direction.Uz >= 1 / Math.Sqrt(2));
        }
        /// <summary>
        /// This test different from others in that it is validated by geometrically
        /// determined results using line from point in air and point on surface
        /// </summary>
        [Test]
        public void Validate_CircularAngledFromCircle_angled_source_in_air()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var radiusOnTissue = 0.0; // make point
            var translationFromOrigin = new Position(0, 0, 0); 
            var radiusInAir = 0.0; // make point so can test angle = line
            var circleInAirTranslationFromOrigin = new Position(0, 0, -10);
            // 45 deg angle
            var circleInAirRotation = new Direction(1/Math.Sqrt(2), 0, 1/Math.Sqrt(2));

            var ps = new CircularAngledFromCircleSource(
                radiusOnTissue,
                profile,
                translationFromOrigin,
                radiusInAir,
                circleInAirTranslationFromOrigin,
                circleInAirRotation)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(0.0, photon.DP.Position.Z);
            // make sure initial position is inside radius
            Assert.IsTrue(Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X +
                                    photon.DP.Position.Y * photon.DP.Position.Y) <= radiusOnTissue);
            // make sure angle is 45 degrees
            Assert.IsTrue(Math.Abs(photon.DP.Direction.Uz - (1 / Math.Sqrt(2))) < 1e-6);
        }
        /// <summary>
        /// This test different from others in that it is validated by geometrically
        /// determined results using line from finite circle in air and point on surface
        /// </summary>
        [Test]
        public void validate_CircularAngledFromCircle_angled_finite_source_in_air()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var radiusOnTissue = 0.0; // make point
            var translationFromOrigin = new Position(0, 0, 0);
            var radiusInAir = 0.5; // make finite radius
            var circleInAirTranslationFromOrigin = new Position(0, 0, -10);
            // 45 deg angle
            var circleInAirRotation = new Direction(1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2));

            var ps = new CircularAngledFromCircleSource(
                radiusOnTissue,
                profile,
                translationFromOrigin,
                radiusInAir,
                circleInAirTranslationFromOrigin,
                circleInAirRotation)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(0.0, photon.DP.Position.Z);
            // make sure initial position is inside radius
            Assert.IsTrue(Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X +
                                    photon.DP.Position.Y * photon.DP.Position.Y) <= radiusOnTissue);
            // make sure angle is not 45 degrees
            Assert.IsTrue(Math.Abs(photon.DP.Direction.Uz - (1 / Math.Sqrt(2))) > 1e-6);
        }
    }
}
