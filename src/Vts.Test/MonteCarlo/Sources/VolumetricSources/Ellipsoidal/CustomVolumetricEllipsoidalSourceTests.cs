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
    /// Unit tests for Surface Emitting Sources: CustomVolumetricEllipsoidalSources
    /// </summary>
    [TestFixture]
    public class CustomVolumetricEllipsoidalSourceTests
    {
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new CustomVolumetricEllipsoidalSourceInput();
            Assert.That(si, Is.InstanceOf<CustomVolumetricEllipsoidalSourceInput>());
            // check full definition
            si = new CustomVolumetricEllipsoidalSourceInput(
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
            Assert.That(si, Is.InstanceOf<CustomVolumetricEllipsoidalSourceInput>());
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.InstanceOf<CustomVolumetricEllipsoidalSource>());
        }
        /// <summary>
        /// Validate General Constructor of Custom Flat VolumetricEllipsoidal Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_custom_VolumetricEllipsoidal_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var aParameter = 2.0;
            var bParameter = 2.0;
            var cParameter = 3.0;
            var polarRange = new DoubleRange(0, Math.PI, 2);
            var azimuthalRange = new DoubleRange(0, 2 * Math.PI, 2);
            var axisDirection = new Direction(0, 0, 1);
            var center = new Position(0, 0, 4);

            var ps = new CustomVolumetricEllipsoidalSource(
                aParameter, 
                bParameter,
                cParameter,
                profile,
                polarRange,
                azimuthalRange,
                axisDirection, 
                center, 
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (var i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                var inside =
                    (photon.DP.Position.X - center.X) * (photon.DP.Position.X - center.X) / (aParameter * aParameter) +
                    (photon.DP.Position.Y - center.Y) * (photon.DP.Position.Y - center.Y) / (bParameter * bParameter) +
                    (photon.DP.Position.Z - center.Z) * (photon.DP.Position.Z - center.Z) / (cParameter * cParameter);
                // check position is inside volume
                Assert.That(inside <= 1.0, Is.True);
            }

        }

        /// <summary>
        /// Validate General Constructor of Custom Gaussian VolumetricEllipsoidal Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_custom_VolumetricEllipsoidal_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(1.0);
            var aParameter = 2.0;
            var bParameter = 2.0;
            var cParameter = 3.0;
            var polarRange = new DoubleRange(0, Math.PI, 2);
            var azimuthalRange = new DoubleRange(0, 2 * Math.PI, 2);
            var axisDirection = new Direction(0, 0, 1);
            var center = new Position(0, 0, 4);

            var ps = new CustomVolumetricEllipsoidalSource(
                aParameter,
                bParameter,
                cParameter,
                profile,
                polarRange,
                azimuthalRange,
                axisDirection,
                center,
                1)
            {
                Rng = rng
            };
            // check 10 photons
            for (var i = 0; i < 10; i++)
            {
                var photon = ps.GetNextPhoton(tissue);
                var inside = 
                    (photon.DP.Position.X - center.X) / aParameter + (photon.DP.Position.X - center.X) / aParameter +
                    (photon.DP.Position.Y - center.Y) / bParameter + (photon.DP.Position.Y - center.Y) / bParameter +
                    (photon.DP.Position.Z - center.Z) / cParameter + (photon.DP.Position.Z - center.Z) / cParameter;
                // check position is inside volume
                Assert.That(inside <= 1.0, Is.True);
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
            var source = new CustomVolumetricEllipsoidalSource(
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
                (SourceProfileType) Enum.GetNames(typeof(SourceProfileType)).Length + 1;

        }

    }
}