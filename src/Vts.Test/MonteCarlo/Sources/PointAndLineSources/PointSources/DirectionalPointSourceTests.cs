﻿using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Directional Point Sources
    /// </summary>
    [TestFixture]
    public class DirectionalPointSourceTests
    {
        private static PointSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void Setup_validation_data()
        {
            if (_validationData == null)
            {
                _validationData = new PointSourcesValidationData();
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
            var si = new DirectionalPointSourceInput();
            Assert.IsInstanceOf<DirectionalPointSourceInput>(si);
            // check full definition
            si = new DirectionalPointSourceInput(
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    0
                );
            Assert.IsInstanceOf<DirectionalPointSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<DirectionalPointSource>(source);
        }
        /// <summary>
        /// Validate General Constructor of Directional Point Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_directional_point_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();       

            var ps = new DirectionalPointSource(_validationData.Direction, _validationData.Translation)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[22]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[23]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[24]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[25]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[26]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[27]), _validationData.AcceptablePrecision);    
        }
    }
}
