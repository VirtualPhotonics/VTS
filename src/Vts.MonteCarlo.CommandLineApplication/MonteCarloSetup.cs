using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.DataStructuresValidation;

namespace Vts.MonteCarlo.CommandLineApplication
{
    public class MonteCarloSetup
    {
        /// <summary>
        /// method to read the simulation input from a specified or default file
        /// </summary>
        public static SimulationInput ReadSimulationInputFromFile(string inputFile)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                Console.WriteLine("\nNo input file specified. Using infile.xml from root mc.exe folder... ");
                return ReadSimulationInputFromFile("infile.xml");
            }

            var path = Path.GetDirectoryName(Path.GetFullPath(inputFile));

            // var basename = Path.GetFileNameWithoutExtension(inputFile);

            var fullFilePath = Path.Combine(path, inputFile);

            if (File.Exists(fullFilePath))
            {
                return SimulationInput.FromFile(fullFilePath);
            }

            if (File.Exists(fullFilePath + ".xml"))
            {
                return SimulationInput.FromFile(fullFilePath + ".xml");
            }

            throw new FileNotFoundException("\nThe following input file could not be found: " + inputFile);
        }

        public static ParameterSweep CreateParameterSweep(string[] parameterSweepString) // todo: check for null returns
        {
            if (parameterSweepString.Length != 4)
            {
                Console.WriteLine("*** Invalid input parameter ***");
                Console.WriteLine("\tinput parameters should have 4 values in the format:");
                Console.WriteLine("\tinputparam=<InputParameterType>,Start,Stop,Delta");
                Console.WriteLine("\tIgnoring this input parameter");
                Console.WriteLine();
                return null;
            }

            try
            {
                var inputParameterType = parameterSweepString[0];

                // batch parameter values should come in fours 
                // eg. inputparam=mua1,-4.0,4.0,0.05 inputparam=mus1,0.5,1.5,0.1 inputparam=mus2,0.5,1.5,0.1 ...
                var start = double.Parse(parameterSweepString[1]);
                var stop = double.Parse(parameterSweepString[2]);
                var delta = double.Parse(parameterSweepString[3]);

                var sweepRange = new DoubleRange(start, stop, (int)((stop - start) / delta) + 1);

                return new ParameterSweep(inputParameterType, sweepRange);
            }
            catch
            {
                Console.WriteLine("Could not parse the input arguments.\n\tIgnoring the following input parameter sweep: " + parameterSweepString);
                return null;
            }
        }

        public static IEnumerable<SimulationInput> ApplyParameterSweeps(SimulationInput input, IEnumerable<ParameterSweep> parameterSweeps)
        {
            IEnumerable<SimulationInput> batchInputs = input.AsEnumerable();

            foreach (var parameterSweep in parameterSweeps)
            {
                var sweepValues = parameterSweep.Range.AsEnumerable();

                // todo: make more dynamic/flexible by removing requirment for enum value, and instead rely on parsing string name and index djc 2011-06-03
                // var inputParameterType = (InputParameterType)Enum.Parse(typeof(InputParameterType), parameterSweep.Name, true);
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

            Output detectorResults = mc.Run();

            input.ToFile(resultsFolder + "\\" + input.OutputName + ".xml");

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
            Parallel.ForEach(inputs, (input, state, index) =>
            {
                input.Options.SimulationIndex = (int)index;
                // todo: should we do something about the seed to avoid correlation? or fix by making wall-clock seed the default?
                RunSimulation(input, outputFolderPath);
            });
        }
    }
}
