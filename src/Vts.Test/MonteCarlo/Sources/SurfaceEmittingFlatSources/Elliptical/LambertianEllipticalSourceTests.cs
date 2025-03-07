using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: LambertianEllipticalSources
    /// </summary>
    [TestFixture]
    public class LambertianEllipticalSourceTests
    {
        private static SurfaceEmitting2DSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void Setup_validation_data()
        {
            if (_validationData != null) return;
            _validationData = new SurfaceEmitting2DSourcesValidationData();
            _validationData.ReadData();
        }
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LambertianEllipticalSourceInput();
            Assert.That(si, Is.InstanceOf<LambertianEllipticalSourceInput>());
            // check full definition
            si = new LambertianEllipticalSourceInput(
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                1,
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0
            );
            Assert.That(si, Is.InstanceOf<LambertianEllipticalSourceInput>());
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.InstanceOf<LambertianEllipticalSource>());
        }

        /// <summary>
        /// Validate General Constructor of Lambertian Flat Elliptical Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_lambertian_elliptical_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new LambertianEllipticalSource(
                _validationData.AParameter, 
                _validationData.BParameter, 
                profile, 
                _validationData.PolRange, 
                _validationData.AziRange, 
                _validationData.LambertOrder,
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[109]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[110]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[111]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[112]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[113]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[114]), Is.LessThan(_validationData.AcceptablePrecision));
        }

        /// <summary>
        /// Validate General Constructor of Lambertian Gaussian Elliptical Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_lambertian_elliptical_source_test()
        {
            Random rng = new MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new LambertianEllipticalSource(
                _validationData.AParameter, 
                _validationData.BParameter, 
                profile, 
                _validationData.PolRange, 
                _validationData.AziRange, 
                _validationData.LambertOrder,
                _validationData.Direction,
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[115]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[116]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[117]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[118]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[119]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[120]), Is.LessThan(_validationData.AcceptablePrecision));
        }

        /// <summary>
        /// test switch statement in GetFinalPositionFromProfileType method for setting other
        /// than Flat or Gaussian verify exception is thrown
        /// </summary>
        [Test]
        public void Verify_that_source_profile_not_set_to_flat_or_Gaussian_throws_exception()
        {
            var tissue = new MultiLayerTissue();
            var source = new LambertianEllipticalSource(
                1.0,
                1.0,
                new FakeSourceProfile(),
                new DoubleRange(),
                new DoubleRange(),
                1,
                new Direction(),
                new Position(),
                new PolarAzimuthalAngles(),
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
