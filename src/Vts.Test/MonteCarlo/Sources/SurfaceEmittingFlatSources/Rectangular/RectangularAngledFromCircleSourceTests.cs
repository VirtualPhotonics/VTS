using System;
using System.IO;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: RectangularAngledFromCircleSource
    /// </summary>
    [TestFixture]
    public class RectangularAngledFromCircleSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new RectangularAngledFromCircleSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new RectangularAngledFromCircleSourceInput(
                    1.0,
                    2.0,
                    new FlatSourceProfile(),
                    0.1,
                    SourceDefaults.DefaultPosition.Clone(),
                    0
            );
            Assert.IsNotNull(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsNotNull(source);
        }

        /// <summary>
        /// This test is validated by geometrically determined results
        /// </summary>
        [Test]
        public void validate_RectangularAngledFromCircle_source()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var rectLengthX = 10.0;
            var rectWidthY = 5.0;
            var radiusInAir = 0.1;
            var circleInAirTranslationFromOrigin = new Position(0, 0, -10);

            var ps = new RectangularAngledFromCircleSource(
                rectLengthX, 
                rectWidthY,
                profile,
                radiusInAir, 
                circleInAirTranslationFromOrigin,
                0)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(photon.DP.Position.Z, 0.0);
            // make sure initial position is inside rectangle
            Assert.IsTrue((photon.DP.Position.X < 5)&&(photon.DP.Position.X > -5));
            Assert.IsTrue((photon.DP.Position.Y < 2.5) && (photon.DP.Position.Y > -2.5));
            // make sure angle is less than 45 degrees
            Assert.IsTrue(photon.DP.Direction.Uz >= 1 / Math.Sqrt(2));
        }
    }
}
