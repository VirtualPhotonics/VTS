using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Vts.MonteCarlo.CommandLineApplication;

namespace Vts.MonteCarlo.CommandLineApplication.Test
{
    [TestFixture]
    public class ProgramTest
    {
        [Test]
        public void validate_generate_infile()
        {
            string[] arguments = new string[] {"geninfile"};
            CommandLineApplication.Program.Main(arguments);
            Assert.Pass();
        }
    }
}
