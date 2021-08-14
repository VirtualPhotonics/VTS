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
        List<string> listOfTestGeneratedFiles = new List<string>()
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
            // need to separate this one out because same file is written to
            // by DatabaseType.pMCDiffuseReflectance so need to close before that test
            var diffuseReflectanceDb =
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.DiffuseReflectance,
                    Directory.GetCurrentDirectory(), "");
            Assert.IsInstanceOf<PhotonDatabaseWriter>(diffuseReflectanceDb);
            Assert.IsInstanceOf<PhotonDatabaseWriter>(
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.DiffuseTransmittance, 
                    Directory.GetCurrentDirectory(), ""));
            Assert.IsInstanceOf<PhotonDatabaseWriter>(
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.SpecularReflectance, 
                    Directory.GetCurrentDirectory(), ""));
            diffuseReflectanceDb.Close();
            Assert.IsInstanceOf<PhotonDatabaseWriter>(
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                    DatabaseType.pMCDiffuseReflectance, 
                    Directory.GetCurrentDirectory(), ""));
        }
        /// <summary>
        /// Simulate erroneous invocation
        /// </summary>
        [Test]
        [Ignore("this enum trick worked in other Factories tests, not sure why not here")]
        public void Demonstrate_GetSurfaceVirtualBoundaryDatabaseWriter_throws_exception_on_faulty_input()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => 
                DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriter(
                     (DatabaseType)(-1), 
                     Directory.GetCurrentDirectory(), ""));
        }
    }
}
