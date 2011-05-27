using System.Threading;
using System.Threading.Tasks;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.Wcf
{
    public class MonteCarloService : IMonteCarloService
    {
        public bool[] ExecuteBatch(SimulationInput[] inputs)
        {
            bool[] success = new bool[inputs.Length];

            Parallel.For(0, inputs.Length, i =>
            {
                Output detectorResults = new MonteCarloSimulation(inputs[i]).Run();
                foreach (var result in detectorResults.ResultsDictionary.Values)
                {
                    // save all detector data to the specified folder
                    DetectorIO.WriteDetectorToFile(result, inputs[i].OutputFolder);
                }

                success[i] = true;
            });

            return success;
        }
    }
}
