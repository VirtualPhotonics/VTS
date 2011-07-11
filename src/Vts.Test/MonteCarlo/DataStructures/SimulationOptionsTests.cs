using System.IO;
using System.Runtime.Serialization;
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
            System.Threading.Thread.Sleep(5000);
            var random2 = new SimulationOptions(-1);
            Assert.AreNotEqual(random1.Seed, random2.Seed);
        }

    }
}
