﻿using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Isotropic Point Sources
    /// </summary>
    [TestFixture]
    public class IsotropicPointSourceTests
    {
        private static PointSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void Setup_validation_data()
        {
            if (_validationData != null) return;
            _validationData = new PointSourcesValidationData();
            _validationData.ReadData();
        }

        /// <summary>
        /// test source input
        /// </summary>
        [Test]
        public void Validate_source_input_constructor()
        {
            // check default constructor
            var si = new IsotropicPointSourceInput();
            Assert.That(si, Is.Not.Null);
            // check full definition
            si = new IsotropicPointSourceInput(
                new Position(0, 0, 0),
                0
            );
            Assert.That(si, Is.InstanceOf<IsotropicPointSourceInput>());
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.That(source, Is.InstanceOf<IsotropicPointSource>());
        }

        /// <summary>
        /// Validate General Constructor of Isotropic Point Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_for_isotropic_point_source_test()
        {
            Random rng = new MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();         

            var ps = new IsotropicPointSource(_validationData.Translation)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.That(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[28]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[29]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[30]), Is.LessThan(_validationData.AcceptablePrecision));

            Assert.That(Math.Abs(photon.DP.Position.X - _validationData.Tp[31]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Y - _validationData.Tp[32]), Is.LessThan(_validationData.AcceptablePrecision));
            Assert.That(Math.Abs(photon.DP.Position.Z - _validationData.Tp[33]), Is.LessThan(_validationData.AcceptablePrecision));
        }

        /// <summary>
        /// Test general constructor and verify that position is correct
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_position()
        {
            var position = new Position(1.0, 2.0, 3.0);

            var ps = new IsotropicPointSourceInput(position, 0)
            {
                SourceType = null
            };

            Assert.That(
                ps.PointLocation.X == 1.0 &&
                ps.PointLocation.Y == 2.0 &&
                ps.PointLocation.Z == 3.0, Is.True);
        }

        /// <summary>
        /// test default constructor and verify position is correct
        /// </summary>
        [Test]
        public void Validate_default_constructor_with_position()
        {
            var ps = new IsotropicPointSourceInput();

            Assert.That(
                ps.PointLocation != null &&
                ps.PointLocation.X == 0.0 &&
                ps.PointLocation.Y == 0.0 &&
                ps.PointLocation.Z == 0.0, Is.True);
        }
    }
}
