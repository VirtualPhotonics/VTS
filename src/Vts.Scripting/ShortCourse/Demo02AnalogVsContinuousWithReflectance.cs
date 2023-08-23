namespace Vts.Scripting.ShortCourse;

/// <summary>
/// Class using the Vts.dll library to demonstrate performing a Monte Carlo simulation 
/// to estimate reflectance versus radial position using different absorption weighting methods
/// </summary>
internal class Demo02AnalogVsContinuousWithReflectance : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Short Course Monte Carlo Example 02: run a Monte Carlo simulation to estimate reflectance versus
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
        var detectorInputs = new IDetectorInput[]
        {
            new ROfRhoDetectorInput { Rho = rhoRange, TallySecondMoment = true, Name = "ROfRho" }, // name can be whatever you want
        };

        // define how many different absorption weighting types to simulate
        var absorptionWeightingTypes = new[] {  AbsorptionWeightingType.Analog, AbsorptionWeightingType.Continuous };

        // create an array of simulations, one for each different photon count value
        var allSimulations = absorptionWeightingTypes.Select(weightingType => 
            new MonteCarloSimulation(
                new SimulationInput(
                    numberOfPhotons: 10000,
                    outputName: "results",
                    simulationOptions: new SimulationOptions
                    {
                        Seed = 0, // -1 will generate a random seed
                        AbsorptionWeightingType = weightingType,
                        PhaseFunctionType = PhaseFunctionType.HenyeyGreenstein
                    },
                    sourceInput: sourceInput,
                    tissueInput: tissueInput,
                    detectorInputs: detectorInputs)
            )).ToArray();

        // run all the simulations in parallel
        var allSimulationOutputs = MonteCarloSimulation.RunAll(allSimulations);

        // collect all the results in helpful arrays
        var allReflectanceDetectors = allSimulationOutputs.Select(detectorResult => 
            (ROfRhoDetector)detectorResult.ResultsDictionary["ROfRho"]).ToArray();
        var allReflectanceMeans = allReflectanceDetectors.Select(detector => detector.Mean).ToArray();
        var allReflectanceSecondMoments = allReflectanceDetectors.Select(detector => detector.SecondMoment).ToArray();

        // compute the relative error (standard deviation / mean) for each simulation
        var allRelativeErrors = absorptionWeightingTypes.Select((awt, awtIdx) =>
            allReflectanceMeans[awtIdx].Zip(allReflectanceSecondMoments[awtIdx], (mean, secondMoment) => 
                Math.Sqrt((secondMoment - mean * mean) / allSimulations[awtIdx].Input.N) / mean).ToArray()).ToArray();

        // compute the relative error difference between the analog and discrete absorption weighting simulations
        var analogRelativeErrors = allRelativeErrors[0];
        var continuousRelativeErrors = allRelativeErrors[1];
        var relativeErrorDifference = analogRelativeErrors.Zip(continuousRelativeErrors, (a, c) => a - c).ToArray();

        // plot the results using Plotly.NET
        var rhos = rhoRange.GetMidpoints();
        var (aawIdx, cawIdx) = (0, 1); // (analog, continuous) absorption weighting indices for convenience
        var reflectanceChart = Chart.Combine(new[]
        {
            LineChart(rhos, allReflectanceMeans[aawIdx].Select(f => Math.Log(f)).ToArray(), xLabel: "ρ [mm]", yLabel: $"log(R(ρ) [mm-2])", title: "Analog"),
            LineChart(rhos, allReflectanceMeans[cawIdx].Select(f => Math.Log(f)).ToArray(), xLabel: "ρ [mm]", yLabel: $"log(R(ρ) [mm-2])", title: "CAW")
        });
        var errorChart = LineChart(rhos, relativeErrorDifference, xLabel: "ρ [mm]", yLabel: $"Δ-error(ρ) (Analog-CAW)");
        var combinedChart = Chart.Grid(new[]{ reflectanceChart, errorChart }, nRows: 2, nCols: 1, Pattern: Plotly.NET.StyleParam.LayoutGridPattern.Coupled);

        if (showPlots)
        {
            combinedChart.Show();
        }
    }
}