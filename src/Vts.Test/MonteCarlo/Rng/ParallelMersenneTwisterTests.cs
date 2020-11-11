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
            // this tries to find a small MT with period 2^521-1
            var mts = rng.get_mt_parameter(32, 521);
            Assert.IsTrue(mts.state != null);
            rng.sgenrand_mt(3241, mts);
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    rng.genrand_mt(mts);
                }
            }
  

            
        }

    }

}
