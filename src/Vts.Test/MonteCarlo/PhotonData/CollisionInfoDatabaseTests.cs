using NUnit.Framework;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class CollisionInfoDatabaseTests
    {
        /// <summary>
        /// test to verify that CollisionInfoDatabaseWriter and CollisionInfoDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void validate_CollisionInfo_deserialized_class_is_correct_when_using_WriteToFile()
        {
            int numberOfSubregions = 3;
            string databaseFilename = "testcollisioninfodatabase";
            
            #region Notes on implementation...
            // todo: which do we like? (#1 requires writing a separate class, #2 requires a little more comfort
            // with using generics day-to-day
            // 1) using (var dbWriter = new CollisionInfoDatabaseWriter("testcollisioninfodatabase", numberOfSubregions))
            // 2) (below)
            // 3) another option would be to "wire" this up with Unity and get both
            //using (var dbWriter = new DatabaseWriter<CollisionInfoDatabase, CollisionInfo>(
            //    databaseFilename,
            //    new CollisionInfoDatabase(numberOfSubregions),
            //    new CollisionInfoSerializer(numberOfSubregions)))
            #endregion

            using (var dbWriter = new CollisionInfoDatabaseWriter(
                VirtualBoundaryType.pMCDiffuseReflectance,"testcollisioninfodatabase", numberOfSubregions))
            {
                dbWriter.Write(
                    new CollisionInfo(numberOfSubregions)
                        {
                            new SubRegionCollisionInfo(10.0, 1000),
                            new SubRegionCollisionInfo(20.0, 2000),
                            new SubRegionCollisionInfo(30.0, 3000)
                        });

                dbWriter.Write(
                    new CollisionInfo(numberOfSubregions)
                        {
                            new SubRegionCollisionInfo(40.0, 4000),
                            new SubRegionCollisionInfo(50.0, 5000),
                            new SubRegionCollisionInfo(60.0, 6000)
                        });
            }

            var dbcloned = CollisionInfoDatabase.FromFile(databaseFilename);

            Assert.AreEqual(dbcloned.NumberOfSubRegions, 3);
            Assert.AreEqual(dbcloned.NumberOfElements, 2);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = dbcloned.DataPoints.GetEnumerator();

            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.AreEqual(dp1[0].PathLength, 10.0);
            Assert.AreEqual(dp1[0].NumberOfCollisions, 1000);
            Assert.AreEqual(dp1[1].PathLength, 20.0);
            Assert.AreEqual(dp1[1].NumberOfCollisions, 2000);
            Assert.AreEqual(dp1[2].PathLength, 30.0);
            Assert.AreEqual(dp1[2].NumberOfCollisions, 3000);

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.AreEqual(dp2[0].PathLength, 40.0);
            Assert.AreEqual(dp2[0].NumberOfCollisions, 4000);
            Assert.AreEqual(dp2[1].PathLength, 50.0);
            Assert.AreEqual(dp2[1].NumberOfCollisions, 5000);
            Assert.AreEqual(dp2[2].PathLength, 60.0);
            Assert.AreEqual(dp2[2].NumberOfCollisions, 6000);
        }
    }
}
