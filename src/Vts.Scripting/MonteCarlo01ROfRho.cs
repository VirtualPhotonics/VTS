using Vts;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;

// Example 01: run a simple Monte Carlo simulation with 1000 photons.
// Notes:
//    - default source is a point source beam normally incident at the origin 
//    - default tissue is a 100mm thick slab with air-tissue boundary

// create a SimulationInput object to define the simulation
var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
var simulationInput = new SimulationInput
{
    // specify the number of photons to run
    N = 1000,

    // define a single R(rho) detector by the endpoints of rho bins
    DetectorInputs = new [] { new ROfRhoDetectorInput { Rho = detectorRange } },
};

// create the simulation
var simulation = new MonteCarloSimulation(simulationInput);

// run the simulation
var simulationOutput = simulation.Run();

// plot the results using Plotly.NET
var detectorMidpoints = detectorRange.GetMidpoints();
var logReflectance = simulationOutput.R_r.Select(r => Math.Log(r)).ToArray();
Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance
    ).WithTraceInfo("log(R(ρ)) [mm-2]", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"))
     .Show();