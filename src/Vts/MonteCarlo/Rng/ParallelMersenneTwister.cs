using System;
using Vts.IO;

namespace Vts.MonteCarlo.Rng
{
    /// <summary>
    /// This class creates a parallelizable representation of the Mersenne Twister class.
    /// Code from Dynamic Creator (dc) Home Page
    /// http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/DC/dc.html.
    /// </summary>
    public class ParallelMersenneTwister : MathNet.Numerics.Random.RandomSource
    {
        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        //private const uint _lower_mask = 0x7fffffff;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        //private const int _m = 397;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private const uint _matrix_a = 0x9908b0df;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private const int _n = 624;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private const double _reciprocal = 1.0 / 4294967296.0;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        //private const uint _upper_mask = 0x80000000;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private static readonly uint[] _mag01 = { 0x0U, _matrix_a };

        /// <summary>
        /// Mersenne twister constant (should not be modified, except for serialization purposes)
        /// </summary>
        private uint[] _mt = new uint[_n];

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private int mti = _n + 1;

        // prescr defines
        private const int LIMIT_IRRED_DEG = 31;
        private const int NIRREDPOLY = 127;
        private const int MAX_IRRED_DEG = 9;

        /* list of irreducible polynomials whose degrees are less than 10 */
        private int[,] irredpolylist = new int[NIRREDPOLY, MAX_IRRED_DEG + 1] {
            {0,1,0,0,0,0,0,0,0,0,},{1,1,0,0,0,0,0,0,0,0,},{1,1,1,0,0,0,0,0,0,0,},
            {1,1,0,1,0,0,0,0,0,0,},{1,0,1,1,0,0,0,0,0,0,},{1,1,0,0,1,0,0,0,0,0,},
            {1,0,0,1,1,0,0,0,0,0,},{1,1,1,1,1,0,0,0,0,0,},{1,0,1,0,0,1,0,0,0,0,},
            {1,0,0,1,0,1,0,0,0,0,},{1,1,1,1,0,1,0,0,0,0,},{1,1,1,0,1,1,0,0,0,0,},
            {1,1,0,1,1,1,0,0,0,0,},{1,0,1,1,1,1,0,0,0,0,},{1,1,0,0,0,0,1,0,0,0,},
            {1,0,0,1,0,0,1,0,0,0,},{1,1,1,0,1,0,1,0,0,0,},{1,1,0,1,1,0,1,0,0,0,},
            {1,0,0,0,0,1,1,0,0,0,},{1,1,1,0,0,1,1,0,0,0,},{1,0,1,1,0,1,1,0,0,0,},
            {1,1,0,0,1,1,1,0,0,0,},{1,0,1,0,1,1,1,0,0,0,},{1,1,0,0,0,0,0,1,0,0,},
            {1,0,0,1,0,0,0,1,0,0,},{1,1,1,1,0,0,0,1,0,0,},{1,0,0,0,1,0,0,1,0,0,},
            {1,0,1,1,1,0,0,1,0,0,},{1,1,1,0,0,1,0,1,0,0,},{1,1,0,1,0,1,0,1,0,0,},
            {1,0,0,1,1,1,0,1,0,0,},{1,1,1,1,1,1,0,1,0,0,},{1,0,0,0,0,0,1,1,0,0,},
            {1,1,0,1,0,0,1,1,0,0,},{1,1,0,0,1,0,1,1,0,0,},{1,0,1,0,1,0,1,1,0,0,},
            {1,0,1,0,0,1,1,1,0,0,},{1,1,1,1,0,1,1,1,0,0,},{1,0,0,0,1,1,1,1,0,0,},
            {1,1,1,0,1,1,1,1,0,0,},{1,0,1,1,1,1,1,1,0,0,},{1,1,0,1,1,0,0,0,1,0,},
            {1,0,1,1,1,0,0,0,1,0,},{1,1,0,1,0,1,0,0,1,0,},{1,0,1,1,0,1,0,0,1,0,},
            {1,0,0,1,1,1,0,0,1,0,},{1,1,1,1,1,1,0,0,1,0,},{1,0,1,1,0,0,1,0,1,0,},
            {1,1,1,1,1,0,1,0,1,0,},{1,1,0,0,0,1,1,0,1,0,},{1,0,1,0,0,1,1,0,1,0,},
            {1,0,0,1,0,1,1,0,1,0,},{1,0,0,0,1,1,1,0,1,0,},{1,1,1,0,1,1,1,0,1,0,},
            {1,1,0,1,1,1,1,0,1,0,},{1,1,1,0,0,0,0,1,1,0,},{1,1,0,1,0,0,0,1,1,0,},
            {1,0,1,1,0,0,0,1,1,0,},{1,1,1,1,1,0,0,1,1,0,},{1,1,0,0,0,1,0,1,1,0,},
            {1,0,0,1,0,1,0,1,1,0,},{1,0,0,0,1,1,0,1,1,0,},{1,0,1,1,1,1,0,1,1,0,},
            {1,1,0,0,0,0,1,1,1,0,},{1,1,1,1,0,0,1,1,1,0,},{1,1,1,0,1,0,1,1,1,0,},
            {1,0,1,1,1,0,1,1,1,0,},{1,1,1,0,0,1,1,1,1,0,},{1,1,0,0,1,1,1,1,1,0,},
            {1,0,1,0,1,1,1,1,1,0,},{1,0,0,1,1,1,1,1,1,0,},{1,1,0,0,0,0,0,0,0,1,},
            {1,0,0,0,1,0,0,0,0,1,},{1,1,1,0,1,0,0,0,0,1,},{1,1,0,1,1,0,0,0,0,1,},
            {1,0,0,0,0,1,0,0,0,1,},{1,0,1,1,0,1,0,0,0,1,},{1,1,0,0,1,1,0,0,0,1,},
            {1,1,0,1,0,0,1,0,0,1,},{1,0,0,1,1,0,1,0,0,1,},{1,1,1,1,1,0,1,0,0,1,},
            {1,0,1,0,0,1,1,0,0,1,},{1,0,0,1,0,1,1,0,0,1,},{1,1,1,1,0,1,1,0,0,1,},
            {1,1,1,0,1,1,1,0,0,1,},{1,0,1,1,1,1,1,0,0,1,},{1,1,1,0,0,0,0,1,0,1,},
            {1,0,1,0,1,0,0,1,0,1,},{1,0,0,1,1,0,0,1,0,1,},{1,1,0,0,0,1,0,1,0,1,},
            {1,0,1,0,0,1,0,1,0,1,},{1,1,1,1,0,1,0,1,0,1,},{1,1,1,0,1,1,0,1,0,1,},
            {1,0,1,1,1,1,0,1,0,1,},{1,1,1,1,0,0,1,1,0,1,},{1,0,0,0,1,0,1,1,0,1,},
            {1,1,0,1,1,0,1,1,0,1,},{1,0,1,0,1,1,1,1,0,1,},{1,0,0,1,1,1,1,1,0,1,},
            {1,0,0,0,0,0,0,0,1,1,},{1,1,0,0,1,0,0,0,1,1,},{1,0,1,0,1,0,0,0,1,1,},
            {1,1,1,1,1,0,0,0,1,1,},{1,1,0,0,0,1,0,0,1,1,},{1,0,0,0,1,1,0,0,1,1,},
            {1,1,0,1,1,1,0,0,1,1,},{1,0,0,1,0,0,1,0,1,1,},{1,1,1,1,0,0,1,0,1,1,},
            {1,1,0,1,1,0,1,0,1,1,},{1,0,0,0,0,1,1,0,1,1,},{1,1,0,1,0,1,1,0,1,1,},
            {1,0,1,1,0,1,1,0,1,1,},{1,1,0,0,1,1,1,0,1,1,},{1,1,1,1,1,1,1,0,1,1,},
            {1,0,1,0,0,0,0,1,1,1,},{1,1,1,1,0,0,0,1,1,1,},{1,0,0,0,0,1,0,1,1,1,},
            {1,0,1,0,1,1,0,1,1,1,},{1,0,0,1,1,1,0,1,1,1,},{1,1,1,0,0,0,1,1,1,1,},
            {1,1,0,1,0,0,1,1,1,1,},{1,0,1,1,0,0,1,1,1,1,},{1,0,1,0,1,0,1,1,1,1,},
            {1,0,0,1,1,0,1,1,1,1,},{1,1,0,0,0,1,1,1,1,1,},{1,0,0,1,0,1,1,1,1,1,},
            {1,1,0,1,1,1,1,1,1,1,},
        };

