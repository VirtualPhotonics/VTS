using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.RayData;
using Vts.MonteCarlo.Sources;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for From Ray Illumination Database Source
    /// </summary>
    [TestFixture]
    public class RayIlluminationDatabaseSourceTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles =
        [
            "inputFromFile.txt",
            "testraydatabase",
            "testraydatabase.txt"
        ];

        /// <summary>
        /// clear all generated folders and files
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// set up MCCL RayDatabase to be used as a source
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            const string databaseFileName = "testraydatabase";
            var firstRayDp = new RayDataPoint(
                1.0, 1.0, 0.0,
                0.0, 1 / Math.Sqrt(2), -1 / Math.Sqrt(2),
                0.1);
            var secondRayDp = new RayDataPoint(
                    2.0, 2.0, 0.0,
                    1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2),
                    0.2);
            using var dbWriter = new RayDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFileName);
            dbWriter.Write(firstRayDp);
            dbWriter.Write(secondRayDp);
            dbWriter.Close();
        }

        /// <summary>
        /// Validate RayIlluminationDatabaseSource reads data from file correctly
        /// </summary>
        [Test]
        public void validate_RayIlluminationDatabaseSource_reads_database_correctly()
        {
            var source = new RayIlluminationDatabaseSource("testraydatabase", 0);
            var tissue = new MultiLayerTissue();
            var dp = source.GetNextPhoton(tissue);
            Assert.That(dp.DP.Position.X, Is.EqualTo(1.0));
            Assert.That(dp.DP.Position.Y, Is.EqualTo(1.0));
            Assert.That(dp.DP.Position.Z, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Ux, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Uy, Is.EqualTo(1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.Direction.Uz, Is.EqualTo(-1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.Weight, Is.EqualTo(0.1));
            dp = source.GetNextPhoton(tissue);
            Assert.That(dp.DP.Position.X, Is.EqualTo(2.0));
            Assert.That(dp.DP.Position.Y, Is.EqualTo(2.0));
            Assert.That(dp.DP.Position.Z, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Ux, Is.EqualTo(1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.Direction.Uy, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Uz, Is.EqualTo(-1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.Weight, Is.EqualTo(0.2));
            source.DatabaseEnumerator.Dispose();
        }

    }
}