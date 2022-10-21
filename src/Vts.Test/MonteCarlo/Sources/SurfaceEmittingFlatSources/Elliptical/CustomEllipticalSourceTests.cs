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
    /// Unit tests for Surface Emitting Sources: CustomEllipticalSources
    /// </summary>
    [TestFixture]
    public class CustomEllipticalSourceTests
    {
        private static SurfaceEmitting2DSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void setup_validation_data()
        {
            if (_validationData == null)
            {
                _validationData = new SurfaceEmitting2DSourcesValidationData();
                _validationData.ReadData();
            }
        }
        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new CustomEllipticalSourceInput();
            Assert.IsInstanceOf<CustomEllipticalSourceInput>(si);
            // check full definition
            si = new CustomEllipticalSourceInput(
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultHalfPolarAngleRange.Clone(),
                SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                0
            );
            Assert.IsInstanceOf<CustomEllipticalSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<CustomEllipticalSource>(source);
        }

        /// <summary>
        /// Validate General Constructor of Custom Flat Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_elliptical_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new CustomEllipticalSource(
                _validationData.AParameter, 
                _validationData.BParameter, 
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[49]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[50]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[51]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[52]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[53]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[54]),  _validationData.AcceptablePrecision);
        }

        /// <summary>
        /// Validate General Constructor of Custom Gaussian Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_elliptical_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new CustomEllipticalSource(
                _validationData.AParameter, 
                _validationData.BParameter, 
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[55]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[56]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[57]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[58]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[59]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[60]),  _validationData.AcceptablePrecision);
        }
        /// <summary>
        /// test switch statement in GetFinalPositionFromProfileType method for setting other
        /// than Flat or Gaussian verify exception is thrown
        /// </summary>
        [Test]
        public void verify_that_source_profile_not_set_to_flat_or_Gaussian_throws_exception()
        {
            var tissue = new MultiLayerTissue();
            var source = new CustomEllipticalSource(
                1.0,
                1.0,
                new MockSourceProfile(),
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
        public class MockSourceProfile : ISourceProfile
        {
            /// <summary>
            /// Initializes the default constructor of MockSourceProfile class
            /// for testing purposes
            /// </summary>
            public MockSourceProfile()
            { }

            /// <summary>
            /// Returns Mock profile type
            /// </summary>
            public SourceProfileType SourceProfileType => (SourceProfileType)3;
        }

    }
}
