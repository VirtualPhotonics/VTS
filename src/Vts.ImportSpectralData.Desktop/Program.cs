using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Vts.Common.Logging;
using Vts.IO;
using Vts.SpectralMapping;

namespace Vts.ImportSpectralData.Desktop
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
        private static ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(Program));

        static void Main(string[] args)
        {
            var skipImport = false;
            string[] filenames = null;
            string path = null;
            string outname = null;
            string outpath = null;

            args.Process(() =>
            {
                logger.Info("\nSpectral Data Import\n");
                logger.Info("For more information type import.exe help");
                logger.Info("For help on a specific topic type import.exe help=<topicname>\n");
            },
            new CommandLine.Switch("help", val =>
            {
                var helpTopic = val.FirstOrDefault();
                ShowHelp(helpTopic);
                skipImport = true;
            }),
            new CommandLine.Switch("@/help", val =>
            {
                var helpTopic = val.FirstOrDefault();
                ShowHelp(helpTopic);
                skipImport = true;
            }),
            new CommandLine.Switch("generatefiles", val =>
            {
                logger.Info(() => "Generating spectral data files...");
                try
                {
                    var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDefaultDatabaseFromFileInResources();
                    SpectralDatabase.WriteDatabaseToFiles(testDictionary);
                }
                catch (Exception e)
                {
                    logger.Info("****  An error occurred while generating the import files  ****");
                    logger.Info("Detailed error: " + e.Message);
                }
                skipImport = true;
            }),
            new CommandLine.Switch("filename", val =>
            {
                filenames = new[] { val.FirstOrDefault() };
                logger.Info(() => "import files specified as: " + filenames[0]);
            }),
            new CommandLine.Switch("filenames", val =>
            {
                filenames = val.ToArray();
                var message = "import files specified as: ";
                if (filenames.Length > 0)
                {
                    message = message + filenames.First();
                    for (int i = 1; i < filenames.Count(); i++)
                    {
                        message = message + ", " + filenames[i];
                    }
                }
                logger.Info(() => message);
            }),
            new CommandLine.Switch("path", val =>
            {
                path = val.FirstOrDefault();
                logger.Info(() => "import path specified as " + path);
            }),
            new CommandLine.Switch("outname", val =>
            {
                outname = val.FirstOrDefault();
                logger.Info(() => "outname specified as " + outname);
            }),
            new CommandLine.Switch("outpath", val =>
            {
                outpath = val.FirstOrDefault();
                logger.Info(() => "outpath specified as " + outpath);
            }));

            if (skipImport)
            {
                return;
            }

            var chromophoreDictionary = SpectralImporter.ImportSpectraFromFile(filenames, path);

            chromophoreDictionary.WriteToJson(Path.Combine(outpath ?? "", outname ?? "SpectralDictionary.txt"));
        }
        
        /// <summary>
        /// Displays the help text for the topic passed as a parameter
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        private static void ShowHelp(string helpTopic)
        {
            if (string.IsNullOrEmpty(helpTopic))
            {
                logger.Info("Spectral Data Importer");
                logger.Info("For more detailed help type import.exe help=<topicname>");
                logger.Info("\nlist of arguments:");
                logger.Info("generatefiles\tgenerates tab-delimited text files with default spectral data");
                logger.Info("importpath\tname of the folder containing import files");
                logger.Info("\nsample usages:");
                logger.Info("\timport.exe generatefiles");
                logger.Info("\timport.exe"); // imports all files matching "absorber-*.txt" in current directory
                logger.Info("\timport.exe path=myDirectory"); // imports all files matching "absorber-*.txt" in specified directory
                logger.Info("\timport.exe filename=myChromophoreFile.txt"); // imports specified file in current directory
                logger.Info("\timport.exe filenames=myChromophoreFile1.txt,myChromophoreFile2.txt,..."); // imports all specified files in current directory
                logger.Info("\timport.exe outname=myChromophoreDictionary.txt"); // specifies the name of the resulting output JSON spectral dictionary
                logger.Info("\timport.exe outpath=myDirectory"); // specifies the output directory of the generated xml dictionary
                return;
            }
            else
            {
                switch (helpTopic)
                {
                    case "generatefiles":
                        logger.Info("\nGENERATEFILES");
                        logger.Info("generates the tab delimited text files with the default spectral data");
                        logger.Info("EXAMPLE:");
                        logger.Info("\nimport.exe generatefiles");
                        break;
                    case "filenames":
                        logger.Info("\nFILENAMES or FILENAME");
                        logger.Info("specifies the file or files to load. FILENAMES should be separated by commas with no spaces.");
                        logger.Info("EXAMPLES:");
                        logger.Info("\timport.exe filename=myChromophoreFile1.txt");
                        logger.Info("\timport.exe filenames=myChromophoreFile1.txt,myChromophoreFile2.txt,...");
                        break;
                    case "path":
                        logger.Info("\nPATH");
                        logger.Info("This is the name of the import path, it can be a relative or absolute path.");
                        logger.Info("If the path name has any spaces enclose it in double quotes.");
                        logger.Info("For relative paths, omit the leading slash.");
                        logger.Info("EXAMPLES:");
                        logger.Info("\timportpath=C:\\SpectralData\\");
                        logger.Info("\timportpath=SpectralData");
                        break;
                    case "outname":
                        logger.Info("\nOUTNAME");
                        logger.Info("specifies the output xml file to create.");
                        logger.Info("EXAMPLES:");
                        logger.Info("\timport.exe outname=myChromophoreDictionary.txt");
                        break;
                    case "outpath":
                        logger.Info("\nOUTPATH");
                        logger.Info("This is the name of the output path, it can be a relative or absolute path.");
                        logger.Info("If the path name has any spaces enclose it in double quotes.");
                        logger.Info("For relative paths, omit the leading slash.");
                        logger.Info("EXAMPLES:");
                        logger.Info("\toutpath=C:\\SpectralData\\OutputFiles");
                        logger.Info("\toutpath=OutputFiles");
                        break;
                }
            }
        }
    }
}
