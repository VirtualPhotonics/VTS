using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vts.Common.Logging;
using Vts.MonteCarlo.Rng;
using System.Reflection;
using Vts.IO;
using System.Threading;
using System;

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
        private ILogger _logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(MonteCarloSimulation));

        /// <summary>
        /// local variable: 
        /// </summary>
        private int _numberOfCPUs;
        /// <summary>
        /// simulation statistics
        /// </summary>
        public SimulationStatistics SummedStatistics { get; set; }

        /// <summary>
        /// Class that takes in SimulationInput and methods to initialize and execute Monte Carlo simulation
        /// </summary>
        /// <param name="input">SimulationInput</param>
        public ParallelMonteCarloSimulation(int numberOfCPUs) 
        {
            // all field/property defaults should be set here
            _numberOfCPUs = numberOfCPUs;            
        }

        public void RunSingleInParallel()
        {
            int[] nums = Enumerable.Range(0, 1000000).ToArray();
            long total = 0;

            // Use type parameter to make subtotal a long, not an int
            Parallel.For<long>(0, nums.Length, () => 0, (j, loop, subtotal) =>
            {
                subtotal += nums[j];
                return subtotal;
            },
                (x) => Interlocked.Add(ref total, x)
            );

            Console.WriteLine("The total is {0:N0}", total);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }


        /// <summary>
        /// Method to run single MC simulation in parallel
        /// </summary>
        /// <returns>array of SimulationOutput</returns>
        public SimulationOutput RunSingleInParallel(SimulationInput input)
        { 
            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = _numberOfCPUs;
            int photonsPerCPU = (int)(input.N / _numberOfCPUs);
            if (photonsPerCPU * _numberOfCPUs != input.N)
            {
                _logger.Info(() => "Note: number of photons run on each CPU = " + photonsPerCPU + 
                   " for a total of N = " + photonsPerCPU * _numberOfCPUs); 
            }
            input.N = photonsPerCPU;
            var simulationOutputs = new List<SimulationOutput>();
            var simulationStatistics = new List<SimulationStatistics>();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            Parallel.For(0, parallelOptions.MaxDegreeOfParallelism, parallelOptions, cpuIndex =>
            {
                // FIX back to factory once know correct call
                var parallelRng = //RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                    new DynamicCreatorMersenneTwister(32, 521, cpuIndex, 4172, (uint)input.Options.Seed);
                input.Options.SimulationIndex = cpuIndex;
                var mc = new MonteCarloSimulation(input, parallelRng);
                mc.Run();
                simulationOutputs.Add(mc.Results);
                if (input.Options.TrackStatistics)
                {
                    simulationStatistics.Add(mc.Statistics);
                }
            });
            // reset N back to original so that infile written to results has correct value
            input.N = _numberOfCPUs * photonsPerCPU;
            var summedResults = SumResultsTogether(simulationOutputs);
            SummedStatistics = SumStatisticsTogether(simulationStatistics);
            // overwrite statistics.txt file (each MC sim will write its own version)
            if (input.Options.TrackStatistics)
            {
                if (input.OutputName == "")
                {
                    SummedStatistics.ToFile("statistics.txt");
                }
                else
                {
                    FileIO.CreateDirectory(input.OutputName);
                    SummedStatistics.ToFile(input.OutputName + "/statistics.txt"); 
                }
            }
            stopwatch.Stop();
            _logger.Info(() => "Monte Carlo simulation complete (N = " + input.N + 
              " photons run on " + _numberOfCPUs + "CPUs; simulation time = "
                + stopwatch.ElapsedMilliseconds / 1000f + " seconds).\r");

            return summedResults;
        }

        public SimulationStatistics SumStatisticsTogether(IList<SimulationStatistics> stats)
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
        public SimulationOutput SumResultsTogether(IList<SimulationOutput> results)
        {    
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
                    var means = detectors.Select(d => (double)((dynamic)d).Mean).Sum() / _numberOfCPUs;
                    var secondMoments = detectors.Select(d => (double)((dynamic)d).SecondMoment).Sum() / _numberOfCPUs;

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
                        means[i] = listOfMeans.Select(d => d[i]).Sum() / _numberOfCPUs;
                        if (listOfSMs.FirstOrDefault() != null)
                        {
                            secondMoments[i] = listOfSMs.Select(d => d[i]).Sum() / _numberOfCPUs;
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
                    var means = new double[dim1,dim2];
                    var secondMoments = new double[dim1,dim2];

                    for (int i = 0; i < dim1; i++)
                    {
                        for (int j = 0; j < dim2; j++)
                        {
                            means[i,j] = listOfMeans.Select(d => d[i,j]).Sum() / _numberOfCPUs;
                            if (listOfSMs.FirstOrDefault() != null)
                            {
                                secondMoments[i,j] = listOfSMs.Select(d => d[i,j]).Sum() / _numberOfCPUs;
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
                                means[i, j, k] = listOfMeans.Select(d => d[i, j, k]).Sum() / _numberOfCPUs;
                                if (listOfSMs.FirstOrDefault() != null)
                                {
                                    secondMoments[i, j, k] = listOfSMs.Select(d => d[i, j, k]).Sum() / _numberOfCPUs;
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
                                    means[i, j, k, m] = listOfMeans.Select(d => d[i, j, k, m]).Sum() / _numberOfCPUs;
                                    if (listOfSMs.FirstOrDefault() != null)
                                    {
                                        secondMoments[i, j, k, m] = listOfSMs.Select(d => d[i, j, k, m]).Sum() / _numberOfCPUs;
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
                                        means[i, j, k, m, n] = listOfMeans.Select(d => d[i, j, k, m, n]).Sum() / _numberOfCPUs;
                                        if (listOfSMs.FirstOrDefault() != null)
                                        {
                                            secondMoments[i, j, k, m, n] = listOfSMs.Select(d => d[i, j, k, m, n]).Sum() / _numberOfCPUs;
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
                        means[i] = listOfMeans.Select(d => d[i].Real).Sum() / _numberOfCPUs +
                            Complex.ImaginaryOne * listOfMeans.Select(d => d[i].Imaginary).Sum() / _numberOfCPUs;
                        if (listOfSMs.FirstOrDefault() != null)
                        {
                            secondMoments[i] = listOfSMs.Select(d => d[i].Real).Sum() / _numberOfCPUs +
                               Complex.ImaginaryOne * listOfSMs.Select(d => d[i].Imaginary).Sum() / _numberOfCPUs;
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
                            means[i, j] = listOfMeans.Select(d => d[i, j].Real).Sum() / _numberOfCPUs +
                                Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j].Imaginary).Sum() / _numberOfCPUs;
                            if (listOfSMs.FirstOrDefault() != null)
                            {
                                secondMoments[i, j] = listOfSMs.Select(d => d[i, j].Real).Sum() / _numberOfCPUs +
                                    Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j].Imaginary).Sum() / _numberOfCPUs;
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
                                means[i, j, k] = listOfMeans.Select(d => d[i, j, k].Real).Sum() / _numberOfCPUs +
                                    Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j, k].Imaginary).Sum() / _numberOfCPUs;
                                if (listOfSMs.FirstOrDefault() != null)
                                {
                                    secondMoments[i, j, k] = listOfSMs.Select(d => d[i, j, k].Real).Sum() / _numberOfCPUs +
                                        Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j, k].Imaginary).Sum() / _numberOfCPUs;
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
                                    means[i, j, k, m] = listOfMeans.Select(d => d[i, j, k, m].Real).Sum() / _numberOfCPUs +
                                        Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j, k, m].Imaginary).Sum() / _numberOfCPUs;
                                    if (listOfSMs.FirstOrDefault() != null)
                                    {
                                        secondMoments[i, j, k, m] = listOfSMs.Select(d => d[i, j, k, m].Real).Sum() / _numberOfCPUs +
                                            Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j, k, m].Imaginary).Sum() / _numberOfCPUs;
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
                                        means[i, j, k, m, n] = listOfMeans.Select(d => d[i, j, k, m, n].Real).Sum() / _numberOfCPUs +
                                            Complex.ImaginaryOne * listOfMeans.Select(d => d[i, j, k, m, n].Imaginary).Sum() / _numberOfCPUs;
                                        if (listOfSMs.FirstOrDefault() != null)
                                        {
                                            secondMoments[i, j, k, m, n] = listOfSMs.Select(d => d[i, j, k, m, n].Real).Sum() / _numberOfCPUs +
                                                Complex.ImaginaryOne * listOfSMs.Select(d => d[i, j, k, m, n].Imaginary).Sum() / _numberOfCPUs;
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
