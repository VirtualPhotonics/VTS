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
        /// clear all previously generated folders and files.
        /// </summary>
        [TestFixtureSetUp]
        public void clear_folders_and_files()
        {
            if (File.Exists("newinfile.xml"))
            {
                File.Delete("newinfile.xml");
            }
            if (Directory.Exists("results"))
            {
                Directory.Delete("results", true); // delete recursively
            }
            if (Directory.Exists("results_mua1_0.01"))
            {
                Directory.Delete("results_mua1_0.01", true);
            }
            if (Directory.Exists("results_mua1_0.02"))
            {
                Directory.Delete("results_mua1_0.02", true);
            }
            if (Directory.Exists("results_mua1_0.03"))
            {
                Directory.Delete("results_mua1_0.03", true);
            }
            if (Directory.Exists("myResults_mua1_0.01"))
            {
                Directory.Delete("myResults_mua1_0.01", true);
            }
            if (Directory.Exists("myResults_mua1_0.02"))
            {
                Directory.Delete("myResults_mua1_0.02", true);
            }
            if (Directory.Exists("myResults_mua1_0.03"))
            {
                Directory.Delete("myResults_mua1_0.03", true);
            }
        }
        /// <summary>
        /// test to verify "geninfile" option works successfully"
        /// </summary>
        [Test]
        public void validate_generate_infile()
        {
            string[] arguments = new string[] {"geninfile"};
            Program.Main(arguments);
            Assert.IsTrue(File.Exists("newinfile.xml"));
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
            // the following string does not work because it sweeps 0.01, 0.03 due to round
            // off error in MonteCarloSetup
            //string[] arguments = new string[] { "paramsweepdelta=mua1,0.01,0.03,0.01" };
            string[] arguments = new string[] { "paramsweep=mua1,0.01,0.03,3" };
            Program.Main(arguments);
            // the default infile.xml that is used has OutputName="results"
            Assert.IsTrue(Directory.Exists("results_mua1_0.01"));
            Assert.IsTrue(Directory.Exists("results_mua1_0.02"));
            Assert.IsTrue(Directory.Exists("results_mua1_0.03"));
        }
        /// <summary>
        /// test to verify correct parameter sweep folder names created for output
        /// </summary>
         //can't get following to work because of the string problem
        [Test]
        public void validate_parameter_sweep_folder_names_when_no_input_file_specified_but_outname_given()
        {
            // have to break up arg. strings, otherwise outname taken to be "myResults paramsweep..."
            string[] args1 = new string[] { "outname=myResults", "paramsweep=mua1,0.01,0.03,3" };
            Program.Main(args1);
            // the default infile.xml that is used has OutputName="results" 
            // so following tests verify that that name got overwritten
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.01"));
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.02"));
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.03"));
        }
    }
}
