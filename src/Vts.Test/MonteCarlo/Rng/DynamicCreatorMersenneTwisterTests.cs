using System;
using NUnit.Framework;
using Vts.MonteCarlo.Rng;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class DynamicCreatorMersenneTwisterTests
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
            var rng = new DynamicCreatorMersenneTwister(0);
            // this tries to find a small MT with period 2^521-1
            DynamicCreatorMersenneTwister.mt_struct mts = rng.get_mt_parameter_st(32, 521, 4172);
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
        /// <summary>
        /// This tries to find three independent small Mersenne Twisters
        ///   with period 2^521-1.
        /// </summary>
        [Test]
        public void validate_results_agree_with_c_code_new_example2()
        {
            uint[,] cCodeResultsUints = new uint[3, 10]
             { {1383339478, 2005363733, 4036914666, 3582104924, 1005457172,
                3153157470, 3681386243, 436138401,  3646769885, 389240451},
               {11477593,   3352863655, 2127153254, 255616186,  2881273202,
                2908447349, 3227522318, 240623780,  759472431,  1653682769},
               {629214080,  3322450751, 2804381282, 588643975,  2162773575,
                1409734999, 3289914192, 2605034630, 3715454267, 482776632}
             };
           
            // instantiate class
            var rng0 = new DynamicCreatorMersenneTwister(0);
            // get first MT id = 0
            DynamicCreatorMersenneTwister.mt_struct mts0 = rng0.get_mt_parameter_id_st(32, 521, 0, 4172);
            Assert.IsTrue(mts0.state != null);
            // get second MT id = 1
            // instantiate class
            var rng1 = new DynamicCreatorMersenneTwister(0);
            DynamicCreatorMersenneTwister.mt_struct mts1 = rng1.get_mt_parameter_id_st(32, 521, 1, 4172);
            Assert.IsTrue(mts1.state != null);
            // get third MT id = 999
            var rng2 = new DynamicCreatorMersenneTwister(0);
            DynamicCreatorMersenneTwister.mt_struct mts2 = rng2.get_mt_parameter_id_st(32, 521, 999, 4172);
            /* id may be any=16bit integers, e.g. id=999 */
            Assert.IsTrue(mts2.state != null);
            rng0.sgenrand_mt(1234, ref mts0); /* initialize mts0 with seed 1234 */
            rng1.sgenrand_mt(4567, ref mts1);
            rng2.sgenrand_mt(8901, ref mts2);
            // compare output of mts0, mts1, mts2, ten times */

            for (int i = 0; i < 10; i++) {               
                Assert.AreEqual(rng0.genrand_mt(ref mts0), cCodeResultsUints[0, i]);
                Assert.AreEqual(rng1.genrand_mt(ref mts1), cCodeResultsUints[1, i]);
                Assert.AreEqual(rng2.genrand_mt(ref mts2), cCodeResultsUints[2, i]);
            }
        }
        /// <summary>
        /// This tries to find three independent small Mersenne Twisters
        ///   with period 2^521-1.
        /// </summary>
        [Test]
        public void validate_results_agree_with_c_code_new_example3()
        {
            int count = 0;
            DynamicCreatorMersenneTwister.mt_struct[] mtss;
            int[] seed = new int[3] { 1234, 4567, 8901 };
            uint[,] cCodeResultsUints = new uint[3, 10]
             { {521416557,  1356975151, 2575064689, 3078917597, 311176367,
                3593596877, 3209520651, 1020983528, 4233839516, 496096299},
               {42672848,   3338413038, 2112735407, 3792644979, 1152381491,
                2891964980, 3261339023, 206774309,  3228790503, 2410053017},
               {2777164674, 1158788601, 610685618,  1696361159, 2241875159,
                2450943761, 2203684162, 1510219478, 2587673387, 1603142378}
             };

            // instantiate class
            var rng = new DynamicCreatorMersenneTwister(0);
            // get first MT id = 0
            mtss = rng.get_mt_parameters_st(32, 521, 3, 5, 4172, ref count);
            Assert.IsTrue(mtss[0].state != null);
            for (int i = 0; i < count; i++)
            {
                rng.sgenrand_mt((uint)seed[i], ref mtss[i]);
            }
            // compare output of mts0, mts1, mts2, ten times */

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    Assert.AreEqual(rng.genrand_mt(ref mtss[j]), cCodeResultsUints[j, i]);
                }
            }
        }
    }

}
