using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Test.MonteCarlo.Extensions
{
    [TestFixture]
    public class PhotonDataExtensionMethodsTests
    {
        [OneTimeTearDown]
        public void remove_files_created()
        {
            FileIO.FileDelete("collisionInfoReflectance");
            FileIO.FileDelete("collisionInfoTransmittance");
            FileIO.FileDelete("collisionInfoPmcReflectance");
        }
        /// <summary>
        /// Validate method WriteToPMCSurfaceVirtualBoundaryDatabases.  This in turnb
        /// validates WriteToPMCSurfaceVirtualBoundaryDatabase
        /// </summary>
        [Test]
        public void validate_WriteToPMCSurfaceVirtualBoundaryDatabases()
        {
            // OneTimeTearDown might not run before this test so clear our files
            FileIO.FileDelete("collisionInfoReflectance");
            FileIO.FileDelete("collisionInfoPmcReflectance");
            var numberOfSubRegions = 3;
            var listOfCollisionInfoDatabaseWriters = new List<CollisionInfoDatabaseWriter>();
            listOfCollisionInfoDatabaseWriters.Add(new CollisionInfoDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, 
                "collisionInfoReflectance",
                numberOfSubRegions));
            listOfCollisionInfoDatabaseWriters.Add(new CollisionInfoDatabaseWriter(
                VirtualBoundaryType.pMCDiffuseReflectance,
                "collisionInfoPmcReflectance",
                numberOfSubRegions));
            var dp = new PhotonDataPoint(
                new Position(0, 0, 0),
                new Direction(0, 0, 1),
                1.0, // weight
                1.0, // total time,
                PhotonStateType.PseudoDiffuseReflectanceVirtualBoundary);
            var collisionInfo = new CollisionInfo(
                numberOfSubRegions);
            listOfCollisionInfoDatabaseWriters.WriteToPMCSurfaceVirtualBoundaryDatabases(
                dp,
                collisionInfo);
            Assert.IsTrue(FileIO.FileExists("collisionInfoReflectance"));
            Assert.IsTrue(FileIO.FileExists("collisionInfoPmcReflectance"));
        }
        /// <summary>
        /// Validate method BelongsToSurfaceVirtualBoundary
        /// </summary>
        [Test]
        public void validate_BelongsToSurfaceVirtualBoundary_returns_correct_value()
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
        }
        /// <summary>
        /// Validate method IsWithinNA for fully open NA
        /// </summary>
        [Test]
        public void validate_IsWithinNA_returns_correct_value_when_fully_open()
        {
            var NA = 1.4;
            var detectorRegionN = 1.4;
            var direction = new Direction(0, 0, -1); // straight up
            var DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            bool result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, true);
        }
        /// <summary>
        /// Validate IsWithinNA for partially open NA
        /// </summary>
        [Test]
        public void validate_IsWithinNA_returns_correct_value_when_partially_open()
        {
            var NA = 0.22;
            var detectorRegionN = 1.4;
            var direction = new Direction(0, 0, -1); // straight up
            var DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            bool result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, true);
            // now select direction right on NA
            var theta = Math.Asin(NA / detectorRegionN);
            direction = new Direction(Math.Sin(theta), 0, -Math.Cos(theta)); // right on NA
            DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, true);
            // now select direction outside of NA
            theta = Math.Asin( (NA * (1.1)) / detectorRegionN);
            direction = new Direction(Math.Sin(theta), 0, -Math.Cos(theta)); 
            DP = new PhotonDataPoint(
                new Position(0, 0, 0),
                direction,
                1.0, // weight: not used
                1.0, // photon time of flight: note used
                PhotonStateType.Alive);
            result = DP.IsWithinNA(NA, Direction.AlongNegativeZAxis, detectorRegionN);
            Assert.AreEqual(result, false);
        }
        /// <summary>
        /// Validate method agrees with Bargo theory [Bargo et al., AO 42(16) 2003]
        /// 
        /// </summary>
        [Test]
        public void validate_IsWithinNA_returns_correct_eta_c_value_per_Bargo()
        {
            Random rng;
            rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                RandomNumberGeneratorType.MersenneTwister, 0);
            double NA0p22 = 0.22;
            double NA0p39 = 0.39;
            double NAOpen = Double.PositiveInfinity;
            double NA0p22_count = 0;
            double NA0p39_count = 0;
            double NAOpen_count = 0;
            double detectorRegionN = 1.4;
            for (int i = 0; i < 1000000; i++)
            {
                double randomUz = -rng.NextDouble();
                var direction = new Direction(0, 0, randomUz);
                var DP = new PhotonDataPoint(
                    new Position(0, 0, 0),
                    direction,
                    1.0, // weight: not used
                    1.0, // photon time of flight: note used
                    PhotonStateType.Alive);
                if (DP.IsWithinNA(NA0p22, Direction.AlongNegativeZAxis, detectorRegionN))
                {
                    NA0p22_count = NA0p22_count + randomUz;
                }
                if (DP.IsWithinNA(NA0p39, Direction.AlongNegativeZAxis, detectorRegionN))
                {
                    NA0p39_count = NA0p39_count + randomUz;
                }
                if (DP.IsWithinNA(NAOpen, Direction.AlongNegativeZAxis, detectorRegionN))
                {
                    NAOpen_count = NAOpen_count + randomUz;
                }
            }
            double eta_c = NA0p22_count / NAOpen_count;
            Assert.Less(Math.Abs(eta_c - (NA0p22/detectorRegionN)*(NA0p22/detectorRegionN)), 0.001);
            eta_c = NA0p39_count / NAOpen_count;
            Assert.Less(Math.Abs(eta_c - (NA0p39 / detectorRegionN) * (NA0p39 / detectorRegionN)), 0.001);
        }
    }
}

