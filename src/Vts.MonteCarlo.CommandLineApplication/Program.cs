//#define GENERATE_INFILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vts.Common;
using Vts.Extensions;
using System.IO;
using Vts.MonteCarlo.Detectors;
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

    #region David's Main consuming CommandLine parser class above
     //static void Main(string[] args)
     //   {
     //       var console = new ConsoleOutput();

     //       Queue<AnalysisAction> actionsToTake = new Queue<AnalysisAction>();

     //       string dataDirectory = "";
     //       string lutInFile = null;
     //       string lutOutFile = null;
     //       string[] tissueFiles = null;
     //       string phantomFile = null;
     //       string optionFile = null;

     //       args.Process(
     //           () =>
     //               {
     //                   Console.WriteLine("Usages are:");
     //                   Console.WriteLine("\t[/genlut or /g] infile=lutinput outfile=mylut");
     //                   Console.WriteLine("\t[/process or /p] tisname=tissue1,tissue2,tissue3 phname=ph1 optionfile=myoptions datadir=\"C:\\Data\\dcuccia\\101113\\\"");
     //               },
     //           new CommandLine.Switch("/genlut", "/g", val =>
     //               {
     //                   Console.WriteLine("Generate lookup table called");
     //                   actionsToTake.Enqueue(AnalysisAction.GenerateLookupTable);
     //               }),
     //           new CommandLine.Switch("infile", val =>
     //               {
     //                   Console.WriteLine("Lookup table input file specified as {0}", string.Join(" ", val));
     //                   lutInFile = val.First();
     //               }),
     //           new CommandLine.Switch("outfile", val =>
     //               {
     //                   Console.WriteLine("Lookup table output file specified as {0}", string.Join(" ", val));
     //                   lutOutFile = val.First();
     //               }),
     //           new CommandLine.Switch("/process", "/p", val =>
     //               {
     //                   Console.WriteLine("Process data called");
     //                   actionsToTake.Enqueue(AnalysisAction.ProcessData);
     //               }),
     //           new CommandLine.Switch("tisnames", val =>
     //               {
     //                   Console.WriteLine("Tissue filenames specified as {0}", string.Join(" ", val));
     //                   tissueFiles = val.ToArray();
     //               }),
     //           new CommandLine.Switch("phname", val =>
     //               {
     //                   Console.WriteLine("Reference phantom filename specified as {0}", string.Join(" ", val));
     //                   phantomFile = val.First();
     //               }),
     //           new CommandLine.Switch("optionfile", val =>
     //               {
     //                   Console.WriteLine("Option file specified as {0}", string.Join(" ", val));
     //                   optionFile = val.First();
     //               }),
     //           new CommandLine.Switch("datadir", val =>
     //               {
     //                   Console.WriteLine("Data directory specified as {0}", string.Join(" ", val));
     //                   dataDirectory = val.First();
     //               })
     //       );

     //       foreach (var analysisAction in actionsToTake)
     //       {
     //           switch (analysisAction)
     //           {
     //               case AnalysisAction.GenerateLookupTable:
     //                   break;
     //               case AnalysisAction.ProcessData:
     //                   AnalysisRoutines.LoadAndProcessAllData(dataDirectory, tissueFiles, phantomFile, optionFile);
     //                   break;
     //               default:
     //                   throw new ArgumentOutOfRangeException();
     //           }
     //       }
     //   }
    #endregion

    class MonteCarloSetup
    {
        // program requires the path to have no spaces so that it can be used from the command line
        string path = "";
        string basename = "newinfile";

        public string InputFile { get; set; }
        public string OutputFile { get; set; }

        public bool RunUnmanagedCode { get; set; }
        public bool WriteHistories { get; set; }
        
        public IEnumerable<SimulationInput> BatchQuery { get; set; }
        public string[] BatchNameQuery { get; set; }
        public SimulationInput Input { get; set; }

        public MonteCarloSetup()
        {
            InputFile = "";
            OutputFile = "results";
            RunUnmanagedCode = false;
            WriteHistories = false;
            BatchQuery = null;
            BatchNameQuery = null;
            Input = null;
        }

        /// <summary>
        /// method to read the simulation input from a specified file
        /// </summary>
        /// <param name="filename">File name from which to read the input</param>
        public void ReadSimulationInputFromFile()
        {
            if (InputFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(InputFile)) + "\\";
                InputFile = System.IO.Path.GetFileNameWithoutExtension(InputFile);
                InputFile = path + InputFile + ".xml";

                if (System.IO.File.Exists(InputFile))
                {
                    Input = SimulationInput.FromFile(InputFile);
                }
                else
                {
                    Console.WriteLine("\nThe following input file could not be found: " + InputFile + ".xml. Hit *Enter* to exit.");
                    Console.Read();
                }
            }
            else
            {
                Console.Write("\nNo input file specified. Using input.xml from resources... ");
                Input = SimulationInput.FromFile("newinfile.xml");
                BatchQuery = Input.AsEnumerable();
                BatchNameQuery = new[] { "" };
            }
            BatchQuery = Input.AsEnumerable();
            BatchNameQuery = new[] { "" };
        }

        /// <summary>
        /// method for seting the range values for a specific InputParameterType
        /// </summary>
        /// <param name="val"></param>
        /// <param name="t"></param>
        public void SetRangeValues(IEnumerable<string> val, InputParameterType t)
        {
            //add a check to make sure val has 3 values
            IEnumerable<double> sweep = null;

            try
            {   // batch parameter values should come in threes 
                // eg. /x=-4.0,4.0,0.05 /mus1=0.5,1.5,0.1 /mus2=0.5,1.5,0.1 ...
                var start = double.Parse(val.ElementAt(0));
                var stop = double.Parse(val.ElementAt(1));
                var delta = double.Parse(val.ElementAt(2));

                sweep = new DoubleRange(start, stop, (int)((stop - start)/delta) + 1).AsEnumerable();
            }
            catch
            {
                Console.WriteLine("Could not parse the input arguments.");
            }
            InputParameterType inputParameterType = t;
            string inputParameterString = t.ToString().ToLower();

            if (inputParameterString.Length > 0)
            {
                BatchQuery = BatchQuery.WithParameterSweep(sweep, inputParameterType);
                BatchNameQuery =
                                (from b in BatchNameQuery
                                    from s in sweep
                                    select (b + inputParameterString + "_" + String.Format("{0:f}", s) + "_")).ToArray();
            }
        }

        /// <summary>
        /// Runs the Monte Carlo simulation
        /// </summary>
        public void RunSimulation()
        {
            SimulationInput[] inputBatch = BatchQuery.ToArray();
            string[] outNames = BatchNameQuery.Select(s => path + basename + "_" + OutputFile + "\\" + basename + "_" + OutputFile + s).ToArray();

            for (int i = 0; i < inputBatch.Length; i++)
                inputBatch[i].OutputFileName = outNames[i];

            Parallel.For(0, inputBatch.Length, i =>
            {
                var mc = RunUnmanagedCode ?
                     new UnmanagedMonteCarloSimulation(
                         inputBatch[i],
                         new UnmanagedSimulationOptions(i))
                   : new MonteCarloSimulation(
                         inputBatch[i],
                         new SimulationOptions(
                             i,
                             RandomNumberGeneratorType.MersenneTwister,
                             AbsorptionWeightingType.Discrete,
                             false,
                             false,
                             WriteHistories,
                             i));

                var p = Path.GetDirectoryName(inputBatch[i].OutputFileName);

                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);

                mc.Run().ToFile(inputBatch[i].OutputFileName);
            });
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //new SimulationInput().ToFile("infile.xml"); return;
            // program requires the path to have no spaces so that it can be used from the command line
            //string path = "";
            //string basename = "newinfile";
            //string filename;
            //string outname = "results";
            //bool runUnmanagedCode = false;
            //bool tallyMomentumTransfer = false; // todo: allow reading of SimulationOptions file from XML
            //bool writeHistories = false;

            //IEnumerable<SimulationInput> batchQuery = null;
            //string[] batchNameQuery = null;
            //SimulationInput input = null;
            
            MonteCarloSetup MonteCarloSetup = new MonteCarloSetup();
            
