
namespace Vts
{
    /// <summary>
    /// ScatteringType used within Modeling\Spectroscopy 
    /// </summary>
    public enum ScatteringType
    {
        /// <summary>
        /// ScatteringType used within PwoerLawScatterer class
        /// </summary>
        PowerLaw,
        /// <summary>
        /// ScatteringType used within IntralipidScatterer class
        /// </summary>
        Intralipid,
        /// <summary>
        /// ScatteringType used within MieScatterer class
        /// </summary>
        Mie,
    }

    /// <summary>
    /// Types of Mie Scatterers
    /// </summary>
    public enum MieScattererType
    {
        /// <summary>
        /// Polystyrene sphere suspension Mie scatterer
        /// </summary>
        PolystyreneSphereSuspension,
        /// <summary>
        /// Non-polysytrene sphere suspension Mie scatterer
        /// </summary>
        Other,
    }

    /// <summary>
    /// Types of Forward solvers in our gui
    /// </summary>
    public enum ForwardSolverType
    {
        /// <summary>
        /// Standard Diffusion Approximation (SDA) with point source forward solver
        /// </summary>
        PointSourceSDA,
        /// <summary>
        /// Standard Diffusion Approximation (SDA) with distributed point source forward solver
        /// </summary>
        DistributedPointSourceSDA,
        /// <summary>
        /// Standard Diffusion Approximation (SDA) with Gaussian distributed source forward solver
        /// </summary>
        DistributedGaussianSourceSDA,
        /// <summary>
        /// delta-P1 forward solver
        /// </summary>
        DeltaPOne,
        /// <summary>
        /// scaled Monte Carlo forward solver
        /// </summary>
        MonteCarlo,
        /// <summary>
        /// scaled Monte Carlo forward solver with non-uniform rational b-splines forward solver
        /// </summary>
        Nurbs,
        /// <summary>
        /// two-layer forward solver based on standard diffusion
        /// </summary>
        TwoLayerSDA,
        //        DiscreteOrdinates
    }

    /// <summary>
    /// spatial-frequency domain types
    /// </summary>
    public enum SpatialDomainType
    {
        /// <summary>
        /// real spatial-frequency domain type
        /// </summary>
        Real,
        /// <summary>
        /// spatial-frequncy domain type
        /// </summary>
        SpatialFrequency,
    }

    /// <summary>
    /// temporal-frequency domain types
    /// </summary>
    public enum TimeDomainType
    {
        /// <summary>
        /// steady-state temporal-frequency domain type
        /// </summary>
        SteadyState,
        /// <summary>
        /// time-domain temporal-frequency domain type
        /// </summary>
        TimeDomain,
        /// <summary>
        /// frequency-domain temporal-frequency domain type
        /// </summary>
        FrequencyDomain,
    }

    /// <summary>
    /// Reflectance solution domain types
    /// </summary>
    public enum SolutionDomainType
    {
        /// <summary>
        /// reflectance as a function of source-detector separation (rho)
        /// </summary>
        ROfRho,
        /// <summary>
        /// reflectance as a function of spatial-frequency (fx)
        /// </summary>
        ROfFx,
        /// <summary>
        /// reflectance as a function of source-detector separation (rho) and time (t)
        /// </summary>
        ROfRhoAndTime,
        /// <summary>
        /// reflectance as a function of spatial-frequency (fx) and time (t)
        /// </summary>
        ROfFxAndTime,
        /// <summary>
        /// reflectance as a function source-detector separation (rho) and temporal-frequency (ft)
        /// </summary>
        ROfRhoAndFt,
        /// <summary>
        /// reflectance as a function of spatial-frequency (fx) and temporal-frequency (ft)
        /// </summary>
        ROfFxAndFt
    }

    /// <summary>
    /// fluence solution domain types 
    /// </summary>
    public enum FluenceSolutionDomainType
    {
        /// <summary>
        /// fluence as a function or source-detector separation (rho) and tissue depth (z)
        /// </summary>
        FluenceOfRhoAndZ,
        /// <summary>
        /// fluence as a function of spatial-frequency (fx) and tissue depth (z)
        /// </summary>
        FluenceOfFxAndZ,
        /// <summary>
        /// fluence as a function or source-detector separation (rho), tissue depth (z) and time (t)
        /// </summary>
        FluenceOfRhoAndZAndTime,
        /// <summary>
        /// fluence as a function of spatial-frequency (fx), tissue depth (z) and time (t)
        /// </summary>
        FluenceOfFxAndZAndTime,
        /// <summary>
        /// fluence as a function of source-detector separation (rho), tissue depth (z) and temporal-frequency (ft)
        /// </summary>
        FluenceOfRhoAndZAndFt,
        /// <summary>
        /// fluence as a function of spatial-frequency (fx), tissue depth (z) and temporal-frequency (ft)
        /// </summary>
        FluenceOfFxAndZAndFt
    }

