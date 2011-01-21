using System.Threading;
using System.Threading.Tasks;

namespace Vts.MonteCarlo.Wcf
{
    public class MonteCarloService : IMonteCarloService
    {
        public bool[] ExecuteBatch(SimulationInput[] inputs)
        {
            bool[] success = new bool[inputs.Length];

            Parallel.For(0, inputs.Length, i =>
            {
                var mc = new MonteCarloSimulation(
                         inputs[i]);

                mc.Run().ToFile(inputs[i].OutputFileName);

                success[i] = true;
            });

            return success;
        }
    }
}
