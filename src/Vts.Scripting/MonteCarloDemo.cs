using Vts.Common;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo;
using Vts;

var simulationInput = new SimulationInput
    {
        N = 1000, // number of photons
        SourceInput = new DirectionalPointSourceInput
        {
            SourceType = "DirectionalPoint",
            PointLocation = new(x: 0, y: 0, z: 0),
            Direction = new(ux: 0, uy: 0, uz: 1),
            InitialTissueRegionIndex = 0
        },
        // specify a semi-infinite slab with an air-tissue boundary
        TissueInput = new MultiLayerTissueInput
        {
            Regions = new ITissueRegion[]
            {
                // air
                new LayerTissueRegion(
                    zRange: new(double.NegativeInfinity, 0),
                    op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0)),
                 // "semi-infinite" slab (10 meters thick)
                new LayerTissueRegion(
                    zRange: new(0, 10000),
                    op: new(mua: 0.01, musp: 1.0, g: 0.9, n: 1.4)) ,
                // air
                new LayerTissueRegion(
                    zRange: new(10000, double.PositiveInfinity),
                    op: new(mua: 0.0, musp: 1E-10, g: 1.0, n: 1.0))
            }
        },
        // specify a single R(rho) detector by the endpoints of rho bins
        DetectorInputs = new IDetectorInput[] { new ROfFxDetectorInput { Fx = new(0, 1, 101), TallySecondMoment = true } },
        Options = new SimulationOptions
        {
            Seed = 0, // -1 will generate a random seed
            AbsorptionWeightingType = AbsorptionWeightingType.Discrete,
            PhaseFunctionType = PhaseFunctionType.HenyeyGreenstein
        }
    };

var simulation = new MonteCarloSimulation(simulationInput);

var simulationOutput = simulation.Run();