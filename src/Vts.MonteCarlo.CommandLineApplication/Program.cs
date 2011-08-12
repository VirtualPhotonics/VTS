//#define PROCESS_ATTACH_DEBUG

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
    /// <summary>
    /// Monte Carlo command line application program.  Type "mc help" for
    /// a description of the different command line parameters.
    /// </summary>
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
            IList<ParameterSweep> paramSweep = new List<ParameterSweep>();

            args.Process(() =>
               {
                Console.WriteLine("Viirtual Photonics MC 1.0");
                Console.WriteLine();
                Console.WriteLine("For more information type mc help");
                Console.WriteLine("For help on a specific topic type mc help=<topicname>");
                Console.WriteLine();
            },
               new CommandLine.Switch("help", val =>
               {
                   var helpTopic = val.First();
                   if (helpTopic != "")
                       ShowHelp(helpTopic);
                   else
                       ShowHelp();
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
                   Console.WriteLine("input file specified as {0}", inFile);
                   // MonteCarloSetup.InputFile = val.First();
               }),
               new CommandLine.Switch("outname", val =>
               {
                   outName = val.First();
                   Console.WriteLine("output name overridden as {0}", outName);
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
                   var sweepString = val.ToArray();
                   var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, false);
                   paramSweep.Add(sweep);
                   Console.WriteLine("parameter sweep specified as {0},{1},{2},{3}", sweepString);
               }),
               new CommandLine.Switch("paramsweepdelta", val =>
               {
                   var sweepString = val.ToArray();
                   var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, true);
                   paramSweep.Add(sweep);
                   Console.WriteLine("parameter sweep specified as {0},{1},{2},{3}", sweepString);
               }));

            if (!displayHelp)
            {
                var input = MonteCarloSetup.ReadSimulationInputFromFile(inFile);
                if (input == null)
                {
                    return;
                }

                var validationResult = MonteCarloSetup.ValidateSimulationInput(input);
                if (!validationResult.IsValid)
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
                if (paramSweep.Count() > 0)
                {
                    //var sweeps = paramSweep.Select(sweep => MonteCarloSetup.CreateParameterSweep(sweep));
                    var inputs = MonteCarloSetup.ApplyParameterSweeps(input, paramSweep);

                    MonteCarloSetup.RunSimulations(inputs, outPath);
                    Console.WriteLine("\nSimulations complete.");
                }
                else
                {
                    MonteCarloSetup.RunSimulation(input, outPath);
                    Console.WriteLine("\nSimulation complete.");
                }
            }
            return;
        }

        private static void GenerateDefaultInputFile()
        {
            var tempInput = new SimulationInput(
                100,  // FIX 1e6 takes about 70 minutes my laptop
                "results",
                new SimulationOptions(
                    0, // random number generator seed, -1=random seed, 0=fixed seed
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete,
                    PhaseFunctionType.HenyeyGreenstein,
                    true, // tally Second Moment
                    false, // track statistics
                    0),
                new DirectionalPointSourceInput(
                    new Position(0.0, 0.0, 0.0),
                    new Direction(0.0, 0.0, 1.0),
                    0), // 0=start in air, 1=start in tissue
                new MultiLayerTissueInput(
                    new LayerRegion[]
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 100.0),
                            new OpticalProperties(0.01, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(0.0, 1e-10, 1.0, 1.0))
                    }
                ),
                new List<IVirtualBoundaryInput>
                {
                    new SurfaceVirtualBoundaryInput(
                        VirtualBoundaryType.DiffuseReflectance,
                        new List<IDetectorInput>()
                        {
                            new RDiffuseDetectorInput(),
                            new ROfAngleDetectorInput(new DoubleRange(Math.PI / 2 , Math.PI, 2)),
                            new ROfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                            new ROfRhoAndAngleDetectorInput(
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(Math.PI / 2 , Math.PI, 2)),
                            new ROfRhoAndTimeDetectorInput(
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(0.0, 10, 101)),
                            new ROfXAndYDetectorInput(
                                new DoubleRange(-100.0, 100.0, 21), // x
                                new DoubleRange(-100.0, 100.0, 21)), // y,
                            new ROfRhoAndOmegaDetectorInput(
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(0.0, 1000, 21))
                        },
                        false, // write to database
                        VirtualBoundaryType.DiffuseReflectance.ToString()
                    ),
                    new SurfaceVirtualBoundaryInput(
                        VirtualBoundaryType.DiffuseTransmittance,
                        //null,
                        new List<IDetectorInput>()
                        {
                            new TDiffuseDetectorInput(),
                            new TOfAngleDetectorInput(new DoubleRange(0.0, Math.PI / 2, 2)),
                            new TOfRhoDetectorInput(new DoubleRange(0.0, 10, 101)),
                            new TOfRhoAndAngleDetectorInput(
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(0.0, Math.PI / 2, 2))
                        },
                        false, // write to database
                        VirtualBoundaryType.DiffuseTransmittance.ToString()
                    ),
                    new GenericVolumeVirtualBoundaryInput(
                        VirtualBoundaryType.GenericVolumeBoundary,
                        new List<IDetectorInput>()
                        {
                            new ATotalDetectorInput(),
                            new AOfRhoAndZDetectorInput(                            
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(0.0, 10, 101)),
                            new FluenceOfRhoAndZDetectorInput(                            
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(0.0, 10, 101)),
                            new RadianceOfRhoAndZAndAngleDetectorInput(
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(0.0, 10, 101),
                                new DoubleRange(0, Math.PI, 5))
                        },
                        false,
                        VirtualBoundaryType.GenericVolumeBoundary.ToString()
                    ),
                    new SurfaceVirtualBoundaryInput(
                        VirtualBoundaryType.SpecularReflectance,
                        new List<IDetectorInput>
                        {
                            new RSpecularDetectorInput(), 
                        },
                        false,
                        VirtualBoundaryType.SpecularReflectance.ToString()
                    ),
                });
            tempInput.ToFile("newinfile.xml");
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
            Console.WriteLine("For more detailed help type mc help=<topicname>");
            Console.WriteLine();
            Console.WriteLine("list of arguments:");
            Console.WriteLine();
            Console.WriteLine("infile\t\tthe input file, accepts relative and absolute paths");
            Console.WriteLine("outpath\t\tthe output path, accepts relative and absolute paths");
            Console.WriteLine("outname\t\toutput name, this value is appended for a parameter sweep");
            Console.WriteLine("paramsweep\ttakes the sweep parameter name and values in the format:");
            Console.WriteLine("\t\tparamsweep=<SweepParameterType>,Start,Stop,Count");
            Console.WriteLine("paramsweepdelta\ttakes the sweep parameter name and values in the format:");
            Console.WriteLine("\t\tparamsweepdelta=<SweepParameterType>,Start,Stop,Delta");
            Console.WriteLine();
            Console.WriteLine("geninfile\t\tgenerates a new infile and names it newinfile.xml");
            Console.WriteLine();
            Console.WriteLine("list of sweep parameters (paramsweep):");
            Console.WriteLine();
            Console.WriteLine("mua1\t\tabsorption coefficient for tissue layer 1");
            Console.WriteLine("mus1\t\tscattering coefficient for tissue layer 1");
            Console.WriteLine("n1\t\trefractive index for tissue layer 1");
            Console.WriteLine("g1\t\tanisotropy for tissue layer 1");
            Console.WriteLine();
            Console.WriteLine("mua2\t\tabsorption coefficient for tissue layer 2");
            Console.WriteLine("mus2\t\tscattering coefficient for tissue layer 2");
            Console.WriteLine("n2\t\trefractive index for tissue layer 2");
            Console.WriteLine("g2\t\tanisotropy for tissue layer 2");
            Console.WriteLine();
            Console.WriteLine("muai\t\tabsorption coefficient for tissue layer i");
            Console.WriteLine("musi\t\tscattering coefficient for tissue layer i");
            Console.WriteLine("ni\t\trefractive index for tissue layer i");
            Console.WriteLine("gi\t\tanisotropy for tissue layer i");
            Console.WriteLine();
            Console.WriteLine("sample usage:");
            Console.WriteLine();
            Console.WriteLine("mc infile=myinput outname=myoutput paramsweep=mua1,0.01,0.04,4 paramsweep=mus1,10,20,2");
        }

        /// <summary>
        /// Displays the help text for the topic passed as a parameter
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        private static void ShowHelp(string helpTopic)
        {
            switch (helpTopic)
            {
                case "infile":
                    Console.WriteLine();
                    Console.WriteLine("INFILE");
                    Console.WriteLine("This is the name of the input file, it can be a relative or absolute path.");
                    Console.WriteLine("If the path name has any spaces enclose it in double quotes.");
                    Console.WriteLine("For relative paths, omit the leading slash.");
                    Console.WriteLine("EXAMPLES:");
                    Console.WriteLine("\tinfile=C:\\MonteCarlo\\InputFiles\\myinfile.xml");
                    Console.WriteLine("\tinfile=\"C:\\Monte Carlo\\InputFiles\\myinfile.xml\"");
                    Console.WriteLine("\tinfile=InputFiles\\myinfile.xml");
                    Console.WriteLine("\tinfile=myinfile.xml");
                    break;
                case "outpath":
                    Console.WriteLine();
                    Console.WriteLine("OUTPATH");
                    Console.WriteLine("This is the name of the output path, it can be a relative or absolute path.");
                    Console.WriteLine("If the path name has any spaces enclose it in double quotes.");
                    Console.WriteLine("For relative paths, omit the leading slash.");
                    Console.WriteLine("EXAMPLES:");
                    Console.WriteLine("\tinfile=C:\\MonteCarlo\\OutputFiles");
                    Console.WriteLine("\tinfile=OutputFiles");
                    break;
                case "outname":
                    Console.WriteLine();
                    Console.WriteLine("OUTNAME");
                    Console.WriteLine("The outname is appended to the folder names if there is a parameter sweep.");
                    Console.WriteLine("EXAMPLE:");
                    Console.WriteLine("\toutname=mcResults");
                    break;
                case "paramsweep":
                    Console.WriteLine();
                    Console.WriteLine("PARAMSWEEP");
                    Console.WriteLine("Defines the parameter sweep and its values.");
                    Console.WriteLine("FORMAT:");
                    Console.WriteLine("\tparamsweep=<SweepParameterType>,Start,Stop,Count");
                    Console.WriteLine("EXAMPLES:");
                    Console.WriteLine("\tparamsweep=mua1,0.01,0.04,4");
                    Console.WriteLine("\tparamsweep=mus1,10,20,2");
                    break;
                case "paramsweepdelta":
                    Console.WriteLine();
                    Console.WriteLine("PARAMSWEEPDELTA");
                    Console.WriteLine("Defines the parameter sweep and its values.");
                    Console.WriteLine("FORMAT:");
                    Console.WriteLine("\tparamsweepdelta=<SweepParameterType>,Start,Stop,Delta");
                    Console.WriteLine("EXAMPLES:");
                    Console.WriteLine("\tparamsweep=mua1,0.01,0.04,0.01");
                    Console.WriteLine("\tparamsweep=mus1,10,20,5");
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }
    }
}


