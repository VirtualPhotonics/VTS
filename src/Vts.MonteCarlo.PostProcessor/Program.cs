//#define PROCESS_ATTACH_DEBUG

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Vts.IO;
using Vts.Common;
using Vts.MonteCarlo.PostProcessing;
using Vts.MonteCarlo.PhotonData;
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

    // put following into common class since MCCL also uses 
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

    public static class Program
    {
        public static void Main(string[] args)
        {
#if PROCESS_ATTACH_DEBUG
            Console.Read();
#endif
            string inFile = "infile.xml";
            string outName = "";
            string outPath = "";
            bool displayHelp = false;
            args.Process(() =>
                {
                    Console.WriteLine("Virtual Photonics MC Post-Processor 1.0");
                    Console.WriteLine();
                    Console.WriteLine("For more information type mc_post help");
                    Console.WriteLine();
                },
                new CommandLine.Switch("help", val =>
                {
                    displayHelp = true;
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
                    Console.WriteLine("input file specified as {0}", val.First());
                    //PostProcessorSetup.InputFilename = val.First();
                }),
                new CommandLine.Switch("outname", val =>
                {
                    outName = val.First();
                    Console.WriteLine("output file specified as {0}", val.First());
                    //PostProcessorSetup.OutputFolder = val.First();
                }),
                new CommandLine.Switch("outpath", val =>
                {
                    outPath = val.First();
                    Console.WriteLine("output path specified as {0}", outPath);
                })
            );
            //if help is passed as an agument do not run PP, just display the help for the topic
            if (!displayHelp)
            {
                var input = PostProcessorSetup.ReadPostProcessorInputFromFile(inFile);
                if (input == null)
                {
                    return;
                }

                var validationResult = PostProcessorSetup.ValidatePostProcessorInput(input);
                if (!validationResult.IsValid)
                {
                    Console.Write("\nPost-processor) completed with errors. Press enter key to exit.");
                    Console.Read();
                    return;
                }
                // override the output name with the user-specified name
                if (!string.IsNullOrEmpty(outName))
                {
                    input.OutputName = outName;
                }
                PostProcessorSetup.RunPostProcessor(input, outPath);
                Console.WriteLine("\nPost-processing complete.");
               
            }
        }
        private static void GenerateDefaultInputFile()
        {
           var tempInput = new PostProcessorInput(
                VirtualBoundaryType.DiffuseReflectance,
                new List<IDetectorInput>()
                {
                    //new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),

                    // NOTE: can run different perturbations by adding detectors
                    new pMCROfRhoDetectorInput(
                        new DoubleRange(0.0, 40, 21),
                        new List<OpticalProperties>() { 
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0)},
                        new List<int>() { 1 },
                        TallyType.pMCROfRho.ToString()),
                    new pMCROfRhoDetectorInput(
                        new DoubleRange(0.0, 40, 21),
                        new List<OpticalProperties>() { 
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                                new OpticalProperties(0.01, 1.5, 0.8, 1.4),
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0)},
                        new List<int>() { 1 },
                        "pMCROfRho_mus1p5"),
                    new pMCROfRhoDetectorInput(
                        new DoubleRange(0.0, 40, 21),
                        new List<OpticalProperties>() { 
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0),
                                new OpticalProperties(0.01, 0.5, 0.8, 1.4),
                                new OpticalProperties(0.01, 1.0, 0.8, 1.4),
                                new OpticalProperties(0.0, 1e-10, 0.0, 1.0)},
                        new List<int>() { 1 },
                        "pMCROfRho_mus0p5"),
                },
                true, // tally second moment
                "results",
                "infile",
                "postprocessorrsults");
            tempInput.ToFile("newinfile.xml");

        }

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
            Console.WriteLine("outfile\t\tthe output file.");
            Console.WriteLine();
            Console.WriteLine("sample usage:");
            Console.WriteLine();
            Console.WriteLine("mc_post infile=myinput outfile=myoutput");
        }
    }
}


