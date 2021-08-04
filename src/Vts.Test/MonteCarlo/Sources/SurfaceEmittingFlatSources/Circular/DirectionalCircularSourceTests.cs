using System;
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
    /// Unit tests for Surface Emitting Sources: CustomCircularSources
    /// </summary>
    [TestFixture]
    public class DirectionalCircularSourceTests
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
            var si = new DirectionalCircularSourceInput();
            Assert.IsInstanceOf<DirectionalCircularSourceInput>(si);
            // check full definition
            si = new DirectionalCircularSourceInput(
                    0.0,
                    1.0,
                    0.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultBeamRoationFromInwardNormal.Clone(),
                    0
            );
            Assert.IsInstanceOf<DirectionalCircularSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<DirectionalCircularSource>(source);
        }
        /// <summary>
        /// Validate General Constructor of Directional Flat Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_flat_profiletype_for_directional_circular_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new DirectionalCircularSource(
                _validationData.PolarAngle, 
                _validationData.OutRad, 
                _validationData.InRad, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[37]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[38]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[39]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[40]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[41]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[42]),  _validationData.AcceptablePrecision);
        }


        /// <summary>
        /// Validate General Constructor of directional Gaussian Circular Source
        /// </summary>
        [Test]
        public void validate_general_constructor_with_gaussian_profiletype_for_directional_circular_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new DirectionalCircularSource(
                _validationData.PolarAngle, 
                _validationData.OutRad, 
                _validationData.InRad, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[43]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[44]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[45]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[46]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[47]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[48]),  _validationData.AcceptablePrecision);
        }

        /// <summary>
        /// test to verify code produces collimated beam when thetaConvOrDiv=0
        /// </summary>
        [Test]
        public void validate_setting_thetaConvOrDiv_to_0_produces_collimated_beam()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var thetaConvOrDiv = 0.0;
            var ps = new DirectionalCircularSource(
                thetaConvOrDiv,
                0.1,
                0.0,
                profile,
                new Direction(0, 0, 1),
                new Position(0,0,0),
                new PolarAzimuthalAngles(0.0,0.0))
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);
            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 1.0), _validationData.AcceptablePrecision);

        }

    }
}