    /// <summary>
    /// forward analysis types
    /// </summary>
    public enum ForwardAnalysisType
    {
        /// <summary>
        /// reflectance forward analysis type
        /// </summary>
        R,
        /// <summary>
        /// the derivative of reflectance (R) with respect to absorption coefficient (mua)
        /// </summary>
        dRdMua,
        /// <summary>
        /// the derivative of reflectance (R) with respect to reduced scattering coefficient (musp)
        /// </summary>
        dRdMusp,
        /// <summary>
        /// the derivative of reflectance (R) with respect to anisotropy coefficient (g)
        /// </summary>
        dRdG,
        /// <summary>
        /// the derivative of reflectance (R) with respect to refractive index (n)
        /// </summary>
        dRdN,
        //dRdIV,
    }

    /// <summary>
    /// map plot types
    /// </summary>
    public enum MapType
    {
        /// <summary>
        /// fluence map type
        /// </summary>
        Fluence,
        /// <summary>
        /// absorbed energy map type
        /// </summary>
        AbsorbedEnergy,
        /// <summary>
        /// photon hitting density map type
        /// </summary>
        PhotonHittingDensity
    }

    /// <summary>
    /// Available choices for mapping grayscale intensity.  These names taken from matlab.
    /// </summary>
    public enum ColormapType
    {
        /// <summary>
        /// varies smoothly from black through shades of red, orange, and yellow, to white
        /// </summary>
        Hot,
        /// <summary>
        /// ranges from blue to red, and passes through the colors cyan, yellow, and orange
        /// </summary>
        Jet,
        /// <summary>
        /// linear grayscale
        /// </summary>
        Gray,
        /// <summary>
        /// varies the hue component of the hue-saturation-value color model
        /// </summary>
        HSV,
        /// <summary>
        /// grayscale colormap with a higher value for the blue component
        /// </summary>
        Bone,
        /// <summary>
        /// varies smoothly from black to bright copper
        /// </summary>
        Copper,
        /// <summary>
        /// map is digitize to two colors (white and black)
        /// </summary>
        Binary,
    }

    /// <summary>
    /// independent variable axis tyeps
    /// </summary>
    public enum IndependentVariableAxis
    {
        // todo: add combinations (e.g. RhoAndFt, etc) and update ComputationFactory
        /// <summary>
        /// source-detector separation (rho)
        /// </summary>
        Rho,
        /// <summary>
        /// time (t)
        /// </summary>
        Time,
        /// <summary>
        /// spatial-frequency (fx)
        /// </summary>
        Fx,
        /// <summary>
        /// temporal-frequency (ft)
        /// </summary>
        Ft,
        /// <summary>
        /// depth in tissue (z)
        /// </summary>
        Z,
        /// <summary>
        /// wavelength (lambda)
        /// </summary>
        Wavelength
    }

    /// <summary>
    /// independent variable axis unit types.  These are the default units used throughout code
    /// </summary>
    public enum IndependentVariableAxisUnits
    {
        /// <summary>
        /// millimeter (mm)
        /// </summary>
        MM,
        /// <summary>
        /// nanosecond (ns)
        /// </summary>
        NS,
        /// <summary>
        /// millimeter^(-1)
        /// </summary>
        InverseMM,
        /// <summary>
        /// giga-Hertz
        /// </summary>
        GHz,
        /// <summary>
        /// nanometers
        /// </summary>
        NM,
    }

