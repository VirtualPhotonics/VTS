using System;
using System.IO;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Custom Line Sources
    /// </summary>
    [TestFixture]
    public class CustomLineSourceTests
    {
        private static LineSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void setup_validation_data()
        {
            if (_validationData == null)
            {
                _validationData = new LineSourcesValidationData();
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
            var si = new CustomLineSourceInput();
            Assert.IsInstanceOf<CustomLineSourceInput>(si);
            // check full definition
            si = new CustomLineSourceInput(
                    1.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultFullPolarAngleRange.Clone(),
                    SourceDefaults.DefaultAzimuthalAngleRange.Clone(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                    0
            );
            Assert.IsInstanceOf<CustomLineSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<CustomLineSource>(source);
        }

        /// <summary>
        /// Validate General Constructor of Custom Flat Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_custom_line_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();             
            var profile = new FlatSourceProfile();

            var ps = new CustomLineSource(
                _validationData.LengthX, 
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[18]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[19]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[20]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[21]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[22]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[23]), _validationData.AcceptablePrecision);
        }


        /// <summary>
        /// Validate General Constructor of Custom Gaussian Line Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_custom_line_source_test()
        {         

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new CustomLineSource(
                _validationData.LengthX, 
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

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[24]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[25]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[26]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[27]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[28]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[29]), _validationData.AcceptablePrecision);
        }

    }
}
