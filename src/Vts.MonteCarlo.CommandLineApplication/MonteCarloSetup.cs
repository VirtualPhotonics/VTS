using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vts.Common;
using Vts.Common.Logging;
using Vts.Extensions;
using Vts.MonteCarlo.DataStructuresValidation;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.CommandLineApplication
{
    public class MonteCarloSetup
    {
        private static ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(MonteCarloSetup));

        /// <summary>
        /// method to read the simulation input from a specified or default file
        /// </summary>
        public static SimulationInput ReadSimulationInputFromFile(string inputFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inputFile))
                {
                    logger.Info(" *** No input file specified ***\n\nDefine an input file using mc.exe infile=infile_name.txt");
                    return null;
                }

                //get the full path for the input file
                var fullFilePath = Path.GetFullPath(inputFile);
                string extension = Path.GetExtension(inputFile);

                if (File.Exists(fullFilePath))
                {
                    return SimulationInput.FromFile(fullFilePath);       
                }

                if (File.Exists(fullFilePath + ".txt"))
                {
                    return SimulationInput.FromFile(fullFilePath + ".txt");
                }

                //throw a file not found exception
                throw new FileNotFoundException("\nThe following input file could not be found: " + fullFilePath + " - type mc help=infile for correct syntax");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //Console.WriteLine(VtsJsonSerializer.TraceWriter.GetTraceMessages());
                return null;
            }
        }

        public static ParameterSweep CreateParameterSweep(string[] parameterSweepString, ParameterSweepType type) // todo: check for null returns
        {
            if ((type == ParameterSweepType.Count || type == ParameterSweepType.Delta))
            {
                if (parameterSweepString.Length != 4)
                {
                    {
                        string message =
                            " *** Invalid sweep parameter ***" +
                            "\n\t\tsweep parameters should have 4 values in the format:";
                        if (type == ParameterSweepType.Delta)
                        {
                            message += "\n\t\tparamsweepdelta=<Parameter>,Start,Stop,Delta";
                        }
                        else
                        {
                            message += "\n\t\tparamsweep=<Parameter>,Start,Stop,Count";
                        }
                        message += "\n\t\tIgnoring this sweep parameter\n";
                        logger.Warn(() => message);
                        return null;
                    }
                }
            }
            else // type==ParameterSweepType.List
            {
                var number = double.Parse(parameterSweepString[1]);
                // check that number is an integer and that number of parameters is 2 more than number
                if ((number == Math.Floor(number)) && (parameterSweepString.Length != number + 2))
                {
                    string message =
                        " *** Invalid sweep parameter: either Number or number of Vals is in error ***" +
                        "\n\t\tsweep parameters should have format paramsweeplist=<Parameter>,NumVals,Val1,...,ValN";
                    message += "\n\t\tIgnoring this sweep parameter\n";
                    logger.Warn(() => message);
                    return null;
                }
            }

            try
            {
                var inputParameterType = parameterSweepString[0];
                double start, stop, delta;
                int count;
                DoubleRange sweepRange = null;
                switch (type)
                {
                    // batch parameter values should come in fours for Delta and Count
                    case ParameterSweepType.Delta:
                        // eg. paramsweepdelta=mua1,-4.0,4.0,0.05 paramsweepdelta=mus1,0.5,1.5,0.1 paramsweepdelta=mus2,0.5,1.5,0.1 ...
                        start = double.Parse(parameterSweepString[1]);
                        stop = double.Parse(parameterSweepString[2]);
                        delta = double.Parse(parameterSweepString[3]);
                        sweepRange = new DoubleRange(start, stop, (int)((stop - start) / delta) + 1);
                        return new ParameterSweep(inputParameterType, sweepRange);
                    case ParameterSweepType.Count: 
                        // eg. paramsweep=mua1,-4.0,4.0,101 paramsweep=mus1,0.5,1.5,3 paramsweep=mus2,0.5,1.5,3 ...
                        start = double.Parse(parameterSweepString[1]);
                        stop = double.Parse(parameterSweepString[2]);
                        count = int.Parse(parameterSweepString[3]);
                        sweepRange = new DoubleRange(start, stop, count);
                        return new ParameterSweep(inputParameterType, sweepRange);
                    case ParameterSweepType.List:
                        var number = int.Parse(parameterSweepString[1]);
                        var sweepList = new double[number];
                        for (int i = 0; i < number; i++)
                        { 
                            sweepList[i]=double.Parse(parameterSweepString[i + 2]);
                        }
                        return new ParameterSweep(inputParameterType, sweepList);
                }
                return null;
            }
            catch
            {
                logger.Error(() => "Could not parse the input arguments.\n\tIgnoring the following input parameter sweep: " + parameterSweepString);
                return null;
            }
        }

        public static IEnumerable<SimulationInput> ApplyParameterSweeps(SimulationInput input, IEnumerable<ParameterSweep> parameterSweeps)
        {
            IEnumerable<SimulationInput> batchInputs = input.AsEnumerable();

            foreach (var parameterSweep in parameterSweeps)
            {
                var sweepValues = parameterSweep.Values.ToEnumerable<double>();

                batchInputs = batchInputs.WithParameterSweep(sweepValues, parameterSweep.Name.ToLower());
            }

            return batchInputs.ToArray();
        }

        public static ValidationResult ValidateSimulationInput(SimulationInput input)
        {
            return SimulationInputValidation.ValidateInput(input);
        }

        public static void RunSimulation(SimulationInput input, string outputFolderPath)
        {
            var mc = new MonteCarloSimulation(input);

            // locate root folder for output, creating it if necessary
            var path = string.IsNullOrEmpty(outputFolderPath) 
                ? Path.GetFullPath(Directory.GetCurrentDirectory()) 
                : Path.GetFullPath(outputFolderPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            // locate destination folder for output, creating it if necessary
            var resultsFolder = Path.Combine(path, input.OutputName);
            if (!Directory.Exists(resultsFolder))
            {
                Directory.CreateDirectory(resultsFolder);
            }

            mc.SetOutputPathForDatabases(path);

            SimulationOutput detectorResults = mc.Run();

            input.ToFile(Path.Combine(resultsFolder, input.OutputName + ".txt"));

            foreach (var result in detectorResults.ResultsDictionary.Values)
            {
                // save all detector data to the specified folder
                DetectorIO.WriteDetectorToFile(result, resultsFolder);
            }
        }

        /// <summary>
        /// Runs multiple Monte Carlo simulations in parallel using all available CPU cores
        /// </summary>
        public static void RunSimulations(IEnumerable<SimulationInput> inputs, string outputFolderPath)
        {
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount};
            Parallel.ForEach(inputs, options, (input, state, index) =>
            {
                input.Options.SimulationIndex = (int)index;
                RunSimulation(input, outputFolderPath);
            });
        }
    }
}
