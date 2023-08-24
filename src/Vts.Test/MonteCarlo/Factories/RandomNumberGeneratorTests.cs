using NUnit.Framework;
using System;
using Vts.MonteCarlo.Factories;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for RandomNumberGeneratorFactory
    /// </summary>
    [TestFixture]
    public class RandomNumberGeneratorFactoryTests
    {
        /// <summary>
        /// Simulate basic usage of RandomNumberGeneratorFactory
        /// </summary>
        [Test]
        public void Demonstrate_GetRandomNumberGenerator_successful_return()
        {
            Assert.IsInstanceOf<Random>(
                RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                RandomNumberGeneratorType.MersenneTwister));
            Assert.IsInstanceOf<Random>(
                RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                    RandomNumberGeneratorType.SerializableMersenneTwister));
            Assert.IsInstanceOf<Random>(
                RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                    RandomNumberGeneratorType.DynamicCreatorMersenneTwister));
        }
        /// <summary>
        /// Simulate erroneous invocation
        /// </summary>
        [Test]
        public void Demonstrate_GetRandomNumberGenerator_throw_exception_on_faulty_input()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                     (RandomNumberGeneratorType)(-1)));
        }
    }
}
