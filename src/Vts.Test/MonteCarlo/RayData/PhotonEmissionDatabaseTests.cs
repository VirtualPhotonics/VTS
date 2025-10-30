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
    public class PhotonEmissionDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "testphotonemissiondatabase",
            "testphotonemissiondatabase.txt"
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
        /// test to verify that PhotonEmissionDatabaseWriter and PhotonEmissionDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void Validate_PhotonEmissionDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            // test serialization
            new SimulationInput().ToFile("SimulationInputTest.txt");

            const string databaseFilename = "testphotonemissiondatabase";

            using(var dbWriter = new PhotonEmissionDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFilename))
            {
                dbWriter.Write(new PhotonEmissionDataPoint(
                                   1.0, 2.0, 3.0,
                                   0.0, 0.0, 1.0,
                                   1.0, // weight
                                   10 // time
                                   ));

                dbWriter.Write(new PhotonEmissionDataPoint(
                                   4.0, 5.0, 6.0,
                                   1.0, 0.0, 0.0,
                                   0.50,
                                   100
                                ));
                dbWriter.Close();
            }

            // read the database from file, and verify the correct number of photons were written
            var dbCloned = PhotonEmissionDatabase.FromFile(databaseFilename);

            Assert.That(dbCloned.NumberOfElements, Is.EqualTo(2));

            // manually enumerate through the first two elements (same as foreach)
            // PhotonEmissionDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = dbCloned.DataPoints.GetEnumerator();

            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            if (dp1 != null)
            {
                Assert.That(dp1.Position, Is.EqualTo(new Position(1, 2, 3)));
                Assert.That(dp1.Direction, Is.EqualTo(new Direction(0, 0, 1)));
                Assert.That(dp1.Weight, Is.EqualTo(1.0));
                Assert.That(dp1.TotalTime, Is.EqualTo(10));
            }

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            if (dp2 != null)
            {
                Assert.That(dp2.Position, Is.EqualTo(new Position(4, 5, 6)));
                Assert.That(dp2.Direction, Is.EqualTo(new Direction(1, 0, 0)));
                Assert.That(dp2.Weight, Is.EqualTo(0.5));
                Assert.That(dp2.TotalTime, Is.EqualTo(100));
            }

            enumerator.Dispose();
        }
    }
}
