namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate comparing two Monte Carlo simulations with different photon counts
/// </summary>
internal class Demo04N1000VsN100 : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 04: run a list of Monte Carlo simulations
        // create a list of two default SimulationInput with different numbers of photons
        // Notes:
        //    - default source is a point source beam normally incident at the origin 
        //    - default tissue is a 100mm thick slab with air-tissue boundary

        // define some shared values between the two simulations
        var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
        var detectorInput = new ROfRhoDetectorInput { Rho = detectorRange, Name = "ROfRho" };

        // create two simulations, one with n=1000 and one with n=100
        // note, we are creating a SimulationInput object here in-line
        var simulation1 = new MonteCarloSimulation(input: new SimulationInput
        {
            N = 1000,
            DetectorInputs = new IDetectorInput[] { detectorInput },
            OutputName = "MonteCarlo04ROfRho-n1000"
        });
        var simulation2 = new MonteCarloSimulation(input: new SimulationInput
        {
            N = 100,
            DetectorInputs = new IDetectorInput[] { detectorInput },
            OutputName = "MonteCarlo04ROfRho-n100"
        });

        // run the simulations
        var simulation1Output = simulation1.Run();
        var simulation2Output = simulation2.Run();

        // plot and compare the results using Plotly.NET
        var detectorResults1 = (ROfRhoDetector)simulation1Output.ResultsDictionary[detectorInput.Name];
        var detectorResults2 = (ROfRhoDetector)simulation2Output.ResultsDictionary[detectorInput.Name];
        var logReflectance1 = detectorResults1.Mean.Select(r => Math.Log(r)).ToArray();
        var logReflectance2 = detectorResults2.Mean.Select(r => Math.Log(r)).ToArray();
        var (detectorMidpoints, xLabel, yLabel) = (detectorRange.GetMidpoints(), "ρ [mm]", "log(R(ρ)) [mm-2]");
        var chart = Chart.Combine(new[]
        {
            LineChart(detectorMidpoints, logReflectance1, xLabel, yLabel, title: "log(R(ρ)) [mm-2] - n=1000"),
            LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: "log(R(ρ)) [mm-2] - n=100")
        }); // show both charts together

        if (showPlots)
        {
            chart.Show();
        }
    }
}