using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Vts.MonteCarlo.PostProcessor.Test
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
            if (File.Exists("infile_PostProcessor_ROfRho.xml"))
            {
                File.Delete("infile_PostProcessor_ROfRho.xml");
            }
            if (File.Exists("infile_PostProcessor_pMC_ROfRho.xml"))
            {
                File.Delete("infile_PostProcessor_pMC_ROfRho.xml");
            }
            if (Directory.Exists("PostProcessor_pMC_ROfRho"))
            {
                Directory.Delete("PostProcessor_pMC_ROfRho", true); // delete recursively
            }
            if (Directory.Exists("PostProcessor_ROfRho"))
            {
                Directory.Delete("PostProcessor_ROfRho", true); 
            }
        }
        /// <summary>
        /// test to verify "geninfile" option works successfully
        /// </summary>
        [Test]
        public void validate_generate_infile()
        {
            string[] arguments = new string[] {"geninfiles"};
            Program.Main(arguments);
            Assert.IsTrue(File.Exists("infile_PostProcessor_ROfRho.xml"));
        }
        ///// <summary>
        ///// test to verify correct folder name created for output
        ///// </summary>
        //[Test]
        //public void validate_output_folder_name_when_using_geninfile_infile()
        //{
        //    string[] arguments = new string[] { "infile=infile_PostProcessor_ROfRho.xml" };
        //    Program.Main(arguments);
        //    Assert.IsTrue(Directory.Exists("PostProcess_ROfRho"));
        //}
    }
}
