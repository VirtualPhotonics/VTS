//#define PROCESS_ATTACH_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NLog;
using Vts.Common;
using Vts.Common.Logging;

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
        private static Vts.Common.Logging.ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(Program));

        /// <summary>
        /// main Monte Carlo CommandLine (MCCL) application
        /// </summary>
        /// <param name="args"></param>
        /// <returns>int = 0 (successful completion)</returns>
        /// <returns>int = 1 (infile null or missing)</returns>
        /// <returns>int = 2 (infile exists but does not pass validation)</returns>
        public static int Main(string[] args)
        {
#if PROCESS_ATTACH_DEBUG
            Console.Read();
#endif
            string inFile = "";
            List<string> inFiles = new List<string>();
            string outName = "";
            string outPath = "";
            bool infoOnlyOption = false;
            IList<ParameterSweep> paramSweep = new List<ParameterSweep>();

            args.Process(() =>
               {
                   logger.Info("\nVirtual Photonics MC 1.0\n");
                   logger.Info("For more information type mc help");
                   logger.Info("For help on a specific topic type mc.exe help=<topicname>\n");
            },
               new CommandLine.Switch("help", val =>
               {
                   var helpTopic = val.First();
                   if (helpTopic != "")
                       ShowHelp(helpTopic);
                   else
                       ShowHelp();
                   infoOnlyOption = true;
                   return;
               }),
               new CommandLine.Switch("geninfiles", val =>
               {
                   GenerateDefaultInputFiles();
                   infoOnlyOption = true;
                   return;
               }),
               new CommandLine.Switch("infile", val =>
               {
                   inFile = val.First();
                   logger.Info(() => "input file specified as " + inFile);
                   // MonteCarloSetup.InputFile = val.First();
               }),
               new CommandLine.Switch("infiles", val =>
               {
                   inFiles.AddRange(val);
                   foreach (var file in inFiles)
                   {
                       logger.Info(() => "input file specified as " + file);
                   }
                   // MonteCarloSetup.InputFile = val.First();
               }),
               new CommandLine.Switch("outname", val =>
               {
                   outName = val.First();
                   logger.Info(() => "output name overridden as " + outName);
                   //MonteCarloSetup.OutputFolder = val.First();
               }),
               new CommandLine.Switch("outpath", val =>
               {
                   outPath = val.First();
                   logger.Info(() => "output path specified as " + outPath);
                   //MonteCarloSetup.OutputFolder = val.First();
               }),
               new CommandLine.Switch("paramsweep", val =>
               {
                   var sweepString = val.ToArray();
                   var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, ParameterSweepType.Count);
                   if (sweep != null)
                   {
                       paramSweep.Add(sweep);
                       logger.Info(() => "parameter sweep specified as " + sweepString[0] + " from " + sweepString[1] + " to " + sweepString[2] + ", with a count of " + sweepString[3]);
                   }
               }),
               new CommandLine.Switch("paramsweepdelta", val =>
               {
                   var sweepString = val.ToArray();
                   var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, ParameterSweepType.Delta);
                   if (sweep != null)
                   {
                       paramSweep.Add(sweep);
                       logger.Info(() => "parameter sweep specified as " + sweepString[0] + " from " + sweepString[1] + " to " + sweepString[2] + ", with a delta of " + sweepString[3]);
                   }
               }),
                new CommandLine.Switch("paramsweeplist", val =>
                {
                    var sweepString = val.ToArray();
                    var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, ParameterSweepType.List);
                    if (sweep != null)
                    {
                        paramSweep.Add(sweep);
                        logger.Info(() => "parameter sweep specified as " + sweepString[0] + " values");
                    }
                }));

            if (!infoOnlyOption)
            {
                Func<SimulationInput, bool> checkValid = simInput =>
                    {
                        var validationResult = MonteCarloSetup.ValidateSimulationInput(simInput);
                        if (!validationResult.IsValid)
                        {
                            Console.Write("\nSimulation(s) contained one or more errors. Details:");
                            Console.Write("\nValidation rule:" + validationResult.ValidationRule);
                            Console.Write("\nRemarks:" + validationResult.Remarks);
                            Console.Write("\nPress enter key to exit.");
                            Console.Read();
                            return false;
                        }
                        return true;
                    };

                if (paramSweep.Count() > 0 || inFiles.Count() > 0)
                {
                    IEnumerable<SimulationInput> inputs = null;
                    if (paramSweep.Count() > 0)
                    {
                        var input = MonteCarloSetup.ReadSimulationInputFromFile(inFile);
                        if (input == null)
                        {
                            return 1;
                        }
                        if (!string.IsNullOrEmpty(outName))
                        {
                            input.OutputName = outName;
                        }

                        //var sweeps = paramSweep.Select(sweep => MonteCarloSetup.CreateParameterSweep(sweep));
                        inputs = MonteCarloSetup.ApplyParameterSweeps(input, paramSweep);
                    }
                    else // if infiles.Count() > 0
                    {
                        inputs = inFiles.Select(file => MonteCarloSetup.ReadSimulationInputFromFile(file));
                        if (inputs.Count() == 0)
                        {
                            return 1;
                        }
                    }

                    foreach (var simulationInput in inputs)
                    {
                        if (!checkValid(simulationInput))
                            return 2;                    // override the output name with the user-specified name
                        
                    }

                    MonteCarloSetup.RunSimulations(inputs, outPath);
                    logger.Info("\nSimulations complete.");
                    return 0;
                }
                else
                {
                    var input = MonteCarloSetup.ReadSimulationInputFromFile(inFile);
                    if (input == null)
                    {
                        return 1;
                    }

                    if (!checkValid(input))
                        return 2;

                    if (!string.IsNullOrEmpty(outName))
                    {
                        input.OutputName = outName;
                    }

                    MonteCarloSetup.RunSimulation(input, outPath);
                    logger.Info("\nSimulation complete.");
                    return 0;
                }
            }

            LogManager.Configuration = null;
            return 0;
        }
        
        private static void GenerateDefaultInputFiles()
        {
            var infiles = SimulationInputProvider.GenerateAllSimulationInputs();
            for (int i = 0; i < infiles.Count; i++)
            {
                infiles[i].ToFile("infile_" + infiles[i].OutputName + ".txt"); // write json to .txt files
            }
            //var sources = SourceInputProvider.GenerateAllSourceInputs();
            //sources.WriteToJson("infile_source_options_test.txt");
        }

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        private static void ShowHelp()
        {
            logger.Info("Virtual Photonics MC 1.0");
            logger.Info("\nFor more detailed help type mc.exe help=<topicname>");
            logger.Info("\nlist of arguments:");
            logger.Info("\ninfile\t\tthe input file, accepts relative and absolute paths");
            logger.Info("outpath\t\tthe output path, accepts relative and absolute paths");
            logger.Info("outname\t\toutput name, this value is appended for a parameter sweep");
            logger.Info("paramsweep\ttakes the sweep parameter name and values in the format:");
            logger.Info("\t\tparamsweep=<SweepParameterType>,Start,Stop,Count");
            logger.Info("paramsweepdelta\ttakes the sweep parameter name and values in the format:");
            logger.Info("\t\tparamsweepdelta=<SweepParameterType>,Start,Stop,Delta");
            logger.Info("paramsweeplist\ttakes the sweep parameter name and values in the format:");
            logger.Info("\t\tparamsweeplist=<SweepParameterType>,NumVals,Val1,Val2,...");
            logger.Info("\ngeninfiles\tgenerates example infiles and names them infile_XXX.txt");
            logger.Info("\t\tinfile_XXX.txt where XXX describes the type of input specified");
            logger.Info("\nlist of sweep parameters (paramsweep):");
            logger.Info("\nmua1\t\tabsorption coefficient for tissue layer 1");
            logger.Info("mus1\t\tscattering coefficient for tissue layer 1");
            logger.Info("n1\t\trefractive index for tissue layer 1");
            logger.Info("g1\t\tanisotropy for tissue layer 1");
            logger.Info("\nmua2\t\tabsorption coefficient for tissue layer 2");
            logger.Info("mus2\t\tscattering coefficient for tissue layer 2");
            logger.Info("n2\t\trefractive index for tissue layer 2");
            logger.Info("g2\t\tanisotropy for tissue layer 2");
            logger.Info("\nmuai\t\tabsorption coefficient for tissue layer i");
            logger.Info("musi\t\tscattering coefficient for tissue layer i");
            logger.Info("ni\t\trefractive index for tissue layer i");
            logger.Info("gi\t\tanisotropy for tissue layer i");
            logger.Info("\nsample usage:");
            logger.Info("\nmc.exe infile=myinput outname=myoutput paramsweep=mua1,0.01,0.04,4 paramsweep=mus1,10,20,2");
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
                    logger.Info("\nINFILE");
                    logger.Info("This is the name of the input file, it can be a relative or absolute path.");
                    logger.Info("If the path name has any spaces enclose it in double quotes.");
                    logger.Info("For relative paths, omit the leading slash.");
                    logger.Info("EXAMPLES for .txt (json) files:");
                    logger.Info("\tinfile=C:\\MonteCarlo\\InputFiles\\myinfile.txt");
                    logger.Info("\tinfile=\"C:\\Monte Carlo\\InputFiles\\myinfile.txt\"");
                    logger.Info("\tinfile=InputFiles\\myinfile.txt");
                    logger.Info("\tinfile=myinfile.txt");
                    break;
                case "outpath":
                    logger.Info("\nOUTPATH");
                    logger.Info("This is the name of the output path, it can be a relative or absolute path.");
                    logger.Info("If the path name has any spaces enclose it in double quotes.");
                    logger.Info("For relative paths, omit the leading slash.");
                    logger.Info("EXAMPLES:");
                    logger.Info("\toutpath=C:\\MonteCarlo\\OutputFiles");
                    logger.Info("\toutpath=OutputFiles");
                    break;
                case "outname":
                    logger.Info("\nOUTNAME");
                    logger.Info("The outname is appended to the folder names if there is a parameter sweep.");
                    logger.Info("EXAMPLE:");
                    logger.Info("\toutname=mcResults");
                    break;
                case "paramsweep":
                    logger.Info("\nPARAMSWEEP");
                    logger.Info("Defines the parameter sweep and its values.");
                    logger.Info("FORMAT:");
                    logger.Info("\tparamsweep=<SweepParameterType>,Start,Stop,Count");
                    logger.Info("EXAMPLES:");
                    logger.Info("\tparamsweep=mua1,0.01,0.04,4");
                    logger.Info("\tparamsweep=mus1,10,20,2");
                    break;
                case "paramsweepdelta":
                    logger.Info("\nPARAMSWEEPDELTA");
                    logger.Info("Defines the parameter sweep and its values.");
                    logger.Info("FORMAT:");
                    logger.Info("\tparamsweepdelta=<SweepParameterType>,Start,Stop,Delta");
                    logger.Info("EXAMPLES:");
                    logger.Info("\tparamsweepdelta=mua1,0.01,0.04,0.01");
                    logger.Info("\tparamsweepdelta=mus1,10,20,5");
                    break;
                case "paramsweepList":
                    logger.Info("\nPARAMSWEEPLIST");
                    logger.Info("Defines the parameter sweep and its values.");
                    logger.Info("FORMAT:");
                    logger.Info("\tparamsweeplist=<SweepParameterType>,NumValues,Val1,Val2,Val3,...");
                    logger.Info("EXAMPLES:");
                    logger.Info("\tparamsweeplist=mua1,3,0.01,0.03,0.04");
                    logger.Info("\tparamsweeplist=mus1,5,0.01,1,10,100,1000");
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }
    }
}


