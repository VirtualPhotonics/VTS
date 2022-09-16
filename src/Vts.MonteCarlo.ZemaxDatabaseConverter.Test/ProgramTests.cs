using NUnit.Framework;
using System.Collections.Generic;
using System;
using Vts.IO;
using Vts.MonteCarlo.RayData;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Vts.MonteCarlo.ZemaxDatabaseConverter.Test
{
    [TestFixture]
    public class ProgramTests
    {
        // This tests code that would be executed to convert ZRD DB to/from MCCL compatible DB
        // An actual Zemax output ZRD file was first used to verify tests, however file too large
        // to add to resources.  Instead this program is used to convert a MCCL DB to Zemax DB
        // and that is used to test

        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "testzrdraydatabase",
            "testzrdraydatabase.txt",
            "testmcclraydatabase",
            "testmcclraydatabase.txt",
         };
        readonly List<string> listOfTestGeneratedFolders = new List<string>()
        {
            "mcclraydatabase",
            "zrdraydatabase"
        };


        /// <summary>
        /// Set up simulation specifying RayDatabase to be written
        /// </summary>
        [OneTimeSetUp]
        public void Setup_simulation_input_components()
        {
            // delete previously generated files
            Clear_folders_and_files();
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
                if (File.Exists(file))
                {
                    FileIO.FileDelete(file);
                }
            }
            // delete any previously generated folders
            foreach (var folder in listOfTestGeneratedFolders)
            {
                if (Directory.Exists(folder))
                {
                    FileIO.DeleteDirectory(folder);
                }
            }

        }
        /// <summary>
        /// Test to verify sanity check on input works correctly
        /// </summary>
        [Test]
        public async Task Validate_VerifyInputs_method_returns_correct_values()
        {
            // the following will fail because only 1 argument and file does not exist
            string[] arguments = new string[] { "infile=databaseToConvert" };
            var status = await Task.Run(() => Program.Main(arguments));
            Assert.IsTrue(status == 0);
            // no successful mccl-to-zrd test is tested here because tested in other tests
        }
        /// <summary>
        /// Test that uses app to convert RayDatabase written in OneTimeSetup to a Zemax ZRD ray database
        /// Validation values used from prior test
        /// </summary>
        [Test]
        public async Task Validate_conversion_from_MCCL_RayDatabase_to_Zemax_ZrdDatabase_successful()
        {
            // copy MCCL RayDatabase from Resources
            var folder = "mcclraydatabase";
            FileIO.CopyFolderFromEmbeddedResources(folder, "",
                    Assembly.GetExecutingAssembly().FullName, true);
            // run database converter on MCCL Ray Database generated in OneTimeSetup
            var arguments = new string[] {
                "infile=mcclraydatabase/RayDiffuseReflectanceDatabase",
                "infiletype=mccl",
                "outfile=testzrdraydatabase" };
            await Task.Run(() => Program.Main(arguments));
            // read file written
            var rayDatabase = Zemax.ZrdRayDatabase.FromFile("testzrdraydatabase");
            Assert.AreEqual(88, rayDatabase.NumberOfElements);

            // manually enumerate through the first element
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.IsTrue(Math.Abs(dp1.X - 4.18911) < 1e-5);
            Assert.IsTrue(Math.Abs(dp1.Y + 22.1217) < 1e-4);
            Assert.IsTrue(Math.Abs(dp1.Z - 0.0) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Ux - 0.654227) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Uy - 0.223239) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Uz + 0.722600) < 1e-6); // negative because exiting
            Assert.IsTrue(Math.Abs(dp1.Weight - 0.021116) < 1e-6);
            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.IsTrue(Math.Abs(dp2.X - 0.382333) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Y + 2.13952) < 1e-5);
            Assert.IsTrue(Math.Abs(dp2.Z - 0.0) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Ux + 0.711575) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Uy + 0.493464) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Uz + 0.500153) < 1e-6);
            Assert.IsTrue(Math.Abs(dp2.Weight - 0.911520) < 1e-6);
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
            //actual ZRD DB is in @"C:\Users\hayakawa\Desktop\RP\Zemax\ZWOutput\lightfromsourcefile"
            // copy ZRD RayDatabase from Resources
            var folder = "zrdraydatabase";
            FileIO.CopyFolderFromEmbeddedResources(folder, "",
                    Assembly.GetExecutingAssembly().FullName, true);
            // use actual zrd database and convert to mccl database
            var arguments = new string[] {
                "infile=zrdraydatabase/Lightfromsourcefile", 
                "infiletype=zrd", 
                "outfile=testmcclraydatabase" };
            await Task.Run(() => Program.Main(arguments));           
            var rayDatabase = RayDatabase.FromFile("testmcclraydatabase");
            Assert.AreEqual(10, rayDatabase.NumberOfElements);
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.IsTrue(Math.Abs(dp1.Position.X - 0.003760) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Position.Y - 0.001500) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Position.Z - 0.490243) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Direction.Ux - 0.005315) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Direction.Uy - 0.992280) < 1e-6);
            Assert.IsTrue(Math.Abs(dp1.Direction.Uz - 0.123897) < 1e-6); // why is this neg?
            Assert.IsTrue(Math.Abs(dp1.Weight - 0.000730) < 1e-6);
            enumerator.Dispose();
        }


    }
}
