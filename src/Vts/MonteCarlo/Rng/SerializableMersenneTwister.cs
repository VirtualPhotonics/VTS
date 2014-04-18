using System;
using MathNet.Numerics;
using MathNet.Numerics.Random;
using Vts.IO;

namespace Vts.MonteCarlo.Rng
{
    /// <summary>
    /// This class creates a serializable representation of the Mersenne Twister class.
    /// Code from MathNet Numerics.
    /// </summary>
    public class SerializableMersenneTwister : AbstractRandomNumberGenerator
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
        private const double _reciprocal = 1.0 / 4294967295.0;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private const uint _upper_mask = 0x80000000;

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private static readonly uint[] _mag01 = { 0x0U, _matrix_a };

        /// <summary>
        /// Mersenne twister constant (should not be modified, except for serialization purposes)
        /// </summary>
        private uint[] _mt = new uint[624];

        /// <summary>
        /// Mersenne twister constant.
        /// </summary>
        private int mti = _n + 1;


        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <param name="threadSafe">if set to <c>true</c>, the class is thread safe.</param>
        public SerializableMersenneTwister(int seed, bool threadSafe)
            : base(threadSafe)
        {
            init_genrand((uint)seed);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class using
        /// the current time as the seed.
        /// </summary>
        /// <remarks>If the seed value is zero, it is set to one. Uses the
        /// value of <see cref="Control.ThreadSafeRandomNumberGenerators"/> to
        /// set whether the instance is thread safe.</remarks>
        public SerializableMersenneTwister()
            : this((int)DateTime.Now.Ticks)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class using
        /// the current time as the seed.
        /// </summary>
        /// <param name="threadSafe">if set to <c>true</c> , the class is thread safe.</param>
        public SerializableMersenneTwister(bool threadSafe)
            : this((int)DateTime.Now.Ticks, threadSafe)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class.
        /// </summary>
        /// <param name="seed">The seed value.</param>
        /// <remarks>Uses the value of <see cref="Control.ThreadSafeRandomNumberGenerators"/> to
        /// set whether the instance is thread safe.</remarks>        
        public SerializableMersenneTwister(int seed)
            : this(seed, Control.ThreadSafeRandomNumberGenerators)
        {
        }

        // public properties for serialization
        /// <summary>
        /// variable within algorithm needed to resume series if interruptes
        /// </summary>
        public uint[] MT { get { return _mt; } set { _mt = value; } }
        /// <summary>
        /// variable within algorithm needed to resume series if interruptes
        /// </summary>
        public int MTI { get { return mti; } set { mti = value; } }

        public static SerializableMersenneTwister Create(MersenneTwisterSerializationInfo info)
        {
            return new SerializableMersenneTwister
            {
                MT = info.MT,
                MTI = info.MTI
            };
        }

        /*/// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class.
        /// </summary>
        /// <param name="init_key">The initialization key.</param>
        public MersenneTwister(int[] init_key)
        {
            if (init_key == null)
            {
                throw new ArgumentNullException("init_key");
            }
            uint[] array = new uint[init_key.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (uint) init_key[i];
            }
            init_by_array(array);
        }
        */
        /* initializes _mt[_n] with a seed */

        private void init_genrand(uint s)
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

        /* generates a random number on [0,0xffffffff]-interval */

        private uint genrand_int32()
        {
            uint y;

            /* mag01[x] = x * MATRIX_A  for x=0,1 */

            if (mti >= _n)
            {
                /* generate _n words at one time */
                int kk;

                if (mti == _n + 1) /* if init_genrand() has not been called, */
                    init_genrand(5489); /* a default initial seed is used */

                for (kk = 0; kk < _n - _m; kk++)
                {
                    y = (_mt[kk] & _upper_mask) | (_mt[kk + 1] & _lower_mask);
                    _mt[kk] = _mt[kk + _m] ^ (y >> 1) ^ _mag01[y & 0x1];
                }
                for (; kk < _n - 1; kk++)
                {
                    y = (_mt[kk] & _upper_mask) | (_mt[kk + 1] & _lower_mask);
                    _mt[kk] = _mt[kk + (_m - _n)] ^ (y >> 1) ^ _mag01[y & 0x1];
                }
                y = (_mt[_n - 1] & _upper_mask) | (_mt[0] & _lower_mask);
                _mt[_n - 1] = _mt[_m - 1] ^ (y >> 1) ^ _mag01[y & 0x1];

                mti = 0;
            }

            y = _mt[mti++];

            /* Tempering */
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= (y >> 18);

            return y;
        }

        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0.
        /// </returns>
        protected override double DoSample()
        {
            return genrand_int32() * _reciprocal;
        }

        /// <summary>
        /// methods to save current state of random number generator
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SerializableMersenneTwister FromFile(string filename)
        {
            //return FileIO.ReadFromXML<SerializableMersenneTwister>(filename);
            var info = FileIO.ReadFromXML<MersenneTwisterSerializationInfo>(filename);

            return SerializableMersenneTwister.Create(info);
        }

        public void ToFile(SerializableMersenneTwister smt, string filename)
        {
            var info = new MersenneTwisterSerializationInfo
            {
                MT = smt.MT,
                MTI = smt.MTI
            };

            FileIO.WriteToXML(info, filename);
        }
    }
}


