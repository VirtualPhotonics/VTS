using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using Vts.Common.Logging;
using Vts.IO;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Rng;
#if BENCHMARK
using BenchmarkDotNet.Attributes;
#endif

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Provides main processing for a single Monte Carlo simulation in parallel. 
    /// </summary>
    public class ParallelMonteCarloSimulation
    {
        /// <summary>
        /// local variables: general
        /// </summary>
        private readonly ILogger _logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(MonteCarloSimulation));

        /// <summary>
        /// SimulationInput class passed in 
        /// </summary>
#if BENCHMARK
        [ParamsSource(nameof(SimulationInputs))]
#endif
        public SimulationInput Input { get; set; }
        /// <summary>
        /// number of CPUs
        /// </summary>
#if BENCHMARK
        public IEnumerable<object> SimulationInputs()
        {
            yield return new SimulationInput { N = 100 };
        }

        [Params(4)]
#endif
        public int NumberOfCPUs { get; set; }
        /// <summary>
        /// simulation statistics
        /// </summary>
        public SimulationStatistics SummedStatistics { get; set; }

        /// <summary>
        /// Class that defines methods to initialize and execute Monte Carlo simulation
        /// </summary>
        /// <param name="input">Simulation Input</param>
        /// <param name="numberOfCPUs">number of parallel CPUs to be run</param>
        public ParallelMonteCarloSimulation(SimulationInput input, int numberOfCPUs)
        {
#if !BENCHMARK
            Input = input;
            NumberOfCPUs = numberOfCPUs;
#endif
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ParallelMonteCarloSimulation() : this(new SimulationInput(), 2) { }

        /// <summary>
        /// Method to run single MC simulation in parallel
        /// </summary>
        /// <returns>array of SimulationOutput</returns>
#if BENCHMARK
        [Benchmark]
#endif
        public SimulationOutput RunSingleInParallel()
        {
            var threads = NumberOfCPUs;
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = NumberOfCPUs
            };
            var photonsPerCPU = Input.N / threads;
            if (photonsPerCPU < 10)
            {
                _logger.Error("The number of CPUs is too high for the number of photons");
                return null;
            }
            var totalPhotons = photonsPerCPU*threads;
            if (photonsPerCPU * threads != Input.N)
            {
                _logger.Info(() => "Note: number of photons run on each CPU = " + photonsPerCPU +
                   " for a total of N = " + totalPhotons);
            }
            Input.N = photonsPerCPU;
            var simulationOutputs = new ConcurrentBag<SimulationOutput>();
            var simulationStatistics = new ConcurrentBag<SimulationStatistics>();

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Code for using partitions
            var partition = Partitioner.Create(0, totalPhotons, photonsPerCPU);
            Parallel.ForEach<Tuple<long, long>, MonteCarloSimulation>(partition, 
                parallelOptions, 
                () => new MonteCarloSimulation(Input.Clone(), true),
                (tSource, parallelLoopState, partitionIndex, monteCarloSimulation) =>
                {
                    monteCarloSimulation.Input.Options.SimulationIndex = (int)partitionIndex;
                    monteCarloSimulation.InitializeParallel(RandomNumberGeneratorFactory.GetRandomNumberGenerator(RandomNumberGeneratorType.DynamicCreatorMersenneTwister, monteCarloSimulation.Input.Options.Seed, (int)partitionIndex)); 
                    monteCarloSimulation.Run();
                    return monteCarloSimulation;
                },
                (x) =>
                {
                    try
                    {
                        if (x.Results == null) return;
                        simulationOutputs.Add(x.Results);
                        if (x.Statistics == null) return;
                        simulationStatistics.Add(x.Statistics);
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"An error occurred creating the simulation outputs: {e.Message}");
                    }
                }
            );
            // reset N back to original so that infile written to results has correct value
            var summedResults = SumResultsTogether(
                simulationOutputs.Select(o => { o.Input.N = totalPhotons; return o; }).ToList()
            );
            SummedStatistics = SumStatisticsTogether(simulationStatistics.ToList());
            // overwrite statistics.txt file (each MC sim will write its own version)
            if (Input.Options.TrackStatistics)
            {
                if (Input.OutputName == "")
                {
                    SummedStatistics.ToFile("statistics.txt");
                }
                else
                {
                    FileIO.CreateDirectory(Input.OutputName);
                    SummedStatistics.ToFile(Input.OutputName + "/statistics.txt");
                }
            }
            stopwatch.Stop();
            _logger.Info(() => "Monte Carlo simulation complete (N = " + totalPhotons +
              " photons run on " + NumberOfCPUs + "CPUs with " + threads + " threads; simulation time = "
                + stopwatch.ElapsedMilliseconds / 1000f + " seconds).\r");

            return summedResults;
        }

        /// <summary>
        /// Sums the statistics from the list of SimulationStatistics
        /// </summary>
        /// <param name="stats">The list of SimulationStatistics</param>
        /// <returns>One instance of SimulationStatistics</returns>
        public SimulationStatistics SumStatisticsTogether(List<SimulationStatistics> stats)
        {
            SimulationStatistics statistics = new SimulationStatistics();
            // check if statistics specified using input.Options.TrackStatistics = true
            if (stats != null)
            {
                PropertyInfo[] properties = typeof(SimulationStatistics).GetProperties();
                //foreach (var prop in properties) // I would like to use this somehow to not have to spell out each Property
                //{
                //var temp = prop.Name;
                statistics.NumberOfPhotonsOutTopOfTissue = stats.Select(s => s.NumberOfPhotonsOutTopOfTissue).Sum();
                statistics.NumberOfPhotonsOutBottomOfTissue = stats.Select(s => s.NumberOfPhotonsOutBottomOfTissue).Sum();
                statistics.NumberOfPhotonsAbsorbed = stats.Select(s => s.NumberOfPhotonsAbsorbed).Sum();
                statistics.NumberOfPhotonsSpecularReflected = stats.Select(s => s.NumberOfPhotonsSpecularReflected).Sum();
                statistics.NumberOfPhotonsKilledOverMaximumPathLength = stats.Select(s => s.NumberOfPhotonsKilledOverMaximumPathLength).Sum();
                statistics.NumberOfPhotonsKilledOverMaximumCollisions = stats.Select(s => s.NumberOfPhotonsKilledOverMaximumCollisions).Sum();
                statistics.NumberOfPhotonsKilledByRussianRoulette = stats.Select(s => s.NumberOfPhotonsKilledByRussianRoulette).Sum();
                //}
            }
            return statistics;
        }

        /// <summary>
        /// Sums the results from the list of SimulationOutputs
        /// </summary>
        /// <param name="results">List of SimulationOutputs</param>
        /// <returns>One instance of SimulationOutput</returns>
        public SimulationOutput SumResultsTogether(List<SimulationOutput> results)
        {
            var simulationOutputKeys = results.First().ResultsDictionary.Keys;
            var simulationInput = results.First().Input;

            var detectorList = results.Select(o => o.GetDetectors(simulationOutputKeys)).FirstOrDefault()?.ToList();
            SimulationOutput summedSimulationOutput = new SimulationOutput(simulationInput, detectorList);

            foreach (var detectorName in simulationOutputKeys)
            {
                // get list of all detectors in list of SimulationOutput with Name=detectorName
                var detectors = results.Select(o => o.GetDetector(detectorName)).ToList();
                var type = detectors.Select(d => ((dynamic)d).Mean).FirstOrDefault().GetType();

                if (type.Equals(typeof(double)))
                {
                    var means = detectors.Select(d => (double)((dynamic)d).Mean).Sum() / NumberOfCPUs;
                    var secondMoments = detectors.Select(d => (double)((dynamic)d).SecondMoment).Sum() / NumberOfCPUs;

                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;

                }
                if (type.Equals(typeof(double[])))
                {
                    var listOfMeans = detectors.Select(d => (double[])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (double[])((dynamic)d).SecondMoment).ToList();
                    int dim = listOfMeans.FirstOrDefault().Length;
                    var means = new double[dim];
                    var secondMoments = new double[dim];

                    for (int i = 0; i < dim; i++)
                    {
                        means[i] = listOfMeans.Select(d => d[i]).Sum() / NumberOfCPUs;
                        if (listOfSMs.FirstOrDefault() != null)
                        {
                            secondMoments[i] = listOfSMs.Select(d => d[i]).Sum() / NumberOfCPUs;
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(double[,])))
                {
                    var listOfMeans = detectors.Select(d => (double[,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (double[,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    var means = new double[dim1, dim2];
                    var secondMoments = new double[dim1, dim2];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            means[i, j] = listOfMeans.Select(d => d[i, j]).Sum() / NumberOfCPUs;
                            if (listOfSMs.FirstOrDefault() != null)
                            {
                                secondMoments[i, j] = listOfSMs.Select(d => d[i, j]).Sum() / NumberOfCPUs;
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(double[,,])))
                {
                    var listOfMeans = detectors.Select(d => (double[,,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (double[,,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    int dim3 = listOfMeans.FirstOrDefault().GetLength(2);
                    var means = new double[dim1, dim2, dim3];
                    var secondMoments = new double[dim1, dim2, dim3];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            for (int k = 0; k < dim3; k++)
                            {
                                means[i, j, k] = listOfMeans.Select(d => d[i, j, k]).Sum() / NumberOfCPUs;
                                if (listOfSMs.FirstOrDefault() != null)
                                {
                                    secondMoments[i, j, k] = listOfSMs.Select(d => d[i, j, k]).Sum() / NumberOfCPUs;
                                }
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(double[,,,])))
                {
                    var listOfMeans = detectors.Select(d => (double[,,,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (double[,,,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    int dim3 = listOfMeans.FirstOrDefault().GetLength(2);
                    int dim4 = listOfMeans.FirstOrDefault().GetLength(3);
                    var means = new double[dim1, dim2, dim3, dim4];
                    var secondMoments = new double[dim1, dim2, dim3, dim4];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            for (int k = 0; k < dim3; k++)
                            {
                                for (int m = 0; m < dim4; m++)
                                {
                                    means[i, j, k, m] = listOfMeans.Select(d => d[i, j, k, m]).Sum() / NumberOfCPUs;
                                    if (listOfSMs.FirstOrDefault() != null)
                                    {
                                        secondMoments[i, j, k, m] = listOfSMs.Select(d => d[i, j, k, m]).Sum() / NumberOfCPUs;
                                    }
                                }
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(double[,,,,])))
                {
                    var listOfMeans = detectors.Select(d => (double[,,,,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (double[,,,,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    int dim3 = listOfMeans.FirstOrDefault().GetLength(2);
                    int dim4 = listOfMeans.FirstOrDefault().GetLength(3);
                    int dim5 = listOfMeans.FirstOrDefault().GetLength(4);
                    var means = new double[dim1, dim2, dim3, dim4, dim5];
                    var secondMoments = new double[dim1, dim2, dim3, dim4, dim5];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            for (int k = 0; k < dim3; k++)
                            {
                                for (int m = 0; m < dim4; m++)
                                {
                                    for (int n = 0; n < dim5; n++)
                                    {
                                        means[i, j, k, m, n] = listOfMeans.Select(d => d[i, j, k, m, n]).Sum() / NumberOfCPUs;
                                        if (listOfSMs.FirstOrDefault() != null)
                                        {
                                            secondMoments[i, j, k, m, n] = listOfSMs.Select(d => d[i, j, k, m, n]).Sum() / NumberOfCPUs;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                // Complex types
                if (type.Equals(typeof(Complex[])))
                {
                    var listOfMeans = detectors.Select(d => (Complex[])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (Complex[])((dynamic)d).SecondMoment).ToList();
                    int dim = listOfMeans.FirstOrDefault().Length;
                    var means = new Complex[dim];
                    var secondMoments = new Complex[dim];

                    for (int i = 0; i < dim; i++)
                    {
                        means[i] = listOfMeans.Select(d => d[i].Real).Sum() / NumberOfCPUs +
                            Complex.ImaginaryOne * listOfMeans.Select(d => d[i].Imaginary).Sum() / NumberOfCPUs;
                        if (listOfSMs.FirstOrDefault() != null)
                        {
                            secondMoments[i] = listOfSMs.Select(d => d[i].Real).Sum() / NumberOfCPUs +
                               Complex.ImaginaryOne * listOfSMs.Select(d => d[i].Imaginary).Sum() / NumberOfCPUs;
                        }
                    }
                   ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(Complex[,])))
                {
                    var listOfMeans = detectors.Select(d => (Complex[,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (Complex[,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    var means = new Complex[dim1, dim2];
                    var secondMoments = new Complex[dim1, dim2];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            means[i, j] = listOfMeans.Select(d => d[i, j].Real).Sum() / NumberOfCPUs +
                                Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j].Imaginary).Sum() / NumberOfCPUs;
                            if (listOfSMs.FirstOrDefault() != null)
                            {
                                secondMoments[i, j] = listOfSMs.Select(d => d[i, j].Real).Sum() / NumberOfCPUs +
                                    Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j].Imaginary).Sum() / NumberOfCPUs;
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(Complex[,,])))
                {
                    var listOfMeans = detectors.Select(d => (Complex[,,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (Complex[,,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    int dim3 = listOfMeans.FirstOrDefault().GetLength(2);
                    var means = new Complex[dim1, dim2, dim3];
                    var secondMoments = new Complex[dim1, dim2, dim3];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            for (int k = 0; k < dim3; k++)
                            {
                                means[i, j, k] = listOfMeans.Select(d => d[i, j, k].Real).Sum() / NumberOfCPUs +
                                    Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j, k].Imaginary).Sum() / NumberOfCPUs;
                                if (listOfSMs.FirstOrDefault() != null)
                                {
                                    secondMoments[i, j, k] = listOfSMs.Select(d => d[i, j, k].Real).Sum() / NumberOfCPUs +
                                        Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j, k].Imaginary).Sum() / NumberOfCPUs;
                                }
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(Complex[,,,])))
                {
                    var listOfMeans = detectors.Select(d => (Complex[,,,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (Complex[,,,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    int dim3 = listOfMeans.FirstOrDefault().GetLength(2);
                    int dim4 = listOfMeans.FirstOrDefault().GetLength(3);
                    var means = new Complex[dim1, dim2, dim3, dim4];
                    var secondMoments = new Complex[dim1, dim2, dim3, dim4];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            for (int k = 0; k < dim3; k++)
                            {
                                for (int m = 0; m < dim4; m++)
                                {
                                    means[i, j, k, m] = listOfMeans.Select(d => d[i, j, k, m].Real).Sum() / NumberOfCPUs +
                                        Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j, k, m].Imaginary).Sum() / NumberOfCPUs;
                                    if (listOfSMs.FirstOrDefault() != null)
                                    {
                                        secondMoments[i, j, k, m] = listOfSMs.Select(d => d[i, j, k, m].Real).Sum() / NumberOfCPUs +
                                            Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j, k, m].Imaginary).Sum() / NumberOfCPUs;
                                    }
                                }
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                if (type.Equals(typeof(Complex[,,,,])))
                {
                    var listOfMeans = detectors.Select(d => (Complex[,,,,])((dynamic)d).Mean).ToList();
                    var listOfSMs = detectors.Select(d => (Complex[,,,,])((dynamic)d).SecondMoment).ToList();
                    int dim1 = listOfMeans.FirstOrDefault().GetLength(0);
                    int dim2 = listOfMeans.FirstOrDefault().GetLength(1);
                    int dim3 = listOfMeans.FirstOrDefault().GetLength(2);
                    int dim4 = listOfMeans.FirstOrDefault().GetLength(3);
                    int dim5 = listOfMeans.FirstOrDefault().GetLength(4);
                    var means = new Complex[dim1, dim2, dim3, dim4, dim5];
                    var secondMoments = new Complex[dim1, dim2, dim3, dim4, dim5];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            for (int k = 0; k < dim3; k++)
                            {
                                for (int m = 0; m < dim4; m++)
                                {
                                    for (int n = 0; n < dim5; n++)
                                    {
                                        means[i, j, k, m, n] = listOfMeans.Select(d => d[i, j, k, m, n].Real).Sum() / NumberOfCPUs +
                                            Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j, k, m, n].Imaginary).Sum() / NumberOfCPUs;
                                        if (listOfSMs.FirstOrDefault() != null)
                                        {
                                            secondMoments[i, j, k, m, n] = listOfSMs.Select(d => d[i, j, k, m, n].Real).Sum() / NumberOfCPUs +
                                                Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j, k, m, n].Imaginary).Sum() / NumberOfCPUs;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).Mean = means;
                    ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).SecondMoment = secondMoments;
                }
                // obtain TallyCounts at end since same type for all detectors
                var tallyCounts = detectors.Select(d => (long)((dynamic)d).TallyCount).Sum();
                ((dynamic)summedSimulationOutput.ResultsDictionary[detectorName]).TallyCount = tallyCounts;
            }

            return summedSimulationOutput;
        }

    }
}
