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
        public void validate_new_example1()
        {
            // normal processing
            var rng = new ParallelMersenneTwister(0);
            // this tries to find a small MT with period 2^521-1
            ParallelMersenneTwister.mt_struct mts = rng.get_mt_parameter_st(32, 521, 4172);
            // Assert.IsTrue(mts.state != null);
            rng.sgenrand_mt(3241, mts);
            for (int i = 0; i < 100; i++)
            {
                rng.genrand_mt(mts);
            }
  

            
        }

    }

}
