//#define GENERATE_INFILE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vts.Common;
using Vts.Extensions;
using System.IO;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;

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

    class Program
    {
        static void Main(string[] args)
        {
            #region Infile Generation (optional)
            //To Generate an infile when running a simulation, uncomment the first line of code in this file
#if GENERATE_INFILE
            var tempInput = new SimulationInput(
                100,  // FIX 1e6 takes about 70 minutes my laptop
                "Output",
                new SimulationOptions(
                     0, // random number generator seed
                     RandomNumberGeneratorType.MersenneTwister,
                     AbsorptionWeightingType.Discrete,
                     PhaseFunctionType.HenyeyGreenstein,
                     new List<DatabaseType>() { DatabaseType.PhotonExitDataPoints, DatabaseType.CollisionInfo },
                     //null,
                     true, // tally Second Moment
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
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    //new RDiffuseDetectorInput(),
                    //new ROfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    //new ROfRhoAndAngleDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, Math.PI / 2, 2)),
                    //new ROfRhoAndTimeDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, 10, 101)),
                    //new ROfXAndYDetectorInput(
                    //    new DoubleRange(-200.0, 200.0, 401), // x
                    //    new DoubleRange(-200.0, 200.0, 401)), // y,
                    //new ROfRhoAndOmegaDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, 1000, 21)),
                    //new TDiffuseDetectorInput(),
                    //new TOfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    //new TOfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    //new TOfRhoAndAngleDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, Math.PI / 2, 2))
                });
            tempInput.ToFile("infile.xml");
#endif
            #endregion

            string inFile = "infile.xml";
            string outName = "";
            string outPath = "";
            IList<string[]> paramSweep = new List<string[]>();

            args.Process(() =>
               {
                   Console.WriteLine("Usages are:");
                   Console.WriteLine("mc infile=myinput.xml");
                   Console.WriteLine("mc infile=myinput.xml outname=myoutput");
                   Console.WriteLine("mc infile=myinput.xml inputparam=mua1,0.01,0.09,0.01 inputparam=mus1,10,20,1");
                   Console.WriteLine("mc infile=myinput.xml outname=myoutput inputparam=mua1,0.01,0.09,0.01 inputparam=mus1,10,20,1");
                   Console.WriteLine();
               },
               new CommandLine.Switch("help", val =>
               {
                    ShowHelp();
                    return;
               }),
               new CommandLine.Switch("geninfile", val =>
               {
                    GenerateDefaultInputFile();
                    return;
               }),
               new CommandLine.Switch("infile", val =>
               {
                   inFile = val.First();
                   Console.WriteLine("input file specified as {0}", inFile);
                   // MonteCarloSetup.InputFile = val.First();
               }),
               new CommandLine.Switch("outname", val =>
               {
                   outName = val.First();
                   Console.WriteLine("output tag specified as {0}", outName);
                   //MonteCarloSetup.OutputFolder = val.First();
               }),
               new CommandLine.Switch("outpath", val =>
               {
                   outPath = val.First();
                   Console.WriteLine("output path specified as {0}", outPath);
                   //MonteCarloSetup.OutputFolder = val.First();
               }),
               new CommandLine.Switch("paramsweep", val =>
               {
                   var sweep = val.ToArray();
                   paramSweep.Add(sweep);
                   Console.WriteLine("parameter sweep specified as {0},{1},{2},{3}", sweep);
               }));

            var input = MonteCarloSetup.ReadSimulationInputFromFile(inFile);

            var validationResult = MonteCarloSetup.ValidateSimulationInput(input);
            if(!validationResult.IsValid)
            {
                Console.Write("\nSimulation(s) completed with errors. Press enter key to exit.");
                Console.Read();
                return;
            }

            // override the output name with the user-specified name
            if (!string.IsNullOrEmpty(outName))
            {
                input.OutputName = outName;
            }

            if(paramSweep.Count > 0)
            {
                var sweeps = paramSweep.Select(sweep => MonteCarloSetup.CreateParameterSweep(sweep));
                var inputs = MonteCarloSetup.ApplyParameterSweeps(input, sweeps);

                MonteCarloSetup.RunSimulations(inputs, outPath);
                Console.WriteLine("\nSimulations complete.");
            }
            else
            {
                MonteCarloSetup.RunSimulation(input, outPath);
                Console.WriteLine("\nSimulation complete.");
            }
            return;
        }

        private static void GenerateDefaultInputFile()
        {
            var tempInput = new SimulationInput(
                100,  // FIX 1e6 takes about 70 minutes my laptop
                "Output",
                new SimulationOptions(
                     0, // random number generator seed
                     RandomNumberGeneratorType.MersenneTwister,
                     AbsorptionWeightingType.Discrete,
                     PhaseFunctionType.HenyeyGreenstein,
                     new List<DatabaseType>() { DatabaseType.PhotonExitDataPoints, DatabaseType.CollisionInfo },
                //null,
                     true, // tally Second Moment
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
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 0.0, 1.0))
                    }
                ),
                new List<IDetectorInput>()
                {
                    //new RDiffuseDetectorInput(),
                    //new ROfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    //new ROfRhoAndAngleDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, Math.PI / 2, 2)),
                    //new ROfRhoAndTimeDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, 10, 101)),
                    //new ROfXAndYDetectorInput(
                    //    new DoubleRange(-200.0, 200.0, 401), // x
                    //    new DoubleRange(-200.0, 200.0, 401)), // y,
                    //new ROfRhoAndOmegaDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, 1000, 21)),
                    //new TDiffuseDetectorInput(),
                    //new TOfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    //new TOfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                    //new TOfRhoAndAngleDetectorInput(
                    //    new DoubleRange(0.0, 10, 101),
                    //    new DoubleRange(0.0, Math.PI / 2, 2))
                });
            tempInput.ToFile("infile.xml");
        }

        //private static SimulationInput LoadDefaultInputFile()
        //{
        //    return SimulationInput.FromFileInResources("infile.xml", "mc");
        //}

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("Virtual Photonics MC 1.0");
            Console.WriteLine();
            Console.WriteLine("list of arguments:");
            Console.WriteLine();
            Console.WriteLine("infile\t\tthe input file.");
            Console.WriteLine("outputtag\t\tthe name of the results folder (infile_outputtag).");
            Console.WriteLine("inputparam\tthe input parameter name and value(s).");
            Console.WriteLine();
            Console.WriteLine("list of input parameters (inputparam):");
            Console.WriteLine();
            Console.WriteLine("mua1\t\tdescription of tissue layer 1 mua and possible values");
            Console.WriteLine("mus1\t\tdescription of tissue layer 1 mus and possible values");
            Console.WriteLine("n1\t\tdescription of tissue layer 1 n and possible values");
            Console.WriteLine("g1\t\tdescription of tissue layer 1 g and possible values");
            Console.WriteLine();
            Console.WriteLine("mua2\t\tdescription of tissue layer 2 mua and possible values");
            Console.WriteLine("mus2\t\tdescription of tissue layer 2 mus and possible values");
            Console.WriteLine();
            Console.WriteLine("sample usage:");
            Console.WriteLine();
            Console.WriteLine("mc infile=myinput outputtag=myoutput inputparam=mua1,0.01,0.04,0.01 inputparam=mus1,10,20,5");
        }

        /// <summary>
        /// Displays the help text for the topic passed as a parameter
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        private static void ShowHelp(string helpTopic)
        {

        }
    }
}


