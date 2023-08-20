namespace Vts.Scripting.MonteCarlo;

/// <summary>
/// Class using the Vts.dll library to demonstrate comparing to well-characterized unit test results
/// </summary>
internal class Demo08UnitTestComparison : IDemoScript
{
    /// <summary>
    /// Sample script to demonstrate this class' stated purpose
    /// </summary>
    public static void RunDemo(bool showPlots = true)
    {
        // Example 08: run a Monte Carlo simulation and verify results with those in unit tests in Visual Studio
        // Spell out all input to ensure same settings as in unit test

        // specify all detector inputs
        var rhoRange = new DoubleRange(start: 0, stop: 10, number: 101);
        var omegaRange = new DoubleRange(start: 0.05, stop: 1, number: 101);
        var timeRange = new DoubleRange(start: 0, stop: 1, number: 101);
        var rOfRhoDetectorInput = new ROfRhoDetectorInput { Rho = rhoRange, Name = "ROfRho" };
        var rOfRhoAndOmegaDetectorInput = new ROfRhoAndOmegaDetectorInput { Rho = rhoRange, Omega = omegaRange, Name = "ROfRhoAndOmega" };
        var rOfRhoAndTimeDetectorInput = new ROfRhoAndTimeDetectorInput { Rho = rhoRange, Time = timeRange, Name = "ROfRhoAndTime" };

        // create a SimulationInput object to define the simulation
        var simulationInput = new SimulationInput
        {
            // specify the number of photons
            N = 100,

            OutputName = "results",

            // define a point source beam normally incident at the origin
            SourceInput = new DirectionalPointSourceInput
            {
                SourceType = "DirectionalPoint",
                PointLocation = new(x: 0, y: 0, z: 0),
                Direction = new(ux: 0, uy: 0, uz: 1),
                InitialTissueRegionIndex = 1
            },

            // define a semi-infinite slab tissue geometry with air-tissue boundary (a bottom air layer is necessary)
            TissueInput = new MultiLayerTissueInput
            {
                Regions = new ITissueRegion[]
                {
                    new LayerTissueRegion(
                        zRange: new(double.NegativeInfinity, 0),         // air "z" range
                        op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)), // air optical properties
                    new LayerTissueRegion(
                        zRange: new(0, 20),                             // tissue "z" range (slab, 20mm thick)
                        op: new(mua: 0.01, musp: 1.0, g: 0.8, n: 1.4)),  // tissue optical properties
                    new LayerTissueRegion(
                        zRange: new(20, double.PositiveInfinity),       // air "z" range
                        op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0))  // air optical properties
                }
            },

            // specify all three detectors to run in the single simulation
            DetectorInputs = new IDetectorInput[] { rOfRhoDetectorInput, rOfRhoAndOmegaDetectorInput, rOfRhoAndTimeDetectorInput},

            // specify all simulation options
            Options = new SimulationOptions
            {
                Seed = 0, // -1 will generate a random seed
                AbsorptionWeightingType = AbsorptionWeightingType.Analog,
                PhaseFunctionType = PhaseFunctionType.HenyeyGreenstein
            }
        };

        // create and run the simulation
        var simulation = new MonteCarloSimulation(simulationInput);
        var simulationOutput = simulation.Run();

        // plot and compare the results using Plotly.NET
        var rOfRhoResults = (ROfRhoDetector)simulationOutput.ResultsDictionary[rOfRhoDetectorInput.Name];
        var rOfRhoAndOmegaResults = (ROfRhoAndOmegaDetector)simulationOutput.ResultsDictionary[rOfRhoAndOmegaDetectorInput.Name];
        var rOfRhoAndTimeResults = (ROfRhoAndTimeDetector)simulationOutput.ResultsDictionary[rOfRhoAndTimeDetectorInput.Name];

        var unitTestsPass = 
            // currently 0.95492965855137191 (good)
            Math.Abs(rOfRhoResults.Mean[0] - 0.95492965855) < 0.00000000001 &&
            // currently [0.9548488285246218, -0.008172650857889858] (bad)
            Math.Abs((rOfRhoAndOmegaResults.Mean[0, 0] - new Complex(0.95492885, -0.000817329)).Magnitude) < 0.00000001 &&
            // currently 95.4929658551372 (good)
            Math.Abs(rOfRhoAndTimeResults.Mean[0, 0] - 95.492965855) < 0.000000001;

        if (!unitTestsPass)
        {
            throw new Exception("Unit tests failed");
        }
    }
}