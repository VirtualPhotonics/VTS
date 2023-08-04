using Vts;
using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PostProcessing;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;

// Example 06: run a Monte Carlo simulation with Perturbation Monte Carlo (pMC) post-processing enabled.
// First run a simulation, then post-process the generated database with pMC, varying optical properties post-simulation
// Notes:
//    - default source is a point source beam normally incident at the origin 

// create a SimulationInput object to define the simulation
var tissueInput = new MultiLayerTissueInput
{
    Regions = new[]
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
var detectorRange = new DoubleRange(start: 0, stop: 40, number: 201);
var simulationInput = new SimulationInput
{
    N = 1000,
    TissueInput = tissueInput,
    DetectorInputs = new IDetectorInput[] { }, // leaving this empty - no on-the-fly detectors needed!
    OutputName = "results",
    Options = new SimulationOptions
    {
        Databases = new DatabaseType[] { DatabaseType.pMCDiffuseTransmittance }
    }
};

// create and run the simulation
var simulation = new MonteCarloSimulation(input: simulationInput);
var simulationOutput = simulation.Run();

// specify post-processing of three post-processor detectors with 1x, 0.5x, and 2x the baseline mua (0.01)
var pMCDetectorInput1xmua = new pMCROfRhoDetectorInput
{ 
    Rho = detectorRange, 
    Name = "ROfRho", 
    PerturbedOps = new OpticalProperties[]
    {
        new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0), // air optical properties
        new(mua: 0.01, musp: 1.0, g: 0.9, n: 1.4),  // tissue optical properties (baseline)
        new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
    },
    PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
};
var pMCDetectorInput0p5xmua = new pMCROfRhoDetectorInput
{
    Rho = detectorRange,
    Name = "ROfRhoMuaHalf",
    PerturbedOps = new OpticalProperties[]
    {
        new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0), // air optical properties
        new(mua: 0.005, musp: 1.0, g: 0.9, n: 1.4),  // tissue optical properties (half baseline)
        new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
    },
    PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
};
var pMCDetectorInput2xmua = new pMCROfRhoDetectorInput
{
    Rho = detectorRange,
    Name = "ROfRhoMuaDouble",
    PerturbedOps = new OpticalProperties[]
    {
        new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0), // air optical properties
        new(mua: 0.02, musp: 1.0, g: 0.9, n: 1.4),  // tissue optical properties (double baseline)
        new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
    },
    PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
};

// create a PostProcessorInput object to define the post-processing, based on the three detector inputs above
var pMCPostProcessorInput = new PostProcessorInput
{
    InputFolder = simulationInput.OutputName,
    DetectorInputs = new IDetectorInput[] { pMCDetectorInput1xmua, pMCDetectorInput0p5xmua, pMCDetectorInput2xmua },
    DatabaseSimulationInputFilename = "infile"
};

// create and run the post-processor
var photonDatabase = PhotonDatabaseFactory.GetPhotonDatabase(
    virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance,
    filePath: simulationInput.OutputName);
var postProcessor = new PhotonDatabasePostProcessor(
    virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance, 
    detectorInputs: pMCPostProcessorInput.DetectorInputs,
    photonDatabase: photonDatabase,
    databaseInput: simulationInput);
var postProcessorOutput = postProcessor.Run();

// plot and compare the results using Plotly.NET
var postProcessorDetectorResults1xmua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pMCDetectorInput1xmua.Name];
var postProcessorDetectorResults0p5xmua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pMCDetectorInput0p5xmua.Name];
var postProcessorDetectorResults2xmua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pMCDetectorInput2xmua.Name];
var logReflectance1 = postProcessorDetectorResults1xmua.Mean.Select(r => Math.Log(r)).ToArray();
var logReflectance2 = postProcessorDetectorResults0p5xmua.Mean.Select(r => Math.Log(r)).ToArray();
var logReflectance3 = postProcessorDetectorResults2xmua.Mean.Select(r => Math.Log(r)).ToArray();
var detectorMidpoints = detectorRange.GetMidpoints();
var chart1 = Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance1
    ).WithTraceInfo("log(R(ρ)) [mm-2] - 1.0x baseline (mua=0.01/mm)", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"));
var chart2 = Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance2
    ).WithTraceInfo("log(R(ρ)) [mm-2] - 0.5x baseline (mua=0.005/mm)", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"));
var chart3 = Chart.Point<double, double, string>(
        x: detectorMidpoints,
        y: logReflectance3
    ).WithTraceInfo("log(R(ρ)) [mm-2] - 2.0x baseline (mua=0.02/mm)", ShowLegend: true)
     .WithXAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("rho [mm]"))
     .WithYAxisStyle<double, double, string>(Title: Plotly.NET.Title.init("log(R(ρ)) [mm-2]"));

// show all three charts together
Chart.Combine(new[] { chart1, chart2, chart3 } ).Show();