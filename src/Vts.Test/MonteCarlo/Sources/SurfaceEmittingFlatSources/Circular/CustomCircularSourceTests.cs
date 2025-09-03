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
    /// Unit tests for Surface Emitting Sources: CustomCircularSources
    /// </summary>
    [TestFixture]
    public class CustomCircularSourceTests
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
            var si = new CustomCircularSourceInput();
            Assert.That(si, Is.InstanceOf<CustomCircularSourceInput>());
            // check full definition
            si = new CustomCircularSourceInput(
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0
            );
            Assert.That(si, Is.InstanceOf<CustomCircularSourceInput>());
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.InstanceOf<CustomCircularSource>());
        }

        /// <summary>
        /// Validate General Constructor of Custom Flat Circular Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_custom_circular_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();             
            var profile = new FlatSourceProfile();

            var ps = new CustomCircularSource(
                _validationData.OutRad, 
                _validationData.InRad, 
                profile, 
                _validationData.PolRange, 
                _validationData.AziRange, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[25]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[26]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[27]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[28]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[29]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[30]), Is.LessThan(_validationData.AcceptablePrecision));
        }

        /// <summary>
        /// Validate General Constructor of Custom Gaussian Circular Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_custom_circular_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new CustomCircularSource(
                _validationData.OutRad, 
                _validationData.InRad, profile, 
                _validationData.PolRange, 
                _validationData.AziRange,
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[31]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[32]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[33]), Is.LessThan( _validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[34]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[35]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[36]), Is.LessThan( _validationData.AcceptablePrecision));
        }

        /// <summary>
        /// test switch statement in GetFinalPositionFromProfileType method for setting other
        /// than Flat or Gaussian verify exception is thrown
        /// </summary>
        [Test]
        public void Verify_that_source_profile_not_set_to_flat_or_Gaussian_throws_exception()
        {
            var tissue = new MultiLayerTissue();
            var source = new CustomCircularSource(
                1.0,
                1.0,
                new FakeSourceProfile(),
                new DoubleRange(),
                new DoubleRange(),
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
            {
            }

            /// <summary>
            /// Returns Mock profile type
            /// </summary>
            public SourceProfileType SourceProfileType => 
                (SourceProfileType)Enum.GetNames(typeof(SourceProfileType)).Length + 1;
        }

    }

}