        // seive constants
        private const int WORDLEN = 32;
        private const uint LSB = 0x1;

        // should the following be classes?
        /// <summary>
        /// structs in dci.h
        /// </summary>
        struct check32_t {
            public uint upper_mask;
            public uint lower_mask;
            public uint word_mask;
        }
        public struct mt_struct {
            public uint aaa;
            public int mm; public int nn; public int rr; public int ww;
            public uint wmask; public uint umask; public uint lmask;
            public int shift0; public int shift1; public int shiftB; public int shiftC;
            public uint maskB; public uint maskC;
            public int i;
            public uint[] state; // if null then struct is mull
        } 
        struct polynomial
        {
            int[] x;
            int deg;
        }
        struct prescr_t
        {
            public int sizeOfA;
            public uint[] modlist;
            public polynomial[] preModPolys;
        }
        /// struc in mt19937.h
        struct org_state
        {
            public uint[] mt;
            public  int mti;
        }


        /// <summary>
        /// Initializes a new instance of the MersenneTwister class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <param name="threadSafe">if set to <c>true</c>, the class is thread safe.</param>
        public ParallelMersenneTwister(int seed, bool threadSafe)
            : base(threadSafe)
        {
            init_genrand_dc((uint)seed);
        }


        /// <summary>
        /// Initializes a new instance of the MersenneTwister class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <remarks>Uses the value of MathNet.Numerics.Control.ThreadSafeRandomNumberGenerators to
        /// set whether the instance is thread safe.</remarks>        
        public ParallelMersenneTwister(int seed)
            : this(seed, MathNet.Numerics.Control.ThreadSafeRandomNumberGenerators)
        {
        }

