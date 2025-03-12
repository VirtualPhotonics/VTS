using MathNet.Numerics.Random;
using NUnit.Framework;
using System;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Lambertian Line Sources
    /// </summary>
    [TestFixture]
    public class LambertianLineSourceTests
    {
        private static LineSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void Setup_validation_data()
        {
            if (_validationData != null) return;
            _validationData = new LineSourcesValidationData();
            _validationData.ReadData();
        }

        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LambertianLineSourceInput();
            Assert.That(si, Is.Not.Null);
            // check full definition
            si = new LambertianLineSourceInput(
                    1.0, // line length
                    new FlatSourceProfile(),
                    1, // Lambert order
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                    0
            );
            Assert.That(si, Is.Not.Null);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.Not.Null);
        }

        /// <summary>
        /// Validate General Constructor of Lambertian Flat Line Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_lambertian_line_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new LambertianLineSource(
                _validationData.LengthX, 
                profile,
                _validationData.LambertOrder,
                _validationData.Direction,
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[54]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[55]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[56]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[57]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[58]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[59]), Is.LessThan(_validationData.AcceptablePrecision));
        }

        /// <summary>
        /// Validate General Constructor of Lambertian Gaussian Line Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_lambertian_line_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new LambertianLineSource(
                _validationData.LengthX, 
                profile,
                _validationData.LambertOrder,
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[60]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[61]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[62]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[63]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[64]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[65]), Is.LessThan(_validationData.AcceptablePrecision));
        }

    }
}
