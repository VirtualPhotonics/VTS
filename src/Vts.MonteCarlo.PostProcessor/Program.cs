//#define GENERATE_INFILE

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
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;
using Vts.IO;
using Vts.MonteCarlo.IO;

namespace Vts.MonteCarlo.PostProcessor
{
    // Detector inputs
    [KnownType(typeof(AOfRhoAndZDetectorInput))]
    [KnownType(typeof(ATotalDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZAndTimeDetectorInput))]
    [KnownType(typeof(FluenceOfRhoAndZDetectorInput))]
    [KnownType(typeof(pMCROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(pMCROfRhoDetectorInput))]
    [KnownType(typeof(RDiffuseDetectorInput))]
    [KnownType(typeof(ROfAngleDetectorInput))]
    [KnownType(typeof(ROfRhoAndAngleDetectorInput))]
    [KnownType(typeof(ROfRhoAndOmegaDetectorInput))]
    [KnownType(typeof(ROfRhoAndTimeDetectorInput))]
    [KnownType(typeof(ROfRhoDetectorInput))]
    [KnownType(typeof(ROfXAndYDetectorInput))]
    [KnownType(typeof(TDiffuseDetectorInput))]
    [KnownType(typeof(TOfAngleDetectorInput))]
    [KnownType(typeof(TOfRhoAndAngleDetectorInput))]
    [KnownType(typeof(TOfRhoDetectorInput))]
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
        public string infileName = "";
        private bool doPMC = false;
        private SimulationInput databaseSimulationInput;
        private PhotonDatabase photonDatabase;
        private pMCDatabase pmcDatabase;

        public string InputFilename { get; set; }
        public string OutputFolder { get; set; }      
        public PostProcessorInput Input { get; set; }

        public bool ValidInput { get; set; }

        public PostProcessorSetup()
        {
            ValidInput = true;
            InputFilename = "";
            OutputFolder = "ppresults";
        }

        /// <summary>
        /// method to read the input from a specified or default files
        /// </summary>
        public bool ReadPostProcessorInputFromFile()
        {
            // read input file then read in elements of input file
            if (InputFilename.Length > 0)
            {
                path = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(InputFilename)) + "\\";
                infileName = System.IO.Path.GetFileNameWithoutExtension(InputFilename);
                InputFilename = path + infileName + ".xml";

                if (System.IO.File.Exists(InputFilename))
                {
                    Input = FileIO.ReadFromXML<PostProcessorInput>(InputFilename);
                }
                else
                {
                    Console.WriteLine("\nThe following input file could not be found: " + infileName + ".xml");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("\nNo detector input file specified. Using newinfile.xml from resources.");
                Input = FileIO.ReadFromXML<PostProcessorInput>("newinfile.xml"); 
                
            }
            // read in SimulationInput that generated database
            if (Input.DatabaseSimulationInputFilename.Length > 0)
            {
                var SimulationInputFile = path + Input.DatabaseSimulationInputFilename + ".xml";

                if (System.IO.File.Exists(SimulationInputFile))
                {
                    databaseSimulationInput = SimulationInput.FromFile(SimulationInputFile);
                }
                else
                {
                    Console.WriteLine("\nThe following input file could not be found: " + 
                        Input.DatabaseSimulationInputFilename + ".xml");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("\nNo SimulationInput file specified in PostProcessorInput. ");
            }
            // read in database names
            if (Input.DatabaseFilenames != null)
            {
                // check if pMC databases first
                if (Input.DatabaseTypes.Contains(DatabaseType.PhotonExitDataPoints) &&
                    Input.DatabaseTypes.Contains(DatabaseType.CollisionInfo))
                {
                    doPMC = true;
                    int pdindex = Input.DatabaseTypes.IndexOf(DatabaseType.PhotonExitDataPoints);
                    var photonDatabaseName = path + Input.DatabaseFilenames[pdindex];
                    int ciindex = Input.DatabaseTypes.IndexOf(DatabaseType.CollisionInfo);
                    var collisionInfoDatabaseName = path + Input.DatabaseFilenames[ciindex];
                    if (System.IO.File.Exists(photonDatabaseName) &&
                        System.IO.File.Exists(collisionInfoDatabaseName))
                    {
                        pmcDatabase = pMCDatabase.FromFile(photonDatabaseName, collisionInfoDatabaseName);
                    }
                    else
                    {
                        Console.WriteLine("\nOne of the following database files could not be found: " + 
                            photonDatabaseName + ".xml or" + collisionInfoDatabaseName + ".xml");
                        return false;
                    }
                }
                if (Input.DatabaseTypes.Contains(DatabaseType.PhotonExitDataPoints) &&
                    !Input.DatabaseTypes.Contains(DatabaseType.CollisionInfo))
                {
                    int index = Input.DatabaseTypes.IndexOf(DatabaseType.PhotonExitDataPoints);
                    var photonDatabaseName = path + Input.DatabaseFilenames[index];
                    if (System.IO.File.Exists(photonDatabaseName))
                    {
                        photonDatabase = PhotonDatabase.FromFile(photonDatabaseName);
                    }
                    else
                    {
                        Console.WriteLine("\nThe following database file could not be found: " + 
                                photonDatabaseName + ".xml");
                        return false;
                    }
                }    
            }
            return true;
        }

        /// <summary>
        /// Runs the Monte Carlo Post-processor
        /// </summary>
        public void RunPostProcessor()
        {
            Output postProcessedOutput;
            if (!doPMC)
            {
                postProcessedOutput = PhotonTerminationDatabasePostProcessor.GenerateOutput(
                    Input.DetectorInputs, photonDatabase, databaseSimulationInput);
            }
            else
            {
                IList<IpMCDetectorInput> pMCDetectorInputs;
                pMCDetectorInputs = Input.DetectorInputs.Select(d => (IpMCDetectorInput)d).ToList();
                postProcessedOutput = PhotonTerminationDatabasePostProcessor.GenerateOutput(
                    pMCDetectorInputs, pmcDatabase, databaseSimulationInput);
            }
            var folderPath = OutputFolder;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            foreach (var result in postProcessedOutput.ResultsDictionary.Values)
            {
                // save all detector data to the specified folder
                DetectorIO.WriteDetectorToFile(result, folderPath);
            }

        }
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            PostProcessorSetup PostProcessorSetup = new PostProcessorSetup();
            
    #region Infile Generation (optional)
        //To Generate an infile, uncomment the first line of code in this file
#if GENERATE_INFILE
            var tempInput = new PostProcessorInput(
                new List<IDetectorInput>()
                {
                    //new RDiffuseDetectorInput(),
                    //new ROfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                    //new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
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
                    new pMCROfRhoDetectorInput(
                        new DoubleRange(0.0, 10, 101),
                        new List<OpticalProperties>() { 
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0)},
                        new List<int>() { 1 },
                        TallyType.pMCROfRho.ToString())
                },
                new List<string>()
                {
                    "infile_photonExitDatabase",
                    "infile_collisionInfoDatabase"
                },
                new List<DatabaseType>()
                {
                    DatabaseType.PhotonExitDataPoints,
                    DatabaseType.CollisionInfo
                },
                "infile");
            tempInput.WriteToXML<PostProcessorInput>("newinfile.xml");
#endif
    #endregion

            args.Process(() =>
                    {
                        Console.WriteLine("Usages are:");
                        Console.WriteLine("mc_post infile=myinput outfile=myoutfile");
                        Console.WriteLine();
                    },
                new CommandLine.Switch("infile", val =>
                    {
                        Console.WriteLine("input file specified as {0}", val.First());
                        PostProcessorSetup.InputFilename = val.First();
                    }),
                new CommandLine.Switch("outfile", val =>
                    {
                        Console.WriteLine("output file specified as {0}", val.First());
                        PostProcessorSetup.OutputFolder = val.First();
                    })
                //new CommandLine.Switch("datafile", val =>
                //    {
                //        Console.WriteLine("database file specified as {0}", val.First());
                //        PostProcessorSetup.DatabaseFile = val.First();
                //    })
            );

            PostProcessorSetup.ReadPostProcessorInputFromFile();

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

        // comment out for now ckh 3/23/11
        //private static IList<IDetector> LoadDefaultInputFile()
        //{
        //    return new IList<IDetector>() { };
        //}
    }
}


