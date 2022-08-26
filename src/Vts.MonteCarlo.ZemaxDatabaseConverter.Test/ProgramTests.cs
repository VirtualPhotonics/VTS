using NUnit.Framework;
using System.Collections.Generic;
using System;
using Vts.IO;
using Vts.MonteCarlo.RayData;
using System.Threading.Tasks;
using System.IO;

namespace Vts.MonteCarlo.ZemaxDatabaseConverter.Test
{
    [TestFixture]
    public class ProgramTests
    {
        // This tests code that would be executed to convert ZRD DB to/from MCCL compatible DB
        // An actual Zemax output ZRD file was first used to verify tests, however file too large
        // to add to resources.  Instead this program is used to convert a MCCL DB to Zemax DB
        // and that is used to test
        
        private static SimulationInput _input;
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "testzrdraydatabase",
            "testzrdraydatabase.txt",
            "testoutput",
            "testoutput.txt",
            "testmcclraydatabase",
            "testmcclraydatabase.txt",
         };
        readonly List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "ray_database_generator"
        };
        readonly List<string> listOfMCCLInfiles = new List<string>()
        {
            "ellip_FluenceOfRhoAndZ",
            "infinite_cylinder_AOfXAndYAndZ",
            "multi_infinite_cylinder_AOfXAndYAndZ",
            "fluorescence_emission_AOfXAndYAndZ_source_infinite_cylinder",
            "embedded_directional_circular_source_ellip_tissue",
            "Flat_2D_source_one_layer_ROfRho",
            "Flat_2D_source_two_layer_bounded_AOfRhoAndZ",
            "Gaussian_2D_source_one_layer_ROfRho",
            "Gaussian_line_source_one_layer_ROfRho",
            "one_layer_all_detectors",
            "one_layer_FluenceOfRhoAndZ_RadianceOfRhoAndZAndAngle",
            "one_layer_ROfRho_FluenceOfRhoAndZ",
            "pMC_one_layer_ROfRho_DAW",
            "three_layer_ReflectedTimeOfRhoAndSubregionHist",
            "two_layer_momentum_transfer_detectors",
            "two_layer_ROfRho",
            "two_layer_ROfRho_with_db",
            "voxel_ROfXAndY_FluenceOfXAndYAndZ",
            "surface_fiber_detector",
            "ray_database_generator"
        };

        /// <summary>
        /// Set up simulation specifying RayDatabase to be written
        /// </summary>
        [OneTimeSetUp]
        public async Task Setup_simulation_input_components()
        {
            // delete previously generated files
            Clear_folders_and_files();

            // generate infiles for MCCL
            string[] arguments = new string[] { "geninfiles" };
            CommandLineApplication.Program.Main(arguments);
            // the following will generate a DB file by default named RayDiffuseReflectanceDatabase
            // in folder ray_database_generator
            arguments = new string[] { "infile=infile_ray_database_generator" };
            await Task.Run(() => CommandLineApplication.Program.Main(arguments));
        }

        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }            
            // delete any previously generated folders
            foreach (var folder in listOfTestGeneratedFolders)
            {
                if (Directory.Exists(folder))
                {
                    FileIO.DeleteDirectory(folder);
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
        /// Test to verify sanity check on input works correctly
        /// </summary>
        [Test]
        public async Task Validate_VerifyInputs_method_returns_correct_values()
        {
            Clear_folders_and_files();

            // the following will fail because only 1 argument and file does not exist
            string[] arguments = new string[] { "infile=databaseToConvert" };
            var status = await Task.Run(() => Program.Main(arguments));
            Assert.IsTrue(status == 0);
            // the following will be successful because 1st argument is file that gets
            // generated in OneTimeSetup
            arguments = new string[] 
                { "infile=ray_database_generator/RayDiffuseReflectanceDatabase","infiletype=mccl","outfile=testoutput" };
            status = await Task.Run(() => Program.Main(arguments));
            Assert.IsTrue(status == 1);
        }
        /// <summary>
        /// Test that uses app to convert RayDatabase written in OneTimeSetup to a Zemax ZRD ray database
        /// Validation values used from prior test
        /// </summary>
        [Test]
        public async Task Validate_conversion_from_MCCL_RayDatabase_to_Zemax_ZrdDatabase_successful()
        {
            // run database converter on MCCL Ray Database generated in OneTimeSetup
            var arguments = new string[] { "infile=ray_database_generator/RayDiffuseReflectanceDatabase","infiletype=mccl","outfile=testzrdraydatabase" };
            await Task.Run(() => Program.Main(arguments));
            // read file written
            var rayDatabase = Zemax.ZrdRayDatabase.FromFile("testzrdraydatabase");
            Assert.AreEqual(95, rayDatabase.NumberOfElements);

            // manually enumerate through the first element
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.IsTrue(Math.Abs(dp1.X - 2.04889) < 1e-5);
            Assert.IsTrue(Math.Abs(dp1.Y - 3.3) < 53191e-5);
            Assert.IsTrue(Math.Abs(dp1.Z - 0.0) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Ux + 0.627379) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Uy - 0.177843) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Uz + 0.758133) < 1e-6); // negative because exiting
            Assert.IsTrue(Math.Abs(dp1.Weight - 0.768176) < 1e-6);
            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.IsTrue(Math.Abs(dp2.X - 3.33747) < 1e-5);
            Assert.IsTrue(Math.Abs(dp2.Y + 15.8257) < 1e-4);
            Assert.IsTrue(Math.Abs(dp2.Z - 0.0) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Ux - 0.384918) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Uy + 0.814311) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Uz + 0.434435) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Weight - 0.221243) < 1e-6);
            enumerator.Dispose();
        }
        /// <summary>
        /// test to verify reading actual Zemax ZRD file on hold until we can generate
        /// a small one ourselves.  In the meantime, convert MCCL Ray Database generated
        /// in OneTimeSetup and then convert back to ZRD formatted database
        /// </summary>
        [Test]
        public async Task Validate_conversion_from_Zemax_ZrdDatabase_to_MCCL_RayDatabase_successful()
        {
            //actual ZRD DB is in @"C:\Users\hayakawa\Desktop\RP\Zemax\MyOutput\ZRDDiffuseReflectanceDatabase"
            // run database converter on MCCL Ray Database generated in OneTimeSetup
            var arguments = new string[] { "infile=ray_database_generator/RayDiffuseReflectanceDatabase", "infiletype=mccl", "outfile=testzrdraydatabase" };
            await Task.Run(() => Program.Main(arguments));
            // convert back to zrd file
            arguments = new string[] { "infile=testzrdraydatabase","infiletype=zrd","outfile=testmcclraydatabase" };
            await Task.Run(() => Program.Main(arguments));
            var rayDatabase = RayDatabase.FromFile("testmcclraydatabase");
            Assert.AreEqual(95, rayDatabase.NumberOfElements);
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            // validation data below is identical to test above
            Assert.IsTrue(Math.Abs(dp1.Position.X - 2.04889) < 1e-5);
            Assert.IsTrue(Math.Abs(dp1.Position.Y - 3.3) < 53191e-5);
            Assert.IsTrue(Math.Abs(dp1.Position.Z - 0.0) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Direction.Ux + 0.627379) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Direction.Uy - 0.177843) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Direction.Uz + 0.758133) < 1e-6); // negative because exiting
            Assert.IsTrue(Math.Abs(dp1.Weight - 0.768176) < 1e-6);
            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.IsTrue(Math.Abs(dp2.Position.X - 3.33747) < 1e-5);
            Assert.IsTrue(Math.Abs(dp2.Position.Y + 15.8257) < 1e-4);
            Assert.IsTrue(Math.Abs(dp2.Position.Z - 0.0) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Direction.Ux - 0.384918) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Direction.Uy + 0.814311) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Direction.Uz + 0.434435) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Weight - 0.221243) < 1e-6);
            enumerator.Dispose();
        }


    }
}
