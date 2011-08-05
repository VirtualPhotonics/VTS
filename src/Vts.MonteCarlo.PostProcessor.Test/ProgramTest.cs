using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Vts.MonteCarlo.PostProcessor.Test
{
    [TestFixture]
    public class ProgramTest
    {
        [Test]
        public void validate_generate_infile()
        {
            string[] arguments = new string[] {"geninfile"};
            Program.Main(arguments);
            Assert.Pass();
        }
    }
}
