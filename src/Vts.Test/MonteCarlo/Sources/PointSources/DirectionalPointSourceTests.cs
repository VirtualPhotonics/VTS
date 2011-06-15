using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class DirectionalPointSourceTests
    {
        [Test]
        public void validate_general_constructor_with_getnextphoton_assigns_correct_values()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var position = new Position(1.0, 2.0, 3.0);
            var direction = new Direction(1.0, 0.0, 0.0);

            var ps = new DirectionalPointSource(direction, position)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.0), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 3.0), 0.0000000001);            
        }

        [Test]
        public void validate_general_constructor_with_no_optionalparameters_and_getnextphoton_assigns_correct_values()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove
            
            var direction = new Direction(0.0, 1.0, 0.0);

            var ps = new DirectionalPointSource(direction)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.0), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 0.0), 0.0000000001);  
        }        
    }
}
