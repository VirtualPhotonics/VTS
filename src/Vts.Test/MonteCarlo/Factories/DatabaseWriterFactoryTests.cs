using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.RayData;
using Vts.MonteCarlo.Tissues;

namespace Vts.Test.MonteCarlo.Factories
{
    /// <summary>
    /// Unit tests for DatabaseWriterFactory
    /// </summary>
    [TestFixture]
    public class DatabaseWriterFactoryTests
    {
        /// <summary>
        /// list of temporary files created by these unit tests
        /// </summary>
        readonly List<string> _listOfTestGeneratedFiles = new List<string>()
        {
            "DiffuseReflectanceDatabase",
            "DiffuseReflectanceDatabase.txt",
            "DiffuseTransmittanceDatabase",
            "DiffuseTransmittanceDatabase.txt",
            "SpecularReflectanceDatabase",
            "SpecularReflectanceDatabase.txt",
            "CollisionInfoDatabase",
            "CollisionInfoDatabase.txt",
            "CollisionInfoTransmittanceDatabase",
            "CollisionInfoTransmittanceDatabase.txt"
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
        /// Test DatabaseWriterFactory GetSurfaceVirtualBoundaryDatabaseWriter method
        /// </summary>
        [Test]
        public void Demonstrate_GetSurfaceVirtualBoundaryDatabaseWriter_successful_return()
        {
            var diffuseReflectanceDb =
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.DiffuseReflectance,
                    Directory.GetCurrentDirectory(), "");
            Assert.That(diffuseReflectanceDb, Is.InstanceOf<PhotonDatabaseWriter>());
            diffuseReflectanceDb.Close();
            var diffuseTransmittanceDb =
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.DiffuseTransmittance,
                    Directory.GetCurrentDirectory(), "");
            Assert.That(diffuseTransmittanceDb, Is.InstanceOf<PhotonDatabaseWriter>());
            diffuseTransmittanceDb.Close();
            var specularDb = DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                DatabaseType.SpecularReflectance,
                Directory.GetCurrentDirectory(), "");
            Assert.That(specularDb, Is.InstanceOf<PhotonDatabaseWriter>());
            specularDb.Close();
            var pMcDiffuseReflectanceDb = DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                DatabaseType.pMCDiffuseReflectance,
                Directory.GetCurrentDirectory(), "");
            Assert.That(pMcDiffuseReflectanceDb, Is.InstanceOf<PhotonDatabaseWriter>());
            pMcDiffuseReflectanceDb.Close();
            var pMcDiffuseTransmittanceDb = DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                DatabaseType.pMCDiffuseTransmittance,
                Directory.GetCurrentDirectory(), "");
            Assert.That(pMcDiffuseTransmittanceDb, Is.InstanceOf<PhotonDatabaseWriter>());
            pMcDiffuseTransmittanceDb.Close();
            // check if enum set to something out of range
            var fakeDatabaseType = (DatabaseType)Enum.GetNames(typeof(DatabaseType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    fakeDatabaseType, Directory.GetCurrentDirectory(), ""));
        }
        /// <summary>
        /// Test DatabaseWriterFactory GetCollisionInfoDatabaseWriter method
        /// </summary>
        [Test]
        public void Demonstrate_GetCollisionInfoDatabaseWriter_successful_return()
        {
            var tissue = new MultiLayerTissue();
            var diffuseReflectanceDb =
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                    DatabaseType.DiffuseReflectance,
                    tissue,
                    Directory.GetCurrentDirectory(), 
                    "");
            Assert.That(diffuseReflectanceDb, Is.Null);
            var diffuseTransmittanceDb =
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                    DatabaseType.DiffuseTransmittance,
                    tissue,
                    Directory.GetCurrentDirectory(), 
                    "");
            Assert.That(diffuseTransmittanceDb, Is.Null);
            var specularDb = 
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                DatabaseType.SpecularReflectance,
                tissue,
                Directory.GetCurrentDirectory(), 
                "");
            Assert.That(specularDb, Is.Null);
            var pMcDiffuseReflectanceDb = 
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                DatabaseType.pMCDiffuseReflectance,
                tissue,
                Directory.GetCurrentDirectory(), 
                "");
            Assert.That(pMcDiffuseReflectanceDb, Is.InstanceOf<CollisionInfoDatabaseWriter>());
            pMcDiffuseReflectanceDb.Close();
            var pMcDiffuseTransmittanceDb = 
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                DatabaseType.pMCDiffuseTransmittance,
                tissue,
                Directory.GetCurrentDirectory(), 
                "");
            Assert.That(pMcDiffuseTransmittanceDb, Is.InstanceOf<CollisionInfoDatabaseWriter>());
            pMcDiffuseTransmittanceDb.Close();
            // check if enum set to something out of range
            var fakeDatabaseType = (DatabaseType)Enum.GetNames(typeof(DatabaseType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                    fakeDatabaseType, 
                    tissue,
                    Directory.GetCurrentDirectory(), 
                    ""));
        }
        /// <summary>
        /// Test DatabaseWriterFactory GetRaySurfaceVirtualBoundaryDatabaseWriter method
        /// </summary>
        [Test]
        public void Demonstrate_GetRaySurfaceVirtualBoundaryDatabaseWriter_successful_return()
        {
            // following are not RaySurfaceVBs
            var diffuseReflectanceDb =
                DatabaseWriterFactory.GetRaySurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.DiffuseReflectance,
                    Directory.GetCurrentDirectory(), "");
            Assert.That(diffuseReflectanceDb, Is.Null);
            var diffuseTransmittanceDb =
                DatabaseWriterFactory.GetRaySurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.DiffuseTransmittance,
                    Directory.GetCurrentDirectory(), "");
            Assert.That(diffuseTransmittanceDb, Is.Null);
            var specularDb = 
                DatabaseWriterFactory.GetRaySurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.SpecularReflectance,
                Directory.GetCurrentDirectory(), "");
            Assert.That(specularDb, Is.Null);
            var pMcDiffuseReflectanceDb =
                DatabaseWriterFactory.GetRaySurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.pMCDiffuseReflectance,
                Directory.GetCurrentDirectory(), "");
            Assert.That(pMcDiffuseReflectanceDb, Is.Null);
            var pMcDiffuseTransmittanceDb = 
                DatabaseWriterFactory.GetRaySurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.pMCDiffuseTransmittance,
                Directory.GetCurrentDirectory(), "");
            Assert.That(pMcDiffuseTransmittanceDb, Is.Null);
            // actual RaySurfaceVB
            var rayDiffuseReflectanceDb = 
                DatabaseWriterFactory.GetRaySurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.RayDiffuseReflectance,
                Directory.GetCurrentDirectory(), "");
            Assert.That(rayDiffuseReflectanceDb, Is.InstanceOf<RayDatabaseWriter>());
            rayDiffuseReflectanceDb.Close();
            // check if enum set to something out of range
            var fakeDatabaseType = (DatabaseType)Enum.GetNames(typeof(DatabaseType)).Length + 1;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => DatabaseWriterFactory.GetRaySurfaceVirtualBoundaryDatabaseWriter(
                    fakeDatabaseType, Directory.GetCurrentDirectory(), ""));
        }
    }
}
