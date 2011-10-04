using System;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// Class containing factory methods returning random number generator instances
    /// </summary>
    public static class RandomNumberGeneratorFactory
    {
        /// <summary>
        /// Returns an instance of the desired random number generator with default seeding
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Random GetRandomNumberGenerator(RandomNumberGeneratorType type)
        {
            // todo: is this the desired default behavior?
            return GetRandomNumberGenerator(type, 0);
        }

        /// <summary>
        /// Returns an instance of the desired random number generator
        /// </summary>
        /// <param name="type"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static Random GetRandomNumberGenerator(RandomNumberGeneratorType type, int seed)
        {
            if (seed == -1)
            {
                seed = GetRandomSeed();
            }
            switch (type)
            {
                case RandomNumberGeneratorType.MersenneTwister:
                    return new MathNet.Numerics.Random.MersenneTwister(seed);
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
