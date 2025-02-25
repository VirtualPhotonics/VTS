using System;
using MathNet.Numerics.Random;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Lambertian Point Sources
    /// </summary>
    [TestFixture]
    public class LambertianPointSourceTests
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
        public void Validate_source_input_with_flat_profile_type()
        {
            // check default constructor
            var si = new LambertianPointSourceInput();
            Assert.IsNotNull(si);
            // check full definition
            si = new LambertianPointSourceInput(
                    new Position(0, 0, 0),
                    0
            );
            Assert.IsInstanceOf<LambertianPointSourceInput>(si);
            // validate CreateSource
            var source = si.CreateSource(new MersenneTwister(0));
            Assert.IsInstanceOf<LambertianPointSource>(source);
        }

        /// <summary>
        /// Validate General Constructor of Lambertian Point Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_lambertian_point_source_test()
        {
            Random rng = new MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();         

            var ps = new LambertianPointSource(
                _validationData.Translation, 
                _validationData.LambertOrder, 
                0)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[34]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[35]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[36]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[37]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[38]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[39]), _validationData.AcceptablePrecision);
        }

        /// <summary>
        /// Test general constructor and verify that position is correct
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_position()
        {
            var position = new Position(1.0, 2.0, 3.0);

            var ps = new LambertianPointSourceInput(position, 0)
            {
                SourceType = null,
                LambertOrder = 1
            };

            Assert.IsTrue(
                ps.PointLocation.X == 1.0 &&
                ps.PointLocation.Y == 2.0 &&
                ps.PointLocation.Z == 3.0
            );
        }

        /// <summary>
        /// test default constructor and verify position is correct
        /// </summary>
        [Test]
        public void Validate_default_constructor_with_position()
        {
            var ps = new LambertianPointSourceInput();

            Assert.IsTrue(
                ps.PointLocation != null &&
                ps.PointLocation.X == 0.0 &&
                ps.PointLocation.Y == 0.0 &&
                ps.PointLocation.Z == 0.0
            );
        }
    }
}
