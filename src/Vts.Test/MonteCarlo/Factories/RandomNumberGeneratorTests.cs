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
            Assert.That(RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                RandomNumberGeneratorType.MersenneTwister), Is.InstanceOf<Random>());
            Assert.That(RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                    RandomNumberGeneratorType.SerializableMersenneTwister), Is.InstanceOf<Random>());
            Assert.That(RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                    RandomNumberGeneratorType.DynamicCreatorMersenneTwister), Is.InstanceOf<Random>());
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
