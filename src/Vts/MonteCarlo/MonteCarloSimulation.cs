using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vts.Common.Logging;
using Vts.IO;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Factories;
#if BENCHMARK
using BenchmarkDotNet.Attributes;
#endif

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Provides main processing for Monte Carlo simulation. 
    /// </summary>
    public class MonteCarloSimulation
    {
        // private properties (fields)
        /// <summary>
        /// Integer index of Simulation specified in infile
        /// </summary>
        private int SimulationIndex { get; set; }

        /// <summary>
        /// Absorption weighting: Discrete, Continuous, Analog, specified in infile
        /// </summary>
        private AbsorptionWeightingType AbsorptionWeightingType { get; set; }

        /// <summary>
        /// flag to track statistics e.g. number of photons out top of tissue, out bottom, etc.
        /// specified in infile
        /// </summary>
        private bool TrackStatistics { get; set; }

        /// <summary>
        /// local variables: general
        /// </summary>
        private readonly ILogger _logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(MonteCarloSimulation));
        private bool _isRunning;
        private bool _isCancelled;
        private bool _resultsAvailable;

        /// <summary>
        /// local variable: input related
        /// </summary>
        private ISource _source;
        private ITissue _tissue;
        private VirtualBoundaryController _virtualBoundaryController;
        private long _numberOfPhotons;
        private DatabaseWriterController _databaseWriterController;
        private pMCDatabaseWriterController _pMcDatabaseWriterController;
        private bool _doPmc;
        private string _outputPath;

        /// <summary>
        /// Default constructor to allow quick-and-easy simulation
        /// </summary>
        public MonteCarloSimulation() : this(new SimulationInput()) { }

        /// <summary>
        /// Constructor that only takes in input and generates a default
        /// for the random number
        /// </summary>
        /// <param name="input">Simulation input</param>
        public MonteCarloSimulation(SimulationInput input)
            : this(input, RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                input.Options.RandomNumberGeneratorType, input.Options.Seed))
        { }

        /// <summary>
        /// Class that takes in SimulationInput and methods to initialize
        /// and execute Monte Carlo simulation
        /// </summary>
        /// <param name="input">SimulationInput</param>
        /// <param name="rng">random number generator already initialized</param>
        public MonteCarloSimulation(SimulationInput input, Random rng)
        {
            Input = input;
            Rng = rng;
            InitializeInputs();
        }

        /// <summary>
        /// Constructor to delay the initialize if we are running parallel
        /// so we can pass in the random number separately in InitializeParallel
        /// </summary>
        /// <param name="input">Simulation input</param>
        /// <param name="isParallel">A Boolean for if parallel</param>
        public MonteCarloSimulation(SimulationInput input, bool isParallel)
        {
            Input = input;
            if (isParallel) return;
            Rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(input.Options.RandomNumberGeneratorType, input.Options.Seed);
            InitializeInputs();
        }

        /// <summary>
        /// The Simulation input
        /// </summary>
        public SimulationInput Input { get; set; }

        /// <summary>
        /// The random number
        /// </summary>
        public Random Rng { get; private set; }

        /// <summary>
        /// Phase function enum type as specified in SimulationOptions
        /// </summary>
        public PhaseFunctionType PhaseFunctionType { get; set; }
        
        /// <summary>
        /// Boolean indicating whether simulation is running or not
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Boolean indicating whether results are available or not
        /// </summary>
        public bool ResultsAvailable => _resultsAvailable;

        /// <summary>
        /// simulation statistics
        /// </summary>
        public SimulationStatistics Statistics { get; set; }
        
        /// <summary>
        /// Results of the simulation 
        /// </summary>
        public SimulationOutput Results { get; private set; }

        /// <summary>
        /// Initializes for parallel processing
        /// </summary>
        /// <param name="rng">random number generator Random class</param>
        public void InitializeParallel(Random rng)
        {
            Rng = rng;
            InitializeInputs();
        }

        /// <summary>
        /// Method to run parallel, individual MC simulations
        /// </summary>
        /// <param name="simulations">array of MonteCarloSimulation</param>
        /// <returns>array of SimulationOutput</returns>
        public static SimulationOutput[] RunAll(MonteCarloSimulation[] simulations)
        {
            var outputs = new SimulationOutput[simulations.Length];
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(simulations, options, (sim, state, index) =>
            {
                try
                {
                    outputs[index] = simulations[index].Run();
                }
                catch
                {
                    Console.WriteLine("Problem occurred running simulation #{0}. Make sure all simulations have distinct 'OutputName' properties?", index);
                }
            });

            return outputs;
        }

        /// <summary>
        /// Method that sets the output path (string) for databases
        /// </summary>
        /// <param name="outputPath">string indicating output path</param>
        public void SetOutputPathForDatabases(string outputPath)
        {
            _outputPath = outputPath;
        }

        /// <summary>
        /// Run the simulation
        /// </summary>
        /// <returns>SimulationOutput</returns>
