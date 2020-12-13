using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vts.Common.Logging;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Detectors;
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
        private SimulationInput _input;
        private IList<ISource> _sources;
        private ITissue _tissue;
        private VirtualBoundaryController _virtualBoundaryController;
        //private IList<IDetectorController> _detectorControllers; // total list indep. of VBs
        private long _numberOfPhotons;
        private SimulationStatistics _simulationStatistics;
        private DatabaseWriterController _databaseWriterController = null;
        private pMCDatabaseWriterController _pMCDatabaseWriterController = null;
        private bool _doPMC = false;
        private Random _rng;
        private string _outputPath;

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
            _outputPath = "";
            _numberOfPhotons = input.N; 
            
        }

        // private properties
        /// <summary>
        /// Integer index of Simulation specified in infile
        /// </summary>
        private int SimulationIndex { get; set; }

        /// <summary>
        /// Results of the simulation 
        /// </summary>
        public SimulationOutput Results { get; private set; }

        /// <summary>
        /// Method to run single MC simulation in parallel
        /// </summary>
        /// <param name="simulations">array of MonteCarloSimulation</param>
        /// <returns>array of SimulationOutput</returns>
        public SimulationOutput RunSingleInParallel()
        {
            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount,
                _input.Options.SimulationIndex + 1);  // try 2 to start make command line option?  
            int photonsPerCPU = (int)( _input.N / parallelOptions.MaxDegreeOfParallelism);
            _input.N = photonsPerCPU;
            var virtualBoundaryControllers = new List<VirtualBoundaryController>();
            Parallel.For(0, parallelOptions.MaxDegreeOfParallelism, parallelOptions, cpuIndex =>
            {
                // FIX back to factory once know correct call
                var parallelRng = //RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                    new DynamicCreatorMersenneTwister(32, 521, cpuIndex, 4172, (uint)_input.Options.Seed);
                var mc = new MonteCarloSimulation(_input, parallelRng);
                var output = mc.Run();
                virtualBoundaryControllers.Add(mc.VBController);

            });
            // All VirtualBoundaries are same in each CPU, add in first one
            VirtualBoundaryController uberVBController = new VirtualBoundaryController(
                virtualBoundaryControllers.First().VirtualBoundaries);
            var detectorsInList = uberVBController.VirtualBoundaries
                .Select(vb => vb.DetectorController)
                .Where(dc => dc != null)
                .SelectMany(dc => dc.Detectors).ToList();
            var detectorsTotal = new List<IDetector>();
            // sum detector results
            foreach (var vbController in virtualBoundaryControllers.Skip(1)) 
            {
                foreach (var virtualBoundary in vbController.VirtualBoundaries)
                {                    
                    if (virtualBoundary.DetectorController != null)
                    {
                        foreach (var detector in virtualBoundary.DetectorController.Detectors)
                        {
                            double dum1 = ((ATotalDetector)detector).Mean;
                            double dum2 = ((ATotalDetector)detectorsInList.Select(d => d == detector)).Mean;
                            ((ATotalDetector)detector).Mean = dum1 + dum2;
                            detectorsTotal.Add(detector);
                        }
                    }
                }
                // TODO assign list to uberVBController
                var virtualBoundaries = vbController.VirtualBoundaries;
            }
            
            //NormalizeResults(_numberOfPhotons, uberVBController);

            Results = new SimulationOutput(_input, detectorsTotal);
            return Results;
        }

    }
}