#region     Infile Generation (optional)
#if GENERATE_INFILE
            var tempInput = new SimulationInput(
                100
                ,  // FIX 1e6 takes about 70 minutes my laptop
                "Output",
                new PointSourceInput(
                    new Position(0, 0, 0),
                    new Direction(0, 0, 1),
                    new DoubleRange(0.0, 0, 1),
                    new DoubleRange(0.0, 0, 1)),
                new MultiLayerTissueInput(
                    new LayerRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0, 2),
                            new OpticalProperties(0.0, 1.0, 0.8, 1.4),
                            AbsorptionWeightingType.Discrete),
                        new LayerRegion(
                            new DoubleRange(10.0, double.PositiveInfinity, 2),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0),
                            AbsorptionWeightingType.Discrete)
                    }
                ),
                new DetectorInput(
                    new List<TallyType>()
                    {
                        TallyType.RDiffuse,
                        TallyType.ROfAngle,
                        TallyType.ROfRho,
                        TallyType.ROfRhoAndAngle,
                        TallyType.ROfRhoAndTime,
                        TallyType.ROfXAndY,
                        TallyType.ROfRhoAndOmega,
                        TallyType.TDiffuse,
                        TallyType.TOfAngle,
                        TallyType.TOfRho,
                        TallyType.TOfRhoAndAngle,
                    },
                    new DoubleRange(0.0, 40.0, 201), // rho: nr=200 dr=0.2mm used for workshop
                    new DoubleRange(0.0, 10.0, 11),  // z
                    new DoubleRange(0.0, Math.PI / 2, 1), // angle
                    new DoubleRange(0.0, 4.0, 801), // time: nt=800 dt=0.005ns used for workshop
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-100.0, 100.0, 81), // x
                    new DoubleRange(-100.0, 100.0, 81) // y
                ));
            tempInput.ToFile("newinfile.xml");

