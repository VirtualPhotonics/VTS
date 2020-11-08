using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo.Rng;

namespace Vts.Test.MonteCarlo
{
    [TestFixture] 
    public class ParalleleMersenneTwisterTests
    {
        [Test]
        public void validate_example1()
        {
            int seed = 0;
            // normal processing
            var rng = new ParallelMersenneTwister(seed);
            // example 1 inputs
            rng.init_dc(4172);
            
            var rng2 = rng.NextDouble();
            var rng3 = rng.NextDouble();
            var rng4 = rng.NextDouble();
            
        }

    }

}
