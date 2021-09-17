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
    /// Unit tests for PhotonDatabaseFactory
    /// </summary>
    [TestFixture]
    public class PhotonDatabaseFactoryTests
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
        /// <summary>
        /// clear all previously generated files.
        /// </summary>
        [OneTimeSetUp]
        [OneTimeTearDown]
        public void clear_folders_and_files()
        {
            // delete any previously generated files
            foreach (var file in listOfTestGeneratedFiles)
            {
                GC.Collect();
                FileIO.FileDelete(file);
            }
        }

        /// <summary>
        /// Simulate GetPhotonDatabase for DiffuseReflectanceDatabase
        /// </summary>
        [Test]
        public void Demonstrate_GetPhotonDatabase_for_diffuse_reflectance_successful_return()
        {
            string databaseFilename = "DiffuseReflectanceDatabase";

            using (var dbWriter = new PhotonDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFilename))
            {
                dbWriter.Write(new PhotonDataPoint(
                    new Position(1, 2, 3),
                    new Direction(0, 0, 1),
                    1.0, // weight
                    10, // time
                    PhotonStateType.None));

                dbWriter.Write(new PhotonDataPoint(
                    new Position(4, 5, 6),
                    new Direction(1, 0, 0),
                    0.50,
                    100,
                    PhotonStateType.None));
            }
            Assert.IsInstanceOf<PhotonDatabase>(
                PhotonDatabaseFactory.GetPhotonDatabase(
                VirtualBoundaryType.DiffuseReflectance, 
                ""));
        }
        /// <summary>
        /// Simulate GetPhotonDatabase for DiffuseTransmittanceDatabase
        /// </summary>
        [Test]
        public void Demonstrate_GetPhotonDatabase_for_diffuse_transmittance_successful_return()
        {
            string databaseFilename = "DiffuseTransmittanceDatabase";

            using (var dbWriter = new PhotonDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFilename))
            {
                dbWriter.Write(new PhotonDataPoint(
                    new Position(1, 2, 3),
                    new Direction(0, 0, 1),
                    1.0, // weight
                    10, // time
                    PhotonStateType.None));

                dbWriter.Write(new PhotonDataPoint(
                    new Position(4, 5, 6),
                    new Direction(1, 0, 0),
                    0.50,
                    100,
                    PhotonStateType.None));
            }
            Assert.IsInstanceOf<PhotonDatabase>(
                PhotonDatabaseFactory.GetPhotonDatabase(
                    VirtualBoundaryType.DiffuseTransmittance,
                    ""));
        }        
        /// <summary>
        /// Simulate GetPhotonDatabase for SpecularReflectanceDatabase
        /// </summary>
        [Test]
        public void Demonstrate_GetPhotonDatabase_for_specular_reflectance_successful_return()
        {
            string databaseFilename = "SpecularReflectanceDatabase";

            using (var dbWriter = new PhotonDatabaseWriter(
                VirtualBoundaryType.DiffuseReflectance, databaseFilename))
            {
                dbWriter.Write(new PhotonDataPoint(
                    new Position(1, 2, 3),
                    new Direction(0, 0, 1),
                    1.0, // weight
                    10, // time
                    PhotonStateType.None));

                dbWriter.Write(new PhotonDataPoint(
                    new Position(4, 5, 6),
                    new Direction(1, 0, 0),
                    0.50,
                    100,
                    PhotonStateType.None));
            }
            Assert.IsInstanceOf<PhotonDatabase>(
                PhotonDatabaseFactory.GetPhotonDatabase(
                    VirtualBoundaryType.SpecularReflectance,
                    ""));
        }
        /// <summary>
        /// Simulate GetPhotonDatabase for SpecularReflectanceDatabase
        /// </summary>
        [Test]
        public void Demonstrate_GetPhotonDatabase_for_pmc_reflectance_successful_return()
        {
            string databaseFilename = "DiffuseReflectanceDatabase";

            using (var dbWriter = new PhotonDatabaseWriter(
                VirtualBoundaryType.pMCDiffuseReflectance, databaseFilename))
            {
                dbWriter.Write(new PhotonDataPoint(
                    new Position(1, 2, 3),
                    new Direction(0, 0, 1),
                    1.0, // weight
                    10, // time
                    PhotonStateType.None));

                dbWriter.Write(new PhotonDataPoint(
                    new Position(4, 5, 6),
                    new Direction(1, 0, 0),
                    0.50,
                    100,
                    PhotonStateType.None));
            }
            Assert.IsInstanceOf<PhotonDatabase>(
                PhotonDatabaseFactory.GetPhotonDatabase(
                    VirtualBoundaryType.pMCDiffuseReflectance,
                    ""));
        }

        /// <summary>
        /// Simulate null invocation
        /// </summary>
        [Test]
        public void Demonstrate_GetPhotonDatabase_returns_null_when_vb_does_not_create_database()
        {
            // provide non-existing database
            Assert.IsNull(PhotonDatabaseFactory.GetPhotonDatabase(
                VirtualBoundaryType.GenericVolumeBoundary, ""));
        }
        /// <summary>
        /// Simulate erroneous invocation
        /// </summary>
        [Test]
        public void Demonstrate_GetPhotonDatabase_returns_exception_on_faulty_input()
        {
            // provide non-existing database
            Assert.Throws<FileNotFoundException>(() => PhotonDatabaseFactory.GetPhotonDatabase(
                VirtualBoundaryType.DiffuseReflectance,"SpecularTransmittance"));
        }

        /// <summary>
        /// Simulate null invocation
        /// </summary>
        [Test]
        public void Demonstrate_GetpMCPhotonDatabase_returns_null_when_vb_does_not_create_database()
        {
            // provide non-existing database
            Assert.IsNull(PhotonDatabaseFactory.GetpMCDatabase(
                VirtualBoundaryType.GenericVolumeBoundary, ""));
        }
    }
}
