using NUnit.Framework;
using Vts.MonteCarlo;

namespace Vts.Test.MonteCarlo
{
    /// <summary>
    /// Unit tests for MonteCarloSimulation
    /// </summary>
    [TestFixture]
    public class MonteCarloSimulationTests
    {
        /// <summary>
        /// Validate General Constructor of Custom Flat Line Source
        /// </summary>
        [Test]
        public void validate_RunAll_given_two_simulations_runs_without_crashing()
        {
            var si1 = new SimulationInput { N = 30 };
            var si2 = new SimulationInput { N = 20 };
            var sim1 = new MonteCarloSimulation(si1);
            var sim2= new MonteCarloSimulation(si2);
            var sims = new[] {sim1, sim2};

            var outputs = MonteCarloSimulation.RunAll(sims);

            Assert.NotNull(outputs[0]);
            Assert.NotNull(outputs[1]);
            Assert.True(outputs[0].Input.N == 30);
            Assert.True(outputs[1].Input.N == 20);
        }
    }
}
