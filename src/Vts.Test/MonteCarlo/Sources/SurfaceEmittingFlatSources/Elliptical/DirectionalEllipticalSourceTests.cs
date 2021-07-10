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
    /// Unit tests for Surface Emitting Sources: DirectionalCircularSources
    /// </summary>
    [TestFixture]
    public class DirectionalEllipticalSourcesTests
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
        /// Validate General Constructor of Directional Flat Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_directional_elliptical_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new DirectionalEllipticalSource(
                _validationData.PolarAngle, 
                _validationData.AParameter, 
                _validationData.BParameter, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[61]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[62]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[63]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[64]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[65]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[66]),  _validationData.AcceptablePrecision);
        }

        /// <summary>
        /// Validate General Constructor of directional Gaussian Elliptical Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_directional_elliptical_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new DirectionalEllipticalSource(
                _validationData.PolarAngle, 
                _validationData.AParameter, 
                _validationData.BParameter, 
                profile, 
                _validationData.Direction,
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[67]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[68]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[69]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[70]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[71]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[72]),  _validationData.AcceptablePrecision);
        }

    }
}
