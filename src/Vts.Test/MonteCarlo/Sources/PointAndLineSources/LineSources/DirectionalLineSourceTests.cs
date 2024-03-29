﻿using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Directional Line Sources
    /// </summary>
    [TestFixture]
    public class DirectionalLineSourceTests
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
            var si = new DirectionalLineSourceInput();
            Assert.IsInstanceOf<DirectionalLineSourceInput>(si);
            // check full definition
            si = new DirectionalLineSourceInput(
                    0.0,
                    1.0,
                    new FlatSourceProfile(),
                    SourceDefaults.DefaultDirectionOfPrincipalSourceAxis.Clone(),
                    SourceDefaults.DefaultPosition.Clone(),
                    SourceDefaults.DefaultBeamRotationFromInwardNormal.Clone(),
                    0
            );
            Assert.IsInstanceOf<DirectionalLineSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<DirectionalLineSource>(source);
        }

        /// <summary>
        /// Validate General Constructor of Directional Flat Line Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_directional_line_source_test()
        {            

            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new FlatSourceProfile();

            var ps = new DirectionalLineSource(
                _validationData.PolarAngle, 
                _validationData.LengthX, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[30]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[31]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[32]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[33]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[34]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[35]), _validationData.AcceptablePrecision);
        }

        /// <summary>
        /// Validate General Constructor of Directional Gaussian Line Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_gaussian_profiletype_for_directional_line_source_test()
        {            
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new GaussianSourceProfile(_validationData.BdFWHM);

            var ps = new DirectionalLineSource(
                _validationData.PolarAngle, 
                _validationData.LengthX, 
                profile, 
                _validationData.Direction, 
                _validationData.Translation, 
                _validationData.AngPair)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[36]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[37]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[38]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[39]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[40]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[41]), _validationData.AcceptablePrecision);
        }
    }
}