    /// <summary>
    /// dependent variable axis unit types
    /// </summary>
    public enum DependentVariableAxisUnits
    {
        /// <summary>
        /// per millimeter squared [1/(mm * mm)], inverse area
        /// </summary>
        PerMMSquared,
        /// <summary>
        /// per millimeter squared per nanosecond [1/(mm * mm * ns)]
        /// </summary>
        PerMMSquaredPerNS,
        /// <summary>
        /// per millimiter squared per giga-Hertz [1/(mm * mm * GHz)]
        /// </summary>
        PerMMSquaredPerGHz,
        /// <summary>
        /// unitless dependent variance axis units
        /// </summary>
        Unitless,
        /// <summary>
        /// per nanosecond [1/ns]
        /// </summary>
        PerNS,
        /// <summary>
        /// per giga-Hertz [1/GHz]
        /// </summary>
        PerGHz,
        /// <summary>
        /// per millimeter cubed [1/(mm * mm * mm)]
        /// </summary>
        PerMMCubed,
        /// <summary>
        /// per millimeter cubed per nanosecond [1/(mm * mm * mm * ns)]
        /// </summary>
        PerMMCubedPerNS,
        /// <summary>
        /// per millimeter cubed per giga-Hertz [1/(mm * mm * mm * GHz)]
        /// </summary>
        PerMMCubedPerGHz,
        /// <summary>
        /// per millimeter [1/mm]
        /// </summary>
        PerMM,
        /// <summary>
        /// per millimeter per nanosecond [1/(mm * ns)]
        /// </summary>
        PerMMPerNS,
        /// <summary>
        /// per millimeter per giga-Hertz [1/(mm * GHz)]
        /// </summary>
        PerMMPerGHz
    }

    /// <summary>
    /// inverse solution parameter types
    /// </summary>
    public enum InverseFitType
    {
        /// <summary>
        /// fit inverse solution using two parameters: mua and musp (mus')
        /// </summary>
        MuaMusp,
        /// <summary>
        /// fit inverse solution using one parameter: mua
        /// </summary>
        Mua,
        /// <summary>
        /// fit inverse solution using one parameter: musp (mus')
        /// </summary>
        Musp,
        /// <summary>
        /// fit inverse solution using three parameters: mua, musp (mus') and g
        /// </summary>
        MuaMuspG,
    }

    /// <summary>
    /// Analyzer types
    /// </summary>
    public enum AnalyzerType
    {
        /// <summary>
        /// numeric analyzer type 
        /// </summary>
        Numeric,
        /// <summary>
        /// analyzer type that comes from analytic solution
        /// </summary>
        AnalyticSDA
    }

    /// <summary>
    /// types of optimization methods
    /// </summary>
    public enum OptimizerType
    {
        /// <summary>
        /// Levenberg-Marquard from MPFit
        /// </summary>
        MPFitLevenbergMarquardt
    }

    /// <summary>
    /// scaling types
    /// </summary>
    public enum ScalingType
    {
        /// <summary>
        /// linear scaling type
        /// </summary>
        Linear,
        /// <summary>
        /// logarithmic scaling type
        /// </summary>
        Log
    }

    /// <summary>
    /// plot toggle types
    /// </summary>
    public enum PlotToggleType
    {
        /// <summary>
        /// real/imag type
        /// </summary>
        Complex,
        /// <summary>
        /// phase type
        /// </summary>
        Phase,
        /// <summary>
        /// amplitude plot
        /// </summary>
        Amp,
    }

    /// <summary>
    /// plot normalization types
    /// </summary>
    public enum PlotNormalizationType
    {
        /// <summary>
        /// no plot normalization
        /// </summary>
        None,
        /// <summary>
        /// plot normalization relative to plot max
        /// </summary>
        RelativeToMax,
        /// <summary>
        /// plot normalization relative to curve
        /// </summary>
        RelativeToCurve
    }

    /// <summary>
    /// reflectance plot types
    /// </summary>
    public enum ReflectancePlotType
    {
        /// <summary>
        /// forward solver solutions plots
        /// </summary>
        ForwardSolver,
        /// <summary>
        /// measured data plot for inverse solver panel
        /// </summary>
        InverseSolverMeasuredData,
        /// <summary>
        /// forward solver at initial guess for inverse solver panel
        /// </summary>
        InverseSolverAtInitialGuess,
        /// <summary>
        /// forward solver at converged optical properties for inverse solver panel
        /// </summary>
        InverseSolverAtConvergedData,
        /// <summary>
        /// clear plot
        /// </summary>
        Clear
    }

    /// <summary>
    /// Absorption weighting type used within Monte Carlo code
    /// </summary>
    public enum AbsorptionWeightingType
    {
        /// <summary>
        /// analog absorption weighting
        /// </summary>
        Analog,
        /// <summary>
        /// discrete absorption weighting
        /// </summary>
        Discrete,
        /// <summary>
        /// continuous absorption weighting
        /// </summary>
        Continuous,
    }

