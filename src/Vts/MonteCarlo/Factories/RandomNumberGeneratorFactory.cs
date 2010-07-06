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
            switch (type)
            {
                case RandomNumberGeneratorType.MersenneTwister:
                    return new MathNet.Numerics.Random.MersenneTwister(seed);
                default:
                    throw new ArgumentOutOfRangeException("type");

            }
        }
    }
}
