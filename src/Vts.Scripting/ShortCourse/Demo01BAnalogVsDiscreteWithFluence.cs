namespace Vts.Scripting.ShortCourse;

/// <summary>
/// Class using the Vts.dll library to demonstrate performing a Monte Carlo simulation 
/// to estimate fluence versus radial position using different absorption weighting methods
/// </summary>
internal class Demo01BAnalogVsDiscreteWithFluence : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Short Course Monte Carlo Example 01b: run a Monte Carlo simulation to estimate fluence versus
        // radial position and depth using different absorption weighting methods. This example is similar to Example 01a
        // but runs 8 total combos: the same 4 different photon counts, now for each of 2 absorption weighting combos

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

        // define how many different photon count and absorption weighting combos to simulate
        var weightingTypeArray = new[] { AbsorptionWeightingType.Analog, AbsorptionWeightingType.Discrete };
        var numPhotonsArray = new[] { 10, 100, 1000, 10000 };
        var simulationTuples = weightingTypeArray.SelectMany(wt => numPhotonsArray.Select(np => (numPhotons: np, weightingType: wt))).ToArray();
        // equivalent to:
        //var simulationTuples = new[]
        //{
        //    (numPhotons:    10, weightingType: AbsorptionWeightingType.Analog),
        //    (numPhotons:   100, weightingType: AbsorptionWeightingType.Analog),
        //    (numPhotons:  1000, weightingType: AbsorptionWeightingType.Analog),
        //    (numPhotons: 10000, weightingType: AbsorptionWeightingType.Analog),
        //    (numPhotons:    10, weightingType: AbsorptionWeightingType.Discrete),
        //    (numPhotons:   100, weightingType: AbsorptionWeightingType.Discrete),
        //    (numPhotons:  1000, weightingType: AbsorptionWeightingType.Discrete),
        //    (numPhotons: 10000, weightingType: AbsorptionWeightingType.Discrete),
        //};

        // create an array of simulations, one for each different photon count value
        var allSimulations = simulationTuples.Select(tuple => 
            new MonteCarloSimulation(
                new SimulationInput(
                    numberOfPhotons: tuple.numPhotons,
                    outputName: "results",
                    simulationOptions: new SimulationOptions
                    {
                        Seed = 0, // -1 will generate a random seed
                        AbsorptionWeightingType = tuple.weightingType,
                        PhaseFunctionType = PhaseFunctionType.HenyeyGreenstein
                    },
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
        var allRelativeErrors = simulationTuples.Select((tuple, tupleIdx) =>
            allFluenceMeans[tupleIdx].Zip(allFluenceSecondMoments[tupleIdx], (mean, secondMoment) => 
                Math.Sqrt((secondMoment - mean * mean) / tuple.numPhotons) / mean).ToArray()).ToArray();

        // compute the relative error difference between the analog and discrete absorption weighting simulations
        var analogRelativeErrors = allRelativeErrors.Take(4).ToArray();
        var discreteRelativeErrors = allRelativeErrors.Skip(4).ToArray();
        var relativeErrorDifference = analogRelativeErrors.Zip(discreteRelativeErrors, (analog, discrete) =>
                analog.Zip(discrete, (a, d) => a - d).ToArray()).ToArray();

        // plot the results using Plotly.NET
        var rhos = rhoRange.GetMidpoints();
        var zs = zRange.GetMidpoints();
        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        var fluenceAndRelativeErrorCharts = simulationTuples.Select((tuple, tupleIdx) =>
        {
            var fluenceRowsToPlot = allFluenceMeans[tupleIdx]
                .Select(f => Math.Log(f)) // take log for visualization purposes (negative infinity/NaN values won't be rendered )
                .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)   
                .ToArray();
            var fluenceDataToPlot = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
            var fluenceMap = Heatmap(values: fluenceDataToPlot, x: allRhos, y: zs,
                xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"log(Φ(ρ, z)) - {tuple.weightingType} N={tuple.numPhotons}");

            var relativeErrorRowsToPlot = allRelativeErrors[tupleIdx]
                .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)   
                .ToArray();
            var relativeErrorDataToPlot = relativeErrorRowsToPlot.Reverse().Concat(relativeErrorRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
            var relativeErrorMap = Heatmap(values: relativeErrorDataToPlot, x: allRhos, y: zs,
                xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"error(ρ, z) - {tuple.weightingType} N={tuple.numPhotons}");

            var combined = Chart.Grid(new[]{ fluenceMap, relativeErrorMap }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

            return combined;
        });
        var analogVsDiscreteRelativeErrorCharts = numPhotonsArray.Select((numPhotons, npIdx) =>
        {
            var differenceRowsToPlot = relativeErrorDifference[npIdx]
                .Chunk(zs.Length) // break the heatmap into rows (inner dimension is zs)   
                .ToArray();
            var differenceDataToPlot = differenceRowsToPlot.Reverse().Concat(differenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
            var differenceMap = Heatmap(values: differenceDataToPlot, x: allRhos, y: zs,
                xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"Δ-error(ρ, z)");
            return differenceMap;
        });

        if (showPlots)
        {
            fluenceAndRelativeErrorCharts.ForEach(chart => chart.Show());
            analogVsDiscreteRelativeErrorCharts.ForEach(chart => chart.Show());
        }
    }
}