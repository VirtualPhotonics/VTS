namespace Vts.MonteCarlo
{
    /// <summary>
    /// detector tally types
    /// </summary>
    public static class TallyType
    {
        /// <summary>
        /// built in types for detector tallies
        /// </summary>
        public static readonly string[] BuiltInTypes =
        {
            // Reflectance detected by a slanted recessed fiber 
            "SlantedRecessedFiber",
            // Reflectance Surface fiber 
            "SurfaceFiber",
            // Reflectance as a function of source-detector separation (rho) and angle
            "ROfRhoAndAngle",
            // Reflectance as a function of source-detector separation (rho)
            "ROfRho",
            // Reflectance as a function of source-detector separation (rho) recessed in air
            "ROfRhoRecessed",
            // Reflectance as a function of angle
            "ROfAngle",
            // Reflectance as a function of source-detector separation (rho) and temporal-frequency (omega)
            "ROfRhoAndOmega",
            // Reflectance as a function of source-detector separation (rho) and time
            "ROfRhoAndTime",
            // Reflectance as a function of source-detector separation (rho) and maximum depth
            "ROfRhoAndMaxDepth",
            // Reflectance as a function of source-detector separation (rho) and maximum depth recessed in air
            "ROfRhoAndMaxDepthRecessed",
            // Reflectance as a function of Cartesian position on the surface of the tissue
            "ROfXAndY",
            // Reflectance as a function of Cartesian position recessed at specified z-plane
            "ROfXAndYRecessed",
            // Reflectance as a function of Cartesian position and time on the surface of the tissue
            "ROfXAndYAndTime",
            // Reflectance as a function of Cartesian position and time on the surface of the tissue recessed in air
            "ROfXAndYAndTimeRecessed",
            // Reflectance as a function of Cartesian position and time and tissue subregion on the surface of the tissue
            "ROfXAndYAndTimeAndSubregion",
            // Reflectance as a function of Cartesian position and time and tissue subregion on the surface of the tissue recessed in air
            "ROfXAndYAndTimeAndSubregionRecessed",
            // Reflectance as a function of x, y, theta, phi
            "ROfXAndYAndThetaAndPhi",
            // Reflectance as a function of Cartesian position and MaxDepth on the surface of the tissue
            "ROfXAndYAndMaxDepth",
            // Reflectance as a function of Cartesian position and MaxDepth recessed at z-plane
            "ROfXAndYAndMaxDepthRecessed",
            // Total diffuse reflectance
            "RDiffuse",
            // Total specular reflectance
            "RSpecular",
            // Reflectance as a function of spatial frequency along the x-axis
            "ROfFx",
            // Reflectance as a function of spatial frequency along the x-axis, and time
            "ROfFxAndTime",
            // Reflectance as a function of spatial frequency along the x-axis, and angle
            "ROfFxAndAngle",
            // Reflectance as a function of spatial frequency along the x-axis and maximum depth
            "ROfFxAndMaxDepth",
            // Total diffuse transmittance
            "TDiffuse",
            // Transmittance as a functino of source-detector separation (rho)
            "TOfRho",
            // Transmittance as a function of angle
            "TOfAngle",
            // Transmittance as a function of source-detector separation (rho) and angle
            "TOfRhoAndAngle",
            // Transmittance as a function of x and y
            "TOfXAndY",
            // Reflectance as a function of Cartesian position and time and tissue subregion on the surface of the tissue
            "TOfXAndYAndTimeAndSubregion",
            // Transmittance as a function of spatial frequency (fx)
            "TOfFx", 
            // Fluence as a function of source-detector separation (rho) and tissue depth (Z)
            "FluenceOfRhoAndZ",
            // Fluence as a function of source-detector separation (rho) and tissue depth (Z) and time
            "FluenceOfRhoAndZAndTime",
            // Fluence as a function of rho, z and omega
            "FluenceOfRhoAndZAndOmega",
            // Fluence as a function of x, y and z
            "FluenceOfXAndYAndZ",
            // Fluence as a function of x, y, z and time
            "FluenceOfXAndYAndZAndTime",
            // Fluence as a function of x, y, z and omega
            "FluenceOfXAndYAndZAndOmega",
            // Fluence as a function of x, y, z, and starting x, y
            "FluenceOfXAndYAndZAndStartingXAndY",
            // Fluence as a function of fx and z
            "FluenceOfFxAndZ",
            // Absorbed energy as a function of source-detector separation (rho) and tissue depth (Z)
            "AOfRhoAndZ",
            // Absorbed energy as a function of X, Y and tissue depth (Z)
            "AOfXAndYAndZ",
            // Total absorbed energy
            "ATotal",
            // Total absorbed energy in bounding volume
            "ATotalBoundingVolume",
            // Reflected momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
            "ReflectedMTOfRhoAndSubregionHist",
            // Reflected momentum transfer as a function of x, y and tissue region with histogram of MT
            "ReflectedMTOfXAndYAndSubregionHist",
            // Transmitted momentum transfer as a function of source-detector separation (rho) and tissue region
            // with histogram of MT
            "TransmittedMTOfRhoAndSubregionHist",
            // Transmitted momentum transfer as a function of x, y and tissue region with histogram of MT
            "TransmittedMTOfXAndYAndSubregionHist", 
            // Reflected dynamic momentum transfer as a function of source-detector separation (rho) and
            // tissue region with histogram of MT
            "ReflectedDynamicMTOfRhoAndSubregionHist",
            // Reflected dynamic momentum transfer as a function of x, y and tissue region with histogram of MT
            "ReflectedDynamicMTOfXAndYAndSubregionHist",
            // Reflected dynamic momentum transfer as a function of spatial frequency fx and
            // tissue region with histogram of MT
            "ReflectedDynamicMTOfFxAndSubregionHist",
            // Transmitted dynamic momentum transfer as a function of source-detector separation (rho) and
            // tissue region with histogram of MT
            "TransmittedDynamicMTOfRhoAndSubregionHist",
            // Transmitted dynamic momentum transfer as a function of x, y and tissue region with histogram of MT
            "TransmittedDynamicMTOfXAndYAndSubregionHist",
            // Transmitted dynamic momentum transfer as a function of fx and tissue region with histogram of MT
            "TransmittedDynamicMTOfFxAndSubregionHist",
            // Reflected subregion time as a function of source-detector separation (rho) and tissue region 
            "ReflectedTimeOfRhoAndSubregionHist",
            // Surface radiance as a function of source-detector separation (rho)
            "RadianceOfRhoAtZ",
            // Volume radiance as a function of source-detector separation (rho), tissue depth (Z) and angle
            "RadianceOfRhoAndZAndAngle",
            // Volume radiance as a function of spatial-frequency (fx), tissue depth (Z) and angle
            "RadianceOfFxAndZAndAngle",
            // Volume radiance as a function of x, y, z, theta and phi
            "RadianceOfXAndYAndZAndThetaAndPhi",
            // perturbation Monte Carlo (pMC) total absorption
            "pMCATotal",
            // perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho) 
            "pMCROfRho",
            // perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho) recessed in air
            "pMCROfRhoRecessed",
            // perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho) and time
            "pMCROfRhoAndTime", 
            // perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho) and time recessed in air
            "pMCROfRhoAndTimeRecessed", 
            // perturbation Monte Carlo (pMC) reflectance as a function of Cartesian coordinates (x,y)
            "pMCROfXAndY",
            // perturbation Monte Carlo (pMC) reflectance as a function of (x,y), time and subregion
            "pMCROfXAndYAndTimeAndSubregion", 
            // perturbation Monte Carlo (pMC) reflectance as a function of (x,y), time and subregion recessed in air
            "pMCROfXAndYAndTimeAndSubregionRecessed", 
            // perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx)
            "pMCROfFx",
            // perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx) and time
            "pMCROfFxAndTime",     
            // perturbation Monte Carlo (pMC) transmittance as a function of source-detector separation (rho) 
            "pMCTOfRho",
            // differential Monte Carlo (dMC) d(reflectance)/dMua as a function of source-detector separation (rho)
            "dMCdROfRhodMua",
            // differential Monte Carlo (dMC) d(reflectance)/dMus as a function of source-detector separation (rho) 
            "dMCdROfRhodMus",
        };
        /// <summary>
        /// slanted recessed fiber detector
        /// </summary>
        public static string SlantedRecessedFiber { get { return "SlantedRecessedFiber"; } }
        /// <summary>
        /// cylindrical fiber detector
        /// </summary>
        public static string SurfaceFiber { get { return "SurfaceFiber"; } }
        /// <summary>
        /// Total diffuse reflectance
        /// </summary>
        public static string RDiffuse { get { return "RDiffuse"; } }
        /// <summary>
        /// Total specular reflectance
        /// </summary>
        public static string RSpecular { get { return "RSpecular"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho)
        /// </summary>
        public static string ROfRho { get { return "ROfRho"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) recessed in air
        /// </summary>
        public static string ROfRhoRecessed { get { return "ROfRhoRecessed"; } }
        /// <summary>
        /// Reflectance as a function of angle
        /// </summary>
        public static string ROfAngle { get { return "ROfAngle"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and angle
        /// </summary>
        public static string ROfRhoAndAngle { get { return "ROfRhoAndAngle"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and time
        /// </summary>
        public static string ROfRhoAndTime { get { return "ROfRhoAndTime"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and maximum depth attained
        /// </summary>
        public static string ROfRhoAndMaxDepth { get { return "ROfRhoAndMaxDepth"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and maximum depth attained recessed in air
        /// </summary>
        public static string ROfRhoAndMaxDepthRecessed { get { return "ROfRhoAndMaxDepthRecessed"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and temporal-frequency (omega)
        /// </summary>
        public static string ROfRhoAndOmega { get { return "ROfRhoAndOmega"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue
        /// </summary>
        public static string ROfXAndY { get { return "ROfXAndY"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue recessed in air
        /// </summary>
        public static string ROfXAndYRecessed { get { return "ROfXAndYRecessed"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue and time
        /// </summary>
        public static string ROfXAndYAndTime { get { return "ROfXAndYAndTime"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue and time recessed in air
        /// </summary>
        public static string ROfXAndYAndTimeRecessed { get { return "ROfXAndYAndTimeRecessed"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue and max depth
        /// </summary>
        public static string ROfXAndYAndMaxDepth { get { return "ROfXAndYAndMaxDepth"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue and max depth recessed in air
        /// </summary>
        public static string ROfXAndYAndMaxDepthRecessed { get { return "ROfXAndYAndMaxDepthRecessed"; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency along the x-axis
        /// </summary>
        public static string ROfFx { get { return "ROfFx"; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency along the x-axis, and time
        /// </summary>
        public static string ROfFxAndTime { get { return "ROfFxAndTime"; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency along the x-axis, and angle
        /// </summary>
        public static string ROfFxAndAngle { get { return "ROfFxAndAngle"; } }
        /// <summary>
        /// Total diffuse transmittance
        /// </summary>
        public static string TDiffuse { get { return "TDiffuse"; } }
        /// <summary>
        /// Transmittance as a function of source-detector separation (rho)
        /// </summary>
        public static string TOfRho { get { return "TOfRho"; } }
        /// <summary>
        /// Transmittance as a function of angle
        /// </summary>
        public static string TOfAngle { get { return "TOfAngle"; } }
        /// <summary>
        /// Transmittance as a function of source-detector separation (rho) and angle
        /// </summary>
        public static string TOfRhoAndAngle { get { return "TOfRhoAndAngle"; } }
        /// <summary>
        /// Transmittance as a function of Cartesian position on the surface of the tissue
        /// </summary>
        public static string TOfXAndY { get { return "TOfXAndY"; } }
        /// <summary>
        /// Transmittance as a function of spatial frequency (fx)
        /// </summary>
        public static string TOfFx { get { return "TOfFx"; } }
        /// <summary>
        /// Fluence as a function of source-detector separation (rho) and tissue depth (Z)
        /// </summary>
        public static string FluenceOfRhoAndZ { get { return "FluenceOfRhoAndZ"; } }
        /// <summary>
        /// Fluence as a function of source-detector separation (rho) and tissue depth (Z) and time
        /// </summary>
        public static string FluenceOfRhoAndZAndTime { get { return "FluenceOfRhoAndZAndTime"; } }
        /// <summary>
        /// Fluence as a function of x, y and z
        /// </summary>
        public static string FluenceOfXAndYAndZ { get { return "FluenceOfXAndYAndZ"; } }
        /// <summary>
        /// Absorbed energy as a function of source-detector separation (rho) and tissue depth (Z)
        /// </summary>
        public static string AOfRhoAndZ { get { return "AOfRhoAndZ"; } }
        /// <summary>
        /// Absorbed energy as a function of Cartesian coordinates X, Y and Z
        /// </summary>
        public static string AOfXAndYAndZ { get { return "AOfXAndYAndZ"; } }
        /// <summary>
        /// Total absorbed energy
        /// </summary>
        public static string ATotal { get { return "ATotal"; } }
        /// <summary>
        /// Total absorbed energy in a bounding volume
        /// </summary>
        public static string ATotalBoundingVolume { get { return "ATotalBoundingVolume"; } }
        /// <summary>
        /// Reflected momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
        /// </summary>
        public static string ReflectedMTOfRhoAndSubregionHist { get { return "ReflectedMTOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Reflected momentum transfer as a function of x, y and tissue region with histogram of MT
        /// </summary>
        public static string ReflectedMTOfXAndYAndSubregionHist { get { return "ReflectedMTOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Reflected momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
        /// </summary>
        public static string TransmittedMTOfRhoAndSubregionHist { get { return "TransmittedMTOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Reflected momentum transfer as a function of x, y and tissue region with histogram of MT
        /// </summary>
        public static string TransmittedMTOfXAndYAndSubregionHist { get { return "TransmittedMTOfXAndYAndSubregionHist"; } }
        /// <summary>
        /// Reflected dynamic momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
        /// </summary>
        public static string ReflectedDynamicMTOfRhoAndSubregionHist { get { return "ReflectedDynamicMTOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Reflected dynamic momentum transfer as a function of x, y and tissue region with histogram of MT
        /// </summary>
        public static string ReflectedDynamicMTOfXAndYAndSubregionHist { get { return "ReflectedDynamicMTOfXAndYAndSubregionHist"; } }
        /// <summary>
        /// Reflected dynamic momentum transfer as a function of spatial frequency fx and tissue region with histogram of MT
        /// </summary>
        public static string ReflectedDynamicMTOfFxAndSubregionHist
        {
            get { return "ReflectedDynamicMTOfFxAndSubregionHist"; }
        }
        /// <summary>
        /// Transmitted dynamic momentum transfer as a function of rho and tissue region with histogram of MT
        /// </summary>
        public static string TransmittedDynamicMTOfRhoAndSubregionHist { get { return "TransmittedDynamnicMTOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Transmitted dynamic momentum transfer as a function of x, y and tissue region with histogram of MT
        /// </summary>
        public static string TransmittedDynamicMTOfXAndYAndSubregionHist { get { return "TransmittedDynamicMTOfXAndYAndSubregionHist"; } }
        /// <summary>
        /// Transmitted dynamic momentum transfer as a function of fx and tissue region with histogram of MT
        /// </summary>
        public static string TransmittedDynamicMTOfFxAndSubregionHist { get { return "TransmittedDynamicMTOfFxAndSubregionHist"; } }
        /// <summary>
        /// Reflected subregion time as a function of source-detector separation (rho) and tissue region 
        /// </summary>
        public static string ReflectedTimeOfRhoAndSubregionHist { get { return "ReflectedTimeOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Surface radiance as a function of source-detector separation (rho)
        /// </summary>
        public static string RadianceOfRho { get { return "RadianceOfRho"; } }
        /// <summary>
        /// Volume radiance as a function of source-detector separation (rho), tissue depth (Z) and angle
        /// </summary>
        public static string RadianceOfRhoAndZAndAngle { get { return "RadianceOfRhoAndZAndAngle"; } }
        /// <summary>
        /// Volume radiance as a function of spatial frequency (fx), tissue depth (Z) and angle
        /// </summary>
        public static string RadianceOfFxAndZAndAngle { get { return "RadianceOfFxAndZAndAngle"; } }
        /// <summary>
        /// Volume radiance as a function of x, y, z, theta and phi
        /// </summary>
        public static string RadianceOfXAndYAndZAndThetaAndPhi { get { return "RadianceOfXAndYAndZAndThetaAndPhi"; } }
        /// <summary>
        /// Total absorbed energy
        /// </summary>
        public static string pMCATotal { get { return "pMCATotal"; } }
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho)
        /// </summary>
        public static string pMCROfRho { get { return "pMCROfRho"; } }
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho) recessed in air
        /// </summary>
        public static string pMCROfRhoRecessed { get { return "pMCROfRhoRecessed"; } }
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho) and time
        /// </summary>
        public static string pMCROfRhoAndTime { get { return "pMCROfRhoAndTime"; } } // maybe these should be in separate enum?
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho) and time recessed in air
        /// </summary>
        public static string pMCROfRhoAndTimeRecessed { get { return "pMCROfRhoAndTimeRecessed"; } }
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of Cartesian coordinates (x,y)
        /// </summary>
        public static string pMCROfXAndY { get { return "pMCROfXAndY"; } }
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx)
        /// </summary>
        public static string pMCROfFx { get { return "pMCROfFx"; } }
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx) and time
        /// </summary>
        public static string pMCROfFxAndTime { get { return "pMCROfFxAndTime"; } }
        /// <summary>
        /// differential Monte Carlo (dMC) d(reflectance)/dMua as a function of source-detector separation (rho)
        /// </summary>
        public static string dMCdROfRhodMua { get { return "dMCdROfRhodMua"; } }
        /// <summary>
        /// differential Monte Carlo (dMC) d(reflectance)/dMus as a function of source-detector separation (rho) 
        /// </summary>
        public static string dMCdROfRhodMus { get { return "dMCdROfRhodMus"; } }
    }
}