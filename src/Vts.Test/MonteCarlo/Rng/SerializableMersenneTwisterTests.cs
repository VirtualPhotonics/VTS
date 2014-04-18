using NUnit.Framework;
using Vts.MonteCarlo.Rng;

namespace Vts.Test.Common
{
    [TestFixture] 
    public class SerializableMersenneTwisterTests
    {
        [Test]
        public void validate_saved_random_number_state_is_correct()
        {
            int seed = 0;
            // normal processing
            var rng = new SerializableMersenneTwister(seed);
            var rng2 = rng.NextDouble();
            var rng3 = rng.NextDouble();
            var rng4 = rng.NextDouble();
            // saved processing
            rng.ToFile(rng, "savedRNG.xml");
            var savedRNG = SerializableMersenneTwister.FromFile("savedRNG.xml");
            // saved processing next rng
            var savedRNG5 = savedRNG.NextDouble();
            // normal processing next rng
            var rng5 = rng.NextDouble();
            Assert.IsTrue(rng5 == savedRNG5);
        }

    }

}
