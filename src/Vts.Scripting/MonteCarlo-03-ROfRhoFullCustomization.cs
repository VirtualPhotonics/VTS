using Vts;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;

// Example 03: run a Monte Carlo simulation with a fully-customized input
// (values used here are the class defaults)

var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);

// create a SimulationInput object to define the simulation
var simulationInput = new SimulationInput
{
    // specify the number of photons
    N = 1000,

    // define a point source beam normally incident at the origin
    SourceInput = new DirectionalPointSourceInput
    {
        SourceType = "DirectionalPoint",
        PointLocation = new(x: 0, y: 0, z: 0),
        Direction = new(ux: 0, uy: 0, uz: 1),
        InitialTissueRegionIndex = 0
    },

    // define a semi-infinite slab tissue geometry with air-tissue boundary (a bottom air layer is necessary)
    TissueInput = new MultiLayerTissueInput
    {
        Regions = new []
        {
            new LayerTissueRegion(
                zRange: new(double.NegativeInfinity, 0),         // air "z" range
                op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)), // air optical properties
            new LayerTissueRegion(
                zRange: new(0, 100),                             // tissue "z" range ("semi-infinite" slab, 100mm thick)
                op: new(mua: 0.01, musp: 1.0, g: 0.9, n: 1.4)),  // tissue optical properties
            new LayerTissueRegion(
                zRange: new(100, double.PositiveInfinity),       // air "z" range
                op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0))  // air optical properties
        }
    },

    // define a single R(rho) detector by the endpoints of rho bins
    DetectorInputs = new [] 
    { 
        new ROfRhoDetectorInput { Rho = detectorRange, TallySecondMoment = true, Name = "ROfRho" }  // name can be whatever you want
    },

    // specify all simulation options
    Options = new SimulationOptions
    {
        Seed = 0, // -1 will generate a random seed
        AbsorptionWeightingType = AbsorptionWeightingType.Discrete,
        PhaseFunctionType = PhaseFunctionType.HenyeyGreenstein
    }
};

// create the simulation
var simulation = new MonteCarloSimulation(simulationInput);

// run the simulation
var simulationOutput = simulation.Run();

// plot the results using Plotly.NET
var detectorResults = (ROfRhoDetector)simulationOutput.ResultsDictionary["ROfRho"];
var logReflectance = detectorResults.Mean.Select(r => Math.Log(r)).ToArray();
var detectorMidpoints = detectorRange.GetMidpoints();
Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance
    ).WithTraceInfo("log(R(ρ)) [mm-2]", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"))
     .Show();