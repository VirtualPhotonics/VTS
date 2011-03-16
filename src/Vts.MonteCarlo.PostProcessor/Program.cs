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
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;
using Vts.IO;

// ParallelFx June '08 CTP
//using System.Threading.Collections;

namespace Vts.MonteCarlo.PostProcessor
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

    class PostProcessorSetup
    {
        private string path = "";
        private string basename = "newdetectorinfile";

        public bool ValidInput { get; set; }

        public string DetectorInputFile { get; set; }
        public string DatabaseFile { get; set; }
        public string SimulationInputFile { get; set; }
        
        public IEnumerable<DetectorController> BatchQuery { get; set; }
        public string[] BatchNameQuery { get; set; }
        public IList<IDetectorInput> DetectorInput { get; set; }
        public PhotonDatabase Database { get; set; }
        public SimulationInput SimulationInputFromDatabaseGeneration { get; set; }

        public PostProcessorSetup()
        {
            ValidInput = true;
            DetectorInputFile = "";
            DatabaseFile = "";
            SimulationInputFile = "";
            BatchQuery = null;
            BatchNameQuery = null;
            DetectorInput = null;
        }

        /// <summary>
        /// method to read the input from a specified or default files
        /// </summary>
        public bool ReadInputFromFile()
        {
            if (DetectorInputFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(DetectorInputFile)) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(DetectorInputFile);
                DetectorInputFile = path + basename + ".xml";

                // comment out following for now 3/16/11
                //if (System.IO.File.Exists(DetectorInputFile))
                //{
                //    DetectorInput = DetectorInput.FromFile(DetectorInputFile);
                //}
                //else
                //{
                //    Console.WriteLine("\nThe following input file could not be found: " + basename + ".xml");
                //    return false;
                //}
            }
            else
            {
                Console.WriteLine("\nNo detector input file specified. Using newdetectorinfile.xml from resources... ");
                //DetectorInput = FileIO.ReadFromXML<DetectorInput>("newdetectorinfile.xml"); //comment out 3/16/11
            }
            if (DatabaseFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(DatabaseFile)) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(DatabaseFile);
                DatabaseFile = path + basename;

                if (System.IO.File.Exists(DatabaseFile))
                {
                    Database = PhotonDatabase.FromFile(DatabaseFile);
                }
                else
                {
                    Console.WriteLine("\nThe following database file could not be found: " + basename);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("\nNo input file specified. Using database from debug directory... ");
                Database = PhotonDatabase.FromFile(
                    "Output_photonBiographies");
            }
            if (SimulationInputFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(SimulationInputFile)) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(SimulationInputFile);
                SimulationInputFile = path + basename + ".xml";

                if (System.IO.File.Exists(SimulationInputFile))
                {
                    SimulationInputFromDatabaseGeneration = SimulationInput.FromFile(SimulationInputFile);
                }
                else
                {
                    Console.WriteLine("\nThe following SimulationInput.xml file could not be found: " + basename);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("\nNo simulation input file specified. Using .xml from resources... ");
                SimulationInputFromDatabaseGeneration = FileIO.ReadFromXML<SimulationInput>("newinfile.xml"); 
            }
            //BatchQuery = new DetectorController(DetectorInput, SimulationInputFromDatabaseGeneration.TissueInput);
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
            PostProcessorInputParameterType inputParameterType;

            if (val.Count() == 4)
            {
                try
                {
                    inputParameterType = (PostProcessorInputParameterType)Enum.Parse(typeof(PostProcessorInputParameterType), val.ElementAt(0), true);

                    // batch parameter values should come in fours 
                    // eg. inputparam=Rho.Count,100,200,100 inputparam=Time.Count,100,200,100 ...
                    var start = double.Parse(val.ElementAt(1));
                    var stop = double.Parse(val.ElementAt(2));
                    var delta = double.Parse(val.ElementAt(3));

                    sweep = new DoubleRange(start, stop, (int)((stop - start) / delta) + 1).AsEnumerable();

                    if (BatchQuery == null)
                    {
                        ValidInput = ReadInputFromFile();
                    }

                    if (ValidInput)
                    {
                        //string inputParameterString = inputParameterType.ToString().ToLower();

                        //if (inputParameterString.Length > 0)
                        //{
                        //    BatchQuery = BatchQuery.WithParameterSweep(sweep, inputParameterType);
                        //    BatchNameQuery =
                        //                    (from b in BatchNameQuery
                        //                     from s in sweep
                        //                     select (b + inputParameterString + "_" + String.Format("{0:f}", s) + "_")).ToArray();
                        //}
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
        /// Runs the Monte Carlo Post-processor
        /// </summary>
        public void RunPostProcessor()
        {
            DetectorController[] inputBatch = BatchQuery.ToArray();
            string[] outNames = BatchNameQuery.Select(s => path + basename + "_" + SimulationInputFile + "\\" + basename + "_" + SimulationInputFile + s).ToArray();
            Output postProcessedOutput;

            //for (int i = 0; i < inputBatch.Length; i++)
            //    inputBatch[i].OutputFileName = outNames[i];

            Parallel.For(0, inputBatch.Length, i =>
            {
 
                //postProcessedOutput = PhotonTerminationDatabasePostProcessor.GenerateOutput(
                //inputBatch[i], Database, SimulationInputFromDatabaseGeneration);

                ////var p = Path.GetDirectoryName("postresults");
                //var p = "postresults";
                //if (!Directory.Exists(p))
                //    Directory.CreateDirectory(p);

                //postProcessedOutput.ToFile(p);
            });
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            PostProcessorSetup PostProcessorSetup = new PostProcessorSetup();
            
    #region Infile Generation (optional)
        //To Generate a detector infile, uncomment the first line of code in this file
        #if GENERATE_INFILE
            var tempInput = new List<IDetectorInput>()
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

            args.Process(
                () =>
                    {
                        Console.WriteLine("Usages are:");
                        Console.WriteLine("mc_post detectorinputinfile=mydetectorinput datafile=mydatafile simulationinputfile=mysimulationinput");
                        Console.WriteLine("inputparam=rhocount,100,200,100 inputparam=timecount,100,200,100");
                        Console.WriteLine();
                    },
                new CommandLine.Switch("detectorinputinfile", val =>
                    {
                        Console.WriteLine("input file specified as {0}", val.First());
                        PostProcessorSetup.DetectorInputFile = val.First();
                    }),
                new CommandLine.Switch("datafile", val =>
                    {
                        Console.WriteLine("database file specified as {0}", val.First());
                        PostProcessorSetup.DatabaseFile = val.First();
                    }),
                new CommandLine.Switch("simulationinputfile", val =>
                    {
                        Console.WriteLine("simulation input file specified as {0}", val.First());
                        PostProcessorSetup.SimulationInputFile = val.First();
                    }),
                new CommandLine.Switch("inputparam", val =>
                {
                    PostProcessorSetup.SetRangeValues(val);
                })
            );

            if (PostProcessorSetup.BatchQuery == null && PostProcessorSetup.ValidInput)
            {
                PostProcessorSetup.ValidInput = PostProcessorSetup.ReadInputFromFile();
            }
            if (PostProcessorSetup.ValidInput)
            {
                PostProcessorSetup.RunPostProcessor();
                Console.Write("\nPostProcessor complete.");
            }
            else
            {
                Console.Write("\nPostProcessor completed with errors. Press enter key to exit.");
                Console.Read();
            }
        }

        private static DetectorController LoadDefaultInputFile()
        {
            //return DetectorInput.FromFileInResources("newdetectorinfile.xml", "mc_post");
            return new DetectorController();
        }
    }
}


