using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Extensions
{
    [TestFixture]
    public class PhotonDataExtensionMethodsTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> _listOfTestGeneratedFiles = new List<string>()
        {
            "collisionInfoReflectance",
            "collisionInfoReflectance.txt",
            "collisionInfoReflectance2",
            "collisionInfoReflectance2.txt",
            "collisionInfoPmcReflectance",
            "collisionInfoPmcReflectance.txt",
            "collisionInfoTransmittance",
            "collisionInfoTransmittance.txt"
        };
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void remove_files()
        {
            foreach (var file in _listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// Validate method WriteToPMCSurfaceVirtualBoundaryDatabases.  This in turn
        /// validates WriteToPMCSurfaceVirtualBoundaryDatabase.
        /// </summary>
        [Test]
        public void Validate_WriteToPMCSurfaceVirtualBoundaryDatabases()
        {
            // TearDown should clear files created prior to this test
            const int numberSubRegions = 3;
            var databases = new List<DatabaseType>() { DatabaseType.pMCDiffuseReflectance };
            var collisionInfoDatabases = new List<CollisionInfoDatabaseWriter>
            {
                new CollisionInfoDatabaseWriter(
                    VirtualBoundaryType.pMCDiffuseReflectance,
                    "collisionInfoReflectance2",
                    numberSubRegions)
            };
            var dp = new PhotonDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0, // weight
                1.0, // total time,
                PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary);
            var collisionInfo = new CollisionInfo(numberSubRegions);
            var dbController = new pMCDatabaseWriterController(
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriters(
                    databases, "", ""),
                collisionInfoDatabases);
            dbController.WriteToSurfaceVirtualBoundaryDatabases(dp, collisionInfo);
            Assert.IsTrue(FileIO.FileExists("collisionInfoReflectance2"));
            dbController.Dispose();
        }
        /// <summary>
        /// Validate method BelongsToSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void Validate_BelongsToSurfaceVirtualBoundary_returns_correct_value()
        {
            // set up photon data points with various PhotonStateTypes
            var dpReflectance = new PhotonDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0, // weight
                1.0, // total time,
                PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary);
            var dpTransmittance = new PhotonDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0, // weight
                1.0, // total time,
                PhotonStateType.PseudoDiffuseTransmittanceVirtualBoundary);
            var numberOfSubRegions = 3;
            // set up collision info database writers with various VB types
            var collisionInfoDatabaseWriterReflectance = new CollisionInfoDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, "collisionInfoReflectance", numberOfSubRegions);
            var collisionInfoDatabaseWriterTransmittance = new CollisionInfoDatabaseWriter(
                VirtualBoundaryType.DiffuseTransmittance, "collisionInfoTransmittance", numberOfSubRegions);
            var collisionInfoDatabaseWriterPmcReflectance = new CollisionInfoDatabaseWriter(
                VirtualBoundaryType.pMCDiffuseReflectance, "collisionInfoPmcReflectance", numberOfSubRegions);
            // test various combinations
            // test for tru
            var result = dpReflectance.BelongsToSurfaceVirtualBoundary(collisionInfoDatabaseWriterReflectance);
            Assert.IsTrue(result);
            result = dpTransmittance.BelongsToSurfaceVirtualBoundary(collisionInfoDatabaseWriterTransmittance);
            Assert.IsTrue(result);
            result = dpReflectance.BelongsToSurfaceVirtualBoundary(collisionInfoDatabaseWriterPmcReflectance);
            Assert.IsTrue(result);
            // test for false
            result = dpReflectance.BelongsToSurfaceVirtualBoundary(collisionInfoDatabaseWriterTransmittance);
            Assert.IsFalse(result);
            result = dpTransmittance.BelongsToSurfaceVirtualBoundary(collisionInfoDatabaseWriterPmcReflectance);
            Assert.IsFalse(result);
            collisionInfoDatabaseWriterPmcReflectance.Close();
            collisionInfoDatabaseWriterReflectance.Close();
            collisionInfoDatabaseWriterTransmittance.Close();
        }
        /// <summary>
        /// Validate method IsWithinNA for fully open NA
        /// </summary>
        [Test]
        public void Validate_IsWithinNA_returns_correct_value_when_fully_open()
        {
            const double na = 1.4;
            const double detectorRegionN = 1.4;
            var direction = new Direction(0, 0, -1); // straight up
            var dp = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            bool result = dp.IsWithinNA(na, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(true, result);
        }
        /// <summary>
        /// Validate IsWithinNA for partially open NA
        /// </summary>
        [Test]
        public void Validate_IsWithinNA_returns_correct_value_when_partially_open()
        {
            const double na = 0.22;
            const double detectorRegionN = 1.4;
            var direction = new Direction(0, 0, -1); // straight up
            var dp = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            var result = dp.IsWithinNA(na, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(true, result);
            // now select direction right on NA
            var theta = Math.Asin(na / detectorRegionN);
            direction = new Direction(Math.Sin(theta), 0, -Math.Cos(theta)); // right on NA
            dp = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            result = dp.IsWithinNA(na, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(true, result);
            // now select direction outside of NA
            theta = Math.Asin( (na * (1.1)) / detectorRegionN);
            direction = new Direction(Math.Sin(theta), 0, -Math.Cos(theta)); 
            dp = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            result = dp.IsWithinNA(na, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(false, result);
        }
        /// <summary>
        /// Validate method agrees with Bargo theory [Bargo et al., AO 42(16) 2003]
        /// 
        /// </summary>
        [Test]
        public void Validate_IsWithinNA_returns_correct_eta_c_value_per_Bargo()
        {
            var rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                RandomNumberGeneratorType.MersenneTwister, 0);
            const double na0P22 = 0.22;
            const double na0P39 = 0.39;
            const double naOpen = double.PositiveInfinity;
            double na0P22Count = 0;
            double na0P39Count = 0;
            double naOpenCount = 0;
            var detectorRegionN = 1.4;
            for (var i = 0; i < 1000000; i++)
            {
                var randomUz = -rng.NextDouble();
                var direction = new Direction(0, 0, randomUz);
                var dp = new PhotonDataPoint(
                    new Position(0, 0, 0),
                    direction,
                    1.0, // weight: not used
                    1.0, // photon time of flight: note used
                    PhotonStateType.Alive);
                if (dp.IsWithinNA(na0P22, Direction.AlongNegativeZAxis, detectorRegionN))
                {
                    na0P22Count += randomUz;
                }
                if (dp.IsWithinNA(na0P39, Direction.AlongNegativeZAxis, detectorRegionN))
                {
                    na0P39Count += randomUz;
                }
                if (dp.IsWithinNA(naOpen, Direction.AlongNegativeZAxis, detectorRegionN))
                {
                    naOpenCount += randomUz;
                }
            }
            var etaC = na0P22Count / naOpenCount;
            Assert.Less(Math.Abs(etaC - na0P22/detectorRegionN*(na0P22/detectorRegionN)), 0.001);
            etaC = na0P39Count / naOpenCount;
            Assert.Less(Math.Abs(etaC - na0P39 / detectorRegionN * (na0P39 / detectorRegionN)), 0.001);
        }
    }
}

