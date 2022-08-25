using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Zemax;

namespace Vts.Test.MonteCarlo.Zemax
{
    [TestFixture]
    public class ZRDRayDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "testzrdraydatabase",
            "testzrdraydatabase.txt",
        };
        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// test to verify ZRDRayDatabaseWriter and  ZRDRayDatabase.FromFile are working correctly
        /// </summary>
        [Test]
        public void Validate_ZRDRayDatabase_writing_and_reading_is_correct()
        {
            string databaseFileName = "testzrdraydatabase";
            var firstRayDP = new ZrdRayDataPoint()
            {
                X = 1, Y = 2, Z = 3,
                Ux = 0, Uy = 1/ Math.Sqrt(2), Uz = 1/Math.Sqrt(2),
                Weight = 1
            };
            var secondRayDP = new ZrdRayDataPoint()
            {
                X = 4, Y = 5, Z = 6,
                Ux = 0, Uy = 0, Uz = 1,
                Weight = 0.555
            };
            using (var dbWriter = new ZrdRayDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFileName))
            {
                dbWriter.Write(firstRayDP);
                dbWriter.Write(secondRayDP);
            }
            // read back file written
            var rayDatabase = ZrdRayDatabase.FromFile(databaseFileName);
            Assert.AreEqual(2, rayDatabase.NumberOfElements);

            // manually enumerate through the first two elements 
            // in memory, so .ToArray() loses the benefits of the lazy-load data points
            var enumerator = rayDatabase.DataPoints.GetEnumerator();
            // advance to the first point and test that the point is valid
            enumerator.MoveNext();
            var dp1 = enumerator.Current;
            Assert.IsTrue(dp1.X == firstRayDP.X);
            Assert.IsTrue(dp1.Y == firstRayDP.Y);
            Assert.IsTrue(dp1.Z == firstRayDP.Z);
            Assert.IsTrue(dp1.Ux == firstRayDP.Ux);
            Assert.IsTrue(dp1.Uy == firstRayDP.Uy);
            Assert.IsTrue(dp1.Uz == firstRayDP.Uz);
            Assert.IsTrue(dp1.Weight == firstRayDP.Weight);
            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            Assert.IsTrue(dp2.X == secondRayDP.X);
            Assert.IsTrue(dp2.Y == secondRayDP.Y);
            Assert.IsTrue(dp2.Z == secondRayDP.Z);
            Assert.IsTrue(dp2.Ux == secondRayDP.Ux);
            Assert.IsTrue(dp2.Uy == secondRayDP.Uy);
            Assert.IsTrue(dp2.Uz == secondRayDP.Uz);
            Assert.IsTrue(dp2.Weight == secondRayDP.Weight);
        }
    }
}