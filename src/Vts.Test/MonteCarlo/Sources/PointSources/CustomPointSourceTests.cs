using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class CustomPointSourceTests
    {
        [Test]
        public void validate_general_constructor_with_getnextphoton_assigns_correct_values()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var position = new Position(1.0, 2.0, 3.0);
            var direction = new Direction(1.0, 0.0, 0.0);
            var polRange = new DoubleRange(0.0, 0.5 * Math.PI);
            var aziRange = new DoubleRange(0.0, Math.PI);

            var ps = new CustomPointSource(polRange, aziRange, direction, position)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.54881350243203653), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.800636293032631), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.240385660610712), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 1.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 2.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 3.0), 0.0000000001);            
        }
        
        [Test]
        public void validate_general_constructor_with_no_optionalparameters_and_getnextphoton_assigns_correct_values1()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove
            
            var polRange = new DoubleRange(0.0, 0.5 * Math.PI);
            var aziRange = new DoubleRange(0.0, Math.PI);

            var ps = new CustomPointSource(polRange, aziRange)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux + 0.240385660610712), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.800636293032631), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 0.54881350243203653), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 0.0), 0.0000000001);
        }

        [Test]
        public void validate_general_constructor_with_no_optionalparameters_and_getnextphoton_assigns_correct_values2()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var polRange = new DoubleRange(0.0, 0.0);
            var aziRange = new DoubleRange(0.0, 0.0);

            var ps = new CustomPointSource(polRange, aziRange)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.Less(Math.Abs(photon.DP.Direction.Ux - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uy - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Direction.Uz - 1.0), 0.0000000001);

            Assert.Less(Math.Abs(photon.DP.Position.X - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Y - 0.0), 0.0000000001);
            Assert.Less(Math.Abs(photon.DP.Position.Z - 0.0), 0.0000000001);
        }
    }
}
