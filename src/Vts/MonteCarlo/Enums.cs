using System;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// PhotonStateType is a bitmap of Photon.StateFlag.  Combinations of bits indicate
    /// the current state of the photon.  These states communicate what to do with the photon.
    /// ref: http://www.codeproject.com/Articles/37921/Enums-Flags-and-Csharp-Oh-my-bad-pun.aspx
    /// or http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations
    /// </summary>
    [Flags]
    public enum PhotonStateType
    {
        //     |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |
        //   8000 4000 2000 1000 0800 0400 0200 0100 0080 0040 0020 0010 0008 0004 0002 0001
        //   <- transport flags                                                           ->
        //   <- virtual flags these with "0000" added in lowest bits                      ->
        /// <summary>
        /// no bits set
        /// </summary>
        None = 0x0,
        // transport flags
        /// <summary>
        /// photon alive
        /// </summary>
        Alive = 0x1,
        /// <summary>
        /// photon exited domain
        /// </summary>
        ExitedDomain = 0x2, // do I need this?
        /// <summary>
        /// photon was absorbed, used only in analog random walk process
        /// </summary>
        Absorbed = 0x4,
        /// <summary>
        /// photon killed because path length too long
        /// </summary>
        KilledOverMaximumPathLength = 0x8,
        /// <summary>
        /// photon killed because number of collisions over maximum
        /// </summary>
        KilledOverMaximumCollisions = 0x10,
        /// <summary>
        /// photon killed by Russian Roulette
        /// </summary>
        KilledRussianRoulette = 0x20,
        // the following get set during photon transport in tissue
        /// <summary>
        /// photon pseudo-collision at reflected tissue boundary
        /// </summary>
        PseudoReflectedTissueBoundary = 0x40,
        /// <summary>
        /// photon pseudo-collision at transmitted tissue boundary
        /// </summary>
        PseudoTransmittedTissueBoundary = 0x80,
        /// <summary>
        /// photon pseudo-collision at specular tissue boundary
        /// </summary>
        PseudoSpecularTissueBoundary = 0x100,
        //PseudoRadianceTissueBoundary = 0x200,

        // virtual boundary flags, can we 1-1 map to virtualBoundary "Name"
        // move up to 16th position
        // the following get set when VB hit (after hit tissue boundary)
        /// <summary>
        /// photon pseudo-collision at DiffuseReflectance Virtual Boundary (VB)
        /// </summary>
        PseudoDiffuseReflectanceVirtualBoundary = 0x10000,
        /// <summary>
        /// photon pseudo-collision at DiffuseTransmittance Virtual Boundary (VB)
        /// </summary>
        PseudoDiffuseTransmittanceVirtualBoundary = 0x20000,
        /// <summary>
        /// photon pseudo-collision at SpecularReflectance Virtual Boundary (VB)
        /// </summary>
        PseudoSpecularReflectanceVirtualBoundary = 0x40000,
        /// <summary>
        /// photon pseudo-collision at Generic Volume Virtual Boundary (VB)
        /// </summary>
        PseudoGenericVolumeVirtualBoundary = 0x80000,
        /// <summary>
        /// photon pseudo-collision at SurfaceRadiance Virtual Boundary (VB)
        /// </summary>
        PseudoSurfaceRadianceVirtualBoundary = 0x100000,
    }
    /// <summary>
    /// Virtual boundaries are entities upon which detectors are attached.
    /// Each detector is associated with one and only one of the following types.
    /// The VBs have a spatial location (surface or volume) and sometimes have
    /// a direction.
    /// </summary>
    public enum VirtualBoundaryType
    {
        /// <summary>
        /// All diffuse reflectance detectors attach to this virtual boundary type
        /// </summary>
        DiffuseReflectance,
        /// <summary>
        /// All diffuse transmittance detectors attach to this virtual boundary type
        /// </summary>
        DiffuseTransmittance,
        /// <summary>
        /// Specular reflection detectors attach to this virtual boundary type
        /// </summary>
        SpecularReflectance,
        /// <summary>
        /// Internal volume detectors attach to this virtual boundary type
        /// </summary>
        GenericVolumeBoundary,
        /// <summary>
        /// Internal surface detectors attach to this virtual boundary type
        /// </summary>
        SurfaceRadiance,
        /// <summary>
        /// Virtual boundary used for pMC diffuse reflectance detectors
        /// </summary>
        pMCDiffuseReflectance,
    }
    /// <summary>
    /// This should match VirtualBoundaryType one for one.  Commented out ones have not made
    /// it to the white list yet.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// All diffuse reflectance detectors 
        /// </summary>
        DiffuseReflectance,
        /// <summary>
        /// All diffuse transmittance detectors 
        /// </summary>
        DiffuseTransmittance,
        /// <summary>
        /// Specular reflection detectors 
        /// </summary>
        SpecularReflectance,
        ///// <summary>
        ///// Internal volume detectors 
        ///// </summary>
        //GenericVolumeBoundary,
        ///// <summary>
        ///// Internal surface detectors 
        ///// </summary>
        //SurfaceRadiance,
        /// <summary>
        /// pMC diffuse reflectance
        /// </summary>
        pMCDiffuseReflectance,
    }
    /// <summary>
    /// Flag indicating whether the photon hit a actual tissue boundary or a virtual boundary
    /// </summary>
    public enum BoundaryHitType
    {
        /// <summary>
        /// No boundary hit
        /// </summary>
        None,
        /// <summary>
        /// Virtual boundary hit by photon
        /// </summary>
        Virtual,
        /// <summary>
        /// Actual (tissue) boundary hit by photon
        /// </summary>
        Tissue
    }
    /// <summary>
    /// Source types defined organized by dimension and geometric type
    /// </summary>
    public static class SourceType
    {
        public static readonly string[] BuiltInTypes = new []
            {
                //POINT AND LINE SOURCES

                //Point Sources
                // Point sources: isotropic
                "IsotropicPoint",
                // Point sources: directional point
                "DirectionalPoint",
                // Point sources: custom point
                "CustomPoint",

                // Line Sources
                // 1D Line sources: isotropic line
                "IsotropicLine",
                // 1D Line sources: directional line 
                "DirectionalLine",
                // 1D Line sources: custom
                "CustomLine",


                // SURFACE EMITTING FLAT SOURCES (2D SURFACE SOURCES)

                // Circular Surface Sources
                // 2D Circular surface sources: directional 
                "DirectionalCircular",
                // 2D Circular surface sources: custom
                "CustomCircular",

                // Elliptical Surface Sources
                // 2D Elliptical surface sources: directional 
                "DirectionalElliptical",
                // 2D Elliptical surface sources: custom
                "CustomElliptical",

                // Rectangular Surface Sources
                // 2D Rectangular surface sources: directional
                "DirectionalRectangular",
                // 2D Rectangular surface sources: custom
                "CustomRectangular",


                // SURFACE EMITTING BULK SOURCES (3D SURFACE SOURCES)

                // Spherical Surface Sources
                // Spherical surface sources: Lambertian
                "LambertianSurfaceEmittingSpherical", // e.g. change to LambertianSphericalSurface
                // Spherical surface sources: custom
                "CustomSurfaceEmittingSpherical",

                // Cubiodal Surface Sources
                // Cubiodal surface sources: Lambertian surface emitting cubiodal
                "LambertianSurfaceEmittingCubiodal",

                // Tubular Sources
                // Tubular sources: Lambertian
                "LambertianSurfaceEmittingTubular",

                //Cylindrical Fiber Source
                // Cylindrical fiber sources: Lambertian cylindrial fiber
                "LambertianSurfaceEmittingCylindricalFiber",


                // VOLUMETRIC SOURCES (3D)

                // Cubiodal Volume Sources
                // 3D Cubiodal volume sources: isotropic
                "IsotropicVolumetricCuboidal",
                // 3D Cubiodal volume sources: custom
                "CustomVolumetricCubiodal",

                // Ellipsoidal Volume Sources
                // 3D Ellipsoidal volume sources: isotropic
                "IsotropicVolumetricEllipsoidal",
                // 3D Ellipsoidal volume sources: custom
                "CustomVolumetricEllipsoidal",

                // Fluorescence Emission Volume Sources
                "FluorescenceEmissionAOfXAndYAndZ",

                // ...others, based on Fluence or Radiance?                  
            };
    }
    /// <summary>
    /// Source beam profile types
    /// </summary>
    public enum SourceProfileType
    {
        /// <summary>
        /// Flat beam source profile
        /// </summary>
        Flat,
        /// <summary>
        /// Gaussian beam source profile
        /// </summary>
        Gaussian,
        /// <summary>
        /// Arbitrary beam source profile
        /// </summary>
        Arbitrary,
    }
    /// <summary>
    /// Source angle distribution types
    /// </summary>
    public enum AngleDistributionType
    {
        /// <summary>
        /// Collimated source distribution
        /// </summary>
        Collimated,
        /// <summary>
        /// Isotropic source distribution
        /// </summary>
        Isotropic,
        /// <summary>
        /// Angle distributed source distribution
        /// </summary>
        AngleDistributed,
    }
    /// <summary>
    /// Orientation types of the source
    /// </summary>
    public enum SourceOrientationType
    {
        /// <summary>
        /// Angled source type
        /// </summary>
        Angled,
        /// <summary>
        /// Normally-oriented source type
        /// </summary>
        Normal,
    }

    /// <summary>
    /// Tissue types
    /// </summary>
    public static class TissueType
    {
        public static readonly string[] BuiltInTypes = new []
        {
            // SemiInfinite tissue type.  Includes homogeneous tissues.
            "SemiInfinite",
            // Multilayer tissue type.  Includes homogeneous tissues.
            "MultiLayer",
            // Tissue slab with embedded ellipsoid
            "SingleEllipsoid",
            // Tissue slab with multiple embedded ellipsoids
            "MultiEllipsoid",    
            // Tissue slab with embedded voxel
            "SingleVoxel",
            // Tissue slab with embedded infinite cylinder
            "SingleInfiniteCylinder",
            // Multiple (2 right now) concentric infinite cylinder
            "MultiConcentricInfiniteCylinder"
        };
    }
    /// <summary>
    /// Tissue region type, sub-regions of layers
    /// </summary>
    public static class TissueRegionType
    {
        public static readonly string[] BuiltInTypes = new []
        {
            "Voxel",
            "Layer",
            "Ellipsoid",
            "Cylinder",
            "InfiniteCylinder"
        };
    }

    public static class TallyType
    {
        public static readonly string[] BuiltInTypes =
        {
            // Reflectance as a function of source-detector separation (rho) and angle
            "ROfRhoAndAngle",
            // Reflectance as a function of source-detector separation (rho)
            "ROfRho",
            // Reflectance as a function of angle
            "ROfAngle",
            // Reflectance as a function of source-detector separation (rho) and temporal-frequency (omega)
            "ROfRhoAndOmega",
            // Reflectance as a function of source-detector separation (rho) and time
            "ROfRhoAndTime",
            // Reflectance as a function of Cartesian position on the surface of the tissue
            "ROfXAndY",
            // Total diffuse reflectance
            "RDiffuse",
            // Total specular reflectance
            "RSpecular",
            // Reflectance as a function of spatial frequency along the x-axis
            "ROfFx",
            // Reflectance as a function of spatial frequency along the x-axis, and time
            "ROfFxAndTime",
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
            // Transmittance as a function of spatial frequency (fx)
            "TOfFx", 
            // Fluence as a function of source-detector separation (rho) and tissue depth (Z)
            "FluenceOfRhoAndZ",
            // Fluence as a function of source-detector separation (rho) and tissue depth (Z) and time
            "FluenceOfRhoAndZAndTime",
            // Fluence as a function of x, y and z
            "FluenceOfXAndYAndZ",
            // Fluence as a function of x, y, z and omega
            "FluenceOfXAndYAndZAndOmega",
            // Fluence as a function of rho, z and omega
            "FluenceOfRhoAndZAndOmega",
            // Fluence as a function of fx and z
            "FluenceOfFxAndZ",
            // Absorbed energy as a function of source-detector separation (rho) and tissue depth (Z)
            "AOfRhoAndZ",
            // Absorbed energy as a function of X, Y and tissue depth (Z)
            "AOfXAndYAndZ",
            // Total absorbed energy
            "ATotal",
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
            // Volume randiance as a function of source-detector separation (rho), tissue depth (Z) and angle
            "RadianceOfRhoAndZAndAngle",
            // Volume randiance as a function of spatial-frequency (fx), tissue depth (Z) and angle
            "RadianceOfFxAndZAndAngle",
            // Volume randiance as a function of x, y, z, theta and phi
            "RadianceOfXAndYAndZAndThetaAndPhi",
            // perturbation Monte Carlo (pMC) reflectance as a function of source-detector sep. (rho) and time
            "pMCROfRhoAndTime", 
            // perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho)
            "pMCROfRho",
            // perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx)
            "pMCROfFx",
            // perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx) and time
            "pMCROfFxAndTime",
            // differential Monte Carlo (dMC) d(reflectance)/dMua as a function of source-detector separation (rho)
            "dMCdROfRhodMua",
            // differential Monte Carlo (dMC) d(reflectance)/dMus as a function of source-detector separation (rho) 
            "dMCdROfRhodMus",
        };
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
        /// Reflectance as a function of source-detector separation (rho) and temporal-frequency (omega)
        /// </summary>
        public static string ROfRhoAndOmega { get { return "ROfRhoAndOmega"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue
        /// </summary>
        public static string ROfXAndY { get { return "ROfXAndY"; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency along the x-axis
        /// </summary>
        public static string ROfFx { get { return "ROfFx"; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency along the x-axis, and time
        /// </summary>
        public static string ROfFxAndTime { get { return "ROfFxAndTime"; } }
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
        public static string TransmittedDynamicMTOfFxAndSubregionHist { get { return "TransmittedDynamicMTOfFxAndSubregionHist";} }
        /// <summary>
        /// Reflected subregion time as a function of source-detector separation (rho) and tissue region 
        /// </summary>
        public static string ReflectedTimeOfRhoAndSubregionHist { get { return "ReflectedTimeOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Surface radiance as a function of source-detector separation (rho)
        /// </summary>
        public static string RadianceOfRho { get { return "RadianceOfRho"; } }
        /// <summary>
        /// Volume randiance as a function of source-detector separation (rho), tissue depth (Z) and angle
        /// </summary>
        public static string RadianceOfRhoAndZAndAngle { get { return "RadianceOfRhoAndZAndAngle"; } }
        /// <summary>
        /// Volume randiance as a function of spatial frequency (fx), tissue depth (Z) and angle
        /// </summary>
        public static string RadianceOfFxAndZAndAngle { get { return "RadianceOfFxAndZAndAngle"; } }
        /// <summary>
        /// Volume randiance as a function of x, y, z, theta and phi
        /// </summary>
        public static string RadianceOfXAndYAndZAndThetaAndPhi { get { return "RadianceOfXAndYAndZAndThetaAndPhi"; } }
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector sep. (rho) and time
        /// </summary>
        public static string pMCROfRhoAndTime { get { return "pMCROfRhoAndTime"; } } // maybe these should be in separate enum?
        /// <summary>
        /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho)
        /// </summary>
        public static string pMCROfRho { get { return "pMCROfRho"; } }
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
