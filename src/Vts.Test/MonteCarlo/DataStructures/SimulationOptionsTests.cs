using NUnit.Framework;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo
{
    [TestFixture]
    public class SimulationOptionsTests
    {
        [Test]
        public void validate_random_number_seed_is_correct()
        {
            var random1 = new SimulationOptions(-1);
            var deterministic = new SimulationOptions(1);
            Assert.AreNotEqual(random1.Seed, deterministic.Seed);
            // Setting a random seed now performed in RNGFactory
            //System.Threading.Thread.Sleep(1000);
            //var random2 = new SimulationOptions(-1);
            //Assert.AreNotEqual(random1.Seed, random2.Seed);
        }

        [Test]
        public void validate_null_database_input_gets_converted_to_empty_list_correctly()
        {
            var so = new SimulationOptions(
                0,
                RandomNumberGeneratorType.MersenneTwister, 
                AbsorptionWeightingType.Discrete,
                PhaseFunctionType.Bidirectional,
                null,
                false,
                0.0,
                0);
            Assert.IsTrue(so.Databases.Count == 0);
        }

    }
}
