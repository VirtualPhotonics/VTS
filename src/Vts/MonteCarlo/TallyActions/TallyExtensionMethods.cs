namespace Vts.MonteCarlo.TallyActions
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
                    return true;
                default:
                case TallyType.ROfRho:
                    return true;
                case TallyType.ROfAngle:
                    return true;
                case TallyType.ROfRhoAndOmega:
                    return true;
                case TallyType.ROfRhoAndTime:
                    return true;
                case TallyType.ROfXAndY:
                    return true;
                case TallyType.RDiffuse:
                    return true;
                case TallyType.pMuaMusInROfRhoAndTime:
                    return true;
                case TallyType.TOfRhoAndAngle:
                    return false;
                case TallyType.TOfRho:
                    return false;
                case TallyType.TOfAngle:
                    return false;
                case TallyType.TDiffuse:
                    return false;
            }
        }

        public static bool IsTransmittanceTally(this TallyType type)
        {
            switch (type)
            {
                case TallyType.ROfRhoAndAngle:
                    return false;
                default:
                case TallyType.ROfRho:
                    return false;
                case TallyType.ROfAngle:
                    return false;
                case TallyType.ROfRhoAndOmega:
                    return false;
                case TallyType.ROfRhoAndTime:
                    return false;
                case TallyType.ROfXAndY:
                    return false;
                case TallyType.RDiffuse:
                    return false;
                case TallyType.pMuaMusInROfRhoAndTime:
                    return false;
                case TallyType.TOfRhoAndAngle:
                    return true;
                case TallyType.TOfRho:
                    return true;
                case TallyType.TOfAngle:
                    return true;
                case TallyType.TDiffuse:
                    return true;
            }
        }
    }
}
