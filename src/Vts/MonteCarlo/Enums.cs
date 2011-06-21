namespace Vts.MonteCarlo
{

    /// <summary>
    /// All Monte Carlo enums.
    /// </summary>

    /// ref: http://www.codeproject.com/Articles/37921/Enums-Flags-and-Csharp-Oh-my-bad-pun.aspx
    /// or http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations
    public enum PhotonStateType 
    {
        ///     |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |
        ///   8000 4000 2000 1000 0800 0400 0200 0100 0080 0040 0020 0010 0008 0004 0002 0001
        ///   <- virtual boundary flags            -> <- transport flags                   ->
        None = 0x0,
        // transport flags
        Alive = 0x1,
        ExitedDomain = 0x2, // do I need this?
        Absorbed = 0x4,
        KilledOverMaximumPathLength = 0x8,
        KilledOverMaximumCollisions = 0x10,
        KilledRussianRoulette = 0x20,
        // virtual boundary flags
        PseudoReflectDomainBoundary = 0x100,
        PseudoTransmitDomainBoundary = 0x200, // these next enums mean Alive = 0
        PseudoReflectInternalBoundary = 0x400,
        PseudoTransmitInternalBoundary = 0x800,
        // DC: these might need to be descriptive enough to pass ContainsPoint
        // or should we redesign transmittance tallies to take z=depth parameter?
        PseudoTransmitTopDomainBoundary = 0x1000, // do I need this?
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
