using System;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Class containing factory methods returning random number generator instances
    /// </summary>
    public static class RandomNumberGeneratorFactory
    {
        /// <summary>
        /// Returns an instance of the desired random number generator with random seeding
        /// </summary>
        /// <param name="type">RandomNumberGeneratorType</param>
        /// <returns>Random</returns>
        public static Random GetRandomNumberGenerator(RandomNumberGeneratorType type)
        {
            return GetRandomNumberGenerator(type, -1);
        }

        /// <summary>
        /// Returns an instance of the desired random number generator
        /// </summary>
        /// <param name="type">RandomNumberGeneratorType enum</param>
        /// <param name="seed">integer seed for the RNG, seed=-1 -> random seed, otherwise seeded with input seed</param>
        /// <param name="index">The </param>
        /// <returns>Random</returns>
        public static Random GetRandomNumberGenerator(RandomNumberGeneratorType type, int seed, int? index = null)
        {
            if (seed == -1)
            {
                seed = GetRandomSeed();
            }
            switch (type)
            {
                case RandomNumberGeneratorType.MersenneTwister:
                    return new MathNet.Numerics.Random.MersenneTwister(seed, true);
                case RandomNumberGeneratorType.SerializableMersenneTwister:
                    return new Rng.SerializableMersenneTwister(seed, true);
                case RandomNumberGeneratorType.DynamicCreatorMersenneTwister:
                    // Word Length is either 31 or 32
                    // Period Exponent is set to 521 to be sufficiently large to cover the number of photons on each thread
                    // Stream Seed 
                    return index != null ? new Rng.DynamicCreatorMersenneTwister(32, 521, (int) index, 4172, (uint) seed) : new Rng.DynamicCreatorMersenneTwister(seed);
                default:
                    throw new ArgumentOutOfRangeException("type");

            }
        }

        private static int GetRandomSeed()  // ckh 10/01/11 moved to RandomNumberGeneratorFactory
        {
            return (int)DateTime.Now.Ticks % (1 << 15);
        }
    }
}
