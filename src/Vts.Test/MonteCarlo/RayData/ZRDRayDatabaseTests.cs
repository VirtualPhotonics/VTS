using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.RayData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class ZRDRayDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "testzrdraydatabase"
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
        public void validate_ZRDRayDatabase_deserialized_class_is_correct_when_using_FromFile()
        {
            var databaseFilename = @"C:\Users\hayakawa\Desktop\RP\Zemax\uncompressed";
            // read the database from file, and verify the correct number of photons were written
            var rayDatabase = ZRDRayDatabase.FromFile(databaseFilename);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data point

            Assert.AreEqual(rayDatabase.NumberOfElements, 1000);
        }

        /// <summary>
        /// test to verify RayDatabase.ToFile is working correctly.
        /// </summary>
        [Test]
        public void validate_ZRDRayDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            //var databaseFilename = @"C:\Users\hayakawa\Desktop\RP\Zemax\UncompressedReturned.ZRD";
            string databaseFileName = "testzrdraydatabase";
            var firstRayDP = new ZRDRayDataPoint(new RayDataPoint(
                    new Position(1, 1, 0),
                    new Direction(0, 1 / Math.Sqrt(2), -1 / Math.Sqrt(2)),
                    1.0));
            var secondRayDP = new ZRDRayDataPoint(new RayDataPoint(
                    new Position(2, 2, 0),
                    new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2)),
                    2.0));
            using (var dbWriter = new ZRDRayDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFileName))
            {       
                dbWriter.Write(firstRayDP);
                dbWriter.Write(secondRayDP);
            }
            // read back file written
            var rayDatabase = ZRDRayDatabase.FromFile(databaseFileName);
            Assert.AreEqual(rayDatabase.NumberOfElements, 2);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.IsTrue(dp1.X == firstRayDP.X);
            Assert.IsTrue(dp1.Y == firstRayDP.Y);
            Assert.IsTrue(dp1.Z == firstRayDP.Z);
            Assert.IsTrue(dp1.Ux == firstRayDP.Ux);
            Assert.IsTrue(dp1.Uy == firstRayDP.Uy);
            Assert.IsTrue(dp1.Uz == firstRayDP.Uz);
            Assert.IsTrue(dp1.Weight == firstRayDP.Weight);
            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.IsTrue(dp2.X == secondRayDP.X);
            Assert.IsTrue(dp2.Y == secondRayDP.Y);
            Assert.IsTrue(dp2.Z == secondRayDP.Z);
            Assert.IsTrue(dp2.Ux == secondRayDP.Ux);
            Assert.IsTrue(dp2.Uy == secondRayDP.Uy);
            Assert.IsTrue(dp2.Uz == secondRayDP.Uz);
            Assert.IsTrue(dp2.Weight == secondRayDP.Weight);
        }
    }
}