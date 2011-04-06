using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Sources
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
            var dbWriter = new PhotonDatabaseWriter("testphotondatabase");
                
            var db = new PhotonDatabase()
            { 
                NumberOfPhotons = 2, 
                DataPoints = new List<PhotonDataPoint>()
                {
                    new PhotonDataPoint( 
                        new Position(1, 2, 3), 
                        new Direction(0, 0, 1),
                        1.0,  // weight
                        10,  // time
                        PhotonStateType.ExitedOutBottom),
                    new PhotonDataPoint(
                        new Position(4, 5, 6),
                        new Direction(1, 0, 0),
                        0.50,
                        100,
                        PhotonStateType.ExitedOutTop)
                }
            };

            if (dbWriter != null)
            {
                dbWriter.Write(db.DataPoints);
            }
            if (dbWriter != null) dbWriter.Dispose();

            var dbcloned = PhotonDatabase.FromFile("testphotondatabase");

            Assert.AreEqual(dbcloned.NumberOfPhotons, 2);
            var dps = dbcloned.DataPoints.ToArray();
            Assert.AreEqual(dps[0].Position, new Position(1, 2, 3));
            Assert.AreEqual(dps[0].Direction, new Direction(0, 0, 1));
            Assert.AreEqual(dps[0].Weight, 1.0);
            Assert.AreEqual(dps[0].TotalTime, 10);
            Assert.AreEqual(dps[0].StateFlag, PhotonStateType.ExitedOutBottom);
            Assert.AreEqual(dps[1].Position, new Position(4, 5, 6));
            Assert.AreEqual(dps[1].Direction, new Direction(1, 0, 0));
            Assert.AreEqual(dps[1].Weight, 0.5);
            Assert.AreEqual(dps[1].TotalTime, 100);
            Assert.AreEqual(dps[1].StateFlag, PhotonStateType.ExitedOutTop);
        }
    }

}
