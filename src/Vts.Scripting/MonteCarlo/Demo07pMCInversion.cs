using System.ComponentModel;

namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate using the Perturbation Monte Carlo post-processor to calculate optical properties (i.e. "inversion")
/// </summary>
internal class Demo07PmcInversion : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 07: run a Monte Carlo simulation with Perturbation Monte Carlo post-processor to calculate
        // optical properties (i.e. "inversion") based on fitting to measured data generated using Nurbs
        // Notes:
        //    - default source is a point source beam normally incident at the origin 
        //    - convergence to measured data optical properties affected by:
        //        - number of photons launched in baseline simulation, N
        //        - placement and number of rho
        //        - distance of initial guess from actual
        //        - normalization of chi2
        //        - weighting of chi2 via the standard deviation provided
        //        - optimization options selected

        // create a SimulationInput object to define the simulation
        var tissueInput = new MultiLayerTissueInput
        {
            Regions = new ITissueRegion[]
            {
                new LayerTissueRegion(
                    zRange: new DoubleRange(double.NegativeInfinity, 0),         // air "z" range
                    op: new OpticalProperties(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)), // air optical properties
                new LayerTissueRegion(
                    zRange: new DoubleRange(0, 100),                             // tissue "z" range ("semi-infinite" slab, 100mm thick)
                    op: new OpticalProperties(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4)),  // tissue optical properties
                new LayerTissueRegion(
                    zRange: new DoubleRange(100, double.PositiveInfinity),       // air "z" range
                    op: new OpticalProperties(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0))  // air optical properties
            }
        };
        var simulationInput = new SimulationInput
        {
            N = 1000,
            TissueInput = tissueInput,
            DetectorInputs = Array.Empty<IDetectorInput>(), // leaving this empty - no on-the-fly detectors needed!
            OutputName = "results",
            Options = new SimulationOptions
            {
                //Seed = 0,
                AbsorptionWeightingType = AbsorptionWeightingType.Discrete,
                Databases = new [] { DatabaseType.pMCDiffuseReflectance }
            }
        };

        // create and run the simulation, generating the pMC database
        var simulation = new MonteCarloSimulation(input: simulationInput);
        var simulationOutput = simulation.Run();

        // Create some measurements, based on a Nurbs-based White Monte Carlo forward solver
        var measuredOPs = new OpticalProperties(mua: 0.04, musp: 0.95, g: 0.8, n: 1.4);
        var detectorRange = new DoubleRange(start: 0, stop: 6, number: 7);
        var detectorMidpoints = detectorRange.GetMidpoints();
        var measurementForwardSolver = new NurbsForwardSolver();
        var measuredData = measurementForwardSolver.ROfRho(measuredOPs, detectorMidpoints);

        // Specify initial guess for optimization equal to the original pMC simulation baseline values
        var baselineOps = tissueInput.Regions[1].RegionOP;

        // Create an ad-hoc forward solver based on pMC prediction (see implementation below; note: implemented for ROfRho only)
        var pmcForwardSolver = new PmcForwardSolver(detectorRange, simulationOutput.Input);

        // Run the inversion, based on Levenberg-Marquardt optimization.
        // Note: chi-squared weighting in the inversion is based on standard deviation of measured data, except for
        // the last point, which is set to infinity to fully-deweight it. This is because the last point contains
        // all photon counts for that bin and beyond, and we therefore don't want to include it in the fit
        var initialGuessOPsAndRhoAxis = new object[] { new[] { baselineOps }, detectorMidpoints }; // "extra" (constant) data for the forward model calls
        var solution = ComputationFactory.SolveInverse(
            forwardSolver: pmcForwardSolver,
            optimizer: new MPFitLevenbergMarquardtOptimizer(),
            solutionDomainType: SolutionDomainType.ROfRho,
            dependentValues: measuredData,
            standardDeviationValues: measuredData[..^1].Append(double.PositiveInfinity).ToArray(),
            inverseFitType: InverseFitType.MuaMusp,
            initialGuessOPsAndRhoAxis
        );
        // convert the returned double[] {mua, musp} array to an OpticalProperties object
        var fitOps = new OpticalProperties(solution[0], solution[1], baselineOps.G, baselineOps.N);

        // For illustration purposes, also run the pMC solver
        var postProcessorDetectorResultsInitialGuess = pmcForwardSolver.ROfRho(baselineOps, detectorMidpoints);
        var postProcessorDetectorResultsFitValues = pmcForwardSolver.ROfRho(fitOps, detectorMidpoints);

        // plot and compare the results using Plotly.NET
        var logReflectance1 = postProcessorDetectorResultsInitialGuess.Select(r => Math.Log(r)).ToArray();
        var logReflectance2 = postProcessorDetectorResultsFitValues.Select(r => Math.Log(r)).ToArray();
        var (xLabel, yLabel) = ("ρ [mm]", "log(R(ρ)) [mm-2]");
        var chart = Chart.Combine(new[]
        {
            ScatterChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: "Measured Data"),
            LineChart(detectorMidpoints, logReflectance1, xLabel, yLabel, title: $"Initial guess (mua={baselineOps.Mua:F3}/mm, musp={baselineOps.Musp:F3}/mm)"),
            LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: $"Converged result (mua={fitOps.Mua:F3}/mm, musp={fitOps.Musp:F3}/mm)")
        }); // show all three charts together

        if (showPlots)
        {
            chart.Show();
        }
    }

    /// <summary>
    /// The PmcForwardSolver class is a private class that implements the IForwardSolver interface. 
    /// It is used for performing forward simulations using a perturbation Monte Carlo (pMC) approach. 
    /// The class uses a specified detector range, simulation input parameters, and a pMC database 
    /// to calculate reflectance as a function of radial distance for given sets of optical properties.
    /// </summary>
    private class PmcForwardSolver : IForwardSolver
    {
        private readonly DoubleRange _detectorRange;
        private readonly SimulationInput _simulationInput;
        private readonly pMCDatabase _photonDatabase;
        private double _beamDiameter;

        /// <summary>
        /// IForwardSolver implements INotifyPropertyChanged to notify if
        /// a property changes 
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        ///  OnPropertyChanged method to raise the property changed event
        /// </summary>
        /// <param name="name">The property name</param>
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Beam Diameter
        /// </summary>
        public double BeamDiameter
        {
            get => _beamDiameter;
            set
            {
                _beamDiameter = value;
                OnPropertyChanged("BeamDiameter");
            }
        }

        /// <summary>
        /// Initializes a new instance of the PmcForwardSolver class using the specified detector range, simulation input, and photon database.
        /// </summary>
        /// <param name="detectorRange">The range of the detector for the simulation.</param>
        /// <param name="simulationInput">The input parameters for the simulation.</param>
        /// <param name="photonDatabase">The pMC (probability Monte Carlo) database used for the simulation.</param>
        /// <remarks>
        /// This is a private constructor used internally by the class for initialization.
        /// </remarks>
        private PmcForwardSolver(DoubleRange detectorRange, SimulationInput simulationInput, pMCDatabase photonDatabase)
        {
            _detectorRange = detectorRange;
            _simulationInput = simulationInput;
            _photonDatabase = photonDatabase;
        }

        /// <summary>
        /// Initializes a new instance of the PmcForwardSolver class using the specified detector range and simulation input.
        /// </summary>
        /// <param name="detectorRange">The range of the detector for the simulation.</param>
        /// <param name="simulationInput">The input parameters for the simulation.</param>
        /// <remarks>
        /// This constructor uses the output name from the simulation input to create a pMC (perturbation Monte Carlo) database.
        /// The pMC database is retrieved using the PhotonDatabaseFactory's GetpMCDatabase method with the virtual boundary type set to pMCDiffuseReflectance.
        /// </remarks>
        public PmcForwardSolver(DoubleRange detectorRange, SimulationInput simulationInput)
            : this(detectorRange, simulationInput, PhotonDatabaseFactory.GetpMCDatabase(
                virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance,
                filePath: simulationInput.OutputName))
        {
        }

        /// <summary>
        /// Calculates the reflectance as a function of radial distance (rho) for a given set of optical properties.
        /// </summary>
        /// <param name="op">The optical properties of the medium.</param>
        /// <param name="rhos">An array of radial distances at which the reflectance is to be calculated.</param>
        /// <returns>An array of reflectance values corresponding to each radial distance in the input array.</returns>
        public double[] ROfRho(OpticalProperties op, double[] rhos)
        {
            // create and run the post-processor
            var pmcDetectorInput = new pMCROfRhoDetectorInput
            {
                Rho = _detectorRange,
                Name = "ROfRho-OnTheFly",
                PerturbedOps = new OpticalProperties[]
                {
                    new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0), // air optical properties
                    new(mua: op.Mua, musp: op.Musp, g: op.G, n: op.N),  // tissue optical properties (perturbed)
                    new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
                },
                PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
            };
            var postProcessor = new PhotonDatabasePostProcessor(
                virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance,
                detectorInputs: new IDetectorInput[] { pmcDetectorInput },
                database: _photonDatabase,
                databaseInput: _simulationInput);
            var postProcessorOutput = postProcessor.Run();
            var postProcessorDetectorResults = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pmcDetectorInput.Name];
            return postProcessorDetectorResults.Mean;
        }

        /// <summary>
        /// Calculates the reflectance as a function of radial distance (rho) for each set of optical properties provided.
        /// </summary>
        /// <param name="ops">An array of optical properties of the medium.</param>
        /// <param name="rhos">An array of radial distances at which the reflectance is to be calculated.</param>
        /// <returns>An array of reflectance values corresponding to each set of optical properties and each radial distance in the input arrays.</returns>
        public double[] ROfRho(OpticalProperties[] ops, double[] rhos)
        {
            return ops.SelectMany(op => ROfRho(op, rhos)).ToArray();   
        }

        #region Not Implemented
        public double ROfRho(OpticalProperties op, double rho)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRho(OpticalProperties[] ops, double rho)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos)
        {
            throw new NotImplementedException();
        }

        public double ROfRho(IOpticalPropertyRegion[] regions, double rho)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRho(IOpticalPropertyRegion[] regions, double[] rhos)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRho(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRho(IOpticalPropertyRegion[][] regions, double[] rhos)
        {
            throw new NotImplementedException();
        }

        public double ROfRhoAndTime(OpticalProperties op, double rho, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties op, double rho, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double[] times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRhoAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public double ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double[] rhos, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double[] rhos, double[] times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRhoAndTime(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(IOpticalPropertyRegion[][] regions, double[] rhos, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties op, double rho, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public Complex ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfRhoAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double ROfFx(OpticalProperties op, double fx)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(OpticalProperties op, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(OpticalProperties[] ops, double fx)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(OpticalProperties[] ops, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {
            throw new NotImplementedException();
        }

        public double ROfFx(IOpticalPropertyRegion[] regions, double fx)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(IOpticalPropertyRegion[] regions, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFx(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(IOpticalPropertyRegion[][] regions, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public double ROfFxAndTime(OpticalProperties op, double fx, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties op, double fx, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFxAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public double ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFxAndTime(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[][] regions, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties op, double fx, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public Complex ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfFxAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[][] regions, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfRhoAndZ(OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfRhoAndZ(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfRhoAndZAndTime(OpticalProperties[] ops, double[] rhos, double[] zs, double[] times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfRhoAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public Complex[] FluenceOfRhoAndZAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] FluenceOfRhoAndZAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfFxAndZ(OpticalProperties[] ops, double[] fxs, double[] zs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfFxAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfFxAndZAndTime(OpticalProperties[] ops, double[] fxs, double[] zs, double[] times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfFxAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public Complex[] FluenceOfFxAndZAndFt(OpticalProperties[] ops, double[] fx, double[] zs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> FluenceOfFxAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}