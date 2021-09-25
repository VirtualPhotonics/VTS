using System;
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
    public class DirectionalArbitrarySurfaceSourceTests
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
        /// Validate General Constructor of Directional Flat Arbitrary Surface Source
        /// </summary>
        [Test]
        public void Validate_general_constructor_with_flat_profiletype_for_directional_arbitrary_surface_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            var profile = new ArbitrarySourceProfile();

            // DirectionalArbitrarySurfaceSource is protected so create 

            //var ps = new DirectionalArbitrarySurfaceSource(
            //    _validationData.PolarAngle, 
            //    sourceLengthX,
            //    sourceWidthY,
            //    profile, 
            //    _validationData.Direction, 
            //    _validationData.Translation, 
            //    _validationData.AngPair,
            //    0)
            //{
            //    Rng = rng
            //};

            //var photon = ps.GetNextPhoton(tissue);

            //Assert.Less(Math.Abs(photon.DP.Direction.Ux - _validationData.Tp[85]), _validationData.AcceptablePrecision);
            //Assert.Less(Math.Abs(photon.DP.Direction.Uy - _validationData.Tp[86]), _validationData.AcceptablePrecision);
            //Assert.Less(Math.Abs(photon.DP.Direction.Uz - _validationData.Tp[87]), _validationData.AcceptablePrecision);

            //Assert.Less(Math.Abs(photon.DP.Position.X - _validationData.Tp[88]), _validationData.AcceptablePrecision);
            //Assert.Less(Math.Abs(photon.DP.Position.Y - _validationData.Tp[89]), _validationData.AcceptablePrecision);
            //Assert.Less(Math.Abs(photon.DP.Position.Z - _validationData.Tp[90]), _validationData.AcceptablePrecision);
        }


    }
}
