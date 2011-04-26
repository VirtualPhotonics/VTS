namespace Vts.MonteCarlo
{
    /// <summary>
    /// All Monte Carlo enums.
    /// </summary>
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
        SpecularReflection = 0x100,
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
        Planar,
        Point,
        Cylindrical,
        Line,
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
        Specular,
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
