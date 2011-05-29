using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vts.Extensions;

namespace Vts.Test.Common
{
    [TestFixture] 
    public class RNGExtensionsTests
    {
        //[Test]
        public void validate_saved_random_number_state_is_correct()
        {
            int seed = 0;
            // normal processing
            var rng =  new MathNet.Numerics.Random.MersenneTwister(seed);
            var rng2 = rng.NextDouble();
            var rng3 = rng.NextDouble();
            var rng4 = rng.NextDouble();
            // saved processing
            RNGExtensions.ToFile(rng, "savedRNG");
            var savedRNG = RNGExtensions.FromFile("savedRNG");
            var savedRNG5 = savedRNG.NextDouble();
            var rng5 = rng.NextDouble();
            //Assert.IsTrue(rng5 == savedRNG5);
        }

    }

}
