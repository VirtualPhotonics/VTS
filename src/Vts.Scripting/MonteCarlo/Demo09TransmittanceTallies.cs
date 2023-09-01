namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate performing transmittance tallies in Monte Carlo simulations
/// </summary>
internal class Demo09TransmittanceTallies : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 09: run a Monte Carlo simulation for transmittance tallies with 1000 photons

        // specify all detector inputs
        var rhoRange = new DoubleRange(start: 0, stop: 10, number: 101);
        var fxRange = new DoubleRange(start: 0, stop: 0.2, number: 51);
        var tOfRhoDetectorInput = new TOfRhoDetectorInput { Rho = rhoRange, Name = "TOfRho" };
        var tOfFxDetectorInput = new TOfFxDetectorInput { Fx = fxRange, Name = "TOfFx" };

        // create a SimulationInput object to define the simulation
        var simulationInput = new SimulationInput
        {
            // specify the number of photons
            N = 1000,

            // define a semi-infinite slab tissue geometry with air-tissue boundary (a bottom air layer is necessary)
            TissueInput = new MultiLayerTissueInput
            {
                Regions = new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        zRange: new(double.NegativeInfinity, 0),         // air "z" range
                        op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)), // air optical properties
                    new LayerTissueRegion(
                        zRange: new(0, 2),                               // tissue "z" range (slab, 2mm thick)
                        op: new(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4)),  // tissue optical properties
                    new LayerTissueRegion(
                        zRange: new(2, double.PositiveInfinity),         // air "z" range
                        op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0))  // air optical properties
                }
            },

            // specify all three detectors to run in the single simulation
            DetectorInputs = new IDetectorInput[] { tOfRhoDetectorInput, tOfFxDetectorInput}
        };

        // create and run the simulation
        var simulation = new MonteCarloSimulation(simulationInput);
        var simulationOutput = simulation.Run();

        // plot the T(rho) results using Plotly.NET
        var tOfRhoResults = (TOfRhoDetector)simulationOutput.ResultsDictionary[tOfRhoDetectorInput.Name];
        var tOfRhoLogTransmittance = tOfRhoResults.Mean.Select(r => Math.Log(r)).ToArray();
        var (detectorMidpoints, xLabel, yLabel) = (rhoRange.GetMidpoints(), "ρ [mm]", "log(T(ρ)) [mm-2]");
        var chart1 = LineChart(detectorMidpoints, tOfRhoLogTransmittance, xLabel, yLabel, title: "log(T(ρ)) [mm-2]");

        // plot the T(rho) results using Plotly.NET
        var tOfFxResults = (TOfFxDetector)simulationOutput.ResultsDictionary[tOfFxDetectorInput.Name];
        var tOfFxTransmittance = tOfFxResults.Mean.Select(t => t.Magnitude).ToArray();
        var chart2 = LineChart(fxRange.ToArray(), tOfFxTransmittance, xLabel, yLabel, title: "T(fx)) [unitless]");

        if (showPlots)
        {
            chart1.Show();
            chart2.Show();
        }
    }
}