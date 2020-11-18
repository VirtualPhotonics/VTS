using System;
using NUnit.Framework;
using Vts.MonteCarlo.Rng;

namespace Vts.Test.MonteCarlo
{
    [TestFixture] 
    public class ParalleleMersenneTwisterTests
    {
        [Test]
        public void validate_new_example1()
        {
            double reciprocal = 1.0 / 4294967296.0;
            double[] cCodeResults = new double[]
                { 0.812134, 0.759003, 0.235254, 0.194382, 0.928250,
                  0.726841, 0.037951, 0.504746, 0.382553, 0.700802 };
            double RN;
            // normal processing
            var rng = new ParallelMersenneTwister(0);
            // this tries to find a small MT with period 2^521-1
            ParallelMersenneTwister.mt_struct mts = rng.get_mt_parameter_st(32, 521, 4172);
            // Assert.IsTrue(mts.state != null);
            rng.sgenrand_mt(3241, mts);
            for (int i = 0; i < 10; i++)
            {
                RN = rng.genrand_mt(mts);
                RN *= reciprocal;
                Assert.IsTrue(Math.Abs(RN - cCodeResults[i]) < 0.000001); 
            }            
        }

    }

}
