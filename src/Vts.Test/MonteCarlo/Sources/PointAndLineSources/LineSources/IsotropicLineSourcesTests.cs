﻿using System;
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
    /// Unit tests for Isotropic Line Sources
    /// </summary>
    [TestFixture]
    public class IsotropicLineSourceTests
    {
        private static LineSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void Setup_validation_data()
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
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new IsotropicLineSourceInput();
            Assert.That(si, Is.Not.Null);
            // check full definition
            si = new IsotropicLineSourceInput(
                    1.0,
                    new FlatSourceProfile(),
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
        /// Validate General Constructor of Isotropic Flat Line Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_isotropic_line_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new IsotropicLineSource(
                _validationData.LengthX, profile, 
                _validationData.Direction,
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[42]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[43]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[44]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[45]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[46]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[47]), Is.LessThan(_validationData.AcceptablePrecision));
        }

        /// <summary>
        /// Validate General Constructor of Isotropic Gaussian Line Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_isotropic_line_source_test()
        {

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new IsotropicLineSource(
                _validationData.LengthX, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[48]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[49]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[50]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[51]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[52]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[53]), Is.LessThan(_validationData.AcceptablePrecision));
        }
        /// <summary>
        /// Validate general constructor and implicitly validate GetFinalPosition
        /// and GetFinalDirection
        /// </summary>
        [Test]
        public void Validate_LineAngledFromLineSource_general_constructor()
        {
            var tissue = new MultiLayerTissue();
            var source = new LineAngledFromLineSource(
                10.0, // tissue line length
                new FlatSourceProfile(),
                new Position(0,0,0),
                1.0, // line in air length
                new Position(0, 0, -10), // center of line in air
                0);
            var photon = source.GetNextPhoton(tissue);
            // Position.X will be random between [-5 5] and Y and Z should be 0
            Assert.That(photon.DP.Position.X < 5, Is.True);
            Assert.That(photon.DP.Position.X > -5, Is.True);
            Assert.That(photon.DP.Position.Y, Is.EqualTo(0.0));
            Assert.That(photon.DP.Position.Z, Is.EqualTo(0.0));
            // Direction.Ux,Uz will be random but Uy should be 0
            Assert.That(photon.DP.Direction.Uy, Is.EqualTo(0.0));
        }

    }
}
