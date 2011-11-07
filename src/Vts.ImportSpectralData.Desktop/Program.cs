using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Vts.IO;
using Vts.Common.Logging;
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
            bool runImport = true;
            string importPath = "";

            args.Process(() =>
            {
                logger.Info("\nSpectral Data Import\n");
                logger.Info("For more information type import.exe help");
                logger.Info("For help on a specific topic type import.exe help=<topicname>\n");
            },
               new CommandLine.Switch("help", val =>
               {
                   var helpTopic = val.First();
                   if (helpTopic != "")
                       ShowHelp(helpTopic);
                   else
                       ShowHelp();
                   runImport = false;
                   return;
               }),
               new CommandLine.Switch("generatefiles", val =>
               {
                   logger.Info(() => "Generating spectral data files...");
                   var testDictionary = Vts.SpectralMapping.SpectralDatabase.GetDatabaseFromFile();
                   SpectralDatabase.WriteDatabaseToFiles(testDictionary);
                   runImport = false;
                   return;
               }),
               new CommandLine.Switch("importpath", val =>
               {
                   importPath = val.First();
                   logger.Info(() => "import path specified as " + importPath);
               }));
            
            if (runImport)
            {
                // import the default files
                if (importPath == "")
                {
                    logger.Info(() => "Importing spectral data files");
                    //import the values for Fat
                    Stream stream = StreamFinder.GetFileStream("absorber-Fat.txt", FileMode.Open);
                    var testDictionary = SpectralDatabase.CreateDatabaseFromFile(stream);
                    //import the values for H2O
                    stream = StreamFinder.GetFileStream("absorber-H2O.txt", FileMode.Open);
                    SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
                    //import the values for Hb
                    stream = StreamFinder.GetFileStream("absorber-Hb.txt", FileMode.Open);
                    SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
                    //import the values for HbO2
                    stream = StreamFinder.GetFileStream("absorber-HbO2.txt", FileMode.Open);
                    SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
                    //import the values for Melanin
                    stream = StreamFinder.GetFileStream("absorber-Melanin.txt", FileMode.Open);
                    SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
                    //import the values for Nigrosin
                    stream = StreamFinder.GetFileStream("absorber-Nigrosin.txt", FileMode.Open);
                    SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
                    testDictionary.WriteToXML("SpectralDictionary.xml");
                }
                else
                {
                    logger.Info(() => "Importing spectral data files");
                    ProcessDir(importPath);
                }
            }
        }

        public static void ProcessDir(string sourceDir)
        {
            bool firstFile = true;
            Dictionary<string, ChromophoreSpectrum> testDictionary = new Dictionary<string,ChromophoreSpectrum>();
            // process the list of files found in the directory 
            string[] fileEntries = Directory.GetFiles(sourceDir);
            foreach (string fileName in fileEntries)
            {
                // if this is the first file then use it to create the dictionary
                if (firstFile)
                {
                    logger.Info("Creating dictionary from file: " + fileName);
                    Stream stream = StreamFinder.GetFileStream(fileName, FileMode.Open);
                    testDictionary = SpectralDatabase.CreateDatabaseFromFile(stream);
                    firstFile = false;
                }
                else
                {
                    // append the remaining files to the dictionary
                    logger.Info("Importing file: " + fileName);
                    Stream stream = StreamFinder.GetFileStream(fileName, FileMode.Open);
                    SpectralDatabase.AppendDatabaseFromFile(testDictionary, stream);
                }
            }
            testDictionary.WriteToXML("SpectralDictionary.xml");
        }

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        private static void ShowHelp()
        {
            logger.Info("Spectral Data Importer");
            logger.Info("For more detailed help type import.exe help=<topicname>");
            logger.Info("\nlist of arguments:");
            logger.Info("generatefiles\tgenerates tab-delimited text files with default spectral data");
            logger.Info("importpath\tname of the folder containing import files");
            logger.Info("\nsample usage:");
            logger.Info("import.exe generatefiles");
            logger.Info("import.exe importpath=ImportFiles");
        }

        /// <summary>
        /// Displays the help text for the topic passed as a parameter
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        private static void ShowHelp(string helpTopic)
        {
            switch (helpTopic)
            {
                case "generatefiles":
                    logger.Info("\nGENERATEFILES");
                    logger.Info("generates the tab delimited text files with the default spectral data");
                    logger.Info("EXAMPLE:");
                    logger.Info("\nimport.exe generatefiles");
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
                case "importpath":
                    logger.Info("\nIMPORTPATH");
                    logger.Info("This is the name of the import path, it can be a relative or absolute path.");
                    logger.Info("If the path name has any spaces enclose it in double quotes.");
                    logger.Info("For relative paths, omit the leading slash.");
                    logger.Info("EXAMPLES:");
                    logger.Info("\timportpath=C:\\SpectralData\\ImportFiles");
                    logger.Info("\timportpath=ImportFiles");
                    break;
                default:
                    ShowHelp();
                    break;
            }
        }
    }
}
