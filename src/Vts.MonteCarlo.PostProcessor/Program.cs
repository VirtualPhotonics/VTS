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
        public string OutputFile { get; set; }
        
        public IEnumerable<DetectorInput> BatchQuery { get; set; }
        public string[] BatchNameQuery { get; set; }
        public DetectorInput Input { get; set; }
        public PhotonTerminationDatabase Database { get; set; }
        public Output OutputFromDatabaseGeneration { get; set; }

        public PostProcessorSetup()
        {
            ValidInput = true;
            DetectorInputFile = "";
            DatabaseFile = "";
            OutputFile = "";
            BatchQuery = null;
            BatchNameQuery = null;
            Input = null;
        }

        /// <summary>
        /// method to read the detector input from a specified or default files
        /// </summary>
        public bool ReadDetectorInputFromFile()
        {
            if (DetectorInputFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(DetectorInputFile)) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(DetectorInputFile);
                DetectorInputFile = path + basename + ".xml";

                if (System.IO.File.Exists(DetectorInputFile))
                {
                    Input = DetectorInput.FromFile(DetectorInputFile);
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
                Input = DetectorInput.FromFile("newinfile.xml");
            }
            if (DatabaseFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(DatabaseFile)) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(DatabaseFile);
                DatabaseFile = path + basename;

                if (System.IO.File.Exists(DatabaseFile))
                {
                    Database = PhotonTerminationDatabase.FromFile(DatabaseFile);
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
                Database = PhotonTerminationDatabase.FromFile(
                    "postprocessing_photonBiographies");
            }
            if (OutputFile.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(OutputFile)) + "\\";
                basename = System.IO.Path.GetFileNameWithoutExtension(OutputFile);
                OutputFile = path + basename + ".xml";

                if (System.IO.File.Exists(OutputFile))
                {
                    OutputFromDatabaseGeneration = Output.FromFile(OutputFile);
                }
                else
                {
                    Console.WriteLine("\nThe following output.xml file could not be found: " + basename);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("\nNo input file specified. Using output.xml from debug directory... ");
                //OutputFromDatabaseGeneration = Output.FromFile("results");
                OutputFromDatabaseGeneration = FileIO.ReadFromXML<Output>("results/output.xml"); 
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
                        ValidInput = ReadDetectorInputFromFile();
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
            DetectorInput[] inputBatch = BatchQuery.ToArray();
            string[] outNames = BatchNameQuery.Select(s => path + basename + "_" + OutputFile + "\\" + basename + "_" + OutputFile + s).ToArray();
            Output postProcessedOutput;

            //for (int i = 0; i < inputBatch.Length; i++)
            //    inputBatch[i].OutputFileName = outNames[i];

            Parallel.For(0, inputBatch.Length, i =>
            {
 
                postProcessedOutput = PhotonTerminationDatabasePostProcessor.GenerateOutput(
                inputBatch[i], Database, OutputFromDatabaseGeneration);

                //var p = Path.GetDirectoryName("postresults");
                var p = "postresults";
                if (!Directory.Exists(p))
                    Directory.CreateDirectory(p);

                postProcessedOutput.ToFile(p);
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
            var tempInput = new DetectorInput(
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
                    new DoubleRange(0.0, Math.PI / 2, 2), // angle
                    new DoubleRange(0.0, 4.0, 801), // time: nt=800 dt=0.005ns used for workshop
                    new DoubleRange(0.0, 1000, 21), // omega
                    new DoubleRange(-100.0, 100.0, 81), // x
                    new DoubleRange(-100.0, 100.0, 81) // y
                );
            tempInput.ToFile("newinfile.xml");
        #endif
    #endregion

             args.Process(
                () =>
                    {
                        Console.WriteLine("Usages are:");
                        Console.WriteLine("mc_post detectorinputinfile=mydetectorinput datafile=mydatafile outputxmlfile=myoutputxml");
                        Console.WriteLine("inputparam=rho,0.01,0.09,0.01 inputparam=time,10,20,1");
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
                new CommandLine.Switch("outfile", val =>
                    {
                        Console.WriteLine("output file specified as {0}", val.First());
                        PostProcessorSetup.OutputFile = val.First();
                    }),
                new CommandLine.Switch("inputparam", val =>
                {
                    PostProcessorSetup.SetRangeValues(val);
                })
            );

            if (PostProcessorSetup.BatchQuery == null && PostProcessorSetup.ValidInput)
            {
                PostProcessorSetup.ValidInput = PostProcessorSetup.ReadDetectorInputFromFile();
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

        private static DetectorInput LoadDefaultInputFile()
        {
            return DetectorInput.FromFileInResources("newdetectorinfile.xml", "mc_post");
        }
    }
}


