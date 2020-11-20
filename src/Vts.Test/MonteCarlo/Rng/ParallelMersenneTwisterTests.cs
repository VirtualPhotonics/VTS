using System;
using NUnit.Framework;
using Vts.MonteCarlo.Rng;

namespace Vts.Test.MonteCarlo
{
    [TestFixture] 
    public class ParalleleMersenneTwisterTests
    {
        /// <summary>
        /// new_example1 tries to find a small MT with period 2^521-1 = 6.86x10^{156}
        /// The arguments to get_mt_parameter_st are:
        /// word size: int needs to be 31 or 32
        /// exponent of MT period: in this example 521 
        /// seed: seed for original MT19937 to generate parameter
        /// </summary>
        [Test]
        public void validate_results_agree_with_c_code_new_example1()
        {
            double reciprocal = 1.0 / 4294967296.0;
            double[] cCodeResultsRNs = new double[]
                { 0.812134, 0.759003, 0.235254, 0.194382, 0.928250,
                  0.726841, 0.037951, 0.504746, 0.382553, 0.700802 };
            uint[] cCodeResultsUints = new uint[]
                { 3488090578, 3259890980, 1010406819, 834862677, 3986802806,
                  3121760294,  162998133, 2167868955, 1643052073, 3009922635 };
            uint genrand_mt_return;
            double RN;
            // instantiate class
            var rng = new ParallelMersenneTwister(0);
            // this tries to find a small MT with period 2^521-1
            ParallelMersenneTwister.mt_struct mts = rng.get_mt_parameter_st(32, 521, 4172);
            Assert.IsTrue(mts.state != null);
            // sgenrand_mt constructs mts struct using 1st parameter seed=3241
            rng.sgenrand_mt(3241, ref mts);
            for (int i = 0; i < 10; i++)
            {
                genrand_mt_return = rng.genrand_mt(ref mts);
                RN = genrand_mt_return * reciprocal;
                Assert.AreEqual(genrand_mt_return, cCodeResultsUints[i]);
                Assert.IsTrue(Math.Abs(RN - cCodeResultsRNs[i]) < 0.000001);
            }            
        }

    }

}
