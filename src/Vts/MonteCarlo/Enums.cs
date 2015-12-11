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
                /// <summary>
                /// Point sources: isotropic
                /// </summary>
                "IsotropicPoint",
                /// <summary>
                /// Point sources: directional point
                /// </summary>
                "DirectionalPoint",
                /// <summary>
                /// Point sources: custom point
                /// </summary>
                "CustomPoint",

                // Line Sources
                /// <summary>
                /// 1D Line sources: isotropic line
                /// </summary>
                "IsotropicLine",
                /// <summary>
                /// 1D Line sources: directional line 
                /// </summary>
                "DirectionalLine",
                /// <summary>
                /// 1D Line sources: custom
                /// </summary>
                "CustomLine",


                // SURFACE EMITTING FLAT SOURCES (2D SURFACE SOURCES)

                // Circular Surface Sources
                /// <summary>
                /// 2D Circular surface sources: directional 
                /// </summary>
                "DirectionalCircular",
                /// <summary>
                /// 2D Circular surface sources: custom
                /// </summary>
                "CustomCircular",

                // Elliptical Surface Sources
                /// <summary>
                /// 2D Elliptical surface sources: directional 
                /// </summary>
                "DirectionalElliptical",
                /// <summary>
                /// 2D Elliptical surface sources: custom
                /// </summary>
                "CustomElliptical",

                // Rectangular Surface Sources
                /// <summary>
                /// 2D Rectangular surface sources: directional
                /// </summary>
                "DirectionalRectangular",
                /// <summary>
                /// 2D Rectangular surface sources: custom
                /// </summary>
                "CustomRectangular",


                // SURFACE EMITTING BULK SOURCES (3D SURFACE SOURCES)

                // Spherical Surface Sources
                /// <summary>
                /// Spherical surface sources: Lambertian
                /// </summary>
                "LambertianSurfaceEmittingSpherical", // e.g. change to LambertianSphericalSurface
                /// <summary>
                /// Spherical surface sources: custom
                /// </summary>
                "CustomSurfaceEmittingSpherical",

                // Cubiodal Surface Sources
                /// <summary>
                /// Cubiodal surface sources: Lambertian surface emitting cubiodal
                /// </summary>
                "LambertianSurfaceEmittingCubiodal",

                // Tubular Sources
                /// <summary>
                /// Tubular sources: Lambertian
                /// </summary>
                "LambertianSurfaceEmittingTubular",

                //Cylindrical Fiber Source
                /// <summary>
                /// Cylindrical fiber sources: Lambertian cylindrial fiber
                /// </summary>
                "LambertianSurfaceEmittingCylindricalFiber",


                // VOLUMETRIC SOURCES (3D)

                // Cubiodal Volume Sources
                /// <summary>
                /// 3D Cubiodal volume sources: isotropic
                /// </summary>
                "IsotropicVolumetricCuboidal",
                /// <summary>
                /// 3D Cubiodal volume sources: custom
                /// </summary>
                "CustomVolumetricCubiodal",

                // Ellipsoidal Volume Sources
                /// <summary>
                /// 3D Ellipsoidal volume sources: isotropic
                /// </summary>
                "IsotropicVolumetricEllipsoidal",
                /// <summary>
                /// 3D Ellipsoidal volume sources: custom
                /// </summary>
                "CustomVolumetricEllipsoidal",

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
            /// <summary>
            /// SemiInfinite tissue type.  Includes homogeneous tissues.
            /// </summary>
            "SemiInfinite",
            /// <summary>
            /// Multilayer tissue type.  Includes homogeneous tissues.
            /// </summary>
            "MultiLayer",
            /// <summary>
            /// Tissue slab with embedded ellipsoid
            /// </summary>
            "SingleEllipsoid",
            /// <summary>
            /// Tissue slab with multiple embedded ellipsoids
            /// </summary>
            "MultiEllipsoid",            
            /// <summary>
            /// Tissue slab with embedded voxel
            /// </summary>
            "SingleVoxel",
        };
    }

    public static class TissueRegionType
    {
        public static readonly string[] BuiltInTypes = new []
        {
            "Voxel",
            "Layer",
            "Ellipsoid",
            "Cylinder"
        };
    }

    public static class TallyType
    {
        public static readonly string[] BuiltInTypes =
        {
             /// <summary>
            /// Reflectance as a function of source-detector separation (rho) and angle
            /// </summary>
            "ROfRhoAndAngle",
            /// <summary>
            /// Reflectance as a function of source-detector separation (rho)
            /// </summary>
            "ROfRho",
            /// <summary>
            /// Reflectance as a function of angle
            /// </summary>
            "ROfAngle",
            /// <summary>
            /// Reflectance as a function of source-detector separation (rho) and temporal-frequency (omega)
            /// </summary>
            "ROfRhoAndOmega",
            /// <summary>
            /// Reflectance as a function of source-detector separation (rho) and time
            /// </summary>
            "ROfRhoAndTime",
            /// <summary>
            /// Reflectance as a function of Cartesian position on the surface of the tissue
            /// </summary>
            "ROfXAndY",
            /// <summary>
            /// Total diffuse reflectance
            /// </summary>
            "RDiffuse",
            /// <summary>
            /// Total specular reflectance
            /// </summary>
            "RSpecular",
            /// <summary>
            /// Reflectance as a function of spatial frequency along the x-axis
            /// </summary>
            "ROfFx",
            /// <summary>
            /// Reflectance as a function of spatial frequency along the x-axis, and time
            /// </summary>
            "ROfFxAndTime",
            /// <summary>
            /// Transmittance as a function of source-detector separation (rho) and angle
            /// </summary>
            "TOfRhoAndAngle",
            /// <summary>
            /// Transmittance as a functino of source-detector separation (rho)
            /// </summary>
            "TOfRho",
            /// <summary>
            /// Transmittance as a function of angle
            /// </summary>
            "TOfAngle",
            /// <summary>
            /// Transmittance as a function of x and y
            /// </summary>
            "TOfXAndY",
            /// <summary>
            /// Total diffuse transmittance
            /// </summary>
            "TDiffuse",
            /// <summary>
            /// Fluence as a function of source-detector separation (rho) and tissue depth (Z)
            /// </summary>
            "FluenceOfRhoAndZ",
            /// <summary>
            /// Fluence as a function of source-detector separation (rho) and tissue depth (Z) and time
            /// </summary>
            "FluenceOfRhoAndZAndTime",
            /// <summary>
            /// Fluence as a function of x, y and z
            /// </summary>
            "FluenceOfXAndYAndZ",
            /// <summary>
            /// Fluence as a function of x, y, z and omega
            /// </summary>
            "FluenceOfXAndYAndZAndOmega",
            /// <summary>
            /// Absorbed energy as a function of source-detector separation (rho) and tissue depth (Z)
            /// </summary>
            "AOfRhoAndZ",
            /// <summary>
            /// Absorbed energy as a function of X, Y and tissue depth (Z)
            /// </summary>
            "AOfXAndYAndZ",
            /// <summary>
            /// Total absorbed energy
            /// </summary>
            "ATotal",
            /// <summary>
            /// Reflected momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
            /// </summary>
            "ReflectedMTOfRhoAndSubregionHist",
            /// <summary>
            /// Reflected momentum transfer as a function of x, y and tissue region with histogram of MT
            /// </summary>
            "ReflectedMTOfXAndYAndSubregionHist",
            /// <summary>
            /// Transmitted momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
            /// </summary>
            "TransmittedMTOfRhoAndSubregionHist",
            /// <summary>
            /// Transmitted momentum transfer as a function of x, y and tissue region with histogram of MT
            /// </summary>
            "TransmittedMTOfXAndYAndSubregionHist",            
            /// <summary>
            /// Reflected dynamic momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
            /// </summary>
            "ReflectedDynamicMTOfRhoAndSubregionHist",
            /// <summary>
            /// Reflected dynamic momentum transfer as a function of x, y and tissue region with histogram of MT
            /// </summary>
            "ReflectedDynamicMTOfXAndYAndSubregionHist",
            /// <summary>
            /// Transmitted dynamic momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
            /// </summary>
            "TransmittedDynamicMTOfRhoAndSubregionHist",
            /// <summary>
            /// Transmitted dynamic momentum transfer as a function of x, y and tissue region with histogram of MT
            /// </summary>
            "TransmittedDynamicMTOfXAndYAndSubregionHist",    
            /// <summary>
            /// Reflected subregion time as a function of source-detector separation (rho) and tissue region 
            /// </summary>
            "ReflectedTimeOfRhoAndSubregionHist",
            /// <summary>
            /// Surface radiance as a function of source-detector separation (rho)
            /// </summary>
            "RadianceOfRhoAtZ",
            /// <summary>
            /// Volume randiance as a function of source-detector separation (rho), tissue depth (Z) and angle
            /// </summary>
            "RadianceOfRhoAndZAndAngle",
            /// <summary>
            /// Volume randiance as a function of spatial-frequency (fx), tissue depth (Z) and angle
            /// </summary>
            "RadianceOfFxAndZAndAngle",
            /// <summary>
            /// Volume randiance as a function of x, y, z, theta and phi
            /// </summary>
            "RadianceOfXAndYAndZAndThetaAndPhi",
            /// <summary>
            /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector sep. (rho) and time
            /// </summary>
            "pMCROfRhoAndTime", // maybe these should be in separate enum?
            /// <summary>
            /// perturbation Monte Carlo (pMC) reflectance as a function of source-detector separation (rho)
            /// </summary>
            "pMCROfRho",
            /// <summary>
            /// perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx)
            /// </summary>
            "pMCROfFx",
            /// <summary>
            /// perturbation Monte Carlo (pMC) reflectance as a function of spatial frequency (fx) and time
            /// </summary>
            "pMCROfFxAndTime",
            /// <summary>
            /// differential Monte Carlo (dMC) d(reflectance)/dMua as a function of source-detector separation (rho)
            /// </summary>
            "dMCdROfRhodMua",
            /// <summary>
            /// differential Monte Carlo (dMC) d(reflectance)/dMus as a function of source-detector separation (rho) 
            /// </summary>
            "dMCdROfRhodMus",
        };

        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and angle
        /// </summary>
        public static string ROfRhoAndAngle { get { return "ROfRhoAndAngle"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho)
        /// </summary>
        public static string ROfRho { get { return "ROfRho"; } }
        /// <summary>
        /// Reflectance as a function of angle
        /// </summary>
        public static string ROfAngle { get { return "ROfAngle"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and temporal-frequency (omega)
        /// </summary>
        public static string ROfRhoAndOmega { get { return "ROfRhoAndOmega"; } }
        /// <summary>
        /// Reflectance as a function of source-detector separation (rho) and time
        /// </summary>
        public static string ROfRhoAndTime { get { return "ROfRhoAndTime"; } }
        /// <summary>
        /// Reflectance as a function of Cartesian position on the surface of the tissue
        /// </summary>
        public static string ROfXAndY { get { return "ROfXAndY"; } }
        /// <summary>
        /// Total diffuse reflectance
        /// </summary>
        public static string RDiffuse { get { return "RDiffuse"; } }
        /// <summary>
        /// Total specular reflectance
        /// </summary>
        public static string RSpecular { get { return "RSpecular"; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency along the x-axis
        /// </summary>
        public static string ROfFx { get { return "ROfFx"; } }
        /// <summary>
        /// Reflectance as a function of spatial frequency along the x-axis, and time
        /// </summary>
        public static string ROfFxAndTime { get { return "ROfFxAndTime"; } }
        /// <summary>
        /// Transmittance as a function of source-detector separation (rho) and angle
        /// </summary>
        public static string TOfRhoAndAngle { get { return "TOfRhoAndAngle"; } }
        /// <summary>
        /// Transmittance as a functino of source-detector separation (rho)
        /// </summary>
        public static string TOfRho { get { return "TOfRho"; } }
        /// <summary>
        /// Transmittance as a function of angle
        /// </summary>
        public static string TOfAngle { get { return "TOfAngle"; } }
        /// <summary>
        /// Transmittance as a function of Cartesian position on the surface of the tissue
        /// </summary>
        public static string TOfXAndY { get { return "TOfXAndY"; } }
        /// <summary>
        /// Total diffuse transmittance
        /// </summary>
        public static string TDiffuse { get { return "TDiffuse"; } }
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
        public static string ReflectedDynamicMTOfXAndYAndSubregionHist { get { return "ReflectedDynamicMTOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Reflected dynamic momentum transfer as a function of source-detector separation (rho) and tissue region with histogram of MT
        /// </summary>
        public static string TransmittedDynamicMTOfRhoAndSubregionHist { get { return "TransmittedDynamnicMTOfRhoAndSubregionHist"; } }
        /// <summary>
        /// Reflected dynamic momentum transfer as a function of x, y and tissue region with histogram of MT
        /// </summary>
        public static string TransmittedDynamicMTOfXAndYAndSubregionHist { get { return "TransmittedDynamicMTOfXAndYAndSubregionHist"; } }
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
