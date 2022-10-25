using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for LambertianSurfaceEmittingCuboidalSource
    /// </summary>
    [TestFixture]
    public class LambertianSurfaceEmittingCuboidalSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LambertianSurfaceEmittingCuboidalSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new LambertianSurfaceEmittingCuboidalSourceInput(
                    1.0,
                    1.0,
                    1.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    0
            );
            Assert.IsNotNull(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsNotNull(source);
        }
        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_off_surface_of_cuboidal()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var cubeLengthX = 1.0;
            var cubeWidthY = 2.0;
            var cubeHeightZ = 2.0;
            var profile = new FlatSourceProfile();
            var directionAxis = new Direction(0, 0, 1);
            var translationFromOrigin = new Position(0, 0, 4);

            var ps = new LambertianSurfaceEmittingCuboidalSource(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                profile,
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
                // if on x is on 1 of 2 constant faces, make sure y and z are within boundaries
                if (Math.Abs(photon.DP.Position.X + cubeLengthX / 2 + translationFromOrigin.X) < 1e-10 ||
                    Math.Abs(photon.DP.Position.X - cubeLengthX / 2 + translationFromOrigin.X) < 1e-10)
                {
                    Assert.IsTrue(photon.DP.Position.Y < cubeWidthY / 2 + translationFromOrigin.Y &&
                                  photon.DP.Position.Y > -cubeWidthY / 2 + translationFromOrigin.Y);
                    Assert.IsTrue(photon.DP.Position.Z < cubeHeightZ / 2 + translationFromOrigin.Z &&
                                  photon.DP.Position.Z > -cubeHeightZ / 2 + translationFromOrigin.Z);
                }
                // if on y is on 1 of 2 constant faces, make sure x and z are within boundaries
                if (Math.Abs(photon.DP.Position.Y + cubeWidthY / 2 + translationFromOrigin.Y) < 1e-10 ||
                    Math.Abs(photon.DP.Position.Y - cubeWidthY / 2 + translationFromOrigin.Y) < 1e-10)
                {
                    Assert.IsTrue(photon.DP.Position.X < cubeLengthX / 2 + translationFromOrigin.X &&
                                  photon.DP.Position.X > -cubeLengthX / 2 + translationFromOrigin.X);
                    Assert.IsTrue(photon.DP.Position.Z < cubeHeightZ / 2 + translationFromOrigin.Z &&
                                  photon.DP.Position.Z > -cubeHeightZ / 2 + translationFromOrigin.Z);
                }
                // if on z is on 1 of 2 constant faces, make sure x and y are within boundaries
                if (Math.Abs(photon.DP.Position.Z - cubeHeightZ / 2 - translationFromOrigin.Z) < 1e-10 ||
                    Math.Abs(photon.DP.Position.Z + cubeHeightZ / 2 - translationFromOrigin.Z) < 1e-10)
                {
                    Assert.IsTrue(photon.DP.Position.X < cubeLengthX / 2 + translationFromOrigin.X &&
                                  photon.DP.Position.X > -cubeLengthX / 2 + translationFromOrigin.X);
                    Assert.IsTrue(photon.DP.Position.Y < cubeWidthY / 2 + translationFromOrigin.Y &&
                                  photon.DP.Position.Y > -cubeWidthY / 2 + translationFromOrigin.Y);
                }
            }
        }
        /// <summary>
        /// test switch statement in GetFinalPositionFromProfileType method for setting other
        /// than Flat or Gaussian verify exception is thrown
        /// </summary>
        [Test]
        public void Verify_that_source_profile_not_set_to_flat_or_Gaussian_throws_exception()
        {
            var tissue = new MultiLayerTissue();
            var source = new LambertianSurfaceEmittingCuboidalSource(
                1.0,
                1.0,
                1.0,
                new FakeSourceProfile(),
                new Direction(),
                new Position(),
                1
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => source.GetNextPhoton(tissue));
        }
        public class FakeSourceProfile : ISourceProfile
        {
            /// <summary>
            /// Initializes the default constructor of FakeSourceProfile class
            /// for testing purposes
            /// </summary>
            public FakeSourceProfile()
            { }

            /// <summary>
            /// Returns Mock profile type
            /// </summary>
            public SourceProfileType SourceProfileType =>
                (SourceProfileType)Enum.GetNames(typeof(SourceProfileType)).Length + 1;

        }
    }
}
