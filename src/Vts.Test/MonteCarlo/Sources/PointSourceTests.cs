using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class PointSourceTests
    {
        [Test]
        public void validate_getnextphoton_assigns_correct_values()
        {
            Random rng;
            var ps = new PointSource(
                new Position(0,0,0),
                new Direction(0,0,1),
                new DoubleRange(0,0,1),
                new DoubleRange(0,0,1),
                false);
            rng = new MathNet.Numerics.Random.MersenneTwister(0);
            var photon = ps.GetNextPhoton(new MultiLayerTissue(), rng);

            Assert.IsTrue(
                photon.DP.Position.X == 0.0 &&
                photon.DP.Position.Y == 0.0 &&
                photon.DP.Position.Z == 0.0 &&
                photon.DP.Direction.Ux == 0.0 &&
                photon.DP.Direction.Uy == 0.0 &&
                photon.DP.Direction.Uz == 1.0
           );
        }

    }
}
