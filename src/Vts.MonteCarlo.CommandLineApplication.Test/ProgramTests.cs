
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Vts.IO;

namespace Vts.MonteCarlo.CommandLineApplication.Test
{
    [TestFixture]
    public class ProgramTests
    {
        // Note: needs to be kept current with SimulationInputProvider.  If an infile is added there, it should be added here.
        List<string> listOfInfiles = new List<string>()
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
        public void setup()
        {
            clear_folders_and_files();
            // generate sample infiles because unit tests below rely on infiles being generated
            string[] arguments = new string[] { "geninfiles" };
            Program.Main(arguments);
        }

        /// <summary>
        /// clear all previously generated folders and files.
        /// </summary>
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
        // delete any previously generated infiles to test that "geninfiles" option creates them
            foreach (var infile in listOfInfiles)
            {
                if (File.Exists("infile_" + infile + ".txt"))
                {
                    File.Delete("infile_" + infile + ".txt");
                }
                if (Directory.Exists(infile))
                {
                    Directory.Delete(infile, true);
                }
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
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01_mus1_1.00"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01_mus1_1.00", true);
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03_mus1_1.00"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03_mus1_1.00", true);
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01_mus1_1.20"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01_mus1_1.20", true);
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03_mus1_1.20"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03_mus1_1.20", true);
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_nphot_10.00"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_nphot_10.00", true);
            }
            if (Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_nphot_20.00"))
            {
                Directory.Delete("one_layer_ROfRho_FluenceOfRhoAndZ_nphot_20.00", true);
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
            if (Directory.Exists("one_layer_ROfRho_Mus_only"))
            {
                Directory.Delete("one_layer_ROfRho_Mus_only", true);
            }
        }

        /// <summary>
        /// test to verify "geninfiles" option works successfully. 
        /// </summary>
        [Test]
        public void validate_geninfiles_option_generates_all_infiles()
        {
            foreach (var infile in listOfInfiles)
            {
                Assert.IsTrue(File.Exists("infile_" + infile + ".txt"));
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
                string[] arguments = new string[] { "infile=" + "infile_" + infile + ".txt" };

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
            // verify infile gets written to output folder
            Assert.IsTrue(File.Exists("one_layer_ROfRho_FluenceOfRhoAndZ/one_layer_ROfRho_FluenceOfRhoAndZ.txt"));
        }
        /// <summary>
        /// test to verify correct parameter sweep folder names created for output
        /// </summary>
        [Test]
        public void validate_parameter_sweep_folder_names_when_using_geninfile_infile_and_paramsweep()
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
        /// test to verify correct parameter sweep folder names created for output when paramsweeplist is used
        /// </summary>
        [Test]
        public void validate_parameter_sweep_folder_names_when_using_genfile_infile_and_paramsweeplist()
        {
            // the following string does not work because it sweeps 0.01, 0.03 due to round
            // off error in MonteCarloSetup
            //string[] arguments = new string[] { "paramsweepdelta=mua1,0.01,0.03,0.01" };
            string[] arguments = new string[] { "infile=infile_one_layer_ROfRho_FluenceOfRhoAndZ.txt", "paramsweeplist=mua1,3,0.01,0.02,0.03" };
            Program.Main(arguments);
            // the default infile.txt that is used has OutputName="results"
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.02"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03"));
        }
        /// <summary>
        /// test to verify 2D parameter sweep works correctly.
        /// Note, 3D parameter sweeps work correctly too, no unit test yet.
        /// </summary>
        [Test]
        public void validate_parameter_sweep_folder_names_for_2D_parameter_sweep()
        {
            string[] arguments = new string[]
            {
                "infile=infile_one_layer_ROfRho_FluenceOfRhoAndZ.txt", "paramsweep=mua1,0.01,0.03,2", "paramsweep=mus1,1.0,1.2,2"
            };
            Program.Main(arguments);
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01_mus1_1.00"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03_mus1_1.00"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.01_mus1_1.20"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_mua1_0.03_mus1_1.20"));
        }
        /// <summary>
        /// test to verify N sweep
        /// </summary>
        [Test]
        public void validate_parameter_sweep_folder_names_for_parameter_sweep_of_N()
        {
            string[] arguments = new string[]
            {
                "infile=infile_one_layer_ROfRho_FluenceOfRhoAndZ.txt", "paramsweep=nphot,10,20,2"
            };
            Program.Main(arguments);
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_nphot_10.00"));
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_FluenceOfRhoAndZ_nphot_20.00"));
        }
        /// <summary>
        /// test to verify correct parameter sweep folder names created for output
        /// </summary>
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
        /// <summary>
        /// Test to verify that change (Jan 2019) to deserialization of infile to handle specification of
        /// 1) Mus only (no Musp)
        /// 2) Musp only (no Mus)
        /// 3) Mus and Musp specified but inconsistent
        /// </summary>
        [Test]
        public void validate_deserialization_of_infile_for_Mus_only_specification()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(
                assemblyName + ".Resources.infile_unit_test_one_layer_ROfRho_Mus_only.txt", 
                "infile_unit_test_one_layer_ROfRho_Mus_only.txt", name);
            string[] arguments = new string[] { "infile=infile_unit_test_one_layer_ROfRho_Mus_only.txt" };
            Program.Main(arguments);
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_Mus_only"));
        }
        [Test]
        public void validate_deserialization_of_infile_for_Musp_only_specification()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(
                assemblyName + ".Resources.infile_unit_test_one_layer_ROfRho_Musp_only.txt",
                "infile_unit_test_one_layer_ROfRho_Musp_only.txt", name);
            string[] arguments = new string[] { "infile=infile_unit_test_one_layer_ROfRho_Musp_only.txt" };
            Program.Main(arguments);
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_Musp_only"));
        }
        [Test]
        public void validate_deserialization_of_infile_for_Mus_and_Musp_inconsistent_specification()
        {
            var name = Assembly.GetExecutingAssembly().FullName;
            var assemblyName = new AssemblyName(name).Name;
            FileIO.CopyFileFromEmbeddedResources(
                assemblyName + ".Resources.infile_unit_test_one_layer_ROfRho_Musp_and_Mus_inconsistent.txt",
                "infile_unit_test_one_layer_ROfRho_Musp_and_Mus_inconsistent.txt", name);
            string[] arguments = new string[] { "infile=infile_unit_test_one_layer_ROfRho_Musp_and_Mus_inconsistent.txt" };
            Program.Main(arguments);
            Assert.IsTrue(Directory.Exists("one_layer_ROfRho_Musp_and_Mus_inconsistent"));
            var writtenInfile = SimulationInput.FromFile(
                "one_layer_RofRho_Musp_and_Mus_inconsistent/one_layer_ROfRho_Musp_and_Mus_inconsistent.txt");
            // infile specifies Mus=5.0 and Musp=1.2 with g=0.8
            // when there is inconsistency in Mus and Musp specification, code modifies Mus to conform to Musp
            // the following test verifies that Mus was modified accordingly
            Assert.Less(Math.Abs(writtenInfile.TissueInput.Regions[1].RegionOP.Mus - 6.0), 1e-6);
        }
    }
}
