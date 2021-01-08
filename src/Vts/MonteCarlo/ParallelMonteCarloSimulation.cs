using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vts.Common.Logging;
using Vts.MonteCarlo.Rng;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Provides main processing for a single Monte Carlo simulation in parallel. 
    /// </summary>
    public class ParallelMonteCarloSimulation : MonteCarloSimulation
    {
        /// <summary>
        /// local variables: general
        /// </summary>
        private ILogger _logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(MonteCarloSimulation));

        /// <summary>
        /// local variable: input related
        /// </summary>
        long _numberOfPhotons;

        /// <summary>
        /// Class that takes in SimulationInput and methods to initialize and execute Monte Carlo simulation
        /// </summary>
        /// <param name="input">SimulationInput</param>
        public ParallelMonteCarloSimulation(SimulationInput input) 
        {
            // all field/property defaults should be set here
            //_rng = rng;
            this.SimulationIndex = input.Options.SimulationIndex;
            _input = input;
            _numberOfPhotons = input.N; 
            
        }

        // private properties
        /// <summary>
        /// Integer index of Simulation specified in infile
        /// </summary>
        private int SimulationIndex { get; set; }

        /// <summary>
        /// Method to run single MC simulation in parallel
        /// </summary>
        /// <returns>array of SimulationOutput</returns>
        public SimulationOutput RunSingleInParallel()
        {
            var numberOfCPUs = Math.Min(Environment.ProcessorCount,
                _input.Options.SimulationIndex + 1);  // try 2 to start make command line option? 
            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = numberOfCPUs;
            int photonsPerCPU = (int)(_input.N / numberOfCPUs);
            _input.N = photonsPerCPU;
            var simulationOutputs = new List<SimulationOutput>();
            Parallel.For(0, parallelOptions.MaxDegreeOfParallelism, parallelOptions, cpuIndex =>
            {
                // FIX back to factory once know correct call
                var parallelRng = //RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                    new DynamicCreatorMersenneTwister(32, 521, cpuIndex, 4172, (uint)_input.Options.Seed);
                var mc = new MonteCarloSimulation(_input, parallelRng);
                mc.Run();
                simulationOutputs.Add(mc.Results);
            });
            return SumResultsTogether(simulationOutputs, numberOfCPUs);
        }
        
        public SimulationOutput SumResultsTogether(IList<SimulationOutput> results, int numberOfCPUs)
        {
            // need to try higher dimension Means
            // what to do about statistics?      

            var simulationOutputKeys = results[0].ResultsDictionary.Keys;
            var simulationInput = results[0].Input;
            var detectorList = results.Select(o => o.GetDetectors(simulationOutputKeys)).FirstOrDefault()?.ToList();
            SimulationOutput summedSimulationOutput = new SimulationOutput(simulationInput, detectorList);

            foreach (var detectorName in simulationOutputKeys)
            {
                // get list of all detectors in list of SimulationOutput with Name=detectorName
                var detectors = results.Select(o => o.GetDetector(detectorName)).ToList();
                var type = detectors.Select(d => ((dynamic)d).Mean).FirstOrDefault().GetType();

                if (type.Equals(typeof(double)))
                {
                    var means = detectors.Select(d => (double)((dynamic)d).Mean).Sum() / numberOfCPUs;
                    var secondMoments = detectors.Select(d => (double)((dynamic)d).SecondMoment).Sum() / numberOfCPUs;

                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;

                }
                if (type.Equals(typeof(double[])))
                {
                    var listOfMeans = detectors.Select(d => (double[])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (double[])((dynamic)d).SecondMoment).ToList();
                    int meanCount = listOfMeans.FirstOrDefault().Length;
                    var means = new double[meanCount];
                    var secondMoments = new double[meanCount];

                    for (int i = 0; i < meanCount; i++)
                    {
                        means[i] = listOfMeans.Select(d => d[i]).Sum() / numberOfCPUs;
                        if (listOfSMs.FirstOrDefault() != null)
                        {
                            secondMoments[i] += listOfSMs.Select(d => d[i]).Sum() / numberOfCPUs;
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    //((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                // can add TallyCounts at end since same type for all detectors
                var tallyCounts = detectors.Select(d => (long)((dynamic)d).TallyCount).Sum();
                ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).TallyCount = tallyCounts;
            }

            return summedSimulationOutput;
        }

    }
}
