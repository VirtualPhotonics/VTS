using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Plotly.NET.CSharp;

namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate comparing Monte Carlo simulations with different absorption weighting types 
/// </summary>
public class MC02_DAWvsCAW : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo()
    {
        // Example 02: run Monte Carlo simulations for two absorption weighting types 
        // with 1000 photons each and compare computation time
        // Notes:
        //    - default source is a point source beam normally incident at the origin 
        //    - default tissue is a 100mm thick slab with air-tissue boundary and optical properties: mua: 0.01, musp: 1.0, g: 0.8, n:1.4

        // define some shared values between the two simulations
        var numPhotons = 1000;
        var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
        var detectorInput = new ROfRhoDetectorInput { Rho = detectorRange, Name = "ROfRho" }; // name can be whatever you want

        // create SimulationInput objects to define the two simulations
        var simulationInput1 = new SimulationInput
        {
            N = numPhotons,
            DetectorInputs = new [] { detectorInput },
            OutputName = "MonteCarlo02ROfRho-DAW",
            Options = new SimulationOptions
            {
                AbsorptionWeightingType = AbsorptionWeightingType.Discrete, // discrete absorption weighting
            },
        };

        var simulationInput2 = new SimulationInput
        {
            N = numPhotons,
            DetectorInputs = new [] { detectorInput },
            OutputName = "MonteCarlo02ROfRho-CAW",
            Options = new SimulationOptions
            {
                AbsorptionWeightingType = AbsorptionWeightingType.Continuous, // continuous absorption weighting
            }
        };

        // create the simulations
        var simulation1 = new MonteCarloSimulation(simulationInput1);
        var simulation2 = new MonteCarloSimulation(simulationInput2);

        // run the simulations
        var simulation1Output = simulation1.Run();
        var simulation2Output = simulation2.Run();

        // plot and compare the results using Plotly.NET
        var detectorResults1 = (ROfRhoDetector)simulation1Output.ResultsDictionary[detectorInput.Name];
        var detectorResults2 = (ROfRhoDetector)simulation2Output.ResultsDictionary[detectorInput.Name];
        var logReflectance1 = detectorResults1.Mean.Select(r => Math.Log(r)).ToArray();
        var logReflectance2 = detectorResults2.Mean.Select(r => Math.Log(r)).ToArray();
        var (detectorMidpoints, xLabel, yLabel) = (detectorRange.GetMidpoints(), "rho [mm]", "log(R(ρ)) [mm-2]");
        Chart.Combine(new[]
        {
            LineChart(detectorMidpoints, logReflectance1, xLabel, yLabel, title: "log(R(ρ)) [mm-2] - CAW"),
            LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: "log(R(ρ)) [mm-2] - DAW")
        }).Show(); // show both charts together
    } 
}