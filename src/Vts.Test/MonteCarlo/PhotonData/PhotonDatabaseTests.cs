using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class PhotonDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "testphotondatabase",
            "testphotondatabase.txt"
        };

        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// test to verify that PhotonDatabaseWriter and PhotonDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void Validate_PhotonDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            // test serialization
            new SimulationInput().ToFile("SimulationInputTest.txt");

            var databaseFilename = "testphotondatabase";

            using(var dbWriter = new PhotonDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFilename))
            {
                dbWriter.Write(new PhotonDataPoint(
                                   new Position(1, 2, 3),
                                   new Direction(0, 0, 1),
                                   1.0, // weight
                                   10, // time
                                   PhotonStateType.None));

                dbWriter.Write(new PhotonDataPoint(
                                   new Position(4, 5, 6),
                                   new Direction(1, 0, 0),
                                   0.50,
                                   100,
                                   PhotonStateType.None));
                dbWriter.Close();
            }

            // read the database from file, and verify the correct number of photons were written
            var dbCloned = PhotonDatabase.FromFile(databaseFilename);

            Assert.AreEqual(2, dbCloned.NumberOfElements);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = dbCloned.DataPoints.GetEnumerator();

            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.AreEqual(new Position(1, 2, 3),dp1.Position);
            Assert.AreEqual(new Direction(0, 0, 1),dp1.Direction);
            Assert.AreEqual(1.0, dp1.Weight);
            Assert.AreEqual(10, dp1.TotalTime);
            Assert.IsTrue(dp1.StateFlag.HasFlag(PhotonStateType.None));

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.AreEqual(new Position(4, 5, 6),dp2.Position);
            Assert.AreEqual(new Direction(1, 0, 0),dp2.Direction);
            Assert.AreEqual(0.5,dp2.Weight);
            Assert.AreEqual(100,dp2.TotalTime);
            Assert.IsTrue(dp2.StateFlag.HasFlag(PhotonStateType.None));

            enumerator.Dispose();
        }
    }
}
