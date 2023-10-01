using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Vts.MonteCarlo.CommandLineApplication;

[assembly: InternalsVisibleTo("Vts.MonteCarlo.PostProcessor.Test")]

namespace Vts.MonteCarlo.PostProcessor
{
    public static class Program
    {
        /// <summary>
        /// main Monte Carlo Post Processor (MCPP) application
        /// </summary>
        /// <returns>int = 0 (successful completion)</returns>
        /// <returns>int = 1 (infile null or missing)</returns>
        /// <returns>int = 2 (infile exists but does not pass validation)</returns>
        public static int Main(string[] args)
        {
            var inFile = "infile.txt";
            var inPath = "";
            var outName = "";
            var outPath = "";
            args.Process(() =>
                {
                    Console.WriteLine(@"Virtual Photonics MC Post-Processor {GetVersionNumber(3)}\n"")");
                    Console.WriteLine();
                    Console.WriteLine(@"For more information type mc_post help");
                    Console.WriteLine();
                },
                new CommandLine.Switch("help", val =>
                {
                    ShowHelp();
                }),
                new CommandLine.Switch("geninfiles", val =>
                {
                    GenerateDefaultInputFiles();
                }),
                new CommandLine.Switch("infile", val =>
                {
                    inFile = val.First();
                    Console.WriteLine(@"input file specified as {0}", inFile);
                }),
                new CommandLine.Switch("inpath", val =>
                {
                    inPath = val.First();
                    Console.WriteLine(@"input path specified as {0}", inPath);
                }),
                new CommandLine.Switch("outname", val =>
                {
                    outName = val.First();
                    Console.WriteLine(@"output file specified as {0}", outName);
                }),
                new CommandLine.Switch("outpath", val =>
                {
                    outPath = val.First();
                    Console.WriteLine(@"output path specified as {0}", outPath);
                })
            );

            if (CheckInfoOnly(args)) return 0;
            var input = PostProcessorSetup.ReadPostProcessorInputFromFile(inFile);
            if (input == null)
            {
                return 1;
            }

            var validationResult = PostProcessorSetup.ValidatePostProcessorInput(input, inPath);
            if (!validationResult.IsValid)
            {
                Console.Write("\nPost-processor) completed with errors. Press enter key to exit.");
                Console.Read();
                return 2;
            }
            // override the output name with the user-specified name
            if (!string.IsNullOrEmpty(outName))
            {
                input.OutputName = outName;
            }
            PostProcessorSetup.RunPostProcessor(input, inPath, outPath);
            Console.WriteLine("\nPost-processing complete.");
            return 0;
        }

        private static bool CheckInfoOnly(string[] args)
        {
            return args.Contains("help") || args.Contains("geninfiles");
        }

        private static void GenerateDefaultInputFiles()
        {
            var infiles = PostProcessorInputProvider.GenerateAllPostProcessorInputs();
            foreach (var file in infiles)
            {
                file.ToFile("infile_" + file.OutputName + ".txt");
            }
        }

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(@"Virtual Photonics MC Post-Processor 1.0");
            Console.WriteLine();
            Console.WriteLine(@"list of arguments:");
            Console.WriteLine();
            Console.WriteLine(@"infile		the input file, accepts relative and absolute paths");
            Console.WriteLine(@"inpath		the input path, accepts relative and absolute paths");
            Console.WriteLine(@"outpath		the output path, accepts relative and absolute paths");
            Console.WriteLine(@"outname		output name, this overwrites output name in input file");
            Console.WriteLine();
            Console.WriteLine(@"geninfiles		generates example infiles and names them infile_XXX.txt");
            Console.WriteLine();
            Console.WriteLine(@"sample usage:");
            Console.WriteLine();
            Console.WriteLine(@"mc_post infile=myinput outname=myoutput");
        }
    }
}


