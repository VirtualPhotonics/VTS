using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vts.MonteCarlo.CommandLineApplication;

internal static class CommandLine
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
            RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(10));

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