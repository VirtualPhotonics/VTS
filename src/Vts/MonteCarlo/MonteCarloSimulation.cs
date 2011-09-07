using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Vts.Common.Logging;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Extensions;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Provides main processing for Monte Carlo simulation. 
    /// </summary>
    public class MonteCarloSimulation
    {
        private static ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(MonteCarloSimulation));
        private ISource _source;
        private ITissue _tissue;
        private VirtualBoundaryController _virtualBoundaryController;
        //private IList<IDetectorController> _detectorControllers; // total list indep. of VBs
        private long _numberOfPhotons;
        private SimulationStatistics _simulationStatistics;
        private DatabaseWriterController _databaseWriterController = null;
        private pMCDatabaseWriterController _pMCDatabaseWriterController = null;
        private bool doPMC = false;

        protected SimulationInput _input;
        private Random _rng;

        private string _outputPath;

        private bool _isRunning;
        private bool _isCancelled;
        private bool _resultsAvailable;

        public MonteCarloSimulation(SimulationInput input)
        {
            _outputPath = "";

            // all field/property defaults should be set here
            _input = input;

            var result = SimulationInputValidation.ValidateInput(_input);
            if (result.IsValid == false)
            {
                throw new ArgumentException(result.ValidationRule + (!string.IsNullOrEmpty(result.Remarks) ? "; " + result.Remarks : ""));
            }

            _numberOfPhotons = input.N;

            AbsorptionWeightingType = input.Options.AbsorptionWeightingType; // CKH add 12/14/09
            TrackStatistics = input.Options.TrackStatistics;
            if (TrackStatistics)
            {
                _simulationStatistics = new SimulationStatistics();
            }
            _rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                input.Options.RandomNumberGeneratorType, input.Options.Seed);

            this.SimulationIndex = input.Options.SimulationIndex;

            _tissue = TissueFactory.GetTissue(input.TissueInput, input.Options.AbsorptionWeightingType, input.Options.PhaseFunctionType);
            _source = SourceFactory.GetSource(input.SourceInput, _tissue, _rng);

            // instantiate vb (and associated detectors) for each vb group
            _virtualBoundaryController = new VirtualBoundaryController(new List<IVirtualBoundary>());
            
            foreach (var vbType in EnumHelper.GetValues<VirtualBoundaryType>())
            {
                IEnumerable<IDetectorInput> detectorInputs = null;

                switch (vbType)
                {
                    case VirtualBoundaryType.DiffuseReflectance:
                    default:
                        detectorInputs = input.DetectorInputs.Where(d => d.TallyType.IsReflectanceTally()).ToList();
                        break;
                    case VirtualBoundaryType.DiffuseTransmittance:
                        detectorInputs = input.DetectorInputs.Where(d => d.TallyType.IsTransmittanceTally()).ToList();
                        break;
                    case VirtualBoundaryType.SpecularReflectance:
                        detectorInputs = input.DetectorInputs.Where(d => d.TallyType.IsSpecularReflectanceTally()).ToList();
                        break;
                    case VirtualBoundaryType.GenericVolumeBoundary:
                        detectorInputs = input.DetectorInputs.Where(d => d.TallyType.IsVolumeTally()).ToList();
                        break;
                    case VirtualBoundaryType.SurfaceRadiance:
                        detectorInputs = input.DetectorInputs.Where(d => d.TallyType.IsInternalSurfaceTally()).ToList();
                        break;
                    case VirtualBoundaryType.pMCDiffuseReflectance:
                        detectorInputs = input.DetectorInputs.Where(d => d.TallyType.IspMCTally()).ToList();
                        break;
                }

                // make sure VB Controller has at least diffuse reflectance and diffuse transmittance
                // may change this in future if tissue OnDomainBoundary changes
                if ((detectorInputs.Count() > 0) || (vbType == VirtualBoundaryType.DiffuseReflectance) || (vbType == VirtualBoundaryType.DiffuseTransmittance))
                {
                    var detectors = DetectorFactory.GetDetectors(detectorInputs, _tissue, input.Options.TallySecondMoment);
                    //var detectorController = DetectorControllerFactory.GetDetectorController(vbType, detectors);
                    var detectorController = new DetectorController(detectors);
                    var virtualBoundary = VirtualBoundaryFactory.GetVirtualBoundary(vbType, _tissue, detectorController);
                    _virtualBoundaryController.VirtualBoundaries.Add(virtualBoundary);
                }
            }

            // needed?
            //_detectorControllers = _virtualBoundaryController.VirtualBoundaries.Select(vb=>vb.DetectorController).ToList();

            // set doPMC flag
            if (input.Options.WriteDatabases.Any(d => d.IspMCDatabase()))
            {
                doPMC = true;
            }

            _isCancelled = false;
            _isRunning = false;
            _resultsAvailable = false;
        }

        /// <summary>
        /// Default constructor to allow quick-and-easy simulation
        /// </summary>
        public MonteCarloSimulation() : this(new SimulationInput()) { }

        // public properties
        public PhaseFunctionType PhaseFunctionType { get; set; }

        public bool IsRunning { get { return _isRunning; } }

        public bool ResultsAvailable { get { return _resultsAvailable; } }

        // private properties
        private int SimulationIndex { get; set; }
        private AbsorptionWeightingType AbsorptionWeightingType { get; set; }
        private bool TrackStatistics { get; set; }

        public Output Results { get; private set; }

        public Output Run(string outputPath)
        {
            _outputPath = outputPath;
            return Run();
        }

        /// <summary>
        /// Run the simulation
        /// </summary>
        /// <returns></returns>
        public Output Run()
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

            Results = new Output(_input, detectors);

            _resultsAvailable = true;

            return Results;
        }

        public void Cancel()
        {
            _isCancelled = true;
            logger.Info(() => "Simulation cancelled.\n");
        }

        /// <summary>
        /// Executes the Monte Carlo Loop
        /// </summary>
        protected virtual void ExecuteMCLoop()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                if (_input.Options.WriteDatabases.Count() > 0)
                {
                    InitialDatabases(doPMC);
                }

                var volumeVBs = _virtualBoundaryController.VirtualBoundaries.Where(
                    v => v.VirtualBoundaryType == VirtualBoundaryType.GenericVolumeBoundary).ToList();

                for (long n = 1; n <= _numberOfPhotons; n++)
                {
                    if (_isCancelled)
                    {
                        return;
                    }

                    // todo: bug - num photons is assumed to be over 10 :)
                    if (n % (_numberOfPhotons / 10) == 0)
                    {
                        DisplayStatus(n, _numberOfPhotons);
                    }

                    var photon = _source.GetNextPhoton(_tissue);

                    do
                    { /* begin do while  */
                        photon.SetStepSize(); // only calls rng if SLeft == 0.0

                        IVirtualBoundary closestVirtualBoundary;

                        BoundaryHitType hitType = Move(photon, out closestVirtualBoundary);

                        // todo: consider moving actual calls to Tally after do-while
                        // for each "hit" virtual boundary, tally respective detectors if exist
                        if ((hitType == BoundaryHitType.Virtual) &&
                            (closestVirtualBoundary.DetectorController != null))
                        {
                            closestVirtualBoundary.DetectorController.Tally(photon);
                            //((ISurfaceDetectorController)closestVirtualBoundary.DetectorController).Tally(photon);
                            //((ISurfaceDetectorController)closestVirtualBoundary.DetectorController).Tally(photon.DP);
                            // reset PhotonStateType after tallying
                            //photon.DP.StateFlag.Remove(closestVirtualBoundary.PhotonStateType);
                        }

                        // kill photon for various reasons, including possible VB crossings
                        photon.TestDeath();

                        // check if virtual boundary 
                        if (hitType == BoundaryHitType.Virtual)
                        {
                            continue;
                        }

                        if (hitType == BoundaryHitType.Tissue)
                        {
                            photon.CrossRegionOrReflect();
                            continue;
                        }

                        photon.Absorb(); // can be added to TestDeath?
                        if (!photon.DP.StateFlag.HasFlag(PhotonStateType.Absorbed))
                        {
                            photon.Scatter();
                        }

                    } while (photon.DP.StateFlag.HasFlag(PhotonStateType.Alive)); /* end do while */

                    //_detectorController.TerminationTally(photon.DP);

                    if (_input.Options.WriteDatabases.Count() > 0)
                    {
                        WriteToDatabases(doPMC, photon);
                    }

                    // note History has possibly 2 more DPs than linux code due to 
                    // final crossing of PseudoReflectedTissueBoundary and then
                    // PseudoDiffuseReflectanceVB
                    foreach (var vb in volumeVBs)
                    {
                        //((IVolumeDetectorController)vb.DetectorController).Tally(photon.History);
                        //((IVolumeDetectorController)vb.DetectorController).Tally(photon);
                        vb.DetectorController.Tally(photon);
                    }

                    if (TrackStatistics)
                    {
                        _simulationStatistics.TrackDeathStatistics(photon.DP);
                    }

                } /* end of for n loop */
            }
            finally
            {
                if (_input.Options.WriteDatabases.Count() > 0)
                {
                    CloseDatabases(doPMC);
                }
            }

            // normalize all detectors by the total number of photons (each tally records it's own "local" count as well)
            foreach (var vb in _virtualBoundaryController.VirtualBoundaries)
            {
                if (vb.DetectorController != null) // check that VB has detectors
                {
                    vb.DetectorController.NormalizeDetectors(_numberOfPhotons);
                }
            }

            if (TrackStatistics)
            {
                _simulationStatistics.ToFile("statistics");
            }

            stopwatch.Stop();

            logger.Info(() => "Monte Carlo simulation complete (N = " + _numberOfPhotons + " photons; simulation time = "
                + stopwatch.ElapsedMilliseconds / 1000f + " seconds).\r");
        }

        private void CloseDatabases(bool doPMC)
        {
            if (!doPMC)
            {
                _databaseWriterController.Dispose();
            }
            else
            {
                _pMCDatabaseWriterController.Dispose();
            }
        }

        private void WriteToDatabases(bool doPMC, Photon photon)
        {
            if (!doPMC)
            {
                _databaseWriterController.WriteToSurfaceVirtualBoundaryDatabases(photon.DP);
            }
            else
            {
                _pMCDatabaseWriterController.WriteToSurfaceVirtualBoundaryDatabases(photon.DP, photon.History.SubRegionInfoList);
            }
        }

        private void InitialDatabases(bool doPMC)
        {
            if (!doPMC)
            {
                _databaseWriterController = new DatabaseWriterController(
                    DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriters(
                        _input.Options.WriteDatabases,
                        _outputPath,
                        _input.OutputName));
            }
            else
            {
                _pMCDatabaseWriterController = new pMCDatabaseWriterController(
                    DatabaseWriterFactory.GetSurfaceVirtualBoundaryDatabaseWriters(
                    _input.Options.WriteDatabases,
                            _outputPath,
                            _input.OutputName),
                    DatabaseWriterFactory.GetCollisionInfoDatabaseWriters(
                    _input.Options.WriteDatabases,
                            _tissue,
                            _outputPath,
                            _input.OutputName));
            }
        }

        private BoundaryHitType Move(Photon photon, out IVirtualBoundary closestVirtualBoundary)
        {
            // get distance to any tissue boundary
            var tissueDistance = _tissue.GetDistanceToBoundary(photon);

            // get distance to any VB
            double vbDistance = double.PositiveInfinity;

            // find closest VB (will return null if no closest VB exists)
            closestVirtualBoundary = _virtualBoundaryController.GetClosestVirtualBoundary(photon.DP, out vbDistance);

            if (tissueDistance < vbDistance) // determine if will hit tissue boundary first
            {
                var hitTissueBoundary = photon.Move(tissueDistance);
                return hitTissueBoundary ? BoundaryHitType.Tissue : BoundaryHitType.None;
            }
            else // otherwise, move to the closest virtual boundary
            {
                // if both tissueDistance and vbDistance are both infinity, then photon dead
                if (vbDistance == double.PositiveInfinity)
                {
                    photon.DP.StateFlag = photon.DP.StateFlag.Remove(PhotonStateType.Alive);
                    return BoundaryHitType.None;
                }
                else
                {
                    var hitVirtualBoundary = photon.Move(vbDistance);
                    photon.DP.StateFlag = photon.DP.StateFlag.Add(closestVirtualBoundary.PhotonStateType); // add pseudo-collision for vb
                    return hitVirtualBoundary ? BoundaryHitType.Virtual : BoundaryHitType.None;
                }
            }
        }

        /********************************************************/
        void DisplayIntro()
        {
            var header = SimulationIndex + ": ";
            logger.Info(() => header + "                                                  ");
            logger.Info(() => header + "      Monte Carlo Simulation of Light Propagation ");
            logger.Info(() => header + "              in a multi-region tissue            ");
            logger.Info(() => header + "                                                  ");
            logger.Info(() => header + "         written by the Virtual Photonics Team    ");
            logger.Info(() => header + "              Beckman Laser Institute             ");
            logger.Info(() => header + "                                                  ");
        }

        /*****************************************************************/
        void DisplayStatus(long n, long num_phot)
        {
            var header = SimulationIndex + ": ";
            /* fraction of photons completed */
            double frac = 100 * n / num_phot;

            logger.Info(() => header + frac + " percent complete");
        }
    }
}
