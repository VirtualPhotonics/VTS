namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate using the Monte Carlo post-processor
/// </summary>
internal class Demo05PostProcessor : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 05: run a Monte Carlo simulation with post-processing enabled. First run a simulation,
        // then post-process the generated database and compare on-the-fly results with post-processed results
        // Notes:
        //    - the on-the-fly ROfRhoDetectorInput is not necessary for post-processing, but is included here for comparison
        //    - default source is a point source beam normally incident at the origin 
        //    - default tissue is a 100mm thick slab with air-tissue boundary

        // create a SimulationInput object to define the simulation
        var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
        var detectorInput = new ROfRhoDetectorInput { Rho = detectorRange, Name = "ROfRho" };
        var simulationInput = new SimulationInput
        {
            N = 1000,
            DetectorInputs = new IDetectorInput[] { detectorInput },
            OutputName = "results",
            Options = new SimulationOptions
            {
                Databases = new [] { DatabaseType.DiffuseReflectance }
            }
        };

        // create and run the simulation
        var simulation = new MonteCarloSimulation(input: simulationInput);
        var simulationOutput = simulation.Run();

        // specify post-processing of generated database 
        var postProcessorInput = new PostProcessorInput
        {
            InputFolder = simulationInput.OutputName,
            DetectorInputs = new IDetectorInput[] { detectorInput },
            DatabaseSimulationInputFilename = "infile"
        };

        // create and run the post-processor
        var photonDatabase = PhotonDatabaseFactory.GetPhotonDatabase(
            virtualBoundaryType: VirtualBoundaryType.DiffuseReflectance,
            filePath: simulationInput.OutputName);
        var postProcessor = new PhotonDatabasePostProcessor(
            virtualBoundaryType: VirtualBoundaryType.DiffuseReflectance, 
            detectorInputs: postProcessorInput.DetectorInputs,
            photonDatabase: photonDatabase,
            databaseInput: simulationInput);
        var postProcessorOutput = postProcessor.Run();

        // plot and compare the results using Plotly.NET
        var onTheFlyDetectorResults = (ROfRhoDetector)simulationOutput.ResultsDictionary[detectorInput.Name];
        var postProcessorDetectorResults = (ROfRhoDetector)postProcessorOutput.ResultsDictionary[detectorInput.Name];
        var logReflectance1 = onTheFlyDetectorResults.Mean.Select(r => Math.Log(r)).ToArray();
        var logReflectance2 = postProcessorDetectorResults.Mean.Select(r => Math.Log(r)).ToArray();
        var (detectorMidpoints, xLabel, yLabel) = (detectorRange.GetMidpoints(), "ρ [mm]", "log(R(ρ)) [mm-2]");
        var chart = Chart.Combine(new[]
        {
            LineChart(detectorMidpoints, logReflectance1, xLabel, yLabel, title: "log(R(ρ)) [mm-2] - on the fly"),
            LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: "log(R(ρ)) [mm-2] - post-processor")
        }); // show both charts together

        if (showPlots)
        {
            chart.Show();
        }
    }
}