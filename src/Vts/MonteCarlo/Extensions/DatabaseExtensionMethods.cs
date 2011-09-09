using System.Collections.Generic;

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
        /// <param name="DatabaseType">VB type</param>
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
    }
}
