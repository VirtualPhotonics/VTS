using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
    public static class Program
    {
         public static int Main(string[] args)
         {
            string databaseToConvertName = "";
            string convertedDatabaseName = "";
            string infileType = "";
            bool zemaxToMCCL = false;
            var outPath = ""; // may need later 
            var infoOnlyOption = false;

            args.Process(() =>
            {
                Console.WriteLine("Virtual Photonics Zemax-MCCL Database converter");
                Console.WriteLine();
                Console.WriteLine("For more information type mc_zemax help");
                Console.WriteLine();
            },
               new CommandLine.Switch("help", val =>
               {
                   ShowHelp();
                   infoOnlyOption = true;
               }),
               new CommandLine.Switch("infile", val =>
               {
                   databaseToConvertName = val.First();
                   Console.WriteLine("input file specified as {0}", databaseToConvertName);
               }),
               new CommandLine.Switch("outfile", val =>
               {
                   convertedDatabaseName = val.First();
                   Console.WriteLine("output file specified as {0}", convertedDatabaseName);
               }),
               new CommandLine.Switch("infiletype", val =>
               {
                   infileType = val.First();
                   if (infileType == "zrd")
                   {
                       zemaxToMCCL = true;
                       Console.WriteLine("converting Zemax database to MCCL database");
                   }
                   else
                   {
                       if (infileType == "mccl")
                       {
                           zemaxToMCCL = false;
                           Console.WriteLine("converting MCCL database to Zemax database");
                       }
                       else
                       {
                           infoOnlyOption = true;
                           Console.WriteLine("infiletype either needs to be set to 'zrd' or 'mccl'");
                       }
                   }
               })
               );

            if (!infoOnlyOption)
            {
                // check infiles exist
                if (DatabaseConverter.VerifyInputs(databaseToConvertName, convertedDatabaseName))
                {
                    if (zemaxToMCCL)
                    {
                        // conversion of Zemax output to MCCL source database process
                        DatabaseConverter.ConvertZemaxDatabaseToMCCLSourceDatabase(
                            databaseToConvertName, convertedDatabaseName);
                    }
                    else
                    {
                        // conversion of MCCL reflectance to Zemax input
                        DatabaseConverter.ConvertMCCLDatabaseToZemaxSourceDatabase(
                            databaseToConvertName, convertedDatabaseName);
                    }
                    return 1;
                }
                else
                {
                    return 0;
                }            
            }
            return 1;               
        }

        /// <summary>
        /// Displays the help text for detailed usage of the application
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("Virtual Photonics Zemax-MCCL Database Converter");
            Console.WriteLine();
            Console.WriteLine("list of arguments:");
            Console.WriteLine();
            Console.WriteLine("infile\t\tthe input file of database to be converted");
            Console.WriteLine("outname\t\toutput name of converted database file");
            Console.WriteLine("infiletype\t\ttype of infile, either 'zrd' or 'mccl'");
            Console.WriteLine();
            Console.WriteLine("sample usage:");
            Console.WriteLine();
            Console.WriteLine("mc_zemax infile=databasetoconvert infiletype=zrd outfile=converteddatabase");
            Console.WriteLine("mc_zemax infile=databasetoconvert infiletype=mccl outfile=converteddatabase");

        }

    }
}