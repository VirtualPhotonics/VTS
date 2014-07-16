namespace Vts.MonteCarlo.Extensions
{
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
                    return true;
                default:
                case DatabaseType.DiffuseReflectance:
                case DatabaseType.DiffuseTransmittance:
                case DatabaseType.SpecularReflectance:
                    return false;
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
                default:
                    return VirtualBoundaryType.DiffuseReflectance;
                case DatabaseType.DiffuseTransmittance:
                    return VirtualBoundaryType.DiffuseTransmittance;
                case DatabaseType.SpecularReflectance:  
                    return VirtualBoundaryType.SpecularReflectance;
                case DatabaseType.pMCDiffuseReflectance:
                    return VirtualBoundaryType.pMCDiffuseReflectance;

            }
        }
    }
}
