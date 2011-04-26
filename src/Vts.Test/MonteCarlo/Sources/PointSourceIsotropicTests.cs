using System;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo.Sources
{
    [TestFixture]
    public class IsotropicPointSourceTests
    {
        [Test]
        public void validate_default_constructor_with_getnextphoton_assigns_correct_values()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var ps = new IsotropicPointSource()
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.IsTrue(
                photon.DP.Position.X == 0.0 &&
                photon.DP.Position.Y == 0.0 &&
                photon.DP.Position.Z == 0.0
           );
        }

        [Test]
        public void validate_general_constructor_with_getnextphoton_assigns_correct_values()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var position = new Position(1.0, 2.0, 3.0);

            var ps = new IsotropicPointSource(position)
            {
                Rng = rng
            };

            var photon = ps.GetNextPhoton(tissue);

            Assert.IsTrue(
                photon.DP.Position.X == 1.0 &&
                photon.DP.Position.Y == 2.0 &&
                photon.DP.Position.Z == 3.0
           );
        }

        [Test]
        public void validate_GetNextPhoton_returns_nonuniform_directions()
        {
            Random rng = new MathNet.Numerics.Random.MersenneTwister(0); // not really necessary here, as this is now the default
            ITissue tissue = new MultiLayerTissue(); // todo: remove

            var ps = new IsotropicPointSource()
            {
                Rng = rng
            };

            var photon1 = ps.GetNextPhoton(tissue);
            var photon2 = ps.GetNextPhoton(tissue);
            var photon3 = ps.GetNextPhoton(tissue);

            Assert.IsFalse(photon1.DP.Direction == photon2.DP.Direction);
            Assert.IsFalse(photon2.DP.Direction == photon3.DP.Direction);
        }
    }
}
