namespace Vts.MonteCarlo
{

    /// <summary>
    /// All Monte Carlo enums.
    /// </summary>

    /// ref: http://www.codeproject.com/Articles/37921/Enums-Flags-and-Csharp-Oh-my-bad-pun.aspx
    /// or http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations
    public enum PhotonStateType 
    {
        None = 0x0,
        Alive = 0x1,
        OnBoundary = 0x2,
        ExitedOutTop = 0x4, // these next enums mean Alive = 0
        ExitedOutBottom = 0x8, 
        Absorbed = 0x10,
        KilledOverMaximumPathLength = 0x20,
        KilledOverMaximumCollisions = 0x40,
        KilledRussianRoulette = 0x80,
        ReflectedOffTop = 0x100,
        //NotSet,
        //ExitedOutTop,
        //ExitedOutBottom,
        //ExitedOutSides,
        //Absorbed,
        //KilledOverMaximumPathLength,
        //KilledOverMaximumCollisions,
        //KilledRussianRoulette,
        //PseudoCollision, can't add until change while check in main MC
    }

    // Source enums
    public enum SourceType
    {
        // 0D Sources:

        // Point Sources
        IsotropicPoint,
        DirectionalPoint,
        CustomPoint,

        // 1D Sources:

        // Line Sources
        IsotropicLine,
        DirectionalLine,
        CustomLine,

        // Ring Sources

        // 2D Surface Sources:

        // Circular Surface Sources
        DirectionalCircular,
        CustomCircular,

        // Cubiodal Surface Sources
        LambertianSurfaceEmittingCubiodal,
        CustomSurfaceEmittingCuboidal,

        //Cylindrical Fiber Source
        LambertianCylindricalFiber,

        // Elliptical Surface Sources
        DirectionalElliptical,
        CustomElliptical,

        // Rectangular Surface Sources
        DirectionalRectangular,
        CustomRectangular,

        // Spherical Surface Sources
        LambertianSurfaceEmittingSpherical, // e.g. change to LambertianSphericalSurface
        CustomSurfaceEmittingSpherical,

        // Tube Sources
        LambertianSurfaceEmittingTubular,
        DiffusingFiber, // e.g. a LambertianSurfaceEmittingTubularSource + CustomCircularSource (for the fiber face)

        // 3D Volumetric Sources

        // Cubiodal Volume Sources
        IsotropicVolumetricCuboidal,
        CustomVolumetricCubiodal,

        // Ellipsoidal Volume Sources
        IsotropicVolumetricEllipsoidal,
        CustomVolumetricEllipsoidal,

        // ...others, based on Fluence or Radiance?
    }

    public enum SourceProfileType
    {
        Flat,
        Gaussian,
    }

    public enum BeamType
    {
        Gaussian,
        Flat,
    }
    public enum AngleDistributionType
    {
        Collimated,
        Isotropic,
        AngleDistributed,
    }
    public enum SourceOrientationType
    {
        Angled,
        Normal,
    }
    // Tissue enums
    public enum TissueType
    {
        MultiLayer,  // includes homogenous
        SingleEllipsoid,
    }
    // Detector enums
    public enum DetectorType
    {
        Detector,
        pMCDetector,
    }

    public enum TallyType
    {
        ROfRhoAndAngle,
        ROfRho,
        ROfAngle,
        ROfRhoAndOmega,
        ROfRhoAndTime,
        ROfXAndY,
        RDiffuse,
        RSpecular,
        TOfRhoAndAngle,
        TOfRho,
        TOfAngle,
        TDiffuse,
        FluenceOfRhoAndZ,
        FluenceOfRhoAndZAndTime,
        AOfRhoAndZ,
        ATotal,
        MomentumTransferOfRhoAndZ,
        pMCROfRhoAndTime, // maybe these should be in separate enum?
        pMCROfRho,
    }

    public enum VirtualBoundaryAxisType
    {
        X,
        Y,
        Z
    }
    public enum VirtualBoundaryDirectionType
    {
        Increasing,
        Decreasing
    }
}
