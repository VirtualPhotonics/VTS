//#define PROCESS_ATTACH_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NLog;
using Vts.Common.Logging;

[assembly: InternalsVisibleTo("Vts.MonteCarlo.CommandLineApplication.Test")]

namespace Vts.MonteCarlo.CommandLineApplication
{

    #region CommandLine Arguments Parser

    /* Simple commandline argument parser written by Ananth B. http://www.ananthonline.net */

    #endregion
    /// <summary>
    /// Monte Carlo command line application program.  Type "mc help" for
    /// a description of the different command line parameters.
    /// </summary>
    public static class Program
    {
        private static readonly Common.Logging.ILogger Logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(Program));

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
            var inFile = "";
            var inFiles = new List<string>();
            var outName = "";
            var outPath = "";
            var CPUCount = "1"; // default is to use 1
            var infoOnlyOption = false;
            IList<ParameterSweep> paramSweep = new List<ParameterSweep>();

            args.Process(() =>
               {
                   Logger.Info($"\nVirtual Photonics MC {GetVersionNumber(3)}\n");
                   Logger.Info("For more information type mc help");
                   Logger.Info("For help on a specific topic type dotnet mc.dll help=<topicname>\n");
            },
               new CommandLine.Switch("help", val =>
               {
                   var helpTopic = val.First();
                   if (helpTopic != "")
                       ShowHelp(helpTopic);
                   else
                       ShowHelp();
                   infoOnlyOption = true;
               }),
               new CommandLine.Switch("geninfiles", val =>
               {
                   GenerateDefaultInputFiles();
                   infoOnlyOption = true;
               }),
               new CommandLine.Switch("infile", val =>
               {
                   inFile = val.First();
                   Logger.Info(() => "input file specified as " + inFile);
               }),
               new CommandLine.Switch("infiles", val =>
               {
                   inFiles.AddRange(val);
                   foreach (var file in inFiles)
                   {
                       Logger.Info(() => "input file specified as " + file);
                   }
               }),
               new CommandLine.Switch("outname", val =>
               {
                   outName = val.First();
                   Logger.Info(() => "output name overridden as " + outName);
               }),
               new CommandLine.Switch("outpath", val =>
               {
                   outPath = val.First();
                   Logger.Info(() => "output path specified as " + outPath);
               }),
               new CommandLine.Switch("cpucount", val =>
               {
                    CPUCount = val.First();
                    if (CPUCount == "all")
                    {
                        CPUCount = Environment.ProcessorCount.ToString();
                        Logger.Info(() => "changed to maximum CPUs on system " + CPUCount);
                    }
                    else
                    {
                        if (!int.TryParse(CPUCount, out var CPUCountInt))
                        {
                            Logger.Info(() => "unknown cpucount option " + CPUCount);
                        }
                        else
                        {
                            Logger.Info(() => "number of CPUs specified as " + CPUCount);
                        }
                    }
               }),
               new CommandLine.Switch("paramsweep", val =>
               {
                   var sweepString = val.ToArray();
                   var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, ParameterSweepType.Count);
                   if (sweep == null) return;
                   paramSweep.Add(sweep);
                   Logger.Info(() => "parameter sweep specified as " + sweepString[0] + " from " + sweepString[1] + " to " + sweepString[2] + ", with a count of " + sweepString[3]);
               }),
               new CommandLine.Switch("paramsweepdelta", val =>
               {
                   var sweepString = val.ToArray();
                   var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, ParameterSweepType.Delta);
                   if (sweep == null) return;
                   paramSweep.Add(sweep);
                   Logger.Info(() => "parameter sweep specified as " + sweepString[0] + " from " + sweepString[1] + " to " + sweepString[2] + ", with a delta of " + sweepString[3]);
               }),
                new CommandLine.Switch("paramsweeplist", val =>
                {
                    var sweepString = val.ToArray();
                    var sweep = MonteCarloSetup.CreateParameterSweep(sweepString, ParameterSweepType.List);
                    if (sweep == null) return;
                    paramSweep.Add(sweep);
                    Logger.Info(() => "parameter sweep specified as " + sweepString[0] + " values");
                }));

            if (!infoOnlyOption)
            {
                Func<SimulationInput, bool> checkValid = simInput =>
                    {
                        var validationResult = MonteCarloSetup.ValidateSimulationInput(simInput);
                        if (validationResult.IsValid) return true;
                        Console.Write("\nSimulation(s) contained one or more errors. Details:");
                        Console.Write("\nValidation rule:" + validationResult.ValidationRule);
                        Console.Write("\nRemarks:" + validationResult.Remarks);
                        return false;
                    };
                SimulationInput input;
                if (paramSweep.Any() || inFiles.Any())
                {
                    IList<SimulationInput> inputs;
                    if (paramSweep.Any())
                    {
                        input = MonteCarloSetup.ReadSimulationInputFromFile(inFile);
                        if (input == null)
                        {
                            return 1;
                        }
                        if (!string.IsNullOrEmpty(outName))
                        {
                            input.OutputName = outName;
                        }

                        inputs = MonteCarloSetup.ApplyParameterSweeps(input, paramSweep).ToList();
                    }
                    else // if infiles.Count() > 0
                    {
                        inputs = inFiles.Select(file => MonteCarloSetup.ReadSimulationInputFromFile(file)).ToList();
                        if (!inputs.Any())
                        {
                            return 1;
                        }
                    }
                    // validate input 
                    if (inputs.Any(simulationInput => !checkValid(simulationInput)))
                    {
                        return 2;
                    }
                    // make sure input does not specify Database if CPUCount>1
                    if (int.Parse(CPUCount) > 1 && (inputs.First().Options.Databases != null && inputs.First().Options.Databases.Count != 0))
                    {
                        CPUCount = 1.ToString();
                        Logger.Info(() => "parallel processing cannot be performed when a Database is specified, changed CPUCount to 1");
                    }

                    MonteCarloSetup.RunSimulations(inputs, outPath, int.Parse(CPUCount));
                    Logger.Info("\nSimulations complete.");
                    return 0;
                }

                input = MonteCarloSetup.ReadSimulationInputFromFile(inFile);
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

                // make sure input does not specify Database if CPUCount>1
                if (int.Parse(CPUCount) > 1 && (input.Options.Databases != null && input.Options.Databases?.Count != 0))
                {
                    CPUCount = 1.ToString();
                    Logger.Info(() =>
                        "parallel processing cannot be performed when a Database is specified, changed CPUCount to 1");
                }

                MonteCarloSetup.RunSimulation(input, outPath, int.Parse(CPUCount));
                Logger.Info("\nSimulation complete.");
                return 0;
            }

            LogManager.Configuration = null;
            return 0;
        }
        
        private static void GenerateDefaultInputFiles()
        {
            try
            {
                var inputFiles = SimulationInputProvider.GenerateAllSimulationInputs();
                foreach (var input in inputFiles)
                {
                    input.ToFile("infile_" + input.OutputName + ".txt"); // write json to .txt files 
                }
            }
            catch (Exception e)
            {
                Logger.Error($"There was an error generating infiles, check your permissions - {e.Message}");
            }
        }

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        private static void ShowHelp()
        {
            Logger.Info($"Virtual Photonics MC {GetVersionNumber(3)}");
            Logger.Info("\nFor more detailed help type dotnet mc.dll help=<topicname>");
            Logger.Info("\ntopics:");
            Logger.Info("\ninfile");
            Logger.Info("outpath");
            Logger.Info("outname");
            Logger.Info("cpucount");
            Logger.Info("paramsweep");
            Logger.Info("paramsweepdelta");
            Logger.Info("paramsweeplist");
            Logger.Info("\nlist of arguments:");
            Logger.Info("\ninfile\t\tthe input file, accepts relative and absolute paths");
            Logger.Info("outpath\t\tthe output path, accepts relative and absolute paths");
            Logger.Info("outname\t\toutput name, this value is appended for a parameter sweep");
            Logger.Info("cpucount\tnumber of CPUs, default is 1");
            Logger.Info("paramsweep\ttakes the sweep parameter name and values in the format:");
            Logger.Info("\t\tparamsweep=<SweepParameterType>,Start,Stop,Count");
            Logger.Info("paramsweepdelta\ttakes the sweep parameter name and values in the format:");
            Logger.Info("\t\tparamsweepdelta=<SweepParameterType>,Start,Stop,Delta");
            Logger.Info("paramsweeplist\ttakes the sweep parameter name and values in the format:");
            Logger.Info("\t\tparamsweeplist=<SweepParameterType>,NumVals,Val1,Val2,...");
            Logger.Info("\ngeninfiles\tgenerates example infiles and names them infile_XXX.txt");
            Logger.Info("\t\tinfile_XXX.txt where XXX describes the type of input specified");
            Logger.Info("\nlist of sweep parameters (SweepParameterType):");
            Logger.Info("\nmua1\t\tabsorption coefficient for tissue layer 1");
            Logger.Info("mus1\t\tscattering coefficient for tissue layer 1");
            Logger.Info("n1\t\trefractive index for tissue layer 1");
            Logger.Info("g1\t\tanisotropy for tissue layer 1");
            Logger.Info("\nmua2\t\tabsorption coefficient for tissue layer 2");
            Logger.Info("mus2\t\tscattering coefficient for tissue layer 2");
            Logger.Info("n2\t\trefractive index for tissue layer 2");
            Logger.Info("g2\t\tanisotropy for tissue layer 2");
            Logger.Info("\nmuai\t\tabsorption coefficient for tissue layer i");
            Logger.Info("musi\t\tscattering coefficient for tissue layer i");
            Logger.Info("ni\t\trefractive index for tissue layer i");
            Logger.Info("gi\t\tanisotropy for tissue layer i");
            Logger.Info("\nnphot\t\tnumber of photons to launch from the source");
            Logger.Info("\nseed\t\tseed of random number generator");
            Logger.Info("\nsample usage:");
            Logger.Info("dotnet mc.dll infile=myinput outname=myoutput paramsweep=mua1,0.01,0.04,4 paramsweep=mus1,10,20,2 paramsweep=nphot,1000000,2000000,2\n");
        }

        /// <summary>
        /// Displays the help text for the topic passed as a parameter
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        private static void ShowHelp(string helpTopic)
        {
            switch (helpTopic.ToLower())
            {
                case "infile":
                    Logger.Info("\nINFILE");
                    Logger.Info("This is the name of the input file, it can be a relative or absolute path.");
                    Logger.Info("If the path name has any spaces enclose it in double quotes.");
                    Logger.Info("For relative paths, omit the leading slash.");
                    Logger.Info("EXAMPLES for .txt (json) files:");
                    Logger.Info("\tinfile=C:\\MonteCarlo\\InputFiles\\myinfile.txt");
                    Logger.Info("\tinfile=\"C:\\Monte Carlo\\InputFiles\\myinfile.txt\"");
                    Logger.Info("\tinfile=InputFiles\\myinfile.txt");
                    Logger.Info("\tinfile=myinfile.txt");
                    break;
                case "outpath":
                    Logger.Info("\nOUTPATH");
                    Logger.Info("This is the name of the output path, it can be a relative or absolute path.");
                    Logger.Info("If the path name has any spaces enclose it in double quotes.");
                    Logger.Info("For relative paths, omit the leading slash.");
                    Logger.Info("EXAMPLES:");
                    Logger.Info("\toutpath=C:\\MonteCarlo\\OutputFiles");
                    Logger.Info("\toutpath=OutputFiles");
                    break;
                case "outname":
                    Logger.Info("\nOUTNAME");
                    Logger.Info("The outname is appended to the folder names if there is a parameter sweep.");
                    Logger.Info("EXAMPLE:");
                    Logger.Info("\toutname=mcResults");
                    break;
                case "cpucount":
                    Logger.Info("\nCPUCOUNT");
                    Logger.Info("The cpucount specifies the number of CPUs utilized to process a single simulation.");
                    Logger.Info($"The number of CPUs on this computer: {Environment.ProcessorCount}");
                    Logger.Info("EXAMPLE:");
                    Logger.Info("\tcpucount=4");
                    break;
                case "paramsweep":
                    Logger.Info("\nPARAMSWEEP");
                    Logger.Info("Defines the parameter sweep and its values.");
                    Logger.Info("FORMAT:");
                    Logger.Info("\tparamsweep=<SweepParameterType>,Start,Stop,Count");
                    Logger.Info("EXAMPLES:");
                    Logger.Info("\tparamsweep=mua1,0.01,0.04,4");
                    Logger.Info("\tparamsweep=mus1,10,20,2");
                    break;
                case "paramsweepdelta":
                    Logger.Info("\nPARAMSWEEPDELTA");
                    Logger.Info("Defines the parameter sweep and its values.");
                    Logger.Info("FORMAT:");
                    Logger.Info("\tparamsweepdelta=<SweepParameterType>,Start,Stop,Delta");
                    Logger.Info("EXAMPLES:");
                    Logger.Info("\tparamsweepdelta=mua1,0.01,0.04,0.01");
                    Logger.Info("\tparamsweepdelta=mus1,10,20,5");
                    break;
                case "paramsweeplist":
                    Logger.Info("\nPARAMSWEEPLIST");
                    Logger.Info("Defines the parameter sweep and its values.");
                    Logger.Info("FORMAT:");
                    Logger.Info("\tparamsweeplist=<SweepParameterType>,NumValues,Val1,Val2,Val3,...");
                    Logger.Info("EXAMPLES:");
                    Logger.Info("\tparamsweeplist=mua1,3,0.01,0.03,0.04");
                    Logger.Info("\tparamsweeplist=mus1,5,0.01,1,10,100,1000");
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }
        
        /// <summary>
        /// Gets the version number of the application
        /// </summary>
        /// <param name="limiter">Determines how many levels of the version to return</param>
        /// <returns>A string with the version</returns>
        internal static string GetVersionNumber(uint limiter = 0)
        {
            var currentAssembly = Assembly.GetExecutingAssembly().GetName();
            if (currentAssembly.Version == null) return null;
            return limiter switch
            {
                1 => $"{currentAssembly.Version.Major}",
                2 => $"{currentAssembly.Version.Major}.{currentAssembly.Version.Minor}",
                3 => $"{currentAssembly.Version.Major}.{currentAssembly.Version.Minor}.{currentAssembly.Version.Build}",
                _ => $"{currentAssembly.Version}"
            };
        }
    }
}


