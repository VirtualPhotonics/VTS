namespace Vts.Scripting.ShortCourse;

/// <summary>
/// Class using the Vts.dll library to demonstrate performing a Monte Carlo simulation 
/// to estimate fluence versus radial position and depth for increasing photon counts
/// </summary>
internal class Demo01APhotonCountWithFluence : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Short Course Monte Carlo Example 01a: run a Monte Carlo simulation to estimate fluence versus
        // radial position and depth for increasing photon counts

        // set up the simulation info that will be constant throughout the set of simulations

        // define a point source beam normally incident at the origin
        var sourceInput = new DirectionalPointSourceInput
        {
            SourceType = "DirectionalPoint",
            PointLocation = new(x: 0, y: 0, z: 0),
            Direction = new(ux: 0, uy: 0, uz: 1),
            InitialTissueRegionIndex = 0
        };

        // define a semi-infinite slab tissue geometry with air-tissue boundary (a bottom air layer is necessary)
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

        // define a single fluence(rho,z) detector by the endpoints of rho and z bins
        var rhoRange = new DoubleRange(start: 0, stop: 10, number: 101);
        var zRange = new DoubleRange(start: 0, stop: 10, number: 101);
        var detectorInputs = new IDetectorInput[]
        {
            new FluenceOfRhoAndZDetectorInput { Rho = rhoRange, Z = zRange, TallySecondMoment = true, Name = "FluenceOfRhoAndZ" }, // name can be whatever you want
        };

        // specify simulation options
        var options = new SimulationOptions
        {
            Seed = 0, // -1 will generate a random seed
            AbsorptionWeightingType = AbsorptionWeightingType.Analog,
            PhaseFunctionType = PhaseFunctionType.HenyeyGreenstein
        };

        // define how many different photon counts to simulate
        var numPhotonsArray = new[] { 10, 100, 1000, 10000 };

        // create an array of simulations, one for each different photon count value
        var allSimulations = numPhotonsArray.Select(n => 
            new MonteCarloSimulation(
                new SimulationInput(
                    numberOfPhotons: n,
                    outputName: "results",
                    simulationOptions: options,
                    sourceInput: sourceInput,
                    tissueInput: tissueInput,
                    detectorInputs: detectorInputs)
            )).ToArray();

        // run all the simulations in parallel
        var allSimulationOutputs = MonteCarloSimulation.RunAll(allSimulations);

        // collect all the results in helpful arrays
        var allFluenceDetectors = allSimulationOutputs.Select(detectorResult => 
            (FluenceOfRhoAndZDetector)detectorResult.ResultsDictionary["FluenceOfRhoAndZ"]).ToArray();
        var allFluenceMeans = allFluenceDetectors.Select(detector => detector.Mean.ToEnumerable<double>().ToArray()).ToArray();
        var allFluenceSecondMoments = allFluenceDetectors.Select(detector => detector.SecondMoment.ToEnumerable<double>().ToArray()).ToArray();

        // compute the relative error (standard deviation / mean) for each simulation
        var allRelativeErrors = numPhotonsArray.Select((numPhotons, npIdx) =>
            allFluenceMeans[npIdx].Zip(allFluenceSecondMoments[npIdx], (mean, secondMoment) => 
                Math.Sqrt((secondMoment - mean * mean) / numPhotons) / mean).ToArray()).ToArray();

        // plot the results using Plotly.NET
        var rhos = rhoRange.GetMidpoints();
        var zs = zRange.GetMidpoints();
        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        var charts = numPhotonsArray.Select((numPhotons, npIdx) =>
        {
            var fluenceRowsToPlot = allFluenceMeans[npIdx]
                .Select(f => Math.Log(f)) // take log for visualization purposes (negative infinity/NaN values won't be rendered )
                .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)   
                .ToArray();
            var fluenceDataToPlot = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
            var fluenceMap = Heatmap(values: fluenceDataToPlot, x: allRhos, y: zs,
                xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"log(Φ(ρ, z))");

            var relativeErrorRowsToPlot = allRelativeErrors[npIdx]
                .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)   
                .ToArray();
            var relativeErrorDataToPlot = relativeErrorRowsToPlot.Reverse().Concat(relativeErrorRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
            var relativeErrorMap = Heatmap(values: relativeErrorDataToPlot, x: allRhos, y: zs,
                xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"error(ρ, z)");

            var combined = Chart.Grid(new[]{ fluenceMap, relativeErrorMap }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

            return combined;
        });

        if (showPlots)
        {
            foreach (var chart in charts)
            {
                chart.Show();
            }
        }
    }
}