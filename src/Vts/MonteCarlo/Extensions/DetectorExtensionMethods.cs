namespace Vts.MonteCarlo.Extensions
{
    /// <summary>
    /// Methods used to determine if tally is reflectance or 
    /// transmittance tally.
    /// </summary>
    /// <remarks>Methods implemented for both IDetector and IDetectorInput interfaces</remarks>
    public static class DetectorExtensionMethods
    {
        /// <summary>
        /// Method to determine if IDetector is a reflectance tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean indicating whether reflectance tally or not</returns>
        public static bool IsReflectanceTally(this IDetector detector)
        {
            return IsReflectanceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetectorInput is a reflectance tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean indicating whether reflectance tally or not</returns>
        public static bool IsReflectanceTally(this IDetectorInput detector)
        {
            return IsReflectanceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetector is transmittance tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsTransmittanceTally(this IDetector detector)
        {
            return IsTransmittanceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetectorInput is transmittance tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsTransmittanceTally(this IDetectorInput detector)
        {
            return IsTransmittanceTallyString(detector.TallyType);
        }      


        /// <summary>
        /// Method to determine if IDetector is a specular tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsSpecularReflectanceTally(this IDetector detector)
        {
            return IsSpecularReflectanceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetectorInput is a specular tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsSpecularReflectanceTally(this IDetectorInput detector)
        {
            return IsSpecularReflectanceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetector is an internal (non-boundary) surface tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsInternalSurfaceTally(this IDetector detector)
        {
            return IsInternalSurfaceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetectorInput is an internal (non-boundary) surface tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsInternalSurfaceTally(this IDetectorInput detector)
        {
            return IsInternalSurfaceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetector is a surface tally or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsSurfaceTally(this IDetector detector)
        {
            return detector.IsTransmittanceTally() || detector.IsReflectanceTally() ||
                   detector.IsSpecularReflectanceTally() || detector.IsInternalSurfaceTally();
        }

        /// <summary>
        /// Method to determine if IDetectorInput is a surface tally or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsSurfaceTally(this IDetectorInput detector)
        {
            return detector.IsTransmittanceTally() || detector.IsReflectanceTally() ||
                   detector.IsSpecularReflectanceTally() || detector.IsInternalSurfaceTally();
        }

        /// <summary>
        /// Method to determine if IDetector is a pMC tally or not
        /// </summary>
        /// <param name="detector">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IspMCReflectanceTally(this IDetector detector)
        {
            return IspMCReflectanceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetectorInput is a pMC tally or not
        /// </summary>
        /// <param name="detector">TallyType enum</param>
        /// <returns>boolean</returns>
        public static bool IspMCReflectanceTally(this IDetectorInput detector)
        {
            return IspMCReflectanceTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetector is volume tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsVolumeTally(this IDetector detector)
        {
            return IsVolumeTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method to determine if IDetector is volume tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsVolumeTally(this IDetectorInput detector)
        {
            return IsVolumeTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method determines whether detector is based on cylindrical coordinates
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsCylindricalTally(this IDetector detector)
        {
            return IsCylindricalTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method determines whether detector is based on cylindrical coordinates
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsCylindricalTally(this IDetectorInput detector)
        {
            return IsCylindricalTallyString(detector.TallyType);
        }

        /// <summary>
        /// Method determines whether tally type is implemented for 
        /// continous absorption weighting (CAW) or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsNotImplementedForCAW(this IDetector detector)
        {
            return IsNotImplementedForCAWString(detector.TallyType);
        }

        /// <summary>
        /// Method determines whether IDetectorInput is implemented for 
        /// continous absorption weighting (CAW) or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsNotImplementedForCAW(this IDetectorInput detector)
        {
            return IsNotImplementedForCAWString(detector.TallyType);
        }

        /// <summary>
        /// Method determines whether tally type is implemented yet or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsNotImplementedYet(this IDetector detector)
        {
            return IsNotImplementedYetString(detector.TallyType);
        }

        /// <summary>
        /// Method determines whether tally type is implemented yet or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        public static bool IsNotImplementedYet(this IDetectorInput detector)
        {
            return IsNotImplementedYetString(detector.TallyType);
        }



        /// <summary>
        /// Method to determine if string represents a reflectance tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean indicating whether reflectance tally or not</returns>
        private static bool IsReflectanceTallyString(string detector)
        {
            switch (detector)
            {
                case "ROfRhoAndAngle":
                case "ROfRho":
                case "ROfAngle":
                case "ROfRhoAndOmega":
                case "ROfRhoAndTime":
                case "ROfXAndY":
                case "RDiffuse":
                case "ROfFx":
                case "ROfFxAndTime":
                case "ReflectedMTOfRhoAndSubregionHist":
                case "ReflectedTimeOfRhoAndSubregionHist":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if represents a transmittance tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        private static bool IsTransmittanceTallyString(this string detector)
        {
            switch (detector)
            {
                case "TOfRhoAndAngle":
                case "TOfRho":
                case "TOfAngle":
                case "TDiffuse":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if string represents a specular tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        private static bool IsSpecularReflectanceTallyString(string detector)
        {
            switch (detector)
            {
                case "RSpecular":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if string represents an internal (non-boundary) surface tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        private static bool IsInternalSurfaceTallyString(string detector)
        {
            switch (detector)
            {
                case "RadianceOfRho":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if string represents a pMC tally or not
        /// </summary>
        /// <param name="detector">TallyType enum</param>
        /// <returns>boolean</returns>
        private static bool IspMCReflectanceTallyString(string detector)
        {
            switch (detector)
            {
                case "pMCROfRho":
                case "pMCROfRhoAndTime":
                case "pMCROfFx":
                case "pMCROfFxAndTime":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method to determine if IDetector is volume tally or not.
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        private static bool IsVolumeTallyString(string detector)
        {
            switch (detector)
            {
                case "FluenceOfRhoAndZ":
                case "FluenceOfRhoAndZAndTime":
                case "FluenceOfXAndYAndZ":
                case "AOfRhoAndZ":
                case "ATotal":
                case "RadianceOfRhoAndZAndAngle":
                case "RadianceOfXAndYAndZAndThetaAndPhi":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method determines whether detector is based on cylindrical coordinates
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        private static bool IsCylindricalTallyString(string detector)
        {
            switch (detector)
            {
                case "ROfRho":
                case "ROfRhoAndOmega":
                case "ROfRhoAndTime":
                case "ROfRhoAndAngle":
                case "TOfRho":
                case "TOfRhoAndAngle":
                case "FluenceOfRhoAndZ":
                case "FluenceOfRhoAndZAndTime":
                case "AOfRhoAndZ":
                case "ReflectedMTOfRhoAndSubregionHist":
                case "ReflectedTimeOfRhoAndSubregionHist":
                case "RadianceOfRho":
                case "RadianceOfRhoAndZAndAngle":
                case "pMCROfRho":
                case "pMCROfRhoAndTime":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method determines whether tally type is implemented for 
        /// continous absorption weighting (CAW) or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        private static bool IsNotImplementedForCAWString(string detector)
        {
            switch (detector)
            {
                case "FluenceOfRhoAndZ":
                case "FluenceOfRhoAndZAndTime":
                case "FluenceOfXAndYAndZ":
                case "AOfRhoAndZ":
                case "ReflectedMTOfRhoAndSubregionHist":
                case "RadianceOfRhoAndZAndAngle":
                case "RadianceOfXAndYAndZAndThetaAndPhi":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Method determines whether tally type is implemented yet or not
        /// </summary>
        /// <param name="detector">detector</param>
        /// <returns>boolean</returns>
        private static bool IsNotImplementedYetString(string detector)
        {
            switch (detector)
            {
                //case "ReflectedMTOfRhoAndSubregionHist":
                //    return true;
                default:
                    return false;
            }
        } 
    }
}