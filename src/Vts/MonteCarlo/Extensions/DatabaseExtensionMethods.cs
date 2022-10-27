using System;
using System.Runtime.CompilerServices;

namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// The <see cref="Extensions"/> namespace contains the Monte Carlo extension classes for working with databases
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Methods used to determine type of virtual boundary.
    /// </summary>
    public static class DatabaseExtensionMethods
    {
        /// <summary>
        /// Method to determine if perturbation Monte Carlo (pMC) database specified or not
        /// </summary>
        /// <param name="databaseType">database type</param>
        /// <returns>true if pMC VB, false if not</returns>
        public static bool IspMCDatabase(this DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.pMCDiffuseReflectance:
                case DatabaseType.pMCDiffuseTransmittance:
                    return true;
                case DatabaseType.DiffuseReflectance:
                case DatabaseType.DiffuseTransmittance:
                case DatabaseType.SpecularReflectance:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(
                        "Database type not recognized: " + databaseType);

            }
        }
        /// <summary>
        /// Method to provide VirtualBoundaryType corresponding to DatabaseType
        /// </summary>
        /// <param name="databaseType">database type</param>
        /// <returns>corresponding virtual boundary type</returns>
        public static VirtualBoundaryType GetCorrespondingVirtualBoundaryType(this DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.DiffuseReflectance:
                    return VirtualBoundaryType.DiffuseReflectance;
                case DatabaseType.DiffuseTransmittance:
                    return VirtualBoundaryType.DiffuseTransmittance;
                case DatabaseType.SpecularReflectance:  
                    return VirtualBoundaryType.SpecularReflectance;
                case DatabaseType.pMCDiffuseReflectance:
                    return VirtualBoundaryType.pMCDiffuseReflectance;
                case DatabaseType.pMCDiffuseTransmittance:
                    return VirtualBoundaryType.pMCDiffuseTransmittance;
                default:
                    throw new ArgumentOutOfRangeException(
                        "Database type not recognized: " + databaseType);

            }
        }
    }
}
