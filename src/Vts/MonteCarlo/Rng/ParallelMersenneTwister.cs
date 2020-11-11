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
        private Func<uint,uint> _tempering_shift_u =  y => y >> 11;
        private Func<uint, uint> _tempering_shift_s = y => y >> 7;
        private Func<uint, uint> _tempering_shift_t = y => y >> 15;
        private Func<uint, uint> _tempering_shift_l = y => y >> 18;

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

        // seive constants
        private const int _wordlen = 32;
        private const uint _lsb = 0x1;
        private const int _max_search = 10000;
        // dci constants
        private const int _not_rejected = 1;
        private const int _irred = 1;
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
        private uint genrand_dc(org_state st)
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
                for (int k = j; k < _n-1; k++) // this starts where last loop left off
                {
                    y = (st.mt[k] & _upper_mask) | (st.mt[k + 1] & _lower_mask);
                    st.mt[k] = st.mt[k + _m] ^ (y >> 1) ^ _mag01[y & 0x1];
                }
                y = (st.mt[_n - 1] & _upper_mask) | (st.mt[0] & _lower_mask);
                st.mt[_n - 1] = st.mt[_m - 1] ^ (y >> 1) ^ _mag01[y & 0x1];
                st.mti = 0;
            }
            y = st.mt[st.mti++];
            y ^= _tempering_shift_u(y);
            y ^= _tempering_shift_s(y) & _tempering_mask_b;
            y ^= _tempering_shift_t(y) & _tempering_mask_c;
            y ^= _tempering_shift_l(y);
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

            pre.preModPolys = new polynomial[(pre.sizeOfA + 1)*sizeof(uint)]; // not sure if this sb 2D
            make_pre_mod_polys(pre, m, n, r, w);
            pre.modlist = new uint[_nirredpoly];
        }

        private void make_pre_mod_polys(prescr_t pre, int mm, int nn, int rr, int ww)
        {
            polynomial t, t0, t1, s, s0, s1;
            int j = 0;
            t = new_poly(0);
            t.deg = 0;
            t.x[0] = 1;
            pre.preModPolys[j++] = t;

            t = make_tntm(nn, mm);
            t0 = make_tntm(nn, mm);
            s = make_tntm(nn - 1, mm - 1);

            for (int i = 0; i < (ww - rr); i++)
            {
                pre.preModPolys[j++] = polynomial_dup(t0);
                t1 = t0;
                t0 = polynomial_mult(t0, t);
                //free_poly(t1);
            }
            pre.preModPolys[j++] = polynomial_dup(t0);
            s0 = polynomial_mult(t0, s);
            //free_poly(t0);
            //free_poly(t);
            for (int i = (rr-2); i >= 0; i--)
            {
                pre.preModPolys[j++] = polynomial_dup(s0);
                s1 = s0;
                s0 = polynomial_mult(s0, s);
                //free_poly(s1);
            }
            pre.preModPolys[j++] = polynomial_dup(s0);
            //free_poly(s0);
            //free_poly(s);
        }
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
            for (int i = 0; i <p1.deg; i++)
            {
                if (p1.x[i] != 0)  // C equivalent to "if (p1.x[i])" if arg is int
                {
                    for (int j = 0; j <= p0.deg; j++)
                    {
                        p.x[i + j] ^= p0.x[j];
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
            // word_mask (least significant w bits)
            ck.word_mask = 0xFFFFFFFF;
            ck.word_mask = ck.word_mask << _wordlen - w;
            ck.word_mask = ck.word_mask >> _wordlen - w;
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
        public mt_struct get_mt_parameter(int w, int p)
        {
            mt_struct mts;
            prescr_t pre;
            check32_t ck = new check32_t();
            mts = init_mt_search(ck, w, p, out pre);
            if (mts.state == null)
                return mts;
            if (!get_irred_param(ck, pre, global_mt199937, mts, 0, 0))
            {
                free_mt_struct(mts);
                return null;
            }
            _get_tempering_parameter_hard_dc(mts);
            end_mt_search(pre);
            return mts;
        }
        public int get_irred_parameter(check32_t ck, prescr_t pre, org_state org,
            mt_struct mts, int id, int idw)
        {
            uint a;
            for (int i = 0; i < _max_search; i++)
            {
                if (idw == 0)
                {
                    a = nextA(org, mts.ww);
                }
                else
                {
                    a = nextA_id(org, mts.ww, id, idw);
                }
                if (_not_rejected == prescreening_dc(pre,a))
                {
                    if (_irred == check_period_dc(ck, org, a, mts.mm, mts.nn, mts.rr, mts.ww))
                    {
                        mts.aaa = a;
                        break;
                    }
                }
            }
        }
        private int check_period_dc(check32_t ck, org_state st, uint a, int m, int n, int r, int w)
        {
            uint y;
            int p = n * w - r;
            uint[] x = new uint[2 * p];
            uint[] init = new uint[n];
            for (int i = 0; i < n; i++)
            {
                init[i] = (ck.word_mask & genrand_dc(st));
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
                for (int i = 0; i < pp; i++)
                {
                    y = (x[i] & ck.upper_mask) | (x[i + 1] & ck.lower_mask);
                    x[i+n] = x[i+m] ^ (y>>1) ^ (mat[y&])
                }
            }
        }
        private uint nextA(org_state org, int w)
        {
            var word_mask = 0xFFFFFFFF;
            word_mask <<= _wordlen - w;
            word_mask >>= _wordlen - 2;
            var x = genrand_dc(org);
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
            var x = genrand_dc(org);
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
                ut = ut << 1;
                ut = ut | _lsb;
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


