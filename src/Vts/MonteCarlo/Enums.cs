using System;
namespace Vts.MonteCarlo
{

    /// <summary>
    /// All Monte Carlo enums.
    /// </summary>

    /// ref: http://www.codeproject.com/Articles/37921/Enums-Flags-and-Csharp-Oh-my-bad-pun.aspx
    /// or http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations
    //[Flags]
    public enum PhotonStateType 
    {
        ///     |    |    |    |    |    |    |    |    |    |    |    |    |    |    |    |
        ///   8000 4000 2000 1000 0800 0400 0200 0100 0080 0040 0020 0010 0008 0004 0002 0001
        ///   <- transport flags                                                           ->
        ///   <- virtual flags these with "0000" added in lowest bits                      ->
        None = 0x0,
        // transport flags
        Alive = 0x1,
        ExitedDomain = 0x2, // do I need this?
        Absorbed = 0x4,
        KilledOverMaximumPathLength = 0x8,
        KilledOverMaximumCollisions = 0x10,
        KilledRussianRoulette = 0x20,
        // the following get set during photon transport in tissue
        PseudoReflectedTissueBoundary = 0x40,
        PseudoTransmittedTissueBoundary = 0x80,
        PseudoSpecularTissueBoundary = 0x100,
        PseudoDosimetryTissueBoundary = 0x200,

        // virtual boundary flags, can we 1-1 map to virtualBoundary "Name"
        // move up to 16th position
        // the following get set when VB hit (after hit tissue boundary)
        PseudoDiffuseReflectanceVirtualBoundary   = 0x10000, 
        PseudoDiffuseTransmittanceVirtualBoundary = 0x20000,
        PseudoSpecularReflectanceVirtualBoundary  = 0x40000,
        PseudoGenericVirtualBoundary              = 0x80000,
        PseudoDosimetryVirtualBoundary            = 0x100000,
    }

    public enum VirtualBoundaryType
    {
        DiffuseReflectance,
        DiffuseTransmittance,
        SpecularReflectance,
        GenericVolumeBoundary,
        Dosimetry,
    }
    public enum BoundaryHitType
    {
        None,
        Virtual,
        Tissue
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
        DosimetryOfRho,
        pMCROfRhoAndTime, // maybe these should be in separate enum?
        pMCROfRho,
    }

}
