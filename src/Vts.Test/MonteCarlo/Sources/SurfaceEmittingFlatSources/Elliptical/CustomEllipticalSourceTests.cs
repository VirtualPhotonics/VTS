using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
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
        private static SourceValidationData _validationData;

        [OneTimeSetUp]
        public void setup_validation_data()
        {
            if (_validationData == null)
            {
                _validationData = new SourceValidationData();
                _validationData.ReadData();
            }
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

    }
}
