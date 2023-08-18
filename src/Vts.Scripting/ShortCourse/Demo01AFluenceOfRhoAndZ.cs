using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Plotly.NET.CSharp;
using CommunityToolkit.HighPerformance;

namespace Vts.Scripting.ShortCourse;

/// <summary>
/// Class using the Vts.dll library to demonstrate performing a Monte Carlo simulation 
/// to estimate fluence versus radial position and depth for increasing photon counts
/// </summary>
internal class Demo01AFluenceOfRhoAndZ : IDemoScript
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

        // define a single fluence(rho,z) detector by the endpoints of rho and z bins
        var rhoRange = new DoubleRange(start: 0, stop: 10, number: 101);
        var zRange = new DoubleRange(start: 0, stop: 10, number: 101);
        var detectorInputs = new[]
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

        // plot the results using Plotly.NET
        var rhos = rhoRange.GetMidpoints();
        var zs = zRange.GetMidpoints();
        var allRhos = rhos.Select(rho => -rho).Reverse().Concat(rhos).ToArray(); // duplicate for -rho to make symmetric
        var charts = numPhotonsArray.Select((numPhotons, npidx) =>
        {
            var detectorResults = (FluenceOfRhoAndZDetector)allSimulationOutputs[npidx].ResultsDictionary["FluenceOfRhoAndZ"];
            var minValueLog = Math.Log(detectorResults.Mean.ToEnumerable<double>().Where(d => d > 0).Min());
            var fluenceRowsToPlot = Enumerable.Range(0, rhos.Length)
                .Select(ridx => detectorResults.Mean.GetRow(ridx).ToArray() // break the heatmap into rows (inner dimension is zs)   
                .Select(f => f > 0 ? Math.Log(f) : minValueLog).ToArray()) // take log for visualization purposes (replace zeros with min value)
                .ToArray();
            var fluenceDataToPlot = fluenceRowsToPlot.Reverse().Concat(fluenceRowsToPlot).ToArray(); // duplicate for -rho to make symmetric
            var map = Heatmap(values: fluenceDataToPlot, x: allRhos, y: zs,
                xLabel: "ρ [mm]", yLabel: "z [mm]", title: $"log(Φ(ρ, z)");
            return map;
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