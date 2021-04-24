using System;
using System.Collections.Generic;
using System.Linq;
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
            var rayDatabase = ZRDRayDatabase.FromFile(databaseFilename, 1000);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data point
        }

        /// <summary>
        /// test to verify RayDatabase.ToFile is working correctly.
        /// </summary>
        [Test]
        public void validate_RayDatabase_serialized_class_is_correct_when_using_ToFile()
        {
            var databaseFilename = @"C:\Users\hayakawa\Desktop\RP\Zemax\UncompressedReturned.ZRD";
            // create list of ZRDRayDataPoint
            var rayDPList = new List<RayDataPoint>();
            var firstRayDP = new RayDataPoint(
                new Position(1, 1, 0),
                new Direction(0, 1/Math.Sqrt(2), -1/Math.Sqrt(2)),
                1.0);
            rayDPList.Add(firstRayDP);
            var secondRayDP = new RayDataPoint(
                new Position(2, 2, 0),
                new Direction(1/Math.Sqrt(2), 0, -1/Math.Sqrt(2)),
                2.0);
            rayDPList.Add(secondRayDP);
            var thirdRayDP = new RayDataPoint(
                new Position(3, 3, 0),
                new Direction(0, 0, -1),
                3.0);
            rayDPList.Add(thirdRayDP);
            ZRDRayDatabase.ToFile(databaseFilename, rayDPList);
            // read back file written
            var rayDatabase = ZRDRayDatabase.FromFile(databaseFilename, rayDPList.Count);
            Assert.IsTrue(rayDatabase.First().Position.X == firstRayDP.Position.X);
            Assert.IsTrue(rayDatabase.First().Position.Y == firstRayDP.Position.Y);
            Assert.IsTrue(rayDatabase.First().Position.Z == firstRayDP.Position.Z);
            Assert.IsTrue(rayDatabase.First().Direction.Ux == firstRayDP.Direction.Ux);
            Assert.IsTrue(rayDatabase.First().Direction.Uy == firstRayDP.Direction.Uy);
            Assert.IsTrue(rayDatabase.First().Direction.Uz == firstRayDP.Direction.Uz);
            Assert.IsTrue(rayDatabase.First().Weight == firstRayDP.Weight);
            Assert.IsTrue(rayDatabase.Skip(1).First().Position.X == secondRayDP.Position.X);
            Assert.IsTrue(rayDatabase.Skip(1).First().Position.Y == secondRayDP.Position.Y);
            Assert.IsTrue(rayDatabase.Skip(1).First().Position.Z == secondRayDP.Position.Z);
            Assert.IsTrue(rayDatabase.Skip(1).First().Direction.Ux == secondRayDP.Direction.Ux);
            Assert.IsTrue(rayDatabase.Skip(1).First().Direction.Uy == secondRayDP.Direction.Uy);
            Assert.IsTrue(rayDatabase.Skip(1).First().Direction.Uz == secondRayDP.Direction.Uz);
            Assert.IsTrue(rayDatabase.Skip(1).First().Weight == secondRayDP.Weight);
            Assert.IsTrue(rayDatabase.Skip(2).First().Position.X == thirdRayDP.Position.X);
            Assert.IsTrue(rayDatabase.Skip(2).First().Position.Y == thirdRayDP.Position.Y);
            Assert.IsTrue(rayDatabase.Skip(2).First().Position.Z == thirdRayDP.Position.Z);
            Assert.IsTrue(rayDatabase.Skip(2).First().Direction.Ux == thirdRayDP.Direction.Ux);
            Assert.IsTrue(rayDatabase.Skip(2).First().Direction.Uy == thirdRayDP.Direction.Uy);
            Assert.IsTrue(rayDatabase.Skip(2).First().Direction.Uz == thirdRayDP.Direction.Uz);
            Assert.IsTrue(rayDatabase.Skip(2).First().Weight == thirdRayDP.Weight);
        }
    }
}
