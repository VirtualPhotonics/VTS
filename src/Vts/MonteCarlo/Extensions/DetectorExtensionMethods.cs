namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods used to determine if tally is reflectance or 
    /// transmittance tally.
    /// </summary>
    public static class DetectorExtensionMethods
    {
        /// <summary>
        /// Method to determine is IDetector is a reflectance tally or not.
        /// </summary>
        /// <param name="type">TallyType enum</param>
        /// <returns>boolean indicating whether reflectance tally or not</returns>
        public static bool IsReflectanceTally(this TallyType type)
        {
            switch (type)
            {
                case TallyType.ROfRhoAndAngle:
                case TallyType.ROfRho:
                case TallyType.ROfAngle:
                case TallyType.ROfRhoAndOmega:
                case TallyType.ROfRhoAndTime:
                case TallyType.ROfXAndY:
                case TallyType.RDiffuse:
                case TallyType.ROfFx:
                case TallyType.ROfFxAndTime:
                case TallyType.ReflectedMTOfRhoAndSubRegionHist:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if IDetector is transmittance tally or not.
        /// </summary>
        /// <param name="type">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsTransmittanceTally(this TallyType type)
        {
            switch (type)
            {
                case TallyType.TOfRhoAndAngle:
                case TallyType.TOfRho:
                case TallyType.TOfAngle:
                case TallyType.TDiffuse:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if IDetector is specular tally or not.
        /// </summary>
        /// <param name="type">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsSpecularReflectanceTally(this TallyType type)
        {
            switch (type)
            {
                case TallyType.RSpecular:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if IDetector is an internal (non-boundary) surface tally or not.
        /// </summary>
        /// <param name="type">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsInternalSurfaceTally(this TallyType type)
        {
            switch (type)
            {
                case TallyType.RadianceOfRho:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if IDetector is a surface tally or not
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsSurfaceTally(this TallyType tallyType)
        {
            return tallyType.IsTransmittanceTally() || tallyType.IsReflectanceTally() ||
                   tallyType.IsSpecularReflectanceTally() || tallyType.IsInternalSurfaceTally();
        }

        /// <summary>
        /// Method to determine if IDetector is pMC tally or not
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IspMCReflectanceTally(this TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.pMCROfRho:
                case TallyType.pMCROfRhoAndTime:
                case TallyType.pMCROfFx:
                case TallyType.pMCROfFxAndTime:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if IDetector is volume tally or not.
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsVolumeTally(this TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.FluenceOfRhoAndZ:
                case TallyType.FluenceOfRhoAndZAndTime:
                case TallyType.FluenceOfXAndYAndZ:
                case TallyType.AOfRhoAndZ:
                case TallyType.ATotal:
                case TallyType.RadianceOfRhoAndZAndAngle:
                case TallyType.RadianceOfXAndYAndZAndThetaAndPhi:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method determines whether tally type is based on cylindrical coordinates
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsCylindricalTally(this TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.ROfRho:
                case TallyType.ROfRhoAndOmega:
                case TallyType.ROfRhoAndTime:
                case TallyType.ROfRhoAndAngle:
                case TallyType.TOfRho:
                case TallyType.TOfRhoAndAngle:
                case TallyType.FluenceOfRhoAndZ:
                case TallyType.FluenceOfRhoAndZAndTime:
                case TallyType.AOfRhoAndZ:
                case TallyType.ReflectedMTOfRhoAndSubRegionHist:
                case TallyType.RadianceOfRho:
                case TallyType.RadianceOfRhoAndZAndAngle:
                case TallyType.pMCROfRho:
                case TallyType.pMCROfRhoAndTime:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method determines whether tally type is implemented for 
        /// continous absorption weighting (CAW) or not
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsNotImplementedForCAW(this TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.FluenceOfRhoAndZ:
                case TallyType.FluenceOfRhoAndZAndTime:
                case TallyType.FluenceOfXAndYAndZ:
                case TallyType.AOfRhoAndZ:
                case TallyType.ReflectedMTOfRhoAndSubRegionHist:
                case TallyType.RadianceOfRhoAndZAndAngle:
                case TallyType.RadianceOfXAndYAndZAndThetaAndPhi:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method determines whether tally type is implemented yet or not
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IsNotImplementedYet(this TallyType tallyType)
        {
            switch (tallyType)
            {
                //case TallyType.ReflectedMTOfRhoAndSubRegionHist:
                //    return true;
                default:
                    return false;
            }
        }

    }
}
