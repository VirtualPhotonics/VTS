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
    /// Unit tests for LambertianSurfaceEmittingTubularSource
    /// </summary>
    [TestFixture]
    public class LambertianSurfaceEmittingTubularSourceTests
    {        
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LambertianSurfaceEmittingTubularSourceInput();
            Assert.That(si, Is.Not.Null);
            // check full definition
            si = new LambertianSurfaceEmittingTubularSourceInput(
                1.0,
                1.0,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
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
        public void Validate_starting_photons_off_surface_of_tubular()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var tubeRadius = 1.0;
            var tubeHeightZ = 2.0;
            var directionAxis = new Direction(0, 0, 1);
            var translationFromOrigin = new Position(0, 0, 1);

            var ps = new LambertianSurfaceEmittingTubularSource(
                tubeRadius,
                tubeHeightZ,
                directionAxis,
                translationFromOrigin,
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (var i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                // make sure initial x-y is on surface
                Assert.IsTrue(tubeRadius - Math.Sqrt(
                    photon.DP.Position.X * photon.DP.Position.X +
                    photon.DP.Position.Y * photon.DP.Position.Y) < 0.00001);
                // make sure initial z is within height
                Assert.IsTrue(photon.DP.Position.Z > tubeHeightZ / 2 - translationFromOrigin.Z &&
                              photon.DP.Position.Z < tubeHeightZ / 2 + translationFromOrigin.Z);

            }
        }
    }
}
