using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.RayData;
using Vts.MonteCarlo.Sources;
using Vts.Common;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for From File Source
    /// </summary>
    [TestFixture]
    public class RayFileSourceTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        List<string> listOfTestGeneratedFolders = new List<string>()
        {
        };
        List<string> listOfTestGeneratedFiles = new List<string>()
        {
            "inputFromFile.txt",
            "testraydatabase",
            "testraydatabase.txt"
        };
        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
            foreach (var folder in listOfTestGeneratedFolders)
            {
                FileIO.DeleteDirectory(folder);
            }
        }
        /// <summary>
        /// set up MCCL RayDatabase
        /// </summary>
        [OneTimeSetUp]
        public void setup()
        {
            string databaseFileName = "testraydatabase";
            var firstRayDP = new RayDataPoint(
                    new Position(1, 1, 0),
                    new Direction(0, 1 / Math.Sqrt(2), -1 / Math.Sqrt(2)),
                    1.0);
            var secondRayDP = new RayDataPoint(
                    new Position(2, 2, 0),
                    new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2)),
                    2.0);
            using (var dbWriter = new RayDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFileName))
            {
                dbWriter.Write(firstRayDP);
                dbWriter.Write(secondRayDP);
            }
        }

        /// <summary>
        /// Validate RayFileSource reads data from file correctly
        /// </summary>
        [Test]
        public void validate_RayFileSource_reads_database_correctly()
        {
            var source = new RayFileSource("testraydatabase", 0);
            var tissue = new MultiLayerTissue();
            Photon dp = source.GetNextPhoton(tissue);
            Assert.IsTrue(dp.DP.Position.X == 1.0);
            Assert.IsTrue(dp.DP.Position.Y == 1.0);
            Assert.IsTrue(dp.DP.Position.Z == 0.0);
            Assert.IsTrue(dp.DP.Direction.Ux == 0.0);
            Assert.IsTrue(dp.DP.Direction.Uy == 1.0 / Math.Sqrt(2));
            Assert.IsTrue(dp.DP.Direction.Uz == -1.0 / Math.Sqrt(2));
            dp = source.GetNextPhoton(tissue);
            Assert.IsTrue(dp.DP.Position.X == 2.0);
            Assert.IsTrue(dp.DP.Position.Y == 2.0);
            Assert.IsTrue(dp.DP.Position.Z == 0.0);
            Assert.IsTrue(dp.DP.Direction.Ux == 1.0 / Math.Sqrt(2));
            Assert.IsTrue(dp.DP.Direction.Uy == 0.0);
            Assert.IsTrue(dp.DP.Direction.Uz == -1.0 / Math.Sqrt(2));
        }
       
    }
}