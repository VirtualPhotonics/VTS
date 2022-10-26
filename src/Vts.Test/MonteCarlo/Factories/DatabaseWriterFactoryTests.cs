using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;
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
            Assert.IsInstanceOf<PhotonDatabaseWriter>(diffuseReflectanceDb);
            diffuseReflectanceDb.Close();
            var diffuseTransmittanceDb =
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.DiffuseTransmittance,
                    Directory.GetCurrentDirectory(), "");
            Assert.IsInstanceOf<PhotonDatabaseWriter>(diffuseTransmittanceDb);
            diffuseTransmittanceDb.Close();
            var specularDb = DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                DatabaseType.SpecularReflectance,
                Directory.GetCurrentDirectory(), "");
            Assert.IsInstanceOf<PhotonDatabaseWriter>(specularDb);
            specularDb.Close();
            var pMcDiffuseReflectanceDb = DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                DatabaseType.pMCDiffuseReflectance,
                Directory.GetCurrentDirectory(), "");
            Assert.IsInstanceOf<PhotonDatabaseWriter>(pMcDiffuseReflectanceDb);
            pMcDiffuseReflectanceDb.Close();
            var pMcDiffuseTransmittanceDb = DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                DatabaseType.pMCDiffuseTransmittance,
                Directory.GetCurrentDirectory(), "");
            Assert.IsInstanceOf<PhotonDatabaseWriter>(pMcDiffuseTransmittanceDb);
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
            Assert.IsNull(diffuseReflectanceDb);
            var diffuseTransmittanceDb =
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                    DatabaseType.DiffuseTransmittance,
                    tissue,
                    Directory.GetCurrentDirectory(), 
                    "");
            Assert.IsNull(diffuseTransmittanceDb);
            var specularDb = 
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                DatabaseType.SpecularReflectance,
                tissue,
                Directory.GetCurrentDirectory(), 
                "");
            Assert.IsNull(specularDb);
            var pMcDiffuseReflectanceDb = 
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                DatabaseType.pMCDiffuseReflectance,
                tissue,
                Directory.GetCurrentDirectory(), 
                "");
            Assert.IsInstanceOf<CollisionInfoDatabaseWriter>(pMcDiffuseReflectanceDb);
            pMcDiffuseReflectanceDb.Close();
            var pMcDiffuseTransmittanceDb = 
                DatabaseWriterFactory.GetCollisionInfoDatabaseWriter(
                DatabaseType.pMCDiffuseTransmittance,
                tissue,
                Directory.GetCurrentDirectory(), 
                "");
            Assert.IsInstanceOf<CollisionInfoDatabaseWriter>(pMcDiffuseTransmittanceDb);
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
    }
}
