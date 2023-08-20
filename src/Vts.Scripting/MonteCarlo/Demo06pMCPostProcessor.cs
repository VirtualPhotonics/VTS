namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate using the Perturbation Monte Carlo post-processor
/// </summary>
internal class Demo06PmcPostProcessor : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 06: run a Monte Carlo simulation with Perturbation Monte Carlo (pMC) post-processing enabled.
        // First run a simulation, then post-process the generated database with pMC, varying optical properties post-simulation
        // Notes:
        //    - default source is a point source beam normally incident at the origin 

        // create a SimulationInput object to define the simulation
        var tissueInput = new MultiLayerTissueInput
        {
            Regions = new ITissueRegion[]
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
        var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
        var simulationInput = new SimulationInput
        {
            N = 1000,
            TissueInput = tissueInput,
            DetectorInputs = new IDetectorInput[] { }, // leaving this empty - no on-the-fly detectors needed!
            OutputName = "results",
            Options = new SimulationOptions
            {
                Seed = 0,
                Databases = new [] { DatabaseType.pMCDiffuseReflectance }
            }
        };

        // create and run the simulation, creating the database
        var simulation = new MonteCarloSimulation(input: simulationInput);
        var simulationOutput = simulation.Run();

        // specify post-processing of three post-processor detectors with 1x, 0.5x, and 2x the baseline mua (0.01)
        var pmcDetectorInput1xMua = new pMCROfRhoDetectorInput
        { 
            Rho = detectorRange, 
            Name = "ROfRho", 
            PerturbedOps = new OpticalProperties[]
            {
                new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0), // air optical properties
                new(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4),  // tissue optical properties (baseline)
                new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
            },
            PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
        };
        var pmcDetectorInput0p5xMua = new pMCROfRhoDetectorInput
        {
            Rho = detectorRange,
            Name = "ROfRhoMuaHalf",
            PerturbedOps = new OpticalProperties[]
            {
                new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0), // air optical properties
                new(mua: 0.005, musp: 1.0, g: 0.8, n: 1.4),  // tissue optical properties (half baseline)
                new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
            },
            PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
        };
        var pmcDetectorInput2xMua = new pMCROfRhoDetectorInput
        {
            Rho = detectorRange,
            Name = "ROfRhoMuaDouble",
            PerturbedOps = new OpticalProperties[]
            {
                new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0), // air optical properties
                new(mua: 0.02, musp: 1.0, g: 0.8, n: 1.4),  // tissue optical properties (double baseline)
                new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
            },
            PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
        };

        // create and run the post-processor
        var photonDatabase = PhotonDatabaseFactory.GetpMCDatabase(
            virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance,
            filePath: simulationOutput.Input.OutputName);
        var postProcessor = new PhotonDatabasePostProcessor(
            virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance, 
            detectorInputs: new IDetectorInput[] { pmcDetectorInput1xMua, pmcDetectorInput0p5xMua, pmcDetectorInput2xMua },
            database: photonDatabase,
            databaseInput: simulationOutput.Input);
        var postProcessorOutput = postProcessor.Run();

        // plot and compare the results using Plotly.NET
        var postProcessorDetectorResults1xMua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pmcDetectorInput1xMua.Name];
        var postProcessorDetectorResults0p5xMua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pmcDetectorInput0p5xMua.Name];
        var postProcessorDetectorResults2xMua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pmcDetectorInput2xMua.Name];
        var logReflectance1 = postProcessorDetectorResults1xMua.Mean.Select(r => Math.Log(r)).ToArray();
        var logReflectance2 = postProcessorDetectorResults0p5xMua.Mean.Select(r => Math.Log(r)).ToArray();
        var logReflectance3 = postProcessorDetectorResults2xMua.Mean.Select(r => Math.Log(r)).ToArray();
        var (detectorMidpoints, xLabel, yLabel) = (detectorRange.GetMidpoints(), "ρ [mm]", "log(R(ρ)) [mm-2]");
        var chart = Chart.Combine(new[]
        {
            LineChart(detectorMidpoints, logReflectance1, xLabel, yLabel, title: $"log(R(ρ)) [mm-2] - 1.0x baseline (mua={0.01:F3}/mm)"),
            LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: $"log(R(ρ)) [mm-2] - 0.5x baseline (mua={0.005:F3}/mm)"),
            LineChart(detectorMidpoints, logReflectance3, xLabel, yLabel, title: $"log(R(ρ)) [mm-2] - 2.0x baseline (mua={0.02:F3}/mm)")
        }); // show all three charts together

        if (showPlots)
        {
            chart.Show();
        }
    }
}