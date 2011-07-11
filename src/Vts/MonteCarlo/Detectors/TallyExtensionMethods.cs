namespace Vts.MonteCarlo.Detectors
{
    /// <summary>
    /// Methods used to determine if tally is reflectance or 
    /// transmittance tally.
    /// </summary>
    public static class TallyExtensionMethods
    {
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

        public static bool IsSurfaceTally(this TallyType tallyType)
        {
            return tallyType.IsTransmittanceTally() || tallyType.IsReflectanceTally();
        }

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

        public static bool IsVolumeTally(this TallyType tallyType)
        {
            switch (tallyType)
            {
                case TallyType.FluenceOfRhoAndZ:
                case TallyType.FluenceOfRhoAndZAndTime:
                case TallyType.AOfRhoAndZ:
                case TallyType.ATotal:
                case TallyType.MomentumTransferOfRhoAndZ:
                case TallyType.DosimetryOfRho:
                    return true;
                default:
                    return false;
            }
        }
    }
}
