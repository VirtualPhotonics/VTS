using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PostProcessing;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;
using Vts.Factories;
using Vts.Modeling.Optimizers;
using System.ComponentModel;
using System.Numerics;
using Vts.MonteCarlo.PhotonData;

namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate using the Perturbation Monte Carlo post-processor to calculate optical properties (i.e. "inversion")
/// </summary>
public class MC07_pMCInversion : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo()
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
        //        - optimset options selected

        // create a SimulationInput object to define the simulation
        var tissueInput = new MultiLayerTissueInput
        {
            Regions = new[]
            {
                new LayerTissueRegion(
                    zRange: new(double.NegativeInfinity, 0),         // air "z" range
                    op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)), // air optical properties
                new LayerTissueRegion(
                    zRange: new(0, 100),                             // tissue "z" range ("semi-infinite" slab, 100mm thick)
                    op: new(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4)),  // tissue optical properties
                new LayerTissueRegion(
                    zRange: new(100, double.PositiveInfinity),       // air "z" range
                    op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0))  // air optical properties
            }
        };
        var simulationInput = new SimulationInput
        {
            N = 10000,
            TissueInput = tissueInput,
            DetectorInputs = new IDetectorInput[] { }, // leaving this empty - no on-the-fly detectors needed!
            OutputName = "results",
            Options = new SimulationOptions
            {
                //Seed = 0,
                AbsorptionWeightingType = AbsorptionWeightingType.Discrete,
                Databases = new DatabaseType[] { DatabaseType.pMCDiffuseReflectance }
            }
        };

        // create and run the simulation, generating the pMC database
        var simulation = new MonteCarloSimulation(input: simulationInput);
        var simulationOutput = simulation.Run();

        // Create some measurements, based on a Nurbs-based White Monte Carlo forward solver
        var measuredOPs = new OpticalProperties(mua: 0.04, musp: 0.95, g: 0.8, n: 1.4);
        var detectorRange = new DoubleRange(start: 0, stop: 6, number: 7);
        var detectorMidpoints = detectorRange.GetMidpoints();
        var measurementForwardSolver = SolverFactory.GetForwardSolver(ForwardSolverType.Nurbs);
        var measuredData = measurementForwardSolver.ROfRho(measuredOPs, detectorMidpoints);

        // Specify initial guess for optimization equal to the original pMC simulation baseline values
        var baselineOps = tissueInput.Regions[1].RegionOP;
        var initialGuess = new double[] { baselineOps.Mua, baselineOps.Mus };

        // Create an ad-hoc forward solver based on pMC prediction (see implementation below; note: implemented for ROfRho only)
        var pMCForwardSolver = new pMCForwardSolver(detectorRange, simulationInput, simulationInput.OutputName);

        // Run the inversion, based on Levenberg-Marquardt optimization
        var initialGuessOPsAndRhoAxis = new object[] { new[] { baselineOps }, detectorMidpoints }; // "extra" (constant) data for the forward model calls
        var solution = ComputationFactory.SolveInverse(
            forwardSolver: pMCForwardSolver,
            optimizer: new MPFitLevenbergMarquardtOptimizer(),
            solutionDomainType: SolutionDomainType.ROfRho,
            dependentValues: measuredData,
            standardDeviationValues: measuredData,
            inverseFitType: InverseFitType.MuaMusp,
            initialGuessOPsAndRhoAxis
        );
        // convert the returned double[] {mua, musp} array to an OpticalProperties object
        var fitOps = new OpticalProperties(solution[0], solution[1], baselineOps.G, baselineOps.N);

        // For illustration purposes, also run the pMC solver
        var postProcessorDetectorResultsInitialGuess = pMCForwardSolver.ROfRho(baselineOps, detectorMidpoints);
        var postProcessorDetectorResultsFitValues = pMCForwardSolver.ROfRho(fitOps, detectorMidpoints);

        // plot and compare the results using Plotly.NET
        var logReflectance1 = postProcessorDetectorResultsInitialGuess.Select(r => Math.Log(r)).ToArray();
        var logReflectance2 = postProcessorDetectorResultsFitValues.Select(r => Math.Log(r)).ToArray();
        var (xLabel, yLabel) = ("rho [mm]", "log(R(ρ)) [mm-2]");
        Chart.Combine(new[]
        {
            PlotHelper.ScatterChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: "Measured Data"),
            PlotHelper.LineChart(detectorMidpoints, logReflectance1, xLabel, yLabel, title: $"Initial guess (mua={baselineOps.Mua:F3}/mm, musp={baselineOps.Musp:F3}/mm)"),
            PlotHelper.LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: $"Fit result (mua={fitOps.Mua:F3}/mm, musp={fitOps.Musp:F3}/mm)")
        }).Show(); // show all three charts together

        // todo: add convergence error info similar to Matlab:
        // xlabel('\rho [mm]');
        // ylabel('log10(R(\rho))');
        // legend('Meas','IG','Converged','Location','SouthWest');
        // title('Inverse solution using pMC/dMC'); 
        // set(f, 'Name', 'Inverse solution using pMC/dMC');
        // disp(sprintf('Meas =    [%f %5.3f]',measOPs(1),measOPs(2)/(1-0.8)));
        // disp(sprintf('IG =      [%f %5.3f] Chi2=%5.3e',x0(1),x0(2),...
        //     (measData-doInitialGuess.Mean')*(measData-doInitialGuess.Mean')'));
        // disp(sprintf('Conv =    [%f %5.3f] Chi2=%5.3e',recoveredOPs(1),recoveredOPs(2),...
        //     (measData-doRecovered.Mean')*(measData-doRecovered.Mean')'));
        // disp(sprintf('error =   [%f %5.3f]',abs(measOPs(1)-recoveredOPs(1))/measOPs(1),...
        //     abs(measMus-recoveredOPs(2))/measOPs(2)));

    }

    private class pMCForwardSolver : IForwardSolver
    {
        private readonly DoubleRange _detectorRange;
        private readonly SimulationInput _simulationInput;
        private readonly pMCDatabase _photonDatabase;


        public pMCForwardSolver(DoubleRange detectorRange, SimulationInput simulationInput, pMCDatabase photonDatabase)
        {
            _detectorRange = detectorRange;
            _simulationInput = simulationInput;
            _photonDatabase = photonDatabase;
        }

        public pMCForwardSolver(DoubleRange detectorRange, SimulationInput simulationInput, string photonDatabaseFilePath)
            : this(detectorRange, simulationInput, PhotonDatabaseFactory.GetpMCDatabase(
                virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance,
                filePath: photonDatabaseFilePath))
        {
        }

        public double[] ROfRho(OpticalProperties op, double[] rhos)
        {
            // create and run the post-processor
            var pMCDetectorInput = new pMCROfRhoDetectorInput
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
                detectorInputs: new IDetectorInput[] { pMCDetectorInput },
                database: _photonDatabase,
                databaseInput: _simulationInput);
            var postProcessorOutput = postProcessor.Run();
            var postProcessorDetectorResults = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pMCDetectorInput.Name];
            return postProcessorDetectorResults.Mean;
        }

        public double[] ROfRho(OpticalProperties[] ops, double[] rhos)
        {
            return ops.SelectMany(op => ROfRho(op, rhos)).ToArray();   
        }

        #region not implemented
        public double BeamDiameter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event PropertyChangedEventHandler? PropertyChanged;

        public double ROfRho(IOpticalPropertyRegion[] regions, double rho)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRho(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRho(IOpticalPropertyRegion[] regions, double[] rhos)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRho(IOpticalPropertyRegion[][] regions, double[] rhos)
        {
            throw new NotImplementedException();
        }

        public double ROfRhoAndTime(IOpticalPropertyRegion[] regions, double rho, double time)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRhoAndTime(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(IOpticalPropertyRegion[] regions, double[] rhos, double[] times)
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

        public double[] ROfRhoAndTime(IOpticalPropertyRegion[][] regions, double[] rhos, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfRhoAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double rho, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[] regions, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double ROfFx(IOpticalPropertyRegion[] regions, double fx)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFx(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(IOpticalPropertyRegion[] regions, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(IOpticalPropertyRegion[][] regions, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public double ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double time)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFxAndTime(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double fx, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[] regions, double[] fxs, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(IOpticalPropertyRegion[][] regions, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfFxAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double fx, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[] regions, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(IOpticalPropertyRegion[][] regions, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfRhoAndZ(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public Complex[] FluenceOfRhoAndZAndFt(IOpticalPropertyRegion[][] regions, double[] rhos, double[] zs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<IOpticalPropertyRegion[]> regions, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public double ROfRho(OpticalProperties op, double rho)
        {
            throw new NotImplementedException();
        }

        public double ROfRhoAndTime(OpticalProperties op, double rho, double time)
        {
            throw new NotImplementedException();
        }

        public Complex ROfRhoAndFt(OpticalProperties op, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public double ROfFx(OpticalProperties op, double fx)
        {
            throw new NotImplementedException();
        }

        public double ROfFxAndTime(OpticalProperties op, double fx, double time)
        {
            throw new NotImplementedException();
        }

        public Complex ROfFxAndFt(OpticalProperties op, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfRhoAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfRhoAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> FluenceOfRhoAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfFxAndZ(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> FluenceOfFxAndZAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> FluenceOfFxAndZAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> zs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRho(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfRhoAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfRhoAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> rhos, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFx(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<double> ROfFxAndTime(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> times)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Complex> ROfFxAndFt(IEnumerable<OpticalProperties> ops, IEnumerable<double> fxs, IEnumerable<double> fts)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties op, double[] rhos, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(OpticalProperties op, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(OpticalProperties[] ops, double[] fxs)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRho(OpticalProperties[] ops, double rho)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties[] ops, double rho, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfRhoAndTime(OpticalProperties[] ops, double[] rhos, double time)
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

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double[] rhos, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties op, double rho, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties op, double[] rhos, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfRhoAndFt(OpticalProperties[] ops, double rho, double ft)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFx(OpticalProperties[] ops, double fx)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double[] fxs, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties op, double fx, double[] times)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties op, double[] fxs, double time)
        {
            throw new NotImplementedException();
        }

        public double[] ROfFxAndTime(OpticalProperties[] ops, double fx, double time)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double[] fxs, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties op, double fx, double[] fts)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties op, double[] fxs, double ft)
        {
            throw new NotImplementedException();
        }

        public Complex[] ROfFxAndFt(OpticalProperties[] ops, double fx, double ft)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfRhoAndZ(OpticalProperties[] ops, double[] rhos, double[] zs)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfRhoAndZAndTime(OpticalProperties[] ops, double[] rhos, double[] zs, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex[] FluenceOfRhoAndZAndFt(OpticalProperties[] ops, double[] rhos, double[] zs, double[] fts)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfFxAndZ(OpticalProperties[] ops, double[] fxs, double[] zs)
        {
            throw new NotImplementedException();
        }

        public double[] FluenceOfFxAndZAndTime(OpticalProperties[] ops, double[] fxs, double[] zs, double[] times)
        {
            throw new NotImplementedException();
        }

        public Complex[] FluenceOfFxAndZAndFt(OpticalProperties[] ops, double[] fx, double[] zs, double[] fts)
        {
            throw new NotImplementedException();
        }


        #endregion
    }
}