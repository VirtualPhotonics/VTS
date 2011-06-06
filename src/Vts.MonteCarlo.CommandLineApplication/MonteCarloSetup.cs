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

            var fullFilePath = path + "\\" + inputFile;

            if (!File.Exists(fullFilePath))
            {
                throw new FileNotFoundException("\nThe following input file could not be found: " + inputFile);
            }

            return SimulationInput.FromFile(fullFilePath);
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
                //var inputParameterType = (InputParameterType)Enum.Parse(typeof(InputParameterType), parameterSweepString[0], true);

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
            //IEnumerable<string> batchNames = input.OutputName.AsEnumerable();
            IEnumerable<SimulationInput> batchInputs = input.AsEnumerable();

            foreach (var parameterSweep in parameterSweeps)
            {
                var sweepValues = parameterSweep.Range.AsEnumerable();

                //batchNames =
                //    (from bn in batchNames
                //     from s in sweepValues
                //     select (bn + "_" + String.Format("{0:f}", s))).ToArray();

                // todo: make more dynamic/flexible by removing requirment for enum value, and instead rely on parsing string name and index djc 2011-06-03
                // var inputParameterType = (InputParameterType)Enum.Parse(typeof(InputParameterType), parameterSweep.Name, true);
                batchInputs = batchInputs.WithParameterSweep(sweepValues, parameterSweep.Name.ToLower());
            }

            return batchInputs;
        }

        public static ValidationResult ValidateSimulationInput(SimulationInput input)
        {
            return SimulationInputValidation.ValidateInput(input);
        }

        public static void RunSimulation(SimulationInput input, string outputFolderPath)
        {
            var mc = new MonteCarloSimulation(input);

            var resultsFolder = outputFolderPath + "\\" + input.OutputName;
            //var p = Path.GetDirectoryName(input.OutputName);
            // create folder for output
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
            Parallel.ForEach(inputs, input => RunSimulation(input, outputFolderPath));
        }
    }
}
