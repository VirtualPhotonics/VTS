using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;
using Vts;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;

// Example 10: run R(fx) detector results

// set number of photons
var numPhotons = 1000;

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
    Regions = new ITissueRegion[]
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
};

// define a single R(fx) detector at spatial frequencies fx
var detectorInput = new ROfFxDetectorInput { Fx = new(0, 1, 101), TallySecondMoment = true };

// create a SimulationInput object to define the simulation
var simulationInput = new SimulationInput
{
    N = numPhotons,
    SourceInput = sourceInput,
    TissueInput = tissueInput,
    DetectorInputs = new IDetectorInput[] { detectorInput },
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

// plot the results
var detectorMidpoints = detectorInput.Fx.GetMidpoints();
var reflectance = simulationOutput.R_fx.Select(r => r.Magnitude).ToArray();
Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: reflectance
    ).WithTraceInfo("R vs fx [unitless]", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("fx [mm-1]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("R(fx) [unitless]"))
     .Show();