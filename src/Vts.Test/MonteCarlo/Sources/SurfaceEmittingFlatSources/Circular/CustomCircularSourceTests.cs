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
    /// Unit tests for Surface Emitting Sources: CustomCircularSources
    /// </summary>
    [TestFixture]
    public class CustomCircularSourceTests
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
        /// Validate General Constructor of Custom Flat Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_circular_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();             
            var profile = new FlatSourceProfile();

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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[25]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[26]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[27]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[28]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[29]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[30]), _validationData.AcceptablePrecision);
        }


        /// <summary>
        /// Validate General Constructor of Custom Gaussian Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_circular_source_test()
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[31]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[32]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[33]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[34]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[35]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[36]),  _validationData.AcceptablePrecision);
        }

    }
}
