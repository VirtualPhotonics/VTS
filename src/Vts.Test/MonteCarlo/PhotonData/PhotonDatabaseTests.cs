using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class PhotonDatabaseTests
    {
        /// <summary>
        /// test to verify that PhotonDatabaseWriter and PhotonDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void validate_PhotonDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            // test serialization
            new SimulationInput().ToFile("SimulationInputTest.txt");

            string databaseFilename = "testphotondatabase";

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
            }

            // read the database from file, and verify the correct number of photons were written
            var dbCloned = PhotonDatabase.FromFile(databaseFilename);

            Assert.AreEqual(dbCloned.NumberOfElements, 2);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = dbCloned.DataPoints.GetEnumerator();

            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.AreEqual(dp1.Position, new Position(1, 2, 3));
            Assert.AreEqual(dp1.Direction, new Direction(0, 0, 1));
            Assert.AreEqual(dp1.Weight, 1.0);
            Assert.AreEqual(dp1.TotalTime, 10);
            Assert.IsTrue(dp1.StateFlag.HasFlag(PhotonStateType.None));

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.AreEqual(dp2.Position, new Position(4, 5, 6));
            Assert.AreEqual(dp2.Direction, new Direction(1, 0, 0));
            Assert.AreEqual(dp2.Weight, 0.5);
            Assert.AreEqual(dp2.TotalTime, 100);
            Assert.IsTrue(dp2.StateFlag.HasFlag(PhotonStateType.None));
        }
    }
}
