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

        public static ParameterSweep CreateParameterSweep(string[] parameterSweepString, bool useDelta) // todo: check for null returns
        {
            if (parameterSweepString.Length != 4)
            {
                string message = 
                    " *** Invalid sweep parameter ***" +
                    "\n\t\tsweep parameters should have 4 values in the format:";

                if (useDelta)
                {
                    message += "\n\t\tparamsweepdelta=<ParameterSweepType>,Start,Stop,Delta";
                }
                else
                {
                     message += "\n\t\tparamsweep=<ParameterSweepType>,Start,Stop,Count";
                }

                message += "\n\t\tIgnoring this sweep parameter\n";

                logger.Warn(() => message);
                return null;
            }

            try
            {
                var inputParameterType = parameterSweepString[0];
                DoubleRange sweepRange = null;
                // batch parameter values should come in fours
                if (useDelta)
                {
                    // eg. paramsweepdelta=mua1,-4.0,4.0,0.05 paramsweepdelta=mus1,0.5,1.5,0.1 paramsweepdelta=mus2,0.5,1.5,0.1 ...
                    var start = double.Parse(parameterSweepString[1]);
                    var stop = double.Parse(parameterSweepString[2]);
                    var delta = double.Parse(parameterSweepString[3]);

                    sweepRange = new DoubleRange(start, stop, (int)((stop - start) / delta) + 1);
                }
                else
                {
                    // batch parameter values should come in fours 
                    // eg. paramsweep=mua1,-4.0,4.0,101 paramsweep=mus1,0.5,1.5,3 paramsweep=mus2,0.5,1.5,3 ...
                    var start = double.Parse(parameterSweepString[1]);
                    var stop = double.Parse(parameterSweepString[2]);
                    var count = int.Parse(parameterSweepString[3]);

                    sweepRange = new DoubleRange(start, stop, count);
                }

                return new ParameterSweep(inputParameterType, sweepRange);
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
                var sweepValues = parameterSweep.Range.AsEnumerable();

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
