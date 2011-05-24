namespace Vts.MonteCarlo
{

    /// <summary>
    /// All Monte Carlo enums.
    /// </summary>
    public enum PhotonStateType : byte
    {
        NotSet,
        ExitedOutTop,
        ExitedOutBottom,
        ExitedOutSides,
        Absorbed,
        KilledOverMaximumPathLength,
        KilledOverMaximumCollisions,
        KilledRussianRoulette,
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

        // Elliptical Surface Sources
        DirectionalElliptical,
        CustomElliptical,

        // Rectangular Surface Sources
        DirectionalRectangular,
        CustomRectangular,

        // Spherical Surface Sources
        LambertianSurfaceEmittingSpherical, // e.g. change to LambertianSphericalSurface
        CustomSurfaceEmittingShperical,

        // Tube Sources
        LambertianSurfaceEmittingTubular,

        // 3D Volumetric Sources

        // Cubiodal Volume Sources
        IsotropicCuboidal,
        CustomCubiodal,

        // Ellipsoidal Volume Sources
        IsotropicEllipsoidal,
        CustomEllipsoidal,

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
}
