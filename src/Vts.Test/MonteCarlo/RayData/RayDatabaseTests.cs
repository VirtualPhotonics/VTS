using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.RayData;

namespace Vts.Test.MonteCarlo.RayData
{
    [TestFixture]
    public class RayDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles =
        [
            "testraydatabase",
            "testraydatabase.txt"
        ];

        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void Clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// test to verify that RayDatabaseWriter and RayDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void Validate_RayDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            // test serialization
            new SimulationInput().ToFile("SimulationInputTest.txt");

            var databaseFilename = "testraydatabase";

            using(var dbWriter = new RayDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFilename))
            {
                dbWriter.Write(new RayDataPoint(1.0, 2.0, 3.0,
                                   0.0, 0.0, 1.0,
                                   1.0 // weight
                                   ));

                dbWriter.Write(new RayDataPoint(4.0, 5.0, 6.0,
                                   1.0, 0.0, 0.0,
                                   0.50
                                   ));
                dbWriter.Close();
            }

            // read the database from file, and verify the correct number of rays were written
            var dbCloned = RayDatabase.FromFile(databaseFilename);

            Assert.That(dbCloned.NumberOfElements, Is.EqualTo(2));

            // manually enumerate through the first two elements (same as foreach)
            // RayDatabase is designed so you don't have to have the whole thing
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
            }

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            if (dp2 != null)
            {
                Assert.That(dp2.Position, Is.EqualTo(new Position(4, 5, 6)));
                Assert.That(dp2.Direction, Is.EqualTo(new Direction(1, 0, 0)));
                Assert.That(dp2.Weight, Is.EqualTo(0.5));
            }

            enumerator.Dispose();
        }
    }
}
