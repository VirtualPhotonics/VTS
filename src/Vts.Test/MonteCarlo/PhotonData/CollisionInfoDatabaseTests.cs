using NUnit.Framework;
using System.Collections.Generic;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class CollisionInfoDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "testcollisioninfodatabase",
            "testcollisioninfodatabase.txt"
        };

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
        /// test to verify that CollisionInfoDatabaseWriter and CollisionInfoDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void Validate_CollisionInfo_deserialized_class_is_correct_when_using_WriteToFile()
        {
            const int numberOfSubRegions = 3;
            const string databaseFilename = "testcollisioninfodatabase";

            #region Notes on implementation...
            // which do we like? (#1 requires writing a separate class, #2 requires a little more comfort
            // with using generics day-to-day
            // 1) using (var dbWriter = new CollisionInfoDatabaseWriter("testcollisioninfodatabase", numberOfSubRegions))
            // 2) (below)
            // 3) another option would be to "wire" this up with Unity and get both
            //using (var dbWriter = new DatabaseWriter<CollisionInfoDatabase, CollisionInfo>(
            //    databaseFilename,
            //    new CollisionInfoDatabase(numberOfSubRegions),
            //    new CollisionInfoSerializer(numberOfSubRegions)))
            #endregion

            using (var dbWriter = new CollisionInfoDatabaseWriter(VirtualBoundaryType.pMCDiffuseReflectance, "testcollisioninfodatabase", numberOfSubRegions))
            {
                dbWriter.Write(
                    new CollisionInfo(numberOfSubRegions)
                        {
                            new SubRegionCollisionInfo(10.0, 1000),
                            new SubRegionCollisionInfo(20.0, 2000),
                            new SubRegionCollisionInfo(30.0, 3000)
                        });

                dbWriter.Write(
                    new CollisionInfo(numberOfSubRegions)
                        {
                            new SubRegionCollisionInfo(40.0, 4000),
                            new SubRegionCollisionInfo(50.0, 5000),
                            new SubRegionCollisionInfo(60.0, 6000)
                        });
            }

            var dbCloned = CollisionInfoDatabase.FromFile(databaseFilename);

            Assert.AreEqual(3, dbCloned.NumberOfSubRegions);
            Assert.AreEqual(2, dbCloned.NumberOfElements);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            using var enumerator = dbCloned.DataPoints.GetEnumerator();

            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.IsNotNull(dp1);
            Assert.AreEqual(10.0, dp1[0].PathLength);
            Assert.AreEqual(1000, dp1[0].NumberOfCollisions);
            Assert.AreEqual(20.0, dp1[1].PathLength);
            Assert.AreEqual(2000, dp1[1].NumberOfCollisions);
            Assert.AreEqual(30.0, dp1[2].PathLength);
            Assert.AreEqual(3000, dp1[2].NumberOfCollisions);

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.IsNotNull(dp2);
            Assert.AreEqual(40.0, dp2[0].PathLength);
            Assert.AreEqual(4000, dp2[0].NumberOfCollisions);
            Assert.AreEqual(50.0, dp2[1].PathLength);
            Assert.AreEqual(5000, dp2[1].NumberOfCollisions);
            Assert.AreEqual(60.0, dp2[2].PathLength);
            Assert.AreEqual(6000, dp2[2].NumberOfCollisions);
        }
    }
}
