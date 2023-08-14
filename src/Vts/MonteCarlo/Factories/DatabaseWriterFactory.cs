using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.Factories
{
    /// <summary>
    /// The <see cref="Factories"/> namespace contains the Monte Carlo factory classes
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Factory methods to provide the PhotonDatabaseWriter (or list of PhotonDatabaseWriters)
    /// or CollisionInfoDatabaseWriter (or list).
    /// </summary>
    public class DatabaseWriterFactory
    {
        /// <summary>
        /// Static method to provide a list of PhotonDatabaseWriters.  It calls the method
        /// to instantiate one PhotonDatabaseWriter, GetSurfaceVirtualBoundaryDatabaseWriter,
        /// for all elements in the list of virtual boundary DatabaseType.
        /// </summary>
        /// <param name="databaseTypes">list of database types</param>
        /// <param name="filePath">path string for database output</param>
        /// <param name="outputName">name string of output</param>
        /// <returns>a list of PhotonDatabaseWriter</returns>
        public static IList<PhotonDatabaseWriter> GetSurfaceVirtualBoundaryDatabaseWriters(
            IList<DatabaseType> databaseTypes, string filePath, string outputName)
        {
            return databaseTypes.Select(v => GetSurfaceVirtualBoundaryDatabaseWriter(v,
                filePath, outputName)).ToList();

        }
        /// <summary>
        /// Static method to instantiate correct PhotonDatabaseWriter given a 
        /// virtual boundary DatabaseType, path to where to output database and database filename.
        /// </summary>
        /// <param name="databaseType">database type enum</param>
        /// <param name="filePath">path string of database output</param>
        /// <param name="outputName">filename string of database file</param>
        /// <returns>a PhotonDatabaseWriter</returns>
        public static PhotonDatabaseWriter GetSurfaceVirtualBoundaryDatabaseWriter(
            DatabaseType databaseType, string filePath, string outputName)
        {
            switch (databaseType)
            {
                case DatabaseType.DiffuseReflectance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.DiffuseReflectance,
                        Path.Combine(filePath, outputName, "DiffuseReflectanceDatabase"));
                case DatabaseType.DiffuseTransmittance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.DiffuseTransmittance,
                        Path.Combine(filePath, outputName, "DiffuseTransmittanceDatabase"));
                case DatabaseType.SpecularReflectance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.SpecularReflectance,
                        Path.Combine(filePath, outputName, "SpecularReflectanceDatabase"));
                case DatabaseType.pMCDiffuseReflectance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.pMCDiffuseReflectance,
                        Path.Combine(filePath, outputName, "DiffuseReflectanceDatabase"));
                case DatabaseType.pMCDiffuseTransmittance:
                    return new PhotonDatabaseWriter(VirtualBoundaryType.pMCDiffuseTransmittance,
                        Path.Combine(filePath, outputName, "DiffuseTransmittanceDatabase"));
                default:
                    throw new ArgumentOutOfRangeException(
                        "Database type not recognized: " + databaseType);
            }
        }
        /// <summary>
        /// Static method to provide list of CollisionInfoDatabaseWriters.  It calls the method
        /// to instantiate one CollisionInfoDatabaseWriter, GetCollisionInfoDatabaseWriter,
        /// for all elements in the list of virtual boundary DatabaseType. 
        /// </summary>
        /// <param name="databaseTypes">list of database types</param>
        /// <param name="tissue">ITissue needed to instantiate Writer to know how many regions</param>
        /// <param name="filePath">path string of database output</param>
        /// <param name="outputName">filename string of output file</param>
        /// <returns>a list of CollisionInfoDatabaseWriter</returns>
        public static IList<CollisionInfoDatabaseWriter> GetCollisionInfoDatabaseWriters(
            IList<DatabaseType> databaseTypes, ITissue tissue, string filePath, string outputName)
        {
            return databaseTypes.Select(v => GetCollisionInfoDatabaseWriter(v, tissue,
                filePath, outputName)).ToList();

        }
        /// <summary>
        /// Static method to instantiate correct CollisionInfoDatabaseWriter given a 
        /// virtual boundary DatabaseType, path to where to output database and database filename.
        /// </summary>
        /// <param name="databaseType">database type enum</param>
        /// <param name="tissue">ITissue to know how many regions</param>
        /// <param name="filePath">path string of database output</param>
        /// <param name="outputName">filename string of database file</param>
        /// <returns>a CollisionInfoDatabaseWriter</returns>
        public static CollisionInfoDatabaseWriter GetCollisionInfoDatabaseWriter(
            DatabaseType databaseType, ITissue tissue, string filePath, string outputName)
        {
            switch (databaseType)
            {
                case DatabaseType.pMCDiffuseReflectance:
                    return new CollisionInfoDatabaseWriter(VirtualBoundaryType.pMCDiffuseReflectance,
                        Path.Combine(filePath, outputName, "CollisionInfoDatabase"),
                        tissue.Regions.Count);
                case DatabaseType.pMCDiffuseTransmittance:
                    return new CollisionInfoDatabaseWriter(VirtualBoundaryType.pMCDiffuseTransmittance,
                        Path.Combine(filePath, outputName, "CollisionInfoTransmittanceDatabase"),
                        tissue.Regions.Count);
                case DatabaseType.DiffuseReflectance:
                case DatabaseType.DiffuseTransmittance:
                case DatabaseType.SpecularReflectance:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(
                        "Database type not recognized: " + databaseType);
            }
        }

    }
}
