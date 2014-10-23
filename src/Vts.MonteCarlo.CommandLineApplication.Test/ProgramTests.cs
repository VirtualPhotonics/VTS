
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace Vts.MonteCarlo.CommandLineApplication.Test
{
    [TestFixture]
    public class ProgramTests
    {
        /// <summary>
        /// clear all previously generated folders and files, then regenerate sample infiles using "geninfiles" option.
        /// </summary>
        
        // Note: needs to be kept current with SimulationInputProvider.  If an infile is added there, it should be added here.
        List<string> listOfInfiles = new List<string>()
        {
            "infile_ellip_FluenceOfRhoAndZ.txt",
            "infile_Gaussian_source_one_layer_ROfRho.txt", // uncomment this when bug fixed in Gaussian source
            "infile_one_layer_all_detectors.txt",
            "infile_one_layer_FluenceOfRhoAndZ_RadianceOfRhoAndZAndAngle.txt",
            "infile_one_layer_ROfRho_FluenceOfRhoAndZ.txt",
            "infile_pMC_one_layer_ROfRho_DAW.txt",
            "infile_three_layer_ReflectedTimeOfRhoAndSubregionHist.txt",
            "infile_two_layer_ReflectedMTOfRhoAndSubregionHist.txt",
            "infile_two_layer_ROfRho.txt",
            "infile_two_layer_ROfRho_with_db.txt",
        };
        [TestFixtureSetUp]
        public void clear_folders_and_files()
        {
        // delete any previously generated infiles to test that "geninfiles" option creates them
            foreach (var infile in listOfInfiles)
            {
                if (File.Exists(infile))
                {
                    File.Delete(infile);
                } 
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
            if (Directory.Exists("pMC_one_layer_ROfRho_DAW"))
            {
                Directory.Delete("pMC_one_layer_ROfRho_DAW", true);
            } 
            if (File.Exists("pMC_one_layer_ROfRho_DAW/DiffuseReflectanceDatabase"))
            {
                File.Delete("pMC_one_layer_ROfRho_DAW/DiffuseReflectanceDatabase");
            } 
            if (File.Exists("pMC_one_layer_ROfRho_DAW/DiffuseReflectanceDatabase.txt"))
            {
                File.Delete("pMC_one_layer_ROfRho_DAW/DiffuseReflectance.txt");
            }
            if (File.Exists("pMC_one_layer_ROfRho_DAW/CollisionInfoDatabase"))
            {
                File.Delete("pMC_one_layer_ROfRho_DAW/CollisionInfoDatabase");
            }
            if (File.Exists("pMC_one_layer_ROfRho_DAW/CollisionInfoDatabase.txt"))
            {
                File.Delete("pMC_one_layer_ROfRho_DAW/CollisionInfoDatabase.txt");
            }
            // generate sample infiles because unit tests below rely on infiles being generated
            string[] arguments = new string[] { "geninfiles" };
            Program.Main(arguments);
        }

        /// <summary>
        /// test to verify "geninfiles" option works successfully. 
        /// </summary>
        [Test]
        public void validate_geninfiles_option_generates_all_infiles()
        {
            foreach (var infile in listOfInfiles)
            {
                Assert.IsTrue(File.Exists(infile));
            }
        }
        /// <summary>
        /// test to verify infiles generated run successfully
        /// </summary>
        [Test]
        public void validate_infiles_generated_using_geninfiles_option_run_successfully()
        {
            foreach (var infile in listOfInfiles)
            {
                string[] arguments = new string[] {"infile=" + infile};

                var result = Program.Main(arguments);
                Assert.IsTrue(result==0);
            }
        }
        /// <summary>
        /// test to verify correct folder name created for output
        /// </summary>
        [Test]
        public void validate_output_folder_name_when_using_geninfile_infile()
        {
            string[] arguments = new string[] { "infile=infile_one_layer_ROfRho_FluenceOfRhoAndZ.txt" };
            Program.Main(arguments);
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
            string[] arguments = new string[] { "infile=infile_one_layer_ROfRho_FluenceOfRhoAndZ.txt", "paramsweep=mua1,0.01,0.03,3" };
            Program.Main(arguments);
            // the default infile.txt that is used has OutputName="results"
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
            string[] arguments = new string[] { "infile=infile_one_layer_ROfRho_FluenceOfRhoAndZ.txt", "outname=myResults", "paramsweep=mua1,0.01,0.03,3" };
            Program.Main(arguments);
            // the default infile.txt that is used has OutputName="results" 
            // so following tests verify that that name got overwritten
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.01"));
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.02"));
            Assert.IsTrue(Directory.Exists("myResults_mua1_0.03"));
        }
        /// <summary>
        /// test to verify database gets generated for post-processing
        /// </summary>
        //can't get following to work because of the string problem
        [Test]
        public void validate_database_generation()
        {
            // have to break up arg. strings, otherwise outname taken to be "myResults paramsweep..."
            string[] arguments = new string[] { "infile=infile_pMC_one_layer_ROfRho_DAW.txt" };
            Program.Main(arguments);
            Assert.IsTrue(Directory.Exists("pMC_one_layer_ROfRho_DAW"));
            Assert.IsTrue(File.Exists("pMC_one_layer_ROfRho_DAW/DiffuseReflectanceDatabase"));
            Assert.IsTrue(File.Exists("pMC_one_layer_ROfRho_DAW/DiffuseReflectanceDatabase.txt"));
            Assert.IsTrue(File.Exists("pMC_one_layer_ROfRho_DAW/CollisionInfoDatabase"));
            Assert.IsTrue(File.Exists("pMC_one_layer_ROfRho_DAW/CollisionInfoDatabase.txt"));
        }
    }
}
