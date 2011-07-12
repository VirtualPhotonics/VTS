using System;
using System.Linq;
using System.Collections.Generic;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;
using System.IO;

namespace Vts.MonteCarlo
{
    /// <summary>
    /// Provides main processing for Monte Carlo simulation. 
    /// </summary>
    public class MonteCarloSimulation
    {
        private ISource _source;
        private ITissue _tissue;
        private VirtualBoundaryController _virtualBoundaryController;
        //private IList<IDetector> _detectors;  // this is total list of detectors indep. of VBs
        private IList<IDetectorController> _detectorControllers; // total list of detectors Controllers indep. of VBs
        private long _numberOfPhotons;
        private SimulationStatistics _simulationStatistics;

        protected SimulationInput _input;
        private Random _rng;

        private string _outputPath;

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

            //WRITE_DATABASES = input.Options.WriteDatabases; // modified ckh 4/9/11
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
            _detectorControllers = new List<IDetectorController>();
            //_detectors = new List<IDetector>(); // need these for Output
            _virtualBoundaryController = new VirtualBoundaryController(new List<IVirtualBoundary>());
            foreach (var vbg in input.VirtualBoundaryGroups)
            {
                // put in VBFactory
                var detectors = DetectorFactory.GetDetectors(vbg.DetectorInputs, _tissue, input.Options.TallySecondMoment);
                //foreach (var detector in detectors)
                //{
                //    _detectors.Add(detector);
                //}
                var detectorController = DetectorControllerFactory.GetDetectorController(vbg.VirtualBoundaryType, detectors);
                _detectorControllers.Add(detectorController);
                //var virtualBoundaryController = VirtualBoundaryControllerFactory.GetVirtualBoundaryController(
                //    vbg.VirtualBoundaryGroupType, detectorController, _tissue);
                var virtualBoundary = VirtualBoundaryFactory.GetVirtualBoundary(vbg.VirtualBoundaryType,
                    _tissue, detectorController);
                _virtualBoundaryController.VirtualBoundaries.Add(virtualBoundary);
            }  

            //_virtualBoundaryController.VirtualBoundaries = VirtualBoundaryFactory.GetVirtualBoundaries(
            //    input.VirtualBoundaryGroups, _tissue, input.Options.TallySecondMoment);

            //// instantiate detector controller for the detectors that apply to each virtual boundary 
            //var detectors = DetectorFactory.GetDetectors(input.DetectorInputs, _tissue, input.Options.TallySecondMoment);
            //_surfaceDetectorController = DetectorControllerFactory.GetStandardSurfaceDetectorController(detectors);

            //_virtualBoundaryController = VirtualBoundaryControllerFactory.GetVirtualBoundaryController(
            //    _detectorController.Detectors, _tissue);
        }

        /// <summary>
        /// Default constructor to allow quick-and-easy simulation
        /// </summary>
        public MonteCarloSimulation() : this(new SimulationInput()) { }

        // private properties
        private int SimulationIndex { get; set; }

        // public properties
        //private IList<DatabaseType> WRITE_DATABASES { get; set; }  // modified ckh 4/9/11
        private AbsorptionWeightingType AbsorptionWeightingType { get; set; }
        public PhaseFunctionType PhaseFunctionType { get; set; }
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
            DisplayIntro();

            ExecuteMCLoop();

            var detectors = _virtualBoundaryController.VirtualBoundaries.SelectMany(vb =>
                vb.DetectorController.Detectors).ToList();

            Results = new Output(_input, detectors);

