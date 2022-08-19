using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vts.Common.Logging;
using Vts.MonteCarlo.RayData;

namespace Vts.MonteCarlo.ZemaxDatabaseConverter
{
    #region Zemax Command Line Arguments Parser

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
    internal class Program
    {
        private static ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(Program));
        static void Main(string[] args)
        {
            string databaseToConvertName = "";
            var convertedDatabaseName = "";
            var zemaxToMC = false;
            var outPath = "";
            var infoOnlyOption = false;

            args.Process(() =>
            {
                logger.Info("For more information type zdc help");
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
               new CommandLine.Switch("infile", val =>
               {
                   databaseToConvertName = val.First();
                   logger.Info(() => "input file specified as " + databaseToConvertName);
               }),
               new CommandLine.Switch("outfile", val =>
               {
                   convertedDatabaseName = val.First();
                   logger.Info(() => "output file specified as " + convertedDatabaseName);
               })
               );

            if (!infoOnlyOption)
            {          
                // validate infile extension to determine direction of conversion
                if ((Path.GetExtension(databaseToConvertName) == ".zrd")||
                    (Path.GetExtension(databaseToConvertName) == ".ZRD"))
                {
                    // conversion of Zemax to MCCL database process
                    zemaxToMC = true;
                    RayDatabase convertedDatabase = DatabaseConverter.ConvertZemaxDatabaseToMCCLSourceDatabase(
                        databaseToConvertName, convertedDatabaseName);
                }
                if ((Path.GetExtension(databaseToConvertName) == ".mcs") ||
                (Path.GetExtension(databaseToConvertName) == ".mcs"))
                {
                    // conversion of MCCL database to Zemax process
                    zemaxToMC = false;
                }
                               
            }
                
        }

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        private static void ShowHelp()
        {
            logger.Info($"Virtual Zemax Database Converter (zdc)");
            logger.Info("\ntopics:");
            logger.Info("\ninfile");
            logger.Info("\noutfile");
            logger.Info("\nsample usage:");
            logger.Info("dotnet zdc.dll infile=myinput outfile=myoutput\n");
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
                    logger.Info("\nINFILE");
                    logger.Info("This is the name of the input database file, it can be a relative or absolute path.");
                    logger.Info("If the path name has any spaces enclose it in double quotes.");
                    logger.Info("For relative paths, omit the leading slash.");
                    logger.Info("EXAMPLES for .txt (json) files:");
                    logger.Info("\tinfile=C:\\MonteCarlo\\InputFiles\\myinfile.txt");
                    logger.Info("\tinfile=\"C:\\Monte Carlo\\InputFiles\\myinfile.txt\"");
                    logger.Info("\tinfile=InputFiles\\myinfile.txt");
                    logger.Info("\tinfile=myinfile.txt");
                    break;
                case "outpath":
                    logger.Info("\nOUTFILE");
                    logger.Info("This is the name of the output database file, it can be a relative or absolute path.");
                    logger.Info("If the path name has any spaces enclose it in double quotes.");
                    logger.Info("For relative paths, omit the leading slash.");
                    logger.Info("EXAMPLES:");
                    logger.Info("\toutfile=C:\\MonteCarlo\\OutputFiles");
                    logger.Info("\toutfile=OutputFiles");
                    break;
            }
        }
    }
}