    /// <summary>
    /// Phase function type used within the Monte Carlo code
    /// </summary>
    public enum PhaseFunctionType
    {
        /// <summary>
        /// Henyey-Greenstein scattering phase function
        /// </summary>
        HenyeyGreenstein,
        /// <summary>
        /// bidirectional scattering phase function
        /// </summary>
        Bidirectional,
    }

    /// <summary>
    /// input parameters types used in the Monte Carlo CommandLine application for parameter sweeps
    /// </summary>
    public enum InputParameterType
    {
        /// <summary>
        /// x position of the source definition
        /// </summary>
        XSourcePosition,
        /// <summary>
        /// y position of the source definition
        /// </summary>
        YSourcePosition,
        /// <summary>
        /// x center position of the embedded ellipse
        /// </summary>
        XEllipsePosition,
        /// <summary>
        /// y center position of the embedded ellipse
        /// </summary>
        YEllipsePosition,
        /// <summary>
        /// z center position of the embedded ellipse
        /// </summary>
        ZEllipsePosition,
        /// <summary>
        /// x-axis radius of embedded ellipse
        /// </summary>
        XEllipseRadius,
        /// <summary>
        /// y-axis radius of embedded ellipse
        /// </summary>
        YEllipseRadius,
        /// <summary>
        /// z-axis radius of embedded ellipse
        /// </summary>
        ZEllipseRadius,
        /// <summary>
        /// absorption coefficient of top layer of tissue
        /// </summary>
        Mua1,
        /// <summary>
        /// absorption coefficient of second layer of tissue
        /// </summary>
        Mua2,
        /// <summary>
        /// scattering coefficient of top layer of tissue
        /// </summary>
        Mus1,
        /// <summary>
        /// scattering coefficient of second layer of tissue
        /// </summary>
        Mus2,
        /// <summary>
        /// anisotropy coefficient of top layer of tissue
        /// </summary>
        G1,
        /// <summary>
        /// anisotropy coefficient of second layer of tissue
        /// </summary>
        G2,
        /// <summary>
        /// refractive index of top layer of tissue
        /// </summary>
        N1,
        /// <summary>
        /// refractive index of second layer of tissue
        /// </summary>
        N2,
        /// <summary>
        /// thickness of top layer of tissue
        /// </summary>
        D1,
        /// <summary>
        /// thickness of second layer of tissue
        /// </summary>
        D2,
    }

    /// <summary>
    /// Random number generator types
    /// </summary>
    public enum RandomNumberGeneratorType
    {
        /// <summary>
        /// 19937 MT by Matsumoto (implemented by Math.NET Numerics)
        /// </summary>
        MersenneTwister,
    }

    #region Spectral Mapping Enums
    /// <summary>
    /// chromophore coefficient types
    /// </summary>
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
    ///// <summary>
    ///// absorption coefficient unit types
    ///// </summary>
    //public enum AbsorptionCoefficientUnits
    //{
    //    /// <summary>
    //    /// units [1/(mm * uM)]
    //    /// </summary>
    //    PerMillimeterPerMicroMolar,
    //    /// <summary>
    //    /// units [1/mm]
    //    /// </summary>
    //    PerMillimeter,
    //}
    /// <summary>
    /// concentration units
    /// </summary>
    public enum ConcentrationUnits
    {
        /// <summary>
        /// [uM] concentraction units
        /// </summary>
        microMolar,
        /// <summary>
        /// percent concentration units
        /// </summary>
        percent
    }
    /// <summary>
    /// units allowed for blood concentration
    /// </summary>
    public enum BloodConcentrationUnit
    {
        /// <summary>
        /// oxy-hemoglobin + deoxy-hemoglobin
        /// </summary>
        OxyPlusDeoxy,
        /// <summary>
        /// total hemoglobin + oxygen saturation
        /// </summary>
        HbTPlusStO2,
        /// <summary>
        /// blood volume + oxygenation
        /// </summary>
        VbPlusOxygenation,
    }
    /// <summary>
    /// chromophore types
    /// </summary>
    public enum ChromophoreType
    {
        /// <summary>
        /// deoxy-hemoglobin
        /// </summary>
        HbO2,
        /// <summary>
        /// hemoglobin
        /// </summary>
        Hb,
        /// <summary>
        /// water
        /// </summary>
        H2O,
        /// <summary>
        /// fat
        /// </summary>
        Fat,
        /// <summary>
        /// melanin
        /// </summary>
        Melanin,
        ///// <summary>
        ///// copper phthalocyanine tetrasulfonic acid
        ///// </summary>
        //CPTA,
        /// <summary>
        /// nigrosin
        /// </summary>
        Nigrosin,
        /// <summary>
        /// baseline chromophore
        /// </summary>
        Baseline
    }
    /// <summary>
    /// tissue types
    /// </summary>
    public enum TissueType
    {
        /// <summary>
        /// skin tissue type
        /// </summary>
        Skin,
        /// <summary>
        /// liver tissue type 
        /// </summary>
        Liver,
        /// <summary>
        /// grey matter of brain tissue type
        /// </summary>
        BrainGrayMatter,
        /// <summary>
        /// white matter of brain tissue type
        /// </summary>
        BrainWhiteMatter,
        /// <summary>
        /// pre-menopausal breast tissue type
        /// </summary>
        BreastPreMenopause,
        /// <summary>
        /// post-menopausal breast tissue type
        /// </summary>
        BreastPostMenopause,
        /// <summary>
        /// intralipid tissue phantom 
        /// </summary>
        IntralipidPhantom,
        /// <summary>
        /// polystyrene sphere suspension tissue phantom
        /// </summary>
        PolystyreneSpherePhantom,
        /// <summary>
        /// custom tissue 
        /// </summary>
        Custom
    }

