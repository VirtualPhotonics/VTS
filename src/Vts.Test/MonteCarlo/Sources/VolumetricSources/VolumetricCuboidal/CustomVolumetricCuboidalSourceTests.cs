using MathNet.Numerics.Random;
using NUnit.Framework;
using System;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for CustomVolumetricCuboidalSource
    /// </summary>
    [TestFixture]
    public class CustomVolumetricCuboidalSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new CustomVolumetricCuboidalSourceInput();
            Assert.IsInstanceOf<CustomVolumetricCuboidalSourceInput>(si);
            // check full definition
            si = new CustomVolumetricCuboidalSourceInput(
                1.0,
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                0
            );
            Assert.IsInstanceOf<CustomVolumetricCuboidalSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<CustomVolumetricCuboidalSource>(source);
        }
        /// <summary>
        /// This test validated using geometry assumptions
        /// </summary>
        [Test]
        public void Validate_starting_photons_inside_of_volume_cuboidal()
        {
            Random rng =
                new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var cubeLengthX = 1.0;
            var cubeWidthY = 2.0;
            var cubeHeightZ = 2.0;
            var profile = new FlatSourceProfile();
            var polarRange = new DoubleRange(0, Math.PI / 2, 2);
            var azimuthalRange = new DoubleRange(0, 2 * Math.PI, 2);
            var directionAxis = new Direction(0, 0, 1);
            var translationFromOrigin = new Position(0, 0, 4);

            var ps = new CustomVolumetricCuboidalSource(
                cubeLengthX,
                cubeWidthY,
                cubeHeightZ,
                profile,
                polarRange,
                azimuthalRange,
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
                // make sure photons start inside cuboidal
                Assert.IsTrue((photon.DP.Position.X < cubeLengthX / 2 + translationFromOrigin.X) &&
                              (photon.DP.Position.X > -cubeLengthX / 2 + translationFromOrigin.X));
                Assert.IsTrue((photon.DP.Position.Y < cubeWidthY / 2 + translationFromOrigin.Y) &&
                              (photon.DP.Position.Y > -cubeWidthY / 2 + translationFromOrigin.Y));
                Assert.IsTrue((photon.DP.Position.Z < cubeHeightZ / 2 + translationFromOrigin.Z) &&
                              (photon.DP.Position.Z > -cubeHeightZ / 2 + translationFromOrigin.Z));

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
            var source = new CustomVolumetricCuboidalSource(
                1.0,
                1.0,
                1.0,
                new FakeSourceProfile(),
                new DoubleRange(),
                new DoubleRange(),
                new Direction(),
                new Position()
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