#if BENCHMARK
        [Benchmark]
#endif
        public virtual SimulationOutput Run()
        {
            _isCancelled = false;
            _isRunning = true;
            _resultsAvailable = false;

            DisplayIntro();

            ExecuteMCLoop();

            _isRunning = false;
            if (_isCancelled)
            {
                _resultsAvailable = false;
                return null;
            }

            var detectors = _virtualBoundaryController.VirtualBoundaries
                    .Select(vb => vb.DetectorController)
                    .Where(dc => dc != null)
                    .SelectMany(dc => dc.Detectors).ToList();

            Results = new SimulationOutput(Input, detectors);

            _resultsAvailable = true;

            return Results;
        }

        /// <summary>
        /// Method to cancel the simulation, for example, from the GUI
        /// </summary>
        public void Cancel()
        {
            _isCancelled = true;
            _logger.Info(() => "Simulation cancelled.\n");
        }

        /// <summary>
        /// Executes the Monte Carlo Loop
        /// </summary>
        protected virtual void ExecuteMCLoop()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (Input.Options.Databases.Any()) InitialDatabases(_doPmc);

                var volumeVBs = _virtualBoundaryController.VirtualBoundaries.Where(
                    v => v.VirtualBoundaryType == VirtualBoundaryType.GenericVolumeBoundary).ToList();

                var parallelOptions = new ParallelOptions();
                parallelOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;

                for (long n = 1; n <= _numberOfPhotons; n++)
                {
                    if (_isCancelled) return;

                    // num photons is assumed to be over 10 
                    if (n % (_numberOfPhotons / 10) == 0) DisplayStatus(n, _numberOfPhotons);

                    var photon = _source.GetNextPhoton(_tissue);

                    do
                    { // begin do while
                        photon.SetStepSize(); // only calls rng if SLeft == 0.0

                        IVirtualBoundary closestVirtualBoundary;

                        // determine BoundaryHitType in MoveToBoundaryCheck
                        var hitType = MoveToBoundaryCheck(photon, out closestVirtualBoundary);

                        // consider moving actual calls to Tally after do-while
                        // for each "hit" virtual boundary, tally respective detectors if exist
                        if (hitType == BoundaryHitType.Virtual &&
                            closestVirtualBoundary.DetectorController != null)
                        {
                            closestVirtualBoundary.DetectorController.Tally(photon);
                        }

                        // kill photon for various reasons, including possible VB crossings
                        photon.TestDeath();

                        // check if virtual boundary 
                        if (hitType == BoundaryHitType.Virtual) continue;

                        // check if tissue boundary and if so, then cross or reflect
                        if (hitType == BoundaryHitType.Tissue)
                        {
                            photon.CrossRegionOrReflect();
                            continue;
                        }

                        photon.Absorb(); // can be added to TestDeath?
                        if (photon.DP.StateFlag.HasFlag(PhotonStateType.Absorbed)) continue;

                        photon.Scatter();

                    } while (photon.DP.StateFlag.HasFlag(PhotonStateType.Alive)); // end do while

                    if (Input.Options.Databases.Any())  WriteToDatabases(_doPmc, photon);

                    // note History has possibly 2 more DPs than linux code due to 
                    // final crossing of PseudoReflectedTissueBoundary and then
                    // PseudoDiffuseReflectanceVB
                    foreach (var vb in volumeVBs)
                    {
                        vb.DetectorController.Tally(photon); // dc: this should use the optimized loop now...
                    }

                    if (!TrackStatistics) continue;
                    Statistics.TrackDeathStatistics(photon.DP);

                } // end of for n loop
            }
            finally
            {
                if (Input.Options.Databases.Any())  CloseDatabases(_doPmc);
            }

            // normalize all detectors by the total number of photons (each tally records it's own "local" count as well)
            if (_virtualBoundaryController != null)
                foreach (var vb in _virtualBoundaryController.VirtualBoundaries)
                {
                    if (vb.DetectorController == null) continue; // check that VB has detectors
                    vb.DetectorController.NormalizeDetectors(_numberOfPhotons);
                }

            if (TrackStatistics)
            {
                if (Input.OutputName == "")
                {
                    Statistics.ToFile("statistics.txt");
                }
                else
                {
                    FileIO.CreateDirectory(Input.OutputName);
                    Statistics.ToFile(Input.OutputName + "/statistics.txt");
                }
            }
            stopwatch.Stop();

            _logger.Info(() => $"Monte Carlo simulation complete (N = {_numberOfPhotons} photons; simulation time = {stopwatch.ElapsedMilliseconds / 1000f} seconds).\r");
        }

        /// <summary>
        /// Initialization of the inputs for the MonteCarloSimulation
        /// </summary>
        private void InitializeInputs()
        {
            // all field/property defaults should be set here
            SimulationIndex = Input.Options.SimulationIndex;
            _outputPath = "";
            _numberOfPhotons = Input.N;
            _tissue = TissueFactory.GetTissue(Input.TissueInput,
                Input.Options.AbsorptionWeightingType,
                Input.Options.PhaseFunctionType,
                Input.Options.RussianRouletteWeightThreshold);

            AbsorptionWeightingType = Input.Options.AbsorptionWeightingType;
            TrackStatistics = Input.Options.TrackStatistics;
            if (TrackStatistics) Statistics = new SimulationStatistics();

            // validate input
            var result = SimulationInputValidation.ValidateInput(Input);
            if (!result.IsValid)  throw new ArgumentException(result.ValidationRule +
                                            (!string.IsNullOrEmpty(result.Remarks) ? "; " + result.Remarks : ""));

            _source = SourceFactory.GetSource(Input.SourceInput, Rng);
            // instantiate Virtual Boundaries (and associated detectors) for each VB group
            _virtualBoundaryController = new VirtualBoundaryController(new List<IVirtualBoundary>());
            var dbVirtualBoundaries = Input.Options.Databases.Select(db => db.GetCorrespondingVirtualBoundaryType()).ToList();
            foreach (var vbType in EnumHelper.GetValues<VirtualBoundaryType>())
            {
                IEnumerable<IDetectorInput> detectorInputs = null;
                detectorInputs = vbType switch
                {
                    VirtualBoundaryType.DiffuseReflectance => Input.DetectorInputs.Where(d => d.TallyDetails.IsReflectanceTally).ToList(),
                    VirtualBoundaryType.DiffuseTransmittance => Input.DetectorInputs.Where(d => d.TallyDetails.IsTransmittanceTally).ToList(),
                    VirtualBoundaryType.SpecularReflectance => Input.DetectorInputs.Where(d => d.TallyDetails.IsSpecularReflectanceTally).ToList(),
                    VirtualBoundaryType.GenericVolumeBoundary => Input.DetectorInputs.Where(d => d.TallyDetails.IsVolumeTally).ToList(),
                    VirtualBoundaryType.InternalSurface => Input.DetectorInputs.Where(d => d.TallyDetails.IsInternalSurfaceTally).ToList(),
                    VirtualBoundaryType.pMCDiffuseReflectance => Input.DetectorInputs.Where(d => d.TallyDetails.IspMCReflectanceTally).ToList(),
                    VirtualBoundaryType.pMCDiffuseTransmittance => Input.DetectorInputs.Where(d => d.TallyDetails.IspMCTransmittanceTally).ToList(),
                    VirtualBoundaryType.BoundingVolume => Input.DetectorInputs.Where(d => d.TallyDetails.IsLateralBoundingVolumeTally).ToList(),
                    _ => throw new ArgumentOutOfRangeException(
                                                "Virtual boundary type not recognized: " + vbType),
                };

                // make sure VB Controller has at least diffuse reflectance and diffuse transmittance
                // may change this in future if tissue OnDomainBoundary changes
                if (!detectorInputs.Any() &&
                    vbType != VirtualBoundaryType.DiffuseReflectance &&
                    vbType != VirtualBoundaryType.DiffuseTransmittance && 
                    dbVirtualBoundaries.All(vb => vb != vbType)) continue;
                var detectors = DetectorFactory.GetDetectors(detectorInputs, _tissue, Rng);
                var detectorController = DetectorControllerFactory.GetDetectorController(vbType, detectors, _tissue);

                var virtualBoundary = VirtualBoundaryFactory.GetVirtualBoundary(vbType, _tissue, detectorController);
                _virtualBoundaryController.VirtualBoundaries.Add(virtualBoundary);
            }

            // set _doPMC flag
            if (Input.Options.Databases.Any(d => d.IspMCDatabase())) _doPmc = true;

            _isCancelled = false;
            _isRunning = false;
            _resultsAvailable = false;
        }

        private void CloseDatabases(bool doPmc)
        {
            if (doPmc)
            {
                _pMcDatabaseWriterController.Dispose();
            }
            else
            {
                _databaseWriterController.Dispose();
            }
        }

        private void WriteToDatabases(bool doPmc, Photon photon)
        {
            if (doPmc)
            {
                _pMcDatabaseWriterController.WriteToSurfaceVirtualBoundaryDatabases(photon.DP,
                    photon.History.SubRegionInfoList);
            }
            else
            {
                _databaseWriterController.WriteToSurfaceVirtualBoundaryDatabases(photon.DP);
            }
        }
        /// <summary>
        /// Initializes databases for diffuse reflectance (1 database) or perturbation
        /// MC (2 databases)
        /// </summary>
        /// <param name="doPmc">flag indicating whether to do pmc or not</param>
        private void InitialDatabases(bool doPmc)
        {
            if (doPmc)
            {
                _pMcDatabaseWriterController = new pMCDatabaseWriterController(
                    DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriters(
                        Input.Options.Databases,
                        _outputPath,
                        Input.OutputName),
                    DatabaseWriterFactory.GetCollisionInfoDatabaseWriters(
                        Input.Options.Databases,
                        _tissue,
                        _outputPath,
                        Input.OutputName));
            }
            else
            {
                _databaseWriterController = new DatabaseWriterController(
                    DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriters(
                        Input.Options.Databases,
                        _outputPath,
                        Input.OutputName));
            }
        }
        /// <summary>
        /// Checks if whether photon is going to first
        /// a) hit tissue boundary (BoundaryHitType.Tissue), and if so Photon.Move to 
        /// intersection position, then
        /// b) Photon.CrossOrReflect checks if at border of system and sets
        /// appropriate photon state flag, e.g. PhotonStateType.PseudoBoundingVolumeTissueBoundary
        /// c) finally sets BoundaryHitType.Virtual and tallies in main MC loop
        /// </summary>
        /// <param name="photon">Photon class</param>
        /// <param name="closestVirtualBoundary">IVirtualBoundary</param>
        /// <returns>BoundaryHitType indicating type of boundary hit</returns>
        private BoundaryHitType MoveToBoundaryCheck(Photon photon, out IVirtualBoundary closestVirtualBoundary)
        {
            // get distance to any tissue boundary
            var tissueDistance = _tissue.GetDistanceToBoundary(photon);

            // get distance to any VB

            // find closest VB (will return null if no closest VB exists)
            // vbDistance is initialized to double.PositiveInfinity at start of GetClosestVB
            closestVirtualBoundary = _virtualBoundaryController.GetClosestVirtualBoundary(photon.DP, out var vbDistance);

            if (tissueDistance < vbDistance) // photon won't hit VB, but might not hit tissue boundary either
            {
                // no pseudo added here because set by tissue class
                var hitTissueBoundary = photon.Move(tissueDistance);
                return hitTissueBoundary ? BoundaryHitType.Tissue : BoundaryHitType.None;
            }

            // otherwise, move to the closest virtual boundary

            // if both tissueDistance and vbDistance are both infinity, then photon dead
            if (double.IsPositiveInfinity(vbDistance))
            {
                photon.DP.StateFlag = photon.DP.StateFlag.Remove(PhotonStateType.Alive);
                return BoundaryHitType.None;
            }

            var hitVirtualBoundary = photon.Move(vbDistance);

            // pseudo for tissue boundary set by tissue class
            photon.DP.StateFlag = photon.DP.StateFlag.Add(closestVirtualBoundary.PhotonStateType); // add pseudo-collision for vb 

            // DC - also confusing that we'd add a pseudo for the vb if hitVirtualBoundary is false...
            return hitVirtualBoundary ? BoundaryHitType.Virtual : BoundaryHitType.None;
        }

        /// <summary>
        /// Method to display introduction to the simulation
        /// </summary>
        private void DisplayIntro()
        {
            var header = Input.OutputName + " (" + SimulationIndex + "): ";
            _logger.Info(() => header + "                                                  \n");
            _logger.Info(() => header + "      Monte Carlo Simulation of Light Propagation \n");
            _logger.Info(() => header + "              in a multi-region tissue            \n");
            _logger.Info(() => header + "                                                  \n");
            _logger.Info(() => header + "         written by the Virtual Photonics Team    \n");
            _logger.Info(() => header + "              Beckman Laser Institute             \n");
            _logger.Info(() => header + "                                                  \n");
        }

        /// <summary>
        /// Method that displays simulation percentage done
        /// </summary>
        /// <param name="n">current number of photons run</param>
        /// <param name="numberOfPhotonsLaunched">total number of photons to be launched</param>
        private void DisplayStatus(long n, long numberOfPhotonsLaunched)
        {
            var header = Input.OutputName + " (" + SimulationIndex + "): ";
            // fraction of photons completed
            var fraction = 100.0 * n / numberOfPhotonsLaunched;

            _logger.Info(() => header + fraction + " percent complete\n");
        }
    }
}
