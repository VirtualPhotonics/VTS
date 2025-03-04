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
            var si = new DirectionalCircularSourceInput();
            Assert.That(si, Is.InstanceOf<DirectionalCircularSourceInput>());
            // check full definition
            si = new DirectionalCircularSourceInput(
                    0.0,
                    1.0,
                    0.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                    0
            );
            Assert.That(si, Is.InstanceOf<DirectionalCircularSourceInput>());
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.InstanceOf<DirectionalCircularSource>());
        }
        /// <summary>
        /// Validate General Constructor of Directional Flat Circular Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_directional_circular_source_test()
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

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[37]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[38]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[39]), Is.LessThan( _validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[40]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[41]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[42]), Is.LessThan( _validationData.AcceptablePrecision));
        }


        /// <summary>
        /// Validate General Constructor of directional Gaussian Circular Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_directional_circular_source_test()
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

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[43]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[44]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[45]), Is.LessThan( _validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[46]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[47]), Is.LessThan( _validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[48]), Is.LessThan( _validationData.AcceptablePrecision));
        }

        /// <summary>
        /// test to verify code produces collimated beam when thetaConvOrDiv=0
        /// </summary>
        [Test]
        public void Validate_setting_thetaConvOrDiv_to_0_produces_collimated_beam()
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
            Assert.That(Math.Abs(photon.DP.Direction.Ux - 0), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - 0), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - 1.0), Is.LessThan(_validationData.AcceptablePrecision));

        }

    }
}
