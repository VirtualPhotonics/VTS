using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Vts.MonteCarlo.CommandLineApplication.Test
{
    [TestFixture]
    public class ProgramTest
    {
        /// <summary>
        /// test to verify "geninfile" option works successfully"
        /// </summary>
        [Test]
        public void validate_generate_infile()
        {
            string[] arguments = new string[] {"geninfile"};
            Program.Main(arguments);
            Assert.Pass();
        }
        /// <summary>
        /// test to verify correct folder name created for output
        /// </summary>
        [Test]
        public void validate_output_folder_name_when_no_input_file_specified()
        {
            string[] arguments = new string[] { "" };
            Program.Main(arguments);
            // the default infile.xml that is used has OutputName="results"
            Assert.IsTrue(Directory.Exists("results"));
        }
        /// <summary>
        /// test to verify correct parameter sweep folder names created for output
        /// </summary>
        [Test]
        public void validate_parameter_sweep_folder_names_when_no_input_file_specified()
        {
            string[] arguments = new string[] { "paramsweepdelta=mua1,0.01,0.04,0.01" };
            Program.Main(arguments);
            // the default infile.xml that is used has OutputName="results"
            Assert.IsTrue(Directory.Exists("results_mua1_0.01"));
            Assert.IsTrue(Directory.Exists("results_mua1_0.02"));
            Assert.IsTrue(Directory.Exists("results_mua1_0.03"));
            Assert.IsTrue(Directory.Exists("results_mua1_0.04"));
        }
        /// <summary>
        /// test to verify correct parameter sweep folder names created for output
        /// </summary>
        // can't get following to work because of the string problem
        //[Test]
        //public void validate_parameter_sweep_folder_names_when_no_input_file_specified_but_outname_given()
        //{
        //    // have to break up arg. strings, otherwise outname taken to be "myResults paramsweepdelta..."
        //    string[] args1 = new string[] { "outname=myResults" };
        //    string[] args2 = new string[] { args1 + "paramsweepdelta=mua1,0.01,0.04,0.01" };
        //    Program.Main(args2);
        //    // the default infile.xml that is used has OutputName="results"
        //    Assert.IsTrue(Directory.Exists("myResults_mua1_0.01"));
        //    Assert.IsTrue(Directory.Exists("myResults_mua1_0.02"));
        //    Assert.IsTrue(Directory.Exists("myResults_mua1_0.03"));
        //    Assert.IsTrue(Directory.Exists("myResults_mua1_0.04"));
        //}
    }
}
