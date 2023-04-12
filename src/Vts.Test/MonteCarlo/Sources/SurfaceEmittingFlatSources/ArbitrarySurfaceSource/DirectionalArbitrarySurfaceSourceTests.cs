using NUnit.Framework;
using System;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: DirectionalArbitrarySurfaceSources
    /// </summary>
    [TestFixture]
    public class DirectionalArbitrarySurfaceSourceTests
    {
        private static SurfaceEmitting2DSourcesValidationData _validationData;

        [OneTimeSetUp]
        public void Setup_validation_data()
        {
            // set up for first test
            if (_validationData != null) return;
            _validationData = new SurfaceEmitting2DSourcesValidationData();
            _validationData.ReadData();

            // set up for second test

        }
        
        /// <summary>
        /// Validate General Constructor of Directional Arbitrary Surface Source designated as flat
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_specification_for_directional_arbitrary_surface_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            // make a flat source
            var image = new double[] { 1 }; 

            // DirectionalArbitrarySurfaceSource is protected so create 

            var ps = new DirectionalArbitrarySource(
                _validationData.PolarAngle,
                _validationData.LengthX,
                _validationData.WidthY,
                new ArbitrarySourceProfile(image, 1, 1),
                _validationData.Direction,
                _validationData.Translation,
                _validationData.AngPair,
                0)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            // fix after confirm GetDirection code
            //Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[3]), _validationData.AcceptablePrecision);
            //Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[4]), _validationData.AcceptablePrecision);
            //Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[0]), _validationData.AcceptablePrecision);

            Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[6]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[7]), _validationData.AcceptablePrecision);
            Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[8]), _validationData.AcceptablePrecision);
        }


    }
}
