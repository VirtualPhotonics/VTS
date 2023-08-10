using Vts.Common;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PostProcessing;
using Vts.Scripting.Utilities;
using Plotly.NET.CSharp;

namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate using the Perturbation Monte Carlo post-processor
/// </summary>
public class MC06_pMCPostProcessor : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo()
    {
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
                        op: new(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4)),  // tissue optical properties
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
                Seed = 0,
                Databases = new DatabaseType[] { DatabaseType.pMCDiffuseReflectance }
            }
        };

        // create and run the simulation, creating the database
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
                new(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4),  // tissue optical properties (baseline)
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
                new(mua: 0.005, musp: 1.0, g: 0.8, n: 1.4),  // tissue optical properties (half baseline)
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
                new(mua: 0.02, musp: 1.0, g: 0.8, n: 1.4),  // tissue optical properties (double baseline)
                new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)  // air optical properties
            },
            PerturbedRegionsIndices = new[] { 1 } // only perturbing the tissue OPs 
        };

        // create and run the post-processor
        var photonDatabase = PhotonDatabaseFactory.GetpMCDatabase(
            virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance,
            filePath: simulationInput.OutputName);
        var postProcessor = new PhotonDatabasePostProcessor(
            virtualBoundaryType: VirtualBoundaryType.pMCDiffuseReflectance, 
            detectorInputs: new IDetectorInput[] { pMCDetectorInput1xmua, pMCDetectorInput0p5xmua, pMCDetectorInput2xmua },
            database: photonDatabase,
            databaseInput: simulationInput);
        var postProcessorOutput = postProcessor.Run();

        // plot and compare the results using Plotly.NET
        var postProcessorDetectorResults1xmua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pMCDetectorInput1xmua.Name];
        var postProcessorDetectorResults0p5xmua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pMCDetectorInput0p5xmua.Name];
        var postProcessorDetectorResults2xmua = (pMCROfRhoDetector)postProcessorOutput.ResultsDictionary[pMCDetectorInput2xmua.Name];
        var logReflectance1 = postProcessorDetectorResults1xmua.Mean.Select(r => Math.Log(r)).ToArray();
        var logReflectance2 = postProcessorDetectorResults0p5xmua.Mean.Select(r => Math.Log(r)).ToArray();
        var logReflectance3 = postProcessorDetectorResults2xmua.Mean.Select(r => Math.Log(r)).ToArray();
        var (detectorMidpoints, xLabel, yLabel) = (detectorRange.GetMidpoints(), "rho [mm]", "log(R(ρ)) [mm-2]");
        Chart.Combine(new[]
        {
            PlotHelper.LineChart(detectorMidpoints, logReflectance1, xLabel, yLabel, title: $"log(R(ρ)) [mm-2] - 1.0x baseline (mua={0.01:F3}/mm)"),
            PlotHelper.LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: $"log(R(ρ)) [mm-2] - 0.5x baseline (mua={0.005:F3}/mm)"),
            PlotHelper.LineChart(detectorMidpoints, logReflectance2, xLabel, yLabel, title: $"log(R(ρ)) [mm-2] - 2.0x baseline (mua={0.02:F3}/mm)")
        }).Show(); // show all three charts together
    }
}