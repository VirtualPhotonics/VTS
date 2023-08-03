using Vts;
using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;

// Example 04: run a list of Monte Carlo simulations
// create a list of two default SimulationInput with different numbers of photons
// Notes:
//    - default source is a point source beam normally incident at the origin 
//    - default tissue is a 100mm thick slab with air-tissue boundary

// define some shared values between the two simulations
var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
var detectorInput = new ROfRhoDetectorInput { Rho = detectorRange };

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
var detectorMidpoints = detectorRange.GetMidpoints();
var logReflectance1 = simulation1Output.R_r.Select(r => Math.Log(r)).ToArray();
var logReflectance2 = simulation2Output.R_r.Select(r => Math.Log(r)).ToArray();
var chart1 = Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance1
    ).WithTraceInfo("log(R(ρ)) [mm-2] - n=1000", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"));
var chart2 = Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance2
    ).WithTraceInfo("log(R(ρ)) [mm-2] - n=100", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"));

// show both charts together
Chart.Combine(new[] { chart1, chart2} ).Show();