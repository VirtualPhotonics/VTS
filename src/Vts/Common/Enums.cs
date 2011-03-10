namespace Vts
{
    public enum ScatteringType
    {
        PowerLaw,
        Intralipid,
        Mie,
    }

    public enum MieScattererType
    {
        PolystyreneSphereSuspension,
        Other,
    }

    public enum ForwardSolverType
    {
        PointSourceSDA,
        DistributedPointSourceSDA,
        DistributedGaussianSourceSDA,
        DeltaPOne,
        MonteCarlo,
        Nurbs,
        pMC,
//        DiscreteOrdinates
    }

    public enum SpatialDomainType
    {
        Real,
        SpatialFrequency,
    }

    public enum TimeDomainType
    {
        SteadyState,
        TimeDomain,
        FrequencyDomain,
    }

    public enum SolutionDomainType
    {
        RofRho,
        RofFx,
        RofRhoAndT,
        RofFxAndT,
        RofRhoAndFt,
        RofFxAndFt
    }
    public enum FluenceSolutionDomainType
    {
        FluenceofRho,
        FluenceofFx,
        FluenceofRhoAndT,
        FluenceofFxAndT,
        FluenceofRhoAndFt,
        FluenceofFxAndFt
    }
    public enum ForwardAnalysisType
    {
        R,
        dRdMua,
        dRdMusp,
        dRdG,
        dRdN,
        //dRdIV,
    }
    public enum MapType
    {
        Fluence,
        AbsorbedEnergy,
        PhotonHittingDensity
    }
    /// <summary>
    /// Available choices for mapping grayscale intensity
    /// </summary>
    public enum ColormapType
    {
        Hot,
        Jet,
        Gray,
        HSV,
        Bone,
        Copper,
        Binary,
    }
    public enum IndependentVariableAxis
    {
        // todo: add combinations (e.g. RhoAndFt, etc) and update ComputationFactory
        Rho,
        T,
        Fx,
        Ft,
        Z,
        Wavelength
    }
    public enum IndependentVariableAxisUnits
    {
        MM,
        NS,
        InverseMM,
        GHz,
        NM
    }
    public enum DependentVariableAxisUnits
    {
        PerMMSquared,
        PerMMSquaredPerNS,
        PerMMSquaredPerGHz,
        Unitless,
        PerNS,
        PerGHz,

        PerMMCubed,
        PerMMCubedPerNS,
        PerMMCubedPerGHz,
        PerMM,
        PerMMPerNS,
        PerMMPerGHz
    }
    public enum InverseFitType
    {
        MuaMusp,
        Mua,
        Musp,
        MuaMuspG,
    }
    public enum AnalyzerType
    {
        Numeric,
        AnalyticSDA
    }
    public enum OptimizerType
    {
        MPFitLevenbergMarquardt
    }

    public enum ScalingType
    {
        Linear,
        Log
    }
    public enum PlotNormalizationType
    {
        None,
        RelativeToMax,
        RelativeToCurve
    }
    public enum MeasuredDataType
    {
        Simulated,
        FromFile
    }
    public enum ReflectancePlotType
    {
        ForwardSolver,
        InverseSolverMeasuredData,
        InverseSolverAtInitialGuess,
        InverseSolverAtConvergedData,
        Clear
    }

    public enum AbsorptionWeightingType
    {
        Analog,
        Discrete,
        Continuous,
    }

    public enum PhaseFunctionType
    {
        HenyeyGreenstein,
        Bidirectional,
    }

    public enum InputParameterType
    {
        XSourcePosition,
        YSourcePosition, 
        XEllipsePosition, 
        YEllipsePosition, 
        ZEllipsePosition, 
        XEllipseRadius, 
        YEllipseRadius, 
        ZEllipseRadius,
        Mua1,
        Mua2,
        Mus1,
        Mus2,
        G1,
        G2,
        N1,
        N2,
        D1,
        D2,
    }

    public enum PostProcessorInputParameterType
    {
        Rho,
        Time,
    }

    public enum RandomNumberGeneratorType
    {
        /// <summary>
        /// 19937 MT by Matsumoto (implemented by Math.NET Numerics)
        /// </summary>
        MersenneTwister,
    }

    #region Spectral Mapping Enums

    public enum ChromophoreCoefficientType
    {
        /// <summary>
        /// Absorption coefficients per unit distance in [1/mm] 
        /// (i.e. 2.303 * Absorbance)
        /// </summary>
        FractionalAbsorptionCoefficient,
        /// <summary>
        /// Extinction coefficients per unit distance per concentration [(1/mm)*(1/microMoloar)]
        /// (i.e. Absorbance / [Concentration * Path-length])
        /// </summary>
        MolarAbsorptionCoefficient,
    }

    public enum AbsorptionCoefficientUnits
    {
        PerMillimeterPerMicroMolar,
        PerMillimeter,
    }

    public enum ConcentrationUnits
    {
        microMolar,
        percent
    }

    public enum BloodConcentrationUnit //units allowed for blood concentration
    {
        OxyPlusDeoxy,
        HbTPlusStO2,
        VbPlusOxygenation,
    }

    public enum ChromophoreType
    {
        HbO2,
        Hb,
        H2O,
        Fat,
        Melanin,
        CPTA,
        Nigrosin,
        Baseline
    }

    public enum TissueType
    {
        Skin,
        Liver,
        BrainGrayMatter,
        BrainWhiteMatter,
        BreastPreMenopause,
        BreastPostMenopause,
        IntralipidPhantom,
        PolystyreneSpherePhantom,
        Custom
    }


    public enum ChromDataDistanceUnits // For future GUI spectral upload tool...
    {
        PerMillimeter,
        PerCentimeter,
        PerMeter,
    }

    public enum ChromDataConcentrationUnits // For future GUI spectral upload tool...
    {
        PerMicroMolar,
        PerMilliMolar,
        PerMolar,
    }

    //public enum ChromDataUnit
    //{
    //    perMillimeter, //units allowed for absorptionCoeff
    //    perCentimeter,

    //    perMolarPerMillimeter, //units allowed for extinctionCoeff
    //    perMilliMolarPerMillimeter,
    //    perMicroMolarPerMillimeter,
    //    perMolarPerCentimeter,
    //    perMilliMolarPerCentimeter,
    //    perMicroMolarPerCentimeter,

    //}
    
    public enum SolverType // Added to determine which panel is in context LMM
    {
        Forward,
        Fluence,
        Inverse,
    }
    #endregion
}
