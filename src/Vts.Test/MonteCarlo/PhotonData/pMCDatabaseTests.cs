using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class pMCDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "testpmcdatabase",
            "testpmcdatabase.txt",
            "testpmccollisiondatabase",
            "testpmccollisiondatabase.txt",
            "SimulationInputTest.txt"
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
        /// test to verify that PhotonDatabaseWriter and PhotonDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void Validate_pMCDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            // test serialization
            new SimulationInput().ToFile("SimulationInputTest.txt");

            const string photonDbFilename = "testpmcdatabase";
            const string collisionDbFilename = "testpmccollisiondatabase";
            const int numberOfSubRegions = 3;

            using (var dbWriter = new PhotonDatabaseWriter(
                VirtualBoundaryType.pMCDiffuseReflectance, photonDbFilename))
            {
                using var collisionDbWriter = new CollisionInfoDatabaseWriter(
                    VirtualBoundaryType.pMCDiffuseReflectance, collisionDbFilename, numberOfSubRegions);
                // write data for first photon (exit DP and collision info)
                dbWriter.Write(new PhotonDataPoint(
                    new Position(1, 2, 3),
                    new Direction(0, 0, 1),
                    1.0, // weight
                    10, // time
                    PhotonStateType.None));
                collisionDbWriter.Write(new CollisionInfo(numberOfSubRegions)
                    {
                        new SubRegionCollisionInfo(10.0, 1000),
                        new SubRegionCollisionInfo(20.0, 2000),
                        new SubRegionCollisionInfo(30.0, 3000)
                    });
                // write data for second photon (exit DP and collision info)
                dbWriter.Write(new PhotonDataPoint(
                    new Position(4, 5, 6),
                    new Direction(1, 0, 0),
                    0.50,
                    100,
                    PhotonStateType.None));
                collisionDbWriter.Write(new CollisionInfo(numberOfSubRegions)
                    {
                        new SubRegionCollisionInfo(40.0, 4000),
                        new SubRegionCollisionInfo(50.0, 5000),
                        new SubRegionCollisionInfo(60.0, 6000)
                    });
            }

            // read the database from file, and verify the correct number of photons were written
            var dbCloned = pMCDatabase.FromFile(photonDbFilename, collisionDbFilename);

            Assert.AreEqual(2, dbCloned.DataPoints.Count());

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            using var enumerator = dbCloned.DataPoints.GetEnumerator();

            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            // verify photon database entries for first photon
            Assert.AreEqual(new Position(1, 2, 3), dp1?.PhotonDataPoint.Position);
            Assert.AreEqual(new Direction(0, 0, 1), dp1.PhotonDataPoint.Direction);
            Assert.AreEqual(1.0, dp1.PhotonDataPoint.Weight);
            Assert.AreEqual(10, dp1.PhotonDataPoint.TotalTime);
            Assert.IsTrue(dp1.PhotonDataPoint.StateFlag.HasFlag(PhotonStateType.None));
            // verify collision info for first photon
            Assert.AreEqual(10.0, dp1.CollisionInfo[0].PathLength);
            Assert.AreEqual(1000, dp1.CollisionInfo[0].NumberOfCollisions);
            Assert.AreEqual(20.0, dp1.CollisionInfo[1].PathLength);
            Assert.AreEqual(2000, dp1.CollisionInfo[1].NumberOfCollisions);
            Assert.AreEqual(30.0, dp1.CollisionInfo[2].PathLength);
            Assert.AreEqual(3000, dp1.CollisionInfo[2].NumberOfCollisions);

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            // verify photon database entries for second photon
            var dp2 = enumerator.Current;
            Assert.AreEqual(new Position(4, 5, 6), dp2?.PhotonDataPoint.Position);
            Assert.AreEqual(new Direction(1, 0, 0), dp2.PhotonDataPoint.Direction);
            Assert.AreEqual(0.5, dp2.PhotonDataPoint.Weight);
            Assert.AreEqual(100, dp2.PhotonDataPoint.TotalTime);
            Assert.IsTrue(dp2.PhotonDataPoint.StateFlag.HasFlag(PhotonStateType.None));
            // verify collision info for second photon
            Assert.AreEqual(40.0, dp2.CollisionInfo[0].PathLength);
            Assert.AreEqual(4000, dp2.CollisionInfo[0].NumberOfCollisions);
            Assert.AreEqual(50.0, dp2.CollisionInfo[1].PathLength);
            Assert.AreEqual(5000, dp2.CollisionInfo[1].NumberOfCollisions);
            Assert.AreEqual(60.0, dp2.CollisionInfo[2].PathLength);
            Assert.AreEqual(6000, dp2.CollisionInfo[2].NumberOfCollisions);
        }
    }
}
