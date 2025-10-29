using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Sources;
using Vts.Common;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Sources
{
    /// <summary>
    /// Unit tests for From Photon Database File Source
    /// </summary>
    [TestFixture]
    public class PhotonDatabaseFileSourceTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        private readonly List<string> _listOfTestGeneratedFiles =
        [
            "inputFromFile.txt",
            "testphotondatabase",
            "testphotondatabase.txt"
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
        /// set up MCCL PhotonDatabase to be used as a source
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            const string databaseFileName = "testphotondatabase";
            var firstRayDp = new PhotonDataPoint(
                    new Position(1, 1, 0),
                    new Direction(0, 1 / Math.Sqrt(2), -1 / Math.Sqrt(2)),
                    0.1,
                    0.0, // Photon does not include total time
                    PhotonStateType.Alive); // Alive is default
            var secondRayDp = new PhotonDataPoint(
                    new Position(2, 2, 0),
                    new Direction(1 / Math.Sqrt(2), 0, -1 / Math.Sqrt(2)),
                    0.2,
                    0.0,
                    PhotonStateType.Alive);
            using var dbWriter = new PhotonDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFileName);
            dbWriter.Write(firstRayDp);
            dbWriter.Write(secondRayDp);
            dbWriter.Close();
        }

        /// <summary>
        /// Validate PhotonFileSource reads data from file correctly
        /// </summary>
        [Test]
        public void validate_PhotonFileSource_reads_database_correctly()
        {
            var source = new PhotonDatabaseFileSource("testphotondatabase", 0);
            var tissue = new MultiLayerTissue();
            var dp = source.GetNextPhoton(tissue);
            Assert.That(dp.DP.Position.X, Is.EqualTo(1.0));
            Assert.That(dp.DP.Position.Y, Is.EqualTo(1.0));
            Assert.That(dp.DP.Position.Z, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Ux, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Uy, Is.EqualTo(1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.Direction.Uz, Is.EqualTo(-1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.TotalTime, Is.EqualTo(0.0));
            Assert.That(dp.DP.StateFlag, Is.EqualTo(PhotonStateType.Alive));
            dp = source.GetNextPhoton(tissue);
            Assert.That(dp.DP.Position.X, Is.EqualTo(2.0));
            Assert.That(dp.DP.Position.Y, Is.EqualTo(2.0));
            Assert.That(dp.DP.Position.Z, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Ux, Is.EqualTo(1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.Direction.Uy, Is.EqualTo(0.0));
            Assert.That(dp.DP.Direction.Uz, Is.EqualTo(-1.0 / Math.Sqrt(2)));
            Assert.That(dp.DP.TotalTime, Is.EqualTo(0.0));
            Assert.That(dp.DP.StateFlag, Is.EqualTo(PhotonStateType.Alive));
            source.DatabaseEnumerator.Dispose();
        }

    }
}