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
        private const uint _lower_mask = 0x7fffffff;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private const int _m = 397;

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
        private const uint _upper_mask = 0x80000000;
        // from mt19937.c
        private const uint _tempering_mask_b = 0x9d2c5680;
        private const uint _tempering_mask_c = 0xefc60000;
        private Func<uint,uint> tempering_shift_u =  y => y >> 11;
        private Func<uint, uint> tempering_shift_s = y => y << 7;
        private Func<uint, uint> tempering_shift_t = y => y << 15;
        private Func<uint, uint> tempering_shift_l = y => y >> 18;

        private Func<eqdeg_t, uint, uint> lsb = (eq, y) => y >> eq.ggap;
        private Func<eqdeg_t, uint, uint> trnstmp = (eq, y) => y ^= (y >> eq.shift_0) & eq.greal_mask;
        private Func<eqdeg_t, uint, uint> masktmp = (eq, y) =>
        {
            y ^= (y << eq.shift_s) & eq.mask_b;
            y ^= (y << eq.shift_t) & eq.mask_c;
            return y;
        };
        private Func<int, _vector, _vector, _vector> add = (int nnn, _vector u, _vector v) =>
        {
            int diff = (v.start -u.start + nnn) % nnn;
            int j = 0;
            for (int i = 0; i < nnn - diff; i++)
            {
                u.cf[i] ^= v.cf[i + diff];
                j = i;
            }
            diff = diff - nnn;
            for (int i = j; i < nnn; i++)
            {
                u.cf[i] ^= v.cf[i + diff];
            }
            u.next ^= v.next;
            return u;
        };


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
        private const int _limit_irred_deg = 31;
        private const int _nirredpoly = 127;
        private const int _max_irred_deg = 9;

        /* list of irreducible polynomials whose degrees are less than 10 */
        private int[,] irredpolylist = new int[_nirredpoly, _max_irred_deg + 1] {
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
        // in eqdeg.c
        private uint[] pivot_calc_tbl = new uint[256] {
            0, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            3, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            2, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            3, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            1, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            3, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            2, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            3, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
            4, 8, 7, 8, 6, 8, 7, 8, 5, 8, 7, 8, 6, 8, 7, 8,
        };

        // seive constants
        private const int _wordlen = 32;
        private const uint _lsb = 0x1;
        private const int _max_search = 10000;
        private const int _found = 1;
        private const int _not_found = 0;
        // dci constants
        private const int _rejected = 0;
        private const int _not_rejected = 1;
        private const int _irred = 1;
        private const int _redu = 0;
        private const int _nonredu = 1;
        // eqdeg constants
        private const int SSS = 7;
        private const int TTT = 15;
        private const int S00 = 12;
        private const int S01 = 18;
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
            public uint aaa { get; set; }
            public int mm; public int nn; public int rr; public int ww;
            public uint wmask; public uint umask; public uint lmask;
            public int shift0; public int shift1; public int shiftB; public int shiftC;
            public uint maskB; public uint maskC;
            public int i;
            public uint[] state; // if null then struct is mull
        } 
        struct polynomial
        {
            public int[] x;
            public int deg;
        }
        struct prescr_t
        {
            public int sizeOfA;
            public uint[][] modlist; // size[_nirredpoly][pre.sizeOfA]
            public polynomial[] preModPolys; // size[pre.sizeOfA+1]
        }
       
        struct eqdeg_t
        {
            public uint[] bitmask; // dimension = 32
            public uint mask_b;
            public uint mask_c;
            public uint upper_v_bits;
            public int shift_0;
            public int shift_1;
            public int shift_s;
            public int shift_t;
            public int mmm;
            public int nnn;
            public int rrr;
            public int www;
            public uint[] aaa; // dimension 2
            public uint gupper_mask;
            public uint glower_mask;
            public uint greal_mask;
            public int ggap;
            public int[] gcur_maxlengs; // dimension 32
            public uint gmax_b;
            public uint gmax_c;
        }
        struct _vector
        {
            public uint[] cf;
            public int start;
            public int count;
            public uint next;
        }
        /// struc in mt19937.h
        struct org_state
        {
            public uint[] mt;
            public  int mti;
        }
        // global in seive
        org_state global_mt19937 = new org_state();

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
        // mt19937.c methods
        private void sgenrand_dc(org_state st, uint seed)
        {
            for (int i = 0; i < _n; i++)
            {
                st.mt[i] = seed;
                seed = (1812433253) * (seed ^ (seed >> 30)) + (uint)i + 1;
            }
            st.mti = _n;
        }
        private uint genrand_dc(ref org_state st)
        {
            uint y;
            if (st.mti >= _n)
            {
                int j = 0;
                for (int k = 0; k < _n - _m; k++)
                {
                    y = (st.mt[k] & _upper_mask) | (st.mt[k + 1] & _lower_mask);
                    st.mt[k] = st.mt[k + _m] ^ (y >> 1) ^ _mag01[y & 0x1];
                    j = k;
                }              
                for (int k = j + 1; k < _n-1; k++) // this starts where last loop left off
                {
                    y = (st.mt[k] & _upper_mask) | (st.mt[k + 1] & _lower_mask);
                    st.mt[k] = st.mt[k + (_m - _n)] ^ (y >> 1) ^ _mag01[y & 0x1];
                }
                y = (st.mt[_n - 1] & _upper_mask) | (st.mt[0] & _lower_mask);
                st.mt[_n - 1] = st.mt[_m - 1] ^ (y >> 1) ^ _mag01[y & 0x1];
                st.mti = 0;
            }
            y = st.mt[st.mti++];
            y ^= tempering_shift_u(y);
            y ^= tempering_shift_s(y) & _tempering_mask_b;
            y ^= tempering_shift_t(y) & _tempering_mask_c;
            y ^= tempering_shift_l(y);
            return y;
        }
        public void sgenrand_mt(uint seed, mt_struct mts)
        {
            for (int i = 0; i < mts.nn; i++)
            {
                mts.state[i] = seed;
                seed = (1812433253) * (seed ^ (seed >> 30)) + (uint)i + 1;
            }
            mts.i = mts.nn;
            for (int i = 0; i < mts.nn; i++)
            {
                mts.state[i] = mts.state[i] & mts.wmask;
            }
        }
        public uint genrand_mt(mt_struct mts)
        {
            uint x;
            if (mts.i >= mts.nn)
            {
                int n = mts.nn;
                int m = mts.mm;
                uint aa = mts.aaa;
                uint uuu = mts.umask;
                uint lll = mts.lmask;
                int lim = n - m;
                // check all of following
                int k = 0;
                for (int j = k; j < lim; j++)
                {
                    x = (mts.state[j] & uuu) | (mts.state[j + 1] & lll);
                    mts.state[j] = mts.state[j + m] ^ (x >> 1) ^ (Convert.ToBoolean(x & 1U) ? aa : 0U); // double check
                    k++;
                }
                lim = n - 1;
                for (int j = k; j < lim; j++)
                {
                    x = (mts.state[j] & uuu) | (mts.state[j+ 1] & lll);
                    mts.state[j] = mts.state[j + m] ^ (x >> 1) ^ (Convert.ToBoolean(x & 1U) ? aa : 0U); // check
                    k++;
                }
                x = (mts.state[n - 1] & uuu) | (mts.state[0] & lll);
                mts.state[n-1] = mts.state[m - 1] ^ (x >> 1) ^ (Convert.ToBoolean(x & 1U) ? aa : 0U);
                mts.i = 0;
            }
            x = mts.state[mts.i];
            mts.i += 1;
            x ^= x >> mts.shift0;
            x ^= (x << mts.shiftB) & (mts.maskB);
            x ^= (x << mts.shiftC) & (mts.maskC);
            x ^= x >> mts.shift1;
            return x;
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

        private mt_struct init_mt_search(check32_t ck, ref prescr_t pre, int w, int p)
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
            mts.state = new uint[n];
            //mts = alloc_mt_struc(n); // no allocation needed since "new" above
            //if (mts.state == null) return mts;  // this check is on malloc

            m = n / 2;
            if (m < 2) m = n - 1;
            r = n * w - p;
            make_masks(r, w, mts);
            init_prescreening_dc(ref pre, m, n, r, w);
            initcheck32_dc(ck, r, w);
            mts.mm = m;
            mts.nn = n;
            mts.rr = r;
            mts.ww = w;
            return mts;
        }
        // methods in prescr.c
        private int prescreening_dc(prescr_t pre, uint aaa)
        {
            for (int i = 0; i < _nirredpoly; i++)
            {
                if (is_reducible(pre, aaa, pre.modlist[i]) == _redu) // major check here
                {
                    return _not_rejected;
                }
            }
            return _not_rejected;
        }
        private void init_prescreening_dc(ref prescr_t pre, int m, int n, int r, int w)
        {
            polynomial pl;
            pre.sizeOfA = w;
            pre.preModPolys = new polynomial[pre.sizeOfA + 1]; 
            make_pre_mod_polys(ref pre, m, n, r, w);
            pre.modlist = new uint[_nirredpoly][];
            for (int i = 0; i < _nirredpoly; i++)
            {
                pre.modlist[i] = new uint[pre.sizeOfA + 1];
            }

            for (int i = 0; i < _nirredpoly; i++)
            {
                pl = new_poly(_max_irred_deg);
                next_irred_poly(ref pl, i);
                make_modlist(pre, pl, i);
            }
            //for (int i = 0; i < pre.sizeOfA; i++) // don't need this
            //{
            //    free_poly 
            //}
        }
        private void next_irred_poly(ref polynomial pl, int nth)
        {
            int max_deg = 0;
            for (int i = 0; i < _max_irred_deg; i++)
            {
                if ( Convert.ToBoolean(irredpolylist[nth,i]))
                {
                    max_deg = i;
                }
                pl.x[i] = irredpolylist[nth, i];
            }
            pl.deg = max_deg;
        }
        private void make_modlist(prescr_t pre, polynomial pl, int nPoly)
        {
            polynomial tmpPl;
            for (int i = 0; i <= pre.sizeOfA; i++)
            {
                tmpPl = polynomial_dup(pre.preModPolys[i]);
                polynomial_mod(ref tmpPl, pl);
                pre.modlist[nPoly][i] = word2bit(tmpPl);  // not sure here
            }
        }
        /// <summary>
        /// method performs wara.x (an int) mod waru.x (another in), with results stored in wara.x
        /// </summary>
        /// <param name="wara">first polynomial</param>
        /// <param name="waru">second polynomial</param>
        private void polynomial_mod(ref polynomial wara, polynomial waru) // waru is "const" in C code
        {
            int deg_diff, i; 
            while (wara.deg >= waru.deg)
            {
                deg_diff = wara.deg - waru.deg;
                for (i = 0; i <= waru.deg; i++)
                {
                    wara.x[i + deg_diff] ^= waru.x[i]; // XOR clears bit set at x[wara.deg-1]
                }
                for (i = wara.deg; i >= 0; i--)
                {
                    if (Convert.ToBoolean(wara.x[i]))
                    {
                        break;
                    }
                }
                wara.deg = i;
            }
        }
        private uint word2bit(polynomial pl)
        {
            uint bx = 0;
            for (int i = pl.deg; i > 0; i--)
            {
                if (Convert.ToBoolean(pl.x[i]))
                {
                    bx |= 0x1;
                }
                bx <<= 1;
            }
            if (Convert.ToBoolean(pl.x[0]))
            {
                bx |= 0x1;
            }
            return bx;
        }
        private int is_reducible(prescr_t pre, uint aaa, uint[] polylist)
        {
            uint x = polylist[pre.sizeOfA];
            for (int i = pre.sizeOfA-1; i >= 0; i++)
            {
                if (Convert.ToBoolean(aaa & 0x1))
                {
                    x ^= polylist[i];
                }
                aaa >>= 1;
            }
            if (x == 0)
            {
                return _redu;
            }
            return _nonredu;
        }
        /// <summary>
        /// method to make prescr_t pre.preModPolys=polynomial[pre.SizeOfA+1] 
        /// </summary>
        /// <param name="pre"></param>
        /// <param name="mm"></param>
        /// <param name="nn"></param>
        /// <param name="rr"></param>
        /// <param name="ww"></param>
        private void make_pre_mod_polys(ref prescr_t pre, int mm, int nn, int rr, int ww)
        {
            polynomial t, t0, t1, s, s0, s1;
            int i;
            int j = 0;
            t = new_poly(0);
            t.deg = 0;
            t.x[0] = 1;
            pre.preModPolys[j++] = t;

            t = make_tntm(nn, mm);
            t0 = make_tntm(nn, mm);
            s = make_tntm(nn - 1, mm - 1);

            for (i = 1; i < (ww - rr); i++)
            {
                pre.preModPolys[j++] = polynomial_dup(t0);
                t1 = t0;    // not used
                t0 = polynomial_mult(t0, t);
                //free_poly(t1);
            }
            pre.preModPolys[j++] = polynomial_dup(t0);
            s0 = polynomial_mult(t0, s);
            //free_poly(t0);
            //free_poly(t);
            for (i = (rr-2); i >= 0; i--)
            {
                pre.preModPolys[j++] = polynomial_dup(s0);
                s1 = s0;  // not used
                s0 = polynomial_mult(s0, s);
                //free_poly(s1);
            }
            pre.preModPolys[j++] = polynomial_dup(s0);
            //free_poly(s0);
            //free_poly(s);
        }
        /// <summary>
        /// method duplicate polynomial 
        /// </summary>
        /// <param name="pl">polynomial to be duplicated</param>
        /// <returns></returns>
        private polynomial polynomial_dup(polynomial pl)
        {
            polynomial pt = new_poly(pl.deg);
            for (int i = pl.deg; i >= 0; i--)
            {
                pt.x[i] = pl.x[i];
            }
            return pt;
        }
        private polynomial polynomial_mult(polynomial p0, polynomial p1)
        {
            polynomial p;
            if ((p0.deg < 0) || (p1.deg < 0))
            {
                p = new_poly(-1);
                return p;
            }
            p = new_poly(p0.deg + p1.deg);
            for (int i = 0; i <= p1.deg; i++)
            {
                if (Convert.ToBoolean(p1.x[i]))  
                {
                    for (int j = 0; j <= p0.deg; j++)
                    {
                        p.x[i + j] ^= p0.x[j]; // XOR
                    }
                }
            }
            return p;
        }
        private polynomial make_tntm(int n, int m)
        {
            polynomial p = new_poly(n);
            p.x[m] = 1;
            p.x[n] = p.x[m];
            return p;
        }
    
        /// <summary>
        /// method creates a new polynomial of degree deg
        /// polynomial members are int[] x and int deg
        /// <param name="degree">degree of polynomial created</param>
        /// <returns></returns>
        private polynomial new_poly(int degree)
        {
            polynomial p = new polynomial();
            p.deg = degree;
            if (degree < 0)
            {
                p.x = null;
                return p;
            }
            p.x = new int[degree + 1];
            return p;
        }
        private void initcheck32_dc(check32_t ck, int r, int w)
        {
            // word_mask, lower_mask, and upper_mask agree with C code!
            // word_mask (least significant w bits)
            ck.word_mask = 0xffffffff;
            ck.word_mask <<= _wordlen - w;
            ck.word_mask >>= _wordlen - w;
            // lower_mask (least significat r bits)
            ck.lower_mask = 0;
            for (int i = 0; i < r; ++i)
            {
                ck.lower_mask = ck.lower_mask << 1;
                ck.lower_mask = ck.lower_mask | _lsb;
            }
            // upper_mask (most significant (w-r) bits)
            ck.upper_mask = (~ck.lower_mask) & ck.word_mask;
        }
        /// <summary>
        /// There are variants of this method:
        /// get_mt_parameter(w,p)
        /// get_mt_parameter_st(w,p,seed)        
        /// get_mt_parameter_id(w,p,id) id must be less than 65536 and positive
        /// get_mt_parameter_id_st(w,p,id,seed) 
        /// </summary>
        /// <param name="w">word size: only w=32 or 31 allowed</param>
        /// <param name="p">Mersenne exponent: p>=521 and p<=44497</param>
        /// <param name="seed">seed of original mt19937 to generate parameter</param>
        /// <returns></returns>
        public mt_struct get_mt_parameter_st(int w, int p, uint seed)
        {
            mt_struct mts;
            prescr_t pre = new prescr_t();
            org_state org = new org_state();
            org.mt = new uint[_n];  // should next two be done in a default struct constructor?
            org.mti = _n;
            check32_t ck = new check32_t();
            sgenrand_dc(org, seed);  // org good to after this call
            mts = init_mt_search(ck, ref pre, w, p);
            //if (mts.state == null) // check on malloc
            //    return mts;
            if (get_irred_param(ck, pre, org, mts, 0, 0) == _not_found) 
            {
                //free_mt_struct(mts); // do I need?
                mts.state = null;  // can't return mts=null so setting state=null
                return mts;
            }
            get_tempering_parameter_hard_dc(mts);
            end_mt_search(pre);
            return mts;
        }
        // methods in eqdeg.c
        public void get_tempering_parameter_hard_dc(mt_struct mts)
        {
            eqdeg_t eq;
            eq = init_tempering(mts);
            optimize_v(eq, 0, 0, 0);
            mts.shift0 = eq.shift_0;
            mts.shift1 = eq.shift_1;
            mts.shiftB = eq.shift_s;
            mts.shiftC = eq.shift_t;
            mts.maskB = eq.mask_b >> eq.ggap;
            mts.maskC = eq.mask_c >> eq.ggap;
        }
        private eqdeg_t init_tempering(mt_struct mts)
        {
            eqdeg_t eq = new eqdeg_t();
            eq.bitmask = new uint[32]; // if class then wouldn't have to do this
            eq.mmm = mts.mm;
            eq.nnn = mts.nn;
            eq.rrr = mts.rr;
            eq.www = mts.ww;
            eq.shift_0 = S00;
            eq.shift_1 = S01;
            eq.shift_s = SSS;
            eq.shift_t = TTT;
            eq.ggap = _wordlen - eq.www;
            // bits are filled in mts.aaa from MSB
            eq.aaa[0] = 0;
            eq.aaa[1] = (mts.aaa) << eq.ggap;
            for (int i = 0; i < _wordlen; i++)
            {
                eq.bitmask[i] = (uint)0x8000000 >> i;
            }
            for (int i = 0; i < eq.rrr; i++) // orig code for (i=0,eq.glower_mask=0; i<eq.rrr; i++)
            {
                eq.glower_mask = (eq.glower_mask << 1) | 0x1;
            }
            eq.gupper_mask = ~eq.glower_mask;
            eq.glower_mask <<= eq.ggap;
            eq.glower_mask <<= eq.ggap;
            eq.greal_mask = (eq.gupper_mask | eq.glower_mask);
            return eq;
            // orig code has debug statements here           
        }
        private void optimize_v(eqdeg_t eq, uint b, uint c, int v)
        {
            uint[] bbb = new uint[8];
            uint[] ccc = new uint[8];
            int ll = push_stack(eq, b, c, v, bbb, ccc);
            int max_i = 0;
            int max_len = 0;
            if (ll > 1)
            {
                for (int i = 0; i < ll; i++)
                {
                    eq.mask_b = bbb[i];
                    eq.mask_c = ccc[i];
                    int t = pivot_reduction(eq, v + 1);
                    if (t > max_len)
                    {
                        max_len = t;
                        max_i = i;
                    }
                }
            }
            if (v >= eq.www-1)
            {
                eq.mask_b = bbb[max_i];
                eq.mask_c = ccc[max_i];
                return;
            }
            optimize_v(eq, bbb[max_i], ccc[max_i], v + 1); // c# allows recursive calling?
        }
        private int push_stack(eqdeg_t eq, uint b, uint c, int v, uint[] bbb, uint[] ccc)
        {
            int ncv;
            uint[] cv_buf = new uint[2];
            int ll = 0;
            if ( ( v+eq.shift_t) < eq.www)
            {
                ncv = 2;
                cv_buf[0] = c | eq.bitmask[v];
                cv_buf[1] = c;
            }
            else
            {
                ncv = 1;
                cv_buf[0] = c;
            }
            for (int i = 0; i < ncv; i++)
            {
                ll += push_mask(eq, ll, v, b, cv_buf[i], bbb, ccc);
            }
            return ll;
        }
        private int push_mask(eqdeg_t eq, int l, int v, uint b, uint c, uint[] bbb, uint[] ccc)
        {
            int nbv, nbvt;
            uint[] bv_buf = new uint[2];
            uint[] bvt_buf = new uint[2];
            int k = l;
            if (eq.shift_s + v >= eq.www)
            {
                nbv = 1;
                bv_buf[0] = 0;
            }
            else
            {
                if ((v >= eq.shift_t) && Convert.ToBoolean(c&eq.bitmask[v-eq.shift_t]))
                {
                    nbv = 1;
                    bv_buf[0] = b & eq.bitmask[v];
                }
                else
                {
                    nbv = 2;
                    bv_buf[0] = eq.bitmask[v];
                    bv_buf[1] = 0;
                }
            }
            if (((v+eq.shift_t+eq.shift_s) < eq.www) && Convert.ToBoolean(c&eq.bitmask[v]))
            {
                nbvt = 2;
                bvt_buf[0] = eq.bitmask[v + eq.shift_t];
                bvt_buf[1] = 0;
            }
            else
            {
                nbvt = 1;
                bvt_buf[0] = 0;
            }
            uint bmask = eq.bitmask[v];
            if ((v+eq.shift_t) < eq.www)
            {
                bmask |= eq.bitmask[v + eq.shift_t];
            }
            bmask = ~bmask;
            for (int i = 0; i < nbvt; ++i)
            {
                for (int j = 0; j < nbv; ++j)
                {
                    bbb[k] = (b & bmask) | bv_buf[j] | bvt_buf[i];
                    ccc[k] = c;
                    ++k;
                }
            }
            return k - l;
        }
        private int pivot_reduction(eqdeg_t eq, int v)
        {
            _vector[] lattice;
            _vector ltmp = new _vector();
            eq.upper_v_bits = 0;
            for (int i = 0; i < v; i++)
            {
                eq.upper_v_bits |= eq.bitmask[i];
            }
            lattice = make_lattice(eq, v);
            while(true)
            {
                int pivot = calc_pivot(lattice[v].next);
                if (lattice[pivot].count < lattice[v].count)
                {
                    ltmp = lattice[pivot];
                    lattice[pivot] = lattice[v];
                    lattice[v] = ltmp;
                }
                add(eq.nnn, lattice[v], lattice[pivot]);
                if (lattice[v].next == 0)
                {
                    int count = 0;
                    int new_count = next_state(eq, lattice[v], count);
                    if (lattice[v].next == 0)
                    {
                        if (Convert.ToBoolean(is_zero(eq.nnn, lattice[v])))
                        {
                            break;
                        }
                        while (lattice[v].next == 0)
                        {
                            new_count++;
                            int newer_count = next_state(eq, lattice[v], new_count);
                            if (newer_count > eq.nnn * (eq.www-1) - eq.rrr)
                            {
                                break;
                            }                          
                        }
                        if (lattice[v].next == 0)
                        {
                            break;
                        }
                    }
                }
            } // while (true)
            int min = lattice[0].count;
            for (int i = 0; i < v; i++)
            {
                if (min > lattice[i].count)
                {
                    min = lattice[i].count;
                }
            }
            return min;
        }
        private int calc_pivot(uint v)
        {
            int p1 = (int)pivot_calc_tbl[v & 0xff]; // I cast here but not sure
            if (Convert.ToBoolean(p1))
            {
                return p1 + 24 - 1;
            }
            int p2 = (int)pivot_calc_tbl[(v >> 8) & 0xff];
            if (Convert.ToBoolean(p2))
            {
                return p2 + 16 - 1;
            }
            int p3 = (int)pivot_calc_tbl[(v >> 16) & 0xff];
            if (Convert.ToBoolean(p3))
            {
                return p3 + 8 - 1;
            }
            int p4 = (int)pivot_calc_tbl[(v >> 24) & 0xff];
            if (Convert.ToBoolean(p4))
            {
                return p4 - 1;
            }
            return -1;
        }
        
        private int is_zero(int size, _vector v)
        {
            if (v.cf[0]!= 0)
            {
                return 0;
            }
            for (int i = 0; i < size - 1; i++)  // totally not sure about this
            {
                v.cf[i] = v.cf[i] + 1;
            }
            return 1;
        }
        private _vector[] make_lattice(eqdeg_t eq, int v)
        {
            _vector[] lattice = new _vector[v + 1];
            _vector bottom = new _vector();
            bottom.cf = new uint[eq.nnn];

            for (int i = 0; i < v; i++) // from 0th row to v-1-th row
            {
                lattice[i].next = eq.bitmask[i];
                lattice[i].start = 0;
                lattice[i].count = 0;
            }
            
            for (int i = 0; i < eq.nnn; i++) // last row, don't think I need to do this 0-ing C# does it
            {            
                bottom.cf[i] = 0;
            }
            bottom.cf[eq.nnn - 1] = 0xc000000 & eq.greal_mask;
            bottom.start = 0;
            bottom.count = 0;
            int count = 0;
            do
            {
                int new_count = next_state(eq, bottom, count);
            } while (bottom.next == 0);
            lattice[v] = bottom;
            return lattice;
        }
        
        private int next_state(eqdeg_t eq, _vector v, int current_count)
        {
            uint tmp;
            do
            {
                tmp = (v.cf[v.start] & eq.gupper_mask)
                    | (v.cf[(v.start + 1) % eq.nnn] & eq.glower_mask);
                v.cf[v.start] = v.cf[(v.start + eq.mmm) % eq.nnn]
                   ^ (tmp >> 1) ^ eq.aaa[lsb(eq, tmp)];
                v.cf[v.start] &= eq.greal_mask;
                tmp = v.cf[v.start];
                v.start = (v.start + 1) % eq.nnn;
                v.count++;
                tmp = trnstmp(eq, tmp);
                tmp = masktmp(eq, tmp);
                v.next = tmp & eq.upper_v_bits;
                current_count++;
                if (current_count > eq.nnn * (eq.www - 1) * eq.rrr)
                {
                    break;
                }
            } while (v.next == 0);
            return current_count;
        }

        
        // methods in seive.c
        private void end_mt_search(prescr_t pre)
        {
            end_prescreening_dc(pre);
        }
        private void end_prescreening_dc(prescr_t pre)
        {
            //for (int i = 0; i < _nirredpoly; i++)
            //{
            //    pre.modlist[i] = 0; // not sure I need this: c code uses "free" here
            //}
        }
        private int get_irred_param(check32_t ck, prescr_t pre, org_state org,
            mt_struct mts, int id, int idw)
        {
            uint a;
            int i;
            for (i = 0; i < _max_search; i++)
            {
                if (idw == 0)
                {
                    a = nextA(ref org, mts.ww);
                }
                else
                {
                    a = nextA_id(org, mts.ww, id, idw);
                }
                if (_not_rejected == prescreening_dc(pre, a))
                {
                    if (_irred == check_period_dc(ck, org, a, mts.mm, mts.nn, mts.rr, mts.ww))
                    {
                        mts.aaa = a;
                        break;
                    }
                }
            }
            if (_max_search == i)
            {
                return _not_found;
            }
            return _found;
        }
        private int check_period_dc(check32_t ck, org_state st, uint a, int m, int n, int r, int w)
        {
            uint y;
            int p = n * w - r;
            uint[] x = new uint[2 * p];
            uint[] init = new uint[n];
            for (int i = 0; i < n; i++) // set initial values
            {
                init[i] = (ck.word_mask & genrand_dc(ref st));
                x[i] = init[i];
            }
            if ((x[2]&_lsb) == (x[3]&_lsb))
            {
                x[3] ^= 1;
                init[3] ^= 1;
            }
            int pp = 2 * p - n;
            uint[] mat = new uint[2] { 0, a };
            for (int j = 0; j < p; j++)
            {
                for (int i = 0; i < pp; i++) // generate
                {
                    y = (x[i] & ck.upper_mask) | (x[i + 1] & ck.lower_mask);
                    x[i + n] = x[i + m] ^ (y >> 1) ^ (mat[y & _lsb]);
                }

                for (int i = 2; i <= p; ++i) // pick up odd subscript elements
                {
                    x[i] = x[(i << 1) - 1];
                }
                for (int i = p - n; i >= 0; --i) // reverse generate
                {
                    y = x[i + n] ^ x[i + m] ^ mat[x[i + 1] & _lsb];
                    y <<= 1; y |= x[i + 1] & _lsb;

                    x[i + 1] = (x[i + 1] & ck.upper_mask) | (y & ck.lower_mask);
                    x[i] = (y & ck.upper_mask) | (x[i] & ck.lower_mask);
                }
            }
            if ((x[0] & ck.upper_mask) == (init[0] & ck.upper_mask))
            {
                int j = 1;
                for (int i = 1; i < n; ++i)
                {
                    if (x[i] != init[i])
                    {
                        j = i;
                        break;
                    }
                }
                if (j == n)
                {
                    return _irred;
                }
            }
            return _redu;
        }
        private uint nextA(ref org_state org, int w)
        {
            uint word_mask = 0xFFFFFFFF;
            word_mask <<= _wordlen - w;
            word_mask >>= _wordlen - w;
            var x = genrand_dc(ref org);
            x &= word_mask;
            x |= (_lsb << (w - 1));
            return x;
        }
        private uint nextA_id(org_state org, int w, int id, int idw)
        {
            var word_mask = 0xFFFFFFFF;
            word_mask <<= _wordlen - w;
            word_mask >>= _wordlen - 2;
            word_mask >>= idw;
            word_mask <<= idw;
            var x = genrand_dc(ref org);
            x &= word_mask;
            x |= (_lsb << (w - 1));
            x |= (uint)id;
            return x;
        }

        private static void make_masks(int r, int w, mt_struct mts)
        {
            uint ut, wm, um, lm;
            wm = 0xFFFFFFFF;
            wm = wm >> (_wordlen - w);

            ut = 0;
            for (int i = 0; i < r; i++)
            {
                ut <<=  1;
                ut |= _lsb;
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
            org_state org = new org_state();
            return genrand_dc(ref org) * _reciprocal;
        }

    }
}


