using Vts;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;

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
var detectorMidpoints = detectorRange.GetMidpoints();
var chart1 = Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance1
    ).WithTraceInfo("log(R(ρ)) [mm-2] - CAW", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"));
var chart2 = Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance2
    ).WithTraceInfo("log(R(ρ)) [mm-2] - DAW", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"));

// show both charts together
Chart.Combine(new[] { chart1, chart2} ).Show();