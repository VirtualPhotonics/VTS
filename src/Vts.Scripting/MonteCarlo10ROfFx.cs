using Vts;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Plotly.NET.CSharp;

// Example 10: run R(fx) detector results

// create a SimulationInput object to define the simulation
var fxRange = new DoubleRange(start: 0, stop: 40, number: 201);
var detectorInput = new ROfFxDetectorInput { Fx = fxRange, TallySecondMoment = true, Name = "ROfFx" };
var simulationInput = new SimulationInput
{
    // specify the number of photons to run
    N = 1000,

    // define a single R(fx) detector at spatial frequencies fx
    DetectorInputs = new [] { detectorInput }
};

// create the simulation
var simulation = new MonteCarloSimulation(simulationInput);

// run the simulation
var simulationOutput = simulation.Run();

// plot the results with Plotly.NET
var detectorResults = (ROfFxDetector)simulationOutput.ResultsDictionary[detectorInput.Name]);
var complexReflectance = ((ROfFxDetector)simulationOutput.ResultsDictionary[detectorInput.Name]).Mean;
var reflectanceMagnitude = complexReflectance.Select(r => r.Magnitude).ToArray();
Chart.Point<double, double, string>(
        x: fxRange.AsEnumerable().ToArray(),
        y: reflectanceMagnitude
    ).WithTraceInfo("R vs fx [unitless]", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("fx [mm-1]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("R(fx) [unitless]"))
     .Show();