    ///// <summary>
    // /// Chromophore data distance units. For future GUI spectral upload tool...
    ///// </summary>
    // public enum ChromDataDistanceUnits 
    // {
    //     /// <summary>
    //     /// [1/mm]
    //     /// </summary>
    //     PerMillimeter,
    //     /// <summary>
    //     /// [1/cm]
    //     /// </summary>
    //     PerCentimeter,
    //     /// <summary>
    //     /// [1/m]
    //     /// </summary>
    //     PerMeter,
    // }
    // /// <summary>
    // /// chromophore data concentration units. For future GUI spectral upload tool...
    // /// </summary>
    // public enum ChromDataConcentrationUnits  
    // {
    //     /// <summary>
    //     /// [1/uM]
    //     /// </summary>
    //     PerMicroMolar,
    //     /// <summary>
    //     /// [1/mM]
    //     /// </summary>
    //     PerMilliMolar,
    //     /// <summary>
    //     /// [1/M]
    //     /// </summary>
    //     PerMolar,
    // }

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
    /// <summary>
    /// solver type. Added to determine which panel is in context LMM 
    /// </summary>
    public enum SolverType
    {
        /// <summary>
        /// forward solver
        /// </summary>
        Forward,
        /// <summary>
        /// fluence solver
        /// </summary>
        Fluence,
        /// <summary>
        /// inverse solver
        /// </summary>
        Inverse,
    }

    /// <summary>
    /// Enum to represent the possible wavelength and wavenumber inputs
    /// </summary>
    public enum WavelengthUnit
    {
        /// <summary>
        /// [nm]
        /// </summary>
        Nanometers,
        /// <summary>
        /// [um] microns
        /// </summary>
        Micrometers,
        /// <summary>
        /// [m] meters
        /// </summary>
        Meters,
        /// <summary>
        /// [1/m]
        /// </summary>
        InverseMeters,
        /// <summary>
        /// [1/cm]
        /// </summary>
        InverseCentimeters,
    }

    /// <summary>
    /// Enum to represent the possible absorption coefficient inputs
    /// </summary>
    public enum AbsorptionCoefficientUnit
    {
        /// <summary>
        /// [1/mm]
        /// </summary>
        InverseMillimeters,
        /// <summary>
        /// [1/m]
        /// </summary>
        InverseMeters,
        /// <summary>
        /// [1/cm]
        /// </summary>
        InverseCentimeters,
        /// <summary>
        /// [1/um]
        /// </summary>
        InverseMicrometers,
    }
    /// <summary>
    /// molar unit units
    /// </summary>
    public enum MolarUnit
    {
        /// <summary>
        /// no molar unit
        /// </summary>
        None,
        /// <summary>
        /// molar units
        /// </summary>
        Molar,
        /// <summary>
        /// millimolar units
        /// </summary>
        MilliMolar,
        /// <summary>
        /// micromolar units
        /// </summary>
        MicroMolar,
        /// <summary>
        /// nanomolar units
        /// </summary>
        NanoMolar,
    }
    #endregion
}
