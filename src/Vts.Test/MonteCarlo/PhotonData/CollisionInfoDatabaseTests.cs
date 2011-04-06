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
    public class CollisionInfoDatabaseTests
    {
        /// <summary>
        /// test to verify that CollisionInfoDatabaseWriter and CollisionInfoDatabase.FromFile
        /// are working correctly.
        /// </summary>
        [Test]
        public void validate_CollisionInfo_deserialized_class_is_correct_when_using_WriteToFile()
        {
            int numberOfSubregions = 2;
            var dbWriter = new CollisionInfoDatabaseWriter("testcollisioninfodatabase", numberOfSubregions);

            var db = new CollisionInfo(numberOfSubregions)
            { 
                new SubRegionCollisionInfo(10, 1000),
                new SubRegionCollisionInfo(20, 2000)
            };

            if (dbWriter != null)
            {
                dbWriter.Write(db);
            }
            if (dbWriter != null) dbWriter.Dispose();

            var dbcloned = CollisionInfoDatabase.FromFile("testcollisioninfodatabase");

            Assert.AreEqual(dbcloned.NumberOfSubRegions, 2);
            Assert.AreEqual(dbcloned.NumberOfPhotons, 2);
            var dps = dbcloned.DataPoints.ToArray();
            //Assert.AreEqual(dps[0].
 
        }
    }

}
