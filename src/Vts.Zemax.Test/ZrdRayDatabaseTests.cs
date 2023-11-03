using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;

namespace Vts.Zemax.Test
{
    [TestFixture]
    public class ZrdRayDatabaseTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles = new()
        {
            "testzrdraydatabase",
            "testzrdraydatabase.txt",
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
        /// test to verify ZRDRayDatabaseWriter and  ZRDRayDatabase.FromFile are working correctly
        /// </summary>
        [Test]
        public void Validate_ZrdRayDatabase_writing_and_reading_is_correct()
        {
            const string databaseFileName = "testzrdraydatabase";
            var firstRayDp = new ZrdRayDataPoint()
            {
                X = 1,
                Y = 2,
                Z = 3,
                Ux = 0,
                Uy = 1 / Math.Sqrt(2),
                Uz = 1 / Math.Sqrt(2),
                Weight = 1
            };
            var secondRayDp = new ZrdRayDataPoint()
            {
                X = 4,
                Y = 5,
                Z = 6,
                Ux = 0,
                Uy = 0,
                Uz = 1,
                Weight = 0.555
            };
            using (var dbWriter = new ZrdRayDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFileName))
            {
                dbWriter.Write(firstRayDp);
                dbWriter.Write(secondRayDp);
                dbWriter.Close();
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
            if (dp1 != null)
            {
                Assert.IsTrue(dp1.X == firstRayDp.X);
                Assert.IsTrue(dp1.Y == firstRayDp.Y);
                Assert.IsTrue(dp1.Z == firstRayDp.Z);
                Assert.IsTrue(dp1.Ux == firstRayDp.Ux);
                Assert.IsTrue(dp1.Uy == firstRayDp.Uy);
                Assert.IsTrue(dp1.Uz == firstRayDp.Uz);
                Assert.IsTrue(dp1.Weight == firstRayDp.Weight);
            }

            // advance to the second point and test that the point is valid
            enumerator.MoveNext();
            var dp2 = enumerator.Current;
            if (dp2 == null) return;
            Assert.IsTrue(dp2.X == secondRayDp.X);
            Assert.IsTrue(dp2.Y == secondRayDp.Y);
            Assert.IsTrue(dp2.Z == secondRayDp.Z);
            Assert.IsTrue(dp2.Ux == secondRayDp.Ux);
            Assert.IsTrue(dp2.Uy == secondRayDp.Uy);
            Assert.IsTrue(dp2.Uz == secondRayDp.Uz);
            Assert.IsTrue(dp2.Weight == secondRayDp.Weight);

            enumerator.Dispose();
        }
    }
}