            return Results;
        }

        /// <summary>
        /// Executes the Monte Carlo Loop
        /// </summary>
        protected virtual void ExecuteMCLoop()
        {
            PhotonDatabaseWriter terminationWriter = null;
            CollisionInfoDatabaseWriter collisionWriter = null;

            try
            {
                // move out to method if works out
                foreach (var vbg in _input.VirtualBoundaryGroups)
                {
                    if (vbg.WriteToDatabase)
                    {
                        switch (vbg.VirtualBoundaryType)
                        {
                            default:
                            case VirtualBoundaryType.DiffuseReflectance:
                                terminationWriter = new PhotonDatabaseWriter(
                                    Path.Combine(_outputPath, _input.OutputName, "photonReflectanceDatabase"));
                                break;
                            case VirtualBoundaryType.DiffuseTransmittance:
                                terminationWriter = new PhotonDatabaseWriter(
                                    Path.Combine(_outputPath, _input.OutputName, "photonTransmittanceDatabase"));
                                break;
                            case VirtualBoundaryType.SpecularReflectance:
                                terminationWriter = new PhotonDatabaseWriter(
                                    Path.Combine(_outputPath, _input.OutputName, "photonSpecularDatabase"));
                                break;
                            case VirtualBoundaryType.pMCDiffuseReflectance:
                                terminationWriter = new PhotonDatabaseWriter(
                                    Path.Combine(_outputPath, _input.OutputName, "photonReflectanceDatabase"));
                                collisionWriter = new CollisionInfoDatabaseWriter(
                                    Path.Combine(_outputPath, _input.OutputName, "collisionInfoDatabase"), _tissue.Regions.Count());
                                break;
                        }
                    }
                }
                //{
                //    if (WRITE_DATABASES.Contains(DatabaseType.PhotonExitDataPoints))
                //    {
                //        terminationWriter = new PhotonDatabaseWriter(
                //            Path.Combine(_outputPath, _input.OutputName, "photonExitDatabase"));
                //    }
                //    if (WRITE_DATABASES.Contains(DatabaseType.CollisionInfo))
                //    {
                //        collisionWriter = new CollisionInfoDatabaseWriter(
                //            Path.Combine(_outputPath, _input.OutputName, "collisionInfoDatabase"), _tissue.Regions.Count());
                //    }
                //}

                for (long n = 1; n <= _numberOfPhotons; n++)
                {
                    // todo: bug - num photons is assumed to be over 10 :)
                    if (n % (_numberOfPhotons / 10) == 0)
                    {
                        DisplayStatus(n, _numberOfPhotons);
                    }

                    var photon = _source.GetNextPhoton(_tissue);

                    do
                    { /* begin do while  */
                        photon.SetStepSize(); // only calls rng if SLeft == 0.0

                        //bool hitBoundary = photon.Move(distance);
                        BoundaryHitType hitType = Move(photon); // in-line?

                        // todo: consider moving actual calls to Tally after do-while
                        // for each "hit" virtual boundary, tally respective detectors. 
                        if (hitType == BoundaryHitType.Virtual)
                        {   
                            ((ISurfaceDetectorController)_virtualBoundaryController.ClosestVirtualBoundary.DetectorController).Tally(photon.DP);     
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
                        if (!photon.DP.StateFlag.Has(PhotonStateType.Absorbed))
                        {
                            photon.Scatter();
                        }

                    } while (photon.DP.StateFlag.Has(PhotonStateType.Alive)); /* end do while */

                    //_detectorController.TerminationTally(photon.DP);

                    if (terminationWriter != null)
                    {
                        //dc: how to check if detector contains DP  ckh: check is on reading side, may need to fix
                        terminationWriter.Write(photon.DP);
                    }
                    if (collisionWriter != null)
                    {
                        collisionWriter.Write(photon.History.SubRegionInfoList);
                    }

                    // note History has possibly 2 more DPs than linux code due to 
                    // final crossing of PseudoReflectedTissueBoundary and then
                    // PseudoDiffuseReflectanceVB
                    var volumeVBs = _virtualBoundaryController.VirtualBoundaries.Where(
                        v => v.VirtualBoundaryType == VirtualBoundaryType.GenericVolumeBoundary).ToList();
                    foreach (var vb in volumeVBs)
                    {
                        ((IVolumeDetectorController)vb.DetectorController).Tally(photon.History);
                    }

                    if (TrackStatistics)
                    {
                        _simulationStatistics.TrackDeathStatistics(photon.History);
                    }

                } /* end of for n loop */
            }
            finally
            {
                if (terminationWriter != null) terminationWriter.Dispose();
                if (collisionWriter != null) collisionWriter.Dispose();
            }

            // normalize all detectors by the total number of photons (each tally records it's own "local" count as well)
            foreach (var detectorController in _detectorControllers)
            {                
                detectorController.NormalizeDetectors(_numberOfPhotons);
            }
            if (TrackStatistics)
            {
                _simulationStatistics.ToFile("statistics");
            }
        }

        private BoundaryHitType Move(Photon photon)
        {
            bool willHitTissueBoundary = false;

            // get distance to any tissue boundary
            var tissueDistance = _tissue.GetDistanceToBoundary(photon);
            // get distance to any VB

            double vbDistance = double.PositiveInfinity;
            var vb =_virtualBoundaryController.GetClosestVirtualBoundary(photon.DP, out vbDistance);

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
                    return BoundaryHitType.Virtual; // set to virtual so fall out of loop
                }
                else
                {
                    var hitVirtualBoundary = photon.Move(vbDistance);
                    photon.DP.StateFlag = photon.DP.StateFlag.Add(vb.PhotonStateType); // add pseudo-collision for vb
                    return hitVirtualBoundary ? BoundaryHitType.Virtual : BoundaryHitType.None;
                }
            }
        }

        /********************************************************/
        void DisplayIntro()
        {
            var header = _input.OutputName + "(" + SimulationIndex + ")";
            string intro = "\n" +
                header + ":                                                  \n" +
                header + ":      Monte Carlo Simulation of Light Propagation \n" +
                header + ":              in a multi-region tissue            \n" +
                header + ":                                                  \n" +
                header + ":         written by the Virtual Photonics Team    \n" +
                header + ":              Beckman Laser Institute             \n" +
                header + ":";
            Console.WriteLine(intro);
        }

        /*****************************************************************/
        void DisplayStatus(long n, long num_phot)
        {
            var header = _input.OutputName + "(" + SimulationIndex + ")";
            /* fraction of photons completed */
            double frac = 100 * n / num_phot;
            Console.WriteLine(header + ": " + frac + " percent complete, " + DateTime.Now);
        }
    }
}
