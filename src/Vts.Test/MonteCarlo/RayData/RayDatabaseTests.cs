using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.RayData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class RayDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
        };

        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var file in listOfTestGeneratedFiles)
            {
                GC.Collect();
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// test to verify RayDatabase.FromFile is working correctly.
        /// </summary>
        [Test]
        public void validate_RayDatabase_deserialized_class_is_correct_when_using_FromFile()
        {
            var databaseFilename = @"C:\Users\hayakawa\Desktop\RP\Zemax\Uncompressed.ZRD";
            // read the database from file, and verify the correct number of photons were written
            var rayDatabase = ZRDRayDatabase.FromFile(databaseFilename);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
  

        }
    }
}