#endif
#endregion

             args.Process(
                () =>
                {
                    Console.WriteLine("Usages are:");
                    Console.WriteLine("\t[/genlut or /g] infile=lutinput outfile=mylut");
                    Console.WriteLine("\t[/process or /p] tisname=tissue1,tissue2,tissue3 phname=ph1 optionfile=myoptions datadir=\"C:\\Data\\dcuccia\\101113\\\"");
                },
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
                new CommandLine.Switch("mua1", val =>
                    {
                        MonteCarloSetup.ReadSimulationInputFromFile();
                        //Call a function to set the InputParameterType and pass the sweep
                        MonteCarloSetup.SetRangeValues(val, (InputParameterType)Enum.Parse(typeof(InputParameterType), "Mua1"));
                    })
            );

            if (MonteCarloSetup.BatchQuery == null)
            {
                MonteCarloSetup.ReadSimulationInputFromFile();
            }
            MonteCarloSetup.RunSimulation();
            Console.Write("\nSimulation(s) complete.");

            //if (args.Length > 0)
            //{
            //    path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(args[0])) + "\\";
            //    basename = System.IO.Path.GetFileNameWithoutExtension(args[0]);
            //    filename = path + basename + ".xml";

            //    if (System.IO.File.Exists(filename))
            //    {
            //        input = SimulationInput.FromFile(filename);
            //    }
            //    else
            //    {
            //        Console.WriteLine("\nThe following input file could not be found: " + basename + ".xml. Hit *Enter* to exit.");
            //        Console.Read();
            //    }
            //}
            //else
            //{
            //    Console.Write("\nNo input file specified. Using input.xml from resources... ");
            //    input = SimulationInput.FromFile("newinfile.xml");
            //}

            //batchQuery = input.AsEnumerable();
            //batchNameQuery = new[] { "" };

            //if (args.Length > 1)
            //{
            //    IEnumerable<double> sweep = null;
            //    int sweepArgumentStart = 1;

            //    while (sweepArgumentStart < args.Length
            //        && (args[sweepArgumentStart] == "/o" || args[sweepArgumentStart] == "/u" || args[sweepArgumentStart] == "/h" || args[sweepArgumentStart] == "/mt"))
            //    {
            //        // if the output name is explicitly stated (ie. "/o output"), assign it here
            //        if (args[sweepArgumentStart] == "/o"
            //            && sweepArgumentStart + 1 < args.Length
            //            && args[sweepArgumentStart + 1] != null
            //            && args[sweepArgumentStart + 1].Length > 0
            //            && args[sweepArgumentStart + 1][0] != '/')
            //        {
            //            outname = args[sweepArgumentStart + 1];
            //            sweepArgumentStart += 2;
            //        }
            //        else if (args[sweepArgumentStart] == "/u")
            //        {
            //            runUnmanagedCode = true;
            //            sweepArgumentStart += 1;
            //        }
            //        else if (args[sweepArgumentStart] == "/mt")
            //        {
            //            tallyMomentumTransfer = true;
            //            sweepArgumentStart += 1;
            //        }
            //        else if (args[sweepArgumentStart] == "/h")
            //        {
            //            writeHistories = true;
            //            sweepArgumentStart += 1;
            //        }
            //    }

            //    InputParameterType inputParameterType = InputParameterType.XSourcePosition;
            //    string inputParameterString = "";

            //    for (int i = sweepArgumentStart; i < args.Length; i += 4)
            //    {
            //        try
            //        {   // batch parameters should come in fours 
            //            // eg. /x -4.0 4.0 0.05 /mus1 0.5 1.5 0.1 /mus2 0.5 1.5 0.1 ...
            //            var start = double.Parse(args[i + 1]);
            //            var stop = double.Parse(args[i + 2]);
            //            var delta = double.Parse(args[i + 3]);

            //            sweep = new DoubleRange(start, stop, (int)((stop - start)/delta) + 1).AsEnumerable();
            //        }
            //        catch
            //        {
            //            Console.WriteLine("Could not parse the input arguments.");
            //            break;
            //        }

            //        #region switch statement for input argument
            //        switch (args[i].ToLower())
            //        {
            //            case "/mua1":
            //                inputParameterType = InputParameterType.Mua1;
            //                inputParameterString = "mua1";
            //                break;
            //            case "/mua2":
            //                inputParameterType = InputParameterType.Mua2;
            //                inputParameterString = "mua2";
            //                break;
            //            case "/mus1":
            //                inputParameterType = InputParameterType.Mus1;
            //                inputParameterString = "mus1";
            //                break;
            //            case "/mus2":
            //                inputParameterType = InputParameterType.Mus2;
            //                inputParameterString = "mus2";
            //                break;
            //            case "/g1":
            //                inputParameterType = InputParameterType.G1;
            //                inputParameterString = "g1";
            //                break;
            //            case "/g2":
            //                inputParameterType = InputParameterType.G2;
            //                inputParameterString = "g2";
            //                break;
            //            case "/n1":
            //                inputParameterType = InputParameterType.N1;
            //                inputParameterString = "n1";
            //                break;
            //            case "/n2":
            //                inputParameterType = InputParameterType.N2;
            //                inputParameterString = "n2";
            //                break;
            //            case "/d1":
            //                inputParameterType = InputParameterType.D1;
            //                inputParameterString = "d1";
            //                break;
            //            case "/d2":
            //                inputParameterType = InputParameterType.D2;
            //                inputParameterString = "d2";
            //                break;
            //            case "/xs":
            //                inputParameterType = InputParameterType.XSourcePosition;
            //                inputParameterString = "xs";
            //                break;
            //            case "/ys":
            //                inputParameterType = InputParameterType.YSourcePosition;
            //                inputParameterString = "xs";
            //                break;
            //            case "/xe":
            //                inputParameterType = InputParameterType.XEllipsePosition;
            //                inputParameterString = "xe";
            //                break;
            //            case "/ye":
            //                inputParameterType = InputParameterType.YEllipsePosition;
            //                inputParameterString = "ye";
            //                break;
            //            case "/ze":
            //                inputParameterType = InputParameterType.ZEllipsePosition;
            //                inputParameterString = "ze";
            //                break;
            //            case "/xer":
            //                inputParameterType = InputParameterType.XEllipseRadius;
            //                inputParameterString = "xer";
            //                break;
            //            case "/yer":
            //                inputParameterType = InputParameterType.YEllipseRadius;
            //                inputParameterString = "yer";
            //                break;
            //            case "/zer":
            //                inputParameterType = InputParameterType.ZEllipseRadius;
            //                inputParameterString = "zer";
            //                break;
            //        }
            //        #endregion

            //        if (inputParameterString.Length > 0)
            //        {
            //            batchQuery = batchQuery.WithParameterSweep(sweep, inputParameterType);
            //            batchNameQuery =
            //                            (from b in batchNameQuery
            //                             from s in sweep
            //                             select (b + inputParameterString + "_" + String.Format("{0:f}", s) + "_")).ToArray();
            //        }
            //    }
            //}

            //SimulationInput[] inputBatch = MonteCarloSetup.BatchQuery.ToArray();
            ////string[] simulationNames = batchNameQuery.ToArray();
            ////string directory = 
            //string[] outNames = MonteCarloSetup.BatchNameQuery.Select(s => path + basename + "_" + outname + "\\" + basename + "_" + outname + s).ToArray();

            //for (int i = 0; i < inputBatch.Length; i++)
            //    inputBatch[i].OutputFileName = outNames[i];

            //Parallel.For(0, inputBatch.Length, i =>
            //{
            //    var mc = runUnmanagedCode ?
            //         new UnmanagedMonteCarloSimulation(
            //             inputBatch[i],
            //             new UnmanagedSimulationOptions(i))
            //       : new MonteCarloSimulation(
            //             inputBatch[i],
            //             new SimulationOptions(
            //                 i, 
            //                 RandomNumberGeneratorType.MersenneTwister, 
            //                 AbsorptionWeightingType.Discrete,
            //                 false, 
            //                 false,
            //                 writeHistories,
            //                 i));

            //    var p = Path.GetDirectoryName(inputBatch[i].OutputFileName);

            //    if (!Directory.Exists(p))
            //        Directory.CreateDirectory(p);

            //    mc.Run().ToFile(inputBatch[i].OutputFileName);
            //});

            // Below are the same things, performed with PLINQ instead:

            //Boolean[] success =
            //    (from i in 0.To(inputBatch.Length - 1).AsParallel()
            //     select new MonteCarloSimulation(
            //                    inputBatch[i],
            //                    new SimulationOptions(i, RandomNumberGeneratorType.Mcch, AbsorptionWeightingType.Discrete)
            //             ).Run().ToFile(inputBatch[i].outputFileName)
            //    ).ToArray();

            //Boolean[] success =
            //    (from i in 0.To(inputBatch.Length - 1).AsParallel()
            //     let m = runUnmanagedCode ?
            //          new UnmanagedMonteCarloSimulation(
            //              inputBatch[i],
            //              new UnmanagedSimulationOptions(i)) // always discrete absorption weighting, mcch
            //        : new MonteCarloSimulation(
            //              inputBatch[i],
            //              new SimulationOptions(i, RandomNumberGeneratorType.Mcch, AbsorptionWeightingType.Discrete))
            //     select m.Run().ToFile(inputBatch[i].outputFileName)).ToArray(); 




            //Console.WriteLine("Hit *Enter* to exit.");
            //Console.Read();
        }

        private static SimulationInput LoadDefaultInputFile()
        {
            return SimulationInput.FromFileInResources("newinfile.xml", "mc");
        }
    }
}


