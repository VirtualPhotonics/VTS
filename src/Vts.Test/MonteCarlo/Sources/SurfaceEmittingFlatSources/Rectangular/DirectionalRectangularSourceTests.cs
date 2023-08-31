using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: DirectionalRectangularSources
    /// </summary>
    [TestFixture]
    public class DirectionalRectangularSourcesTests
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
            var si = new DirectionalRectangularSourceInput();
            Assert.IsInstanceOf<DirectionalRectangularSourceInput>(si);
            // check full definition
            si = new DirectionalRectangularSourceInput(
                0.0,
                1.0,
                2.0,
                new FlatSourceProfile(),
                SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                SourceDefaults.DefaultPosition.Clone(),
                SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                0
            );
            Assert.IsInstanceOf<DirectionalRectangularSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<DirectionalRectangularSource>(source);
        }
        /// <summary>
        /// Validate General Constructor of Directional Flat Rectangular Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_directional_rectangular_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new DirectionalRectangularSource(
                _validationData.PolarAngle, 
                _validationData.LengthX, 
                _validationData.WidthY, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[85]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[86]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[87]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[88]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[89]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[90]),  _validationData.AcceptablePrecision);
        }


        /// <summary>
        /// Validate General Constructor of directional Gaussian Rectangular Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_directional_rectangular_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new DirectionalRectangularSource(
                _validationData.PolarAngle, 
                _validationData.LengthX, 
                _validationData.WidthY, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[91]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[92]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[93]),  _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[94]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[95]),  _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[96]),  _validationData.AcceptablePrecision);
        }

        /// <summary>
        /// This test different from others in that it is validated by geometrically
        /// determined results
        /// </summary>
        [Test]
        public void Validate_CircularAngledFromPoint_source()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            const double radius = 1.0;
            var pointLocation = new Position(0, 0, -1); // put directly above
            var translationFromOrigin = new Position(0, 0, 0);  

            var ps = new CircularAngledFromPointSource(radius, profile, pointLocation, translationFromOrigin)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(0.0, photon.DP.Position.Z);
            // make sure initial position is inside radius
            Assert.IsTrue(Math.Sqrt(
                (photon.DP.Position.X - translationFromOrigin.X) *
                (photon.DP.Position.X - translationFromOrigin.X) +
                (photon.DP.Position.Y - translationFromOrigin.Y) *
                (photon.DP.Position.Y - translationFromOrigin.Y)) <= radius);
            // make sure angle is less than 45 degrees
            Assert.IsTrue(photon.DP.Direction.Uz >= 1 / Math.Sqrt(2));
        }
        /// <summary>
        /// This test different from others in that it is validated by geometrically
        /// determined results
        /// </summary>
        [Test]
        public void Validate_CircularAngledFromCircle_source()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();
            var radiusOnTissue = 10.0;
            var translationFromOrigin = new Position(0, 0, 0);
            const double radiusInAir = 0.0;
            var circleInAirTranslationFromOrigin = new Position(0, 0, -10);
            var circleInAirRotation = new Direction(0, 0, 1); // make perpendicular

            var ps = new CircularAngledFromCircleSource(
                radiusOnTissue, 
                profile,
                translationFromOrigin,
                radiusInAir, 
                circleInAirTranslationFromOrigin,
                circleInAirRotation)
            {
                Rng = rng
            };
            var photon = ps.GetNextPhoton(tissue);
            // make sure initial position is at tissue surface
            Assert.AreEqual(0.0, photon.DP.Position.Z);
            // make sure initial position is inside radius
            Assert.IsTrue(Math.Sqrt(photon.DP.Position.X * photon.DP.Position.X +
                                    photon.DP.Position.Y * photon.DP.Position.Y) <= radiusOnTissue);
            // make sure angle is less than 45 degrees
            Assert.IsTrue(photon.DP.Direction.Uz >= 1 / Math.Sqrt(2));
        }
    }
}
