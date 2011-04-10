#define GENERATE_INFILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vts.Common;
using Vts.Extensions;
using System.IO;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

// ParallelFx June '08 CTP
//using System.Threading.Collections;

namespace Vts.MonteCarlo.CommandLineApplication
{
    #region CommandLine Arguments Parser

    /* Simple commandline argument parser written by Ananth B. http://www.ananthonline.net */
    static class CommandLine
    {
        public class Switch // Class that encapsulates switch data.
        {
            public Switch(string name, string shortForm, Action<IEnumerable<string>> handler)
            {
                Name = name;
                ShortForm = shortForm;
                Handler = handler;
            }

            public Switch(string name, Action<IEnumerable<string>> handler)
            {
                Name = name;
                ShortForm = null;
                Handler = handler;
            }

            public string Name { get; private set; }
            public string ShortForm { get; private set; }
            public Action<IEnumerable<string>> Handler { get; private set; }

            public int InvokeHandler(string[] values)
            {
                Handler(values);
                return 1;
            }
        }

        /* The regex that extracts names and comma-separated values for switches 
        in the form (<switch>[="value 1",value2,...])+ */
        private static readonly Regex ArgRegex =
            new Regex(@"(?<name>[^=]+)=?((?<quoted>\""?)(?<value>(?(quoted)[^\""]+|[^,]+))\""?,?)*",
                RegexOptions.Compiled | RegexOptions.CultureInvariant |
                RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        private const string NameGroup = "name"; // Names of capture groups
        private const string ValueGroup = "value";

        public static void Process(this string[] args, Action printUsage, params Switch[] switches)
        {
            /* Run through all matches in the argument list and if any of the switches 
            match, get the values and invoke the handler we were given. We do a Sum() 
            here for 2 reasons; a) To actually run the handlers
            and b) see if any were invoked at all (each returns 1 if invoked).
            If none were invoked, we simply invoke the printUsage handler. */
            if ((from arg in args
                 from Match match in ArgRegex.Matches(arg)
                 from s in switches
                 where match.Success &&
                     ((string.Compare(match.Groups[NameGroup].Value, s.Name, true) == 0) ||
                     (string.Compare(match.Groups[NameGroup].Value, s.ShortForm, true) == 0))
                 select s.InvokeHandler(match.Groups[ValueGroup].Value.Split(','))).Sum() == 0)
                printUsage(); // We didn't find any switches
        }
    }

    #endregion

    class MonteCarloSetup
    {
        private string path = "";
        private string basename = "newinfile";

        public bool ValidSimulation { get; set; }

        public string InputFile { get; set; }
        public string OutputFile { get; set; }

        public bool RunUnmanagedCode { get; set; }
        public bool WriteHistories { get; set; }

        public IEnumerable<SimulationInput> BatchQuery { get; set; }
        public string[] BatchNameQuery { get; set; }
        public SimulationInput Input { get; set; }

        public MonteCarloSetup()
        {
            ValidSimulation = true;
            InputFile = "";
            OutputFile = "results";
            RunUnmanagedCode = false;
            WriteHistories = false;
            BatchQuery = null;
            BatchNameQuery = null;
            Input = null;
        }

        /// <summary>
        /// method to read the simulation input from a specified or default file
        /// </summary>
        public bool ReadSimulationInputFromFile()
        {
            if (InputFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(InputFile)) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(InputFile);
                InputFile = path + basename + ".xml";

                if (System.IO.File.Exists(InputFile))
                {
                    Input = SimulationInput.FromFile(InputFile);
                }
                else
                {
                    Console.WriteLine("\nThe following input file could not be found: " + basename + ".xml");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("\nNo input file specified. Using newinfile.xml from resources... ");
                Input = SimulationInput.FromFile("newinfile.xml");
            }
            BatchQuery = Input.AsEnumerable();
            BatchNameQuery = new[] { "" };
            return true;
        }

        /// <summary>
        /// method for seting the range values for a specific InputParameterType
        /// </summary>
        /// <param name="val">IEnumerable of string values representing the InputParameterType and the range</param>
        public void SetRangeValues(IEnumerable<string> val)
        {
            IEnumerable<double> sweep = null;
            InputParameterType inputParameterType;

            if (val.Count() == 4)
            {
                try
                {
                    inputParameterType = (InputParameterType)Enum.Parse(typeof(InputParameterType), val.ElementAt(0), true);

                    // batch parameter values should come in fours 
                    // eg. inputparam=mua1,-4.0,4.0,0.05 inputparam=mus1,0.5,1.5,0.1 inputparam=mus2,0.5,1.5,0.1 ...
                    var start = double.Parse(val.ElementAt(1));
                    var stop = double.Parse(val.ElementAt(2));
                    var delta = double.Parse(val.ElementAt(3));

                    sweep = new DoubleRange(start, stop, (int)((stop - start) / delta) + 1).AsEnumerable();

                    if (BatchQuery == null)
                    {
                        ValidSimulation = ReadSimulationInputFromFile();
                    }

                    if (ValidSimulation)
                    {
                        string inputParameterString = inputParameterType.ToString().ToLower();

                        if (inputParameterString.Length > 0)
                        {
                            BatchQuery = BatchQuery.WithParameterSweep(sweep, inputParameterType);
                            BatchNameQuery =
                                            (from b in BatchNameQuery
                                             from s in sweep
                                             select (b + inputParameterString + "_" + String.Format("{0:f}", s) + "_")).ToArray();
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Could not parse the input arguments.");
                }
            }
            else
            {
                Console.WriteLine("Input parameters should have 4 values in the format inputparam=<InputParameterType>,Start,Stop,Delta.");
            }
        }

        /// <summary>
        /// Runs the Monte Carlo simulation
        /// </summary>
        public void RunSimulation()
        {
            SimulationInput[] inputBatch = BatchQuery.ToArray();
            string[] outNames = BatchNameQuery
                .Select(s => path + basename + "_" + OutputFile + "\\" + basename + "_" + OutputFile + s)
                .ToArray();

            for (int i = 0; i < inputBatch.Length; i++)
            {
                inputBatch[i].OutputFileName = outNames[i];
            }

            Parallel.ForEach(inputBatch, input =>
            {
                var mc = new MonteCarloSimulation(input);

                var p = Path.GetDirectoryName(input.OutputFileName);

                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }

                Output detectorResults = mc.Run();

                foreach (var result in detectorResults.ResultsDictionary.Values)
                {
                    // save all detector data to the specified folder
                    DetectorIO.WriteDetectorToFile(result, input.OutputFileName);
                }
            });
        }

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        public void ShowHelp()
        {
            Console.WriteLine("Virtual Photonics MC 1.0");
            Console.WriteLine();
            Console.WriteLine("list of arguments:");
            Console.WriteLine();
            Console.WriteLine("infile\t\tthe input file.");
            Console.WriteLine("outfile\t\tthe output file.");
            Console.WriteLine("inputparam\tthe input parameter name and value(s).");
            Console.WriteLine();
            Console.WriteLine("list of input parameters (inputparam):");
            Console.WriteLine();
            Console.WriteLine("mua1\t\tdescription of mua1 and possible values");
            Console.WriteLine("mus1\t\tdescription of mus1 and possible values");
            Console.WriteLine();
            Console.WriteLine("sample usage:");
            Console.WriteLine();
            Console.WriteLine("mc infile=myinput outfile=myoutput inputparam=mua1,0.01,0.09,0.01 inputparam=mus1,10,20,1");
        }

        /// <summary>
        /// Displays the help text for the topic passed as a parameter
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        public void ShowHelp(string helpTopic)
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MonteCarloSetup MonteCarloSetup = new MonteCarloSetup();

            bool _showHelp = false;


            #region Infile Generation (optional)
            //To Generate an infile when running a simulation, uncomment the first line of code in this file
#if GENERATE_INFILE
            var tempInput = new SimulationInput(
                100,  // FIX 1e6 takes about 70 minutes my laptop
                "Output",
                new SimulationOptions(
                     0,
                     RandomNumberGeneratorType.MersenneTwister,
                     AbsorptionWeightingType.Discrete,
                     PhaseFunctionType.HenyeyGreenstein,
                     DatabaseType.NoDatabaseGeneration,
                     0),
                new CustomPointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)),
                new MultiLayerTissueInput(
                    new LayerRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    new RDiffuseDetectorInput(),
                    new ROfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    new ROfRhoAndAngleDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoAndTimeDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 10, 101)),
                    new ROfXAndYDetectorInput(
                        new DoubleRange(-200.0, 200.0, 401), // x
                        new DoubleRange(-200.0, 200.0, 401)), // y,
                    new ROfRhoAndOmegaDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, 1000, 21)),
                    new TDiffuseDetectorInput(),
                    new TOfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new TOfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    new TOfRhoAndAngleDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new DoubleRange(0.0, Math.PI / 2, 2))
                });
            tempInput.ToFile("newinfile.xml");
