namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate performing a Monte Carlo simulation that tallies in the native spatial frequency domain
/// </summary>
internal class Demo10ROfFx : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 10: run R(fx) detector results

        // create a SimulationInput object to define the simulation
        var fxRange = new DoubleRange(start: 0, stop: .5, number: 51);
        var detectorInput = new ROfFxDetectorInput { Fx = fxRange, TallySecondMoment = true, Name = "ROfFx" };
        var simulationInput = new SimulationInput
        {
            // specify the number of photons to run
            N = 1000,

            // define a single R(fx) detector at spatial frequencies fx
            DetectorInputs = new IDetectorInput[] { detectorInput }
        };

        // create the simulation
        var simulation = new MonteCarloSimulation(simulationInput);

        // run the simulation
        var simulationOutput = simulation.Run();

        // plot the results with Plotly.NET
        var detectorResults = (ROfFxDetector)simulationOutput.ResultsDictionary[detectorInput.Name];
        var complexReflectance = detectorResults.Mean;
        var reflectanceMagnitude = complexReflectance.Select(r => r.Magnitude).ToArray();
        var (detectorMidpoints, xLabel, yLabel) = (fxRange.ToArray(), "fx [mm-1]", "R(fx) [unitless]");
        var chart = LineChart(detectorMidpoints, reflectanceMagnitude, xLabel, yLabel, title: "R vs fx [unitless]");

        if (showPlots)
        {
            chart.Show();
        }
    }
}