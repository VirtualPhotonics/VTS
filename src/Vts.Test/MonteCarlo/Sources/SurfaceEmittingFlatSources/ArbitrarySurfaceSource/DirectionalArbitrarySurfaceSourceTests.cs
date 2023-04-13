using NUnit.Framework;
using System;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Helpers;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Sources.SourceProfiles;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for Surface Emitting Sources: DirectionalArbitrarySources
    /// </summary>
    [TestFixture]
    public class DirectionalArbitrarySourceTests
    {
        DirectionalArbitrarySourceInput _infile;

        [OneTimeSetUp]
        public void Setup()
        {
            //_infile = new DirectionalArbitrarySourceInput(


        }
        
        /// <summary>
        /// Instantiate ArbitrarySourceProfile and validate GetBinaryPixelMap,
        /// then use this object in general constructor of DirectionalArbitrarySource
        /// and validate initiated new Photon
        /// </summary>
        [Test]
        public void Validate_binary_map_and_general_constructor_for_directional_arbitrary_source_test()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue();
            // make a intensity varied source
            var image = new double[4] { 1, 2, 2, 0 }; // representing a 2x2 pixel image
            var arbitrarySourceProfile = new ArbitrarySourceProfile(
                image,
                2,
                2,
                1,
                1,
                new Vts.Common.Position(0, 0, 0));
            var binaryMap = arbitrarySourceProfile.GetBinaryPixelMap();
            Assert.IsTrue(binaryMap[0] == 1);
            Assert.IsTrue(binaryMap[1] == 1);
            Assert.IsTrue(binaryMap[2] == 1);
            Assert.IsTrue(binaryMap[3] == 0);

            var ps = new DirectionalArbitrarySource(
                0.0, // normal
                2.0,
                2.0,
                arbitrarySourceProfile,
                new Direction(0, 0, 1),
                new Position(0,0,0),
                new PolarAzimuthalAngles(),
                0)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            // normal launch
            Assert.Less(Math.Abs(photon.DP.Direction.Ux), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 1), 1e-6);

            Assert.Less(Math.Abs(photon.DP.Position.X + 0.5), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 0.5), 1e-6);
            Assert.Less(Math.Abs(photon.DP.Position.Z), 1e-6);
        }
 

    }
}
