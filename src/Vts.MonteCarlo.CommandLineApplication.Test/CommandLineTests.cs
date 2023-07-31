using System;
using NUnit.Framework;
using System.Linq;

namespace Vts.MonteCarlo.CommandLineApplication.Test
{
    [TestFixture]
    internal class CommandLineTests
    {
        [Test]
        public void CommandLine_switch_test()
        {
            var arguments = new [] { "h=topic" };
            arguments.Process(() => Console.WriteLine("Usage"),
                new CommandLine.Switch("help", "h", arg =>
                {
                    var helpTopic = arg.First();
                    Assert.AreEqual("topic", helpTopic);
                }));
        }

        [Test]
        public void CommandLine_no_switches_test()
        {
            var arguments = new[] { "undefined=true" };
            arguments.Process(() => Console.WriteLine("Usage"),
                new CommandLine.Switch("help", "h", arg =>
                {
                    var undefined = arg.First();
                    Assert.AreEqual("topic", undefined);
                }));
        }
    }
}
