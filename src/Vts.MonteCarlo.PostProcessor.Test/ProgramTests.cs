using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Vts.MonteCarlo.PostProcessor.Test
{
    [TestFixture]
    public class ProgramTests
    {
        /// <summary>
        /// clear all previously generated folders and files.
        /// </summary>
        
        // Note: needs to be kept current with PostProcessorInputProvider.  If an infile is added there, it should be added here.
        List<string> listOfMCPPInfiles = new List<string>()
        {
            "PostProcessor_ROfRho", 
            "PostProcessor_pMC_ROfRhoROfRhoAndTime",
            "PostProcessor_pMC_ROfFxROfFxAndTime",
        };
        List<string> listOfMCCLInfiles = new List<string>()
        {
            "ellip_FluenceOfRhoAndZ", 
            "embeddedDirectionalCircularSourceEllipTissue",
            "Flat_source_one_layer_ROfRho",
            "Gaussian_source_one_layer_ROfRho",
            "one_layer_all_detectors",
            "one_layer_FluenceOfRhoAndZ_RadianceOfRhoAndZAndAngle",
            "one_layer_ROfRho_FluenceOfRhoAndZ",
            "pMC_one_layer_ROfRho_DAW",
            "three_layer_ReflectedTimeOfRhoAndSubregionHist",
            "two_layer_momentum_transfer_detectors",
            "two_layer_ROfRho",
            "two_layer_ROfRho_with_db",
            "voxel_ROfXAndY_FluenceOfXAndYAndZ",
        };

        /// <summary>
        /// clear all previously generated folders and files, then regenerate sample infiles using "geninfiles" option.
        /// </summary>
        [OneTimeSetUp]
        public async Task setup()
        {
            clear_folders_and_files();

            string[] arguments = new string[] { "geninfiles" };
            // generate sample MCPP infiles because unit tests below rely on infiles being generated
            Program.Main(arguments);

            // generate MCCL output so that can post-process
            // generate MCCL infiles - this is overkill but it verifies that we have an MCCL infile
            // that pairs with a MCPP infile
            CommandLineApplication.Program.Main(arguments);
            arguments = new string[] { "infile=infile_pMC_one_layer_ROfRho_DAW" };
            await Task.Run(() => CommandLineApplication.Program.Main(arguments));
        }

        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            // delete any previously generated infiles to test that "geninfiles" option creates them
            foreach (var infile in listOfMCPPInfiles)
            {
                if (File.Exists("infile_" + infile + ".txt"))
                {
                    File.Delete("infile_" + infile + ".txt");
                }
                if (Directory.Exists(infile))
                {
                    Directory.Delete(infile, true); // delete recursively
                }
            }
            foreach (var infile in listOfMCCLInfiles)
            {
                if (File.Exists("infile_" + infile + ".txt"))
                {
                    File.Delete("infile_" + infile + ".txt");
                }
                if (Directory.Exists(infile))
                {
                    Directory.Delete(infile, true); // delete recursively
                }
            }
        }

        /// <summary>
        /// test to verify "geninfiles" option works successfully. 
        /// </summary>
        [Test]
        public void validate_geninfiles_option_generates_all_infiles()
        {
            foreach (var infile in listOfMCPPInfiles)
            {
                Assert.IsTrue(File.Exists("infile_" + infile + ".txt"));
            }
        }

        /// <summary>
        /// test to verify correct output files generated when post process MCCL database
        /// </summary>
        [Test]
        public void validate_output_folders_and_files_correct_when_using_geninfile_infile()
        {
            string[] arguments = new string[] { "infile=infile_PostProcessor_pMC_ROfRhoROfRhoAndTime.txt" };
            Program.Main(arguments);
            Assert.IsTrue(Directory.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime"));
            // verify infile gets written to output folder
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/PostProcessor_pMC_ROfRhoROfRhoAndTime.txt"));
            // verify detectors specified in MCPP infile get written
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRhoReference.txt"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRhoReference"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRhoReference_2"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRho_mus1p5.txt"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRho_mus1p5"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRho_mus1p5_2"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRho_mus0p5.txt"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRho_mus0p5"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRho_mus0p5_2"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRhoAndTime_mus1p5.txt"));
            Assert.IsTrue(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRhoAndTime_mus1p5"));
            Assert.IsFalse(File.Exists("PostProcessor_pMC_ROfRhoROfRhoAndTime/pMCROfRhoAndTime_mus1p5_2"));
        }
    }
}
