using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.PhotonData
{
    [TestFixture]
    public class pMCDatabaseTests
    {
        /// <summary>
        /// test to verify that PhotonDatabaseWriter and PhotonDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void validate_pMCDatabase_deserialized_class_is_correct_when_using_WriteToFile()
        {
            // test serialization
            new SimulationInput().ToFile("SimulationInputTest.xml");

            string photonDbFilename = "testpmcdatabase";
            string collisionDbFilename = "testpmccollisiondatabase";
            int numberOfSubregions = 3;

            using(var dbWriter = new PhotonDatabaseWriter(photonDbFilename))
            {
                using (var collisionDbWriter = new CollisionInfoDatabaseWriter(
                    collisionDbFilename, numberOfSubregions))
                    {
                        // write data for first photon (exit DP and collision info)
                        dbWriter.Write(new PhotonDataPoint(
                                           new Position(1, 2, 3),
                                           new Direction(0, 0, 1),
                                           1.0, // weight
                                           10, // time
                                           PhotonStateType.PseudoTransmissionDomainBottomBoundary));
                        collisionDbWriter.Write(new CollisionInfo(numberOfSubregions)
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
                                                   PhotonStateType.PseudoTransmissionDomainTopBoundary));
                        collisionDbWriter.Write(new CollisionInfo(numberOfSubregions)
                                {
                                    new SubRegionCollisionInfo(40.0, 4000),
                                    new SubRegionCollisionInfo(50.0, 5000),
                                    new SubRegionCollisionInfo(60.0, 6000)
                                });
                    }
            }
 
            // read the database from file, and verify the correct number of photons were written
            var dbCloned = pMCDatabase.FromFile(photonDbFilename, collisionDbFilename);

            Assert.AreEqual(dbCloned.DataPoints.Count(), 2);

            // manually enumerate through the first two elements (same as foreach)
            // PhotonDatabase is designed so you don't have to have the whole thing
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = dbCloned.DataPoints.GetEnumerator();

            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            // verify photon database entries for first photon
            Assert.AreEqual(dp1.PhotonDataPoint.Position, new Position(1, 2, 3));
            Assert.AreEqual(dp1.PhotonDataPoint.Direction, new Direction(0, 0, 1));
            Assert.AreEqual(dp1.PhotonDataPoint.Weight, 1.0);
            Assert.AreEqual(dp1.PhotonDataPoint.TotalTime, 10);
            Assert.AreEqual(dp1.PhotonDataPoint.StateFlag, PhotonStateType.PseudoTransmissionDomainBottomBoundary);
            // verify collision info for first photon
            Assert.AreEqual(dp1.CollisionInfo[0].PathLength, 10.0);
            Assert.AreEqual(dp1.CollisionInfo[0].NumberOfCollisions, 1000);
            Assert.AreEqual(dp1.CollisionInfo[1].PathLength, 20.0);
            Assert.AreEqual(dp1.CollisionInfo[1].NumberOfCollisions, 2000);
            Assert.AreEqual(dp1.CollisionInfo[2].PathLength, 30.0);
            Assert.AreEqual(dp1.CollisionInfo[2].NumberOfCollisions, 3000);

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            // verify photon database entries for second photon
            var dp2 = enumerator.Current;
            Assert.AreEqual(dp2.PhotonDataPoint.Position, new Position(4, 5, 6));
            Assert.AreEqual(dp2.PhotonDataPoint.Direction, new Direction(1, 0, 0));
            Assert.AreEqual(dp2.PhotonDataPoint.Weight, 0.5);
            Assert.AreEqual(dp2.PhotonDataPoint.TotalTime, 100);
            Assert.AreEqual(dp2.PhotonDataPoint.StateFlag, PhotonStateType.PseudoTransmissionDomainTopBoundary);
            // verify collision info for second photon
            Assert.AreEqual(dp2.CollisionInfo[0].PathLength, 40.0);
            Assert.AreEqual(dp2.CollisionInfo[0].NumberOfCollisions, 4000);
            Assert.AreEqual(dp2.CollisionInfo[1].PathLength, 50.0);
            Assert.AreEqual(dp2.CollisionInfo[1].NumberOfCollisions, 5000);
            Assert.AreEqual(dp2.CollisionInfo[2].PathLength, 60.0);
            Assert.AreEqual(dp2.CollisionInfo[2].NumberOfCollisions, 6000);
        }
    }
}
