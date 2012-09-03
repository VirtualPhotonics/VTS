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
        /// <returns>Random</returns>
        public static Random GetRandomNumberGenerator(RandomNumberGeneratorType type, int seed)
        {
            if (seed == -1)
            {
                seed = GetRandomSeed();
            }
            switch (type)
            {
                case RandomNumberGeneratorType.MersenneTwister:
                    return new MathNet.Numerics.Random.MersenneTwister(seed, true);
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
