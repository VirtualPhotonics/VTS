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
        /// <returns></returns>
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
                case TallyType.pMCROfRhoAndTime:
                case TallyType.pMCROfRho:
                case TallyType.RSpecular:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if IDetector is transmittance tally or not.
        /// </summary>
        /// <param name="type">TallyType enum</param>
        /// <returns></returns>
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
        /// Method to determine if IDetector is an internal (non-boundary) surface tally or not.
        /// </summary>
        /// <param name="type">TallyType enum</param>
        /// <returns></returns>
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
        /// <returns></returns>
        public static bool IsSurfaceTally(this TallyType tallyType)
        {
            return tallyType.IsTransmittanceTally() || tallyType.IsReflectanceTally();
        }
        /// <summary>
        /// Method to determine if IDetector is pMC tally or not
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns></returns>
        public static bool IspMCTally(this TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.pMCROfRho:
                case TallyType.pMCROfRhoAndTime:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Method to determine if IDetector is volume tally or not.
        /// </summary>
        /// <param name="tallyType">TallyType enum</param>
        /// <returns></returns>
        public static bool IsVolumeTally(this TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.FluenceOfRhoAndZ:
                case TallyType.FluenceOfRhoAndZAndTime:
                case TallyType.AOfRhoAndZ:
                case TallyType.ATotal:
                case TallyType.MomentumTransferOfRhoAndZ:
                case TallyType.RadianceOfRhoAndZAndAngle:
                    return true;
                default:
                    return false;
            }
        }
    }
}