#endif
            #endregion

            args.Process(() =>
               {
                   Console.WriteLine("Usages are:");
                   Console.WriteLine("mc infile=myinput outfile=myoutput");
                   Console.WriteLine("inputparam=mua1,0.01,0.09,0.01 inputparam=mus1,10,20,1");
                   Console.WriteLine();
               },
               new CommandLine.Switch("help", val =>
               {
                   _showHelp = true;
               }),
               new CommandLine.Switch("infile", val =>
               {
                   Console.WriteLine("input file specified as {0}", val.First());
                   MonteCarloSetup.InputFile = val.First();
               }),
               new CommandLine.Switch("outfile", val =>
               {
                   Console.WriteLine("output file specified as {0}", val.First());
                   MonteCarloSetup.OutputFile = val.First();
               }),
               new CommandLine.Switch("/unmanaged", "/u", val =>
               {
                   Console.WriteLine("Run unmanaged code");
                   MonteCarloSetup.RunUnmanagedCode = true;
               }),
               new CommandLine.Switch("/history", "/h", val =>
               {
                   Console.WriteLine("Write histories");
                   MonteCarloSetup.WriteHistories = true;
               }),
               new CommandLine.Switch("inputparam", val =>
               {
                   MonteCarloSetup.SetRangeValues(val);
               }));

            //if help is passed as an agument do not run the Monte Carlo, just display the help for the topic
            if (_showHelp)
            {
                MonteCarloSetup.ShowHelp();
            }
            else
            {
                if (MonteCarloSetup.BatchQuery == null && MonteCarloSetup.ValidSimulation)
                {
                    MonteCarloSetup.ValidSimulation = MonteCarloSetup.ReadSimulationInputFromFile();
                }

                if (MonteCarloSetup.ValidSimulation)
                {
                    MonteCarloSetup.RunSimulation();
                    Console.Write("\nSimulation(s) complete.");
                }
                else
                {
                    Console.Write("\nSimulation(s) completed with errors. Press enter key to exit.");
                    Console.Read();
                }
            }
        }

        private static SimulationInput LoadDefaultInputFile()
        {
            return SimulationInput.FromFileInResources("newinfile.xml", "mc");
        }
    }
}


