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
            if (File.Exists("newinfile_one_layer_ROfRho_FluenceOfRhoAndZ.xml"))
            {
                File.Delete("newinfile_one_layer_ROfRho_FluenceOfRhoAndZ.xml");
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ", true); // delete recursively
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01", true);
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.02"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.02", true);
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03", true);
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
            string[] arguments = new string[] {"geninfiles"};
            Program.Main(arguments);
            Assert.IsTrue(File.Exists("newinfile_one_layer_ROfRho_FluenceOfRhoAndZ.xml"));
        }
        /// <summary>
        /// test to verify correct folder name created for output
        /// </summary>
        [Test]
        public void validate_output_folder_name_when_using_geninfile_infile()
        {
            string[] arguments = new string[] { "infile=newinfile_one_layer_ROfRho_FluenceOfRhoAndZ.xml" };
            Program.Main(arguments);
            // newinfile.xml has OutputName="results"
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ"));
        }
        /// <summary>
        /// test to verify correct parameter sweep folder names created for output
        /// </summary>
        [Test]
        public void validate_parameter_sweep_folder_names_when_using_geninfile_infile()
        {
            // the following string does not work because it sweeps 0.01, 0.03 due to round
            // off error in MonteCarloSetup
            //string[] arguments = new string[] { "paramsweepdelta=mua1,0.01,0.03,0.01" };
            string[] arguments = new string[] { "infile=newinfile_one_layer_ROfRho_FluenceOfRhoAndZ.xml", "paramsweep=mua1,0.01,0.03,3" };
            Program.Main(arguments);
            // the default infile.xml that is used has OutputName="results"
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.02"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03"));
        }
        /// <summary>
        /// test to verify correct parameter sweep folder names created for output
        /// </summary>
         //can't get following to work because of the string problem
        [Test]
        public void validate_parameter_sweep_folder_names_when_specifying_outname()
        {
            // have to break up arg. strings, otherwise outname taken to be "myResults paramsweep..."
            string[] arguments = new string[] { "infile=newinfile_one_layer_ROfRho_FluenceOfRhoAndZ.xml", "outname=myResults", "paramsweep=mua1,0.01,0.03,3" };
            Program.Main(arguments);
            // the default infile.xml that is used has OutputName="results" 
            // so following tests verify that that name got overwritten
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.01"));
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.02"));
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.03"));
        }
    }
}
