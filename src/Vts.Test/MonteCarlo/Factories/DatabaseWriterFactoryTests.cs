using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Vts.Common;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;

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
        readonly List<string> listOfTestGeneratedFiles = new List<string>()
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
            foreach (var file in listOfTestGeneratedFiles)
            {
                FileIO.FileDelete(file);
            }
        }
        /// <summary>
        /// Simulate basic usage of DatabaseWriterFactory
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
        }
    }
}