        public void init_dc(uint seed)
        {
            org_state global_mt19937 = new org_state();
            global_mt19937.mt = new uint[_n];
            sgenrand_dc(global_mt19937, seed);
        }
        private void sgenrand_dc(org_state st, uint seed)
        {
            for (int i = 0; i < _n; i++)
            {
                st.mt[i] = seed;
                seed = (1812433253) * (seed ^ (seed >> 30)) + (uint)i + 1;
            }
            st.mti = _n;
        }
        /// <summary>
        /// initializing the array with a seed
        /// </summary>
        /// <param name="s"></param>
        private void init_genrand_dc(uint s)
        {
            _mt[0] = s & 0xffffffff;
            for (mti = 1; mti < _n; mti++)
            {
                _mt[mti] = (1812433253 * (_mt[mti - 1] ^ (_mt[mti - 1] >> 30)) + (uint)mti);
                /* See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier. */
                /* In the previous versions, MSBs of the seed affect   */
                /* only MSBs of the array _mt[].                        */
                /* 2002/01/09 modified by Makoto Matsumoto             */
                _mt[mti] &= 0xffffffff;
                /* for >32 bit machines */
            }
        }

        private mt_struct init_mt_search(check32_t ck, prescr_t pre, int w, int p)
        {
            int n, m, r;
            mt_struct mts = new mt_struct();
            if ((w>32) || (w<31))
            {
                Console.WriteLine("Sorry, currently only w=32 or 31 is allowed.");
                mts.state = null;
                return mts;
            }
            if (!proper_mersenne_exponent(p))
            {
                if (p<521)
                {
                    Console.WriteLine("p is too small.");
                    mts.state = null;
                    return mts;
                }
                if (p>44497)
                {
                    Console.WriteLine("P is too large.");
                    mts.state = null;
                    return mts;
                }
                Console.WriteLine("p is not a Mersenne exponent.");
                {
                    mts.state = null;
                    return mts;
                }
            }
            n = p / w + 1; // since p is Mersenne exponent, w never divides p
            //mts = alloc_mt_struc(n); // no allocation needed since "new" above
            if (mts.state == null) return mts;

            m = n / 2;
            if (m < 2) m = n - 1;
            r = n * w - p;
            make_masks(r, w, mts);
            initPrescreening_dc(pre, m, n, r, w);
            initcheck32_dc(ck, r, w);
            mts.mm = m;
            mts.nn = n;
            mts.rr = r;
            mts.ww = w;
            return mts;
        }
        private void initPrescreening_dc(prescr_t pre, int m, int n, int r, int w)
        {
            int i;
            polynomial pl;
            pre.sizeOfA = w;

            //pre.preModPolys = new polynomial(); // stuck

        }
        private void initcheck32_dc(check32_t ck, int r, int w)
        {
            // word_mask (least significant w bits)
            ck.word_mask = 0xFFFFFFFF;
            ck.word_mask = ck.word_mask << WORDLEN - w;
            ck.word_mask = ck.word_mask >> WORDLEN - w;
            // lower_mask (least significat r bits)
            ck.lower_mask = 0;
            for (int i = 0; i < r; ++i)
            {
                ck.lower_mask = ck.lower_mask << 1;
                ck.lower_mask = ck.lower_mask | LSB;
            }
            // upper_mask (most significant (w-r) bits)
            ck.upper_mask = (~ck.lower_mask) & ck.word_mask;
        }
        //private mt_struct get_mt_parameter(int w, int p)
        //{
        //    mt_struct mts;
        //    prescr_t pre;
        //    check32_t ck;
        //    mts = init_mt_search(ck, w, p, out pre);
        //    if (mts.state == null)
        //        return mts;
        //    if (!get_irred_param(ck,pre,global_mt199937, mts, 0, 0))
        //    {
        //        free_mt_struct(mts);
        //        return null;
        //    }
        //    _get_tempering_parameter_hard_dc(mts);
        //    end_mt_search(pre);
        //    return mts;
        //}
        private static void make_masks(int r, int w, mt_struct mts)
        {
            uint ut, wm, um, lm;
            wm = 0xFFFFFFFF;
            wm = wm >> (WORDLEN - w);

            ut = 0;
            for (int i = 0; i < r; i++)
            {
                ut = ut << 1;
                ut = ut | LSB;
            }
            lm = ut;
            um = (~ut) & wm;

            mts.wmask = wm;
            mts.umask = um;
            mts.lmask = lm;
        }
        private static bool proper_mersenne_exponent(int p)
        {
            switch(p)
            {
                case 521:
                case 607:
                case 1279:
                case 2203:
                case 2281:
                case 3217:
                case 4253:
                case 4423:
                case 9689:
                case 9941:
                case 11213:
                case 19937:
                case 21701:
                case 23209:
                case 44497:
                    return true;
                default:
                    return false;            
            }
        }


        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0.
        /// </returns>
        protected override double DoSample()
        {
            return 0.1;   // fix to compile orig: genrand_dc() * _reciprocal;
        }

        /// <summary>
        /// methods to save current state of random number generator
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SerializableMersenneTwister FromFile(string filename)
        {
            var info = FileIO.ReadFromJson<MersenneTwisterSerializationInfo>(filename);

            return SerializableMersenneTwister.Create(info);
        }

        public void ToFile(SerializableMersenneTwister smt, string filename)
        {
            var info = new MersenneTwisterSerializationInfo
            {
                MT = smt.MT,
                MTI = smt.MTI
            };

            FileIO.WriteToJson(info, filename);
        }
    }
}


