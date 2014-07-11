using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.PostProcessing
{
    /// <summary>
    /// Sets up and postprocesses Monte Carlo termination data that has been 
    /// saved in a database.
    /// </summary>
    public class PhotonDatabasePostProcessor
    {

        private VirtualBoundaryType _virtualBoundaryType;
        private ITissue _tissue;
        private IList<IDetector> _detectors;
        private DetectorController _detectorController;
        private pMCDatabase _pMCDatabase;
        private PhotonDatabase _photonDatabase;
        private SimulationInput _databaseInput;
        private bool _ispMCPostProcessor;

        /// <summary>
        /// Creates an instance of PhotonDatabasePostProcessor for pMC database processing
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">pMCDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        public PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            pMCDatabase database,
            SimulationInput databaseInput)
            : this(virtualBoundaryType, detectorInputs, databaseInput)
        {
            _pMCDatabase = database;
            _ispMCPostProcessor = true;
        }

        /// <summary>
        /// Creates an instance of PhotonDatabasePostProcessor for standard (photon) database processing
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="photonDatabase">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        public PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            PhotonDatabase photonDatabase,
            SimulationInput databaseInput)
            : this(virtualBoundaryType, detectorInputs, databaseInput)
        {
            _photonDatabase = photonDatabase;
            _ispMCPostProcessor = false;
        }

        /// <summary>
        /// Shared constructor for both pMC and standard (photon) databases
        /// </summary>
        /// <param name="virtualBoundaryType"></param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        private PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            SimulationInput databaseInput)
        {
            _virtualBoundaryType = virtualBoundaryType;

            _databaseInput = databaseInput;

            _tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput,
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType,
                databaseInput.Options.RussianRouletteWeightThreshold);

            _detectors = DetectorFactory.GetDetectors(detectorInputs, _tissue);

            _detectorController = new DetectorController(_detectors);
        }

        /// <summary>
        /// Helper static method to run a group of post-processors in parallel
        /// </summary>
        /// <param name="postProcessors"></param>
        /// <returns></returns>
        public static SimulationOutput[] RunAll(PhotonDatabasePostProcessor[] postProcessors)
        {
            var outputs = new SimulationOutput[postProcessors.Length];
            var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            Parallel.ForEach(postProcessors, options, (sim, state, index) =>
            {
                try
                {
                    outputs[index] = postProcessors[index].Run();
                }
                catch
                {
                    Console.WriteLine("Problem occurred running simulation #{0}. Make sure all simulations have distinct 'OutputName' properties?", index);
                }
            });

            return outputs;
        }

        /// <summary>
        /// Executes the post-processor
        /// </summary>
        /// <returns></returns>
        public SimulationOutput Run()
        {
            if (_virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                var photon = new Photon();
                if (_ispMCPostProcessor)
                {
                    foreach (var dp in _pMCDatabase.DataPoints)
                    {
                        photon.DP = dp.PhotonDataPoint;
                        photon.History.SubRegionInfoList = dp.CollisionInfo;
                        _detectorController.Tally(photon);
                    }
                }
                else // "standard" post-processor
                {
                    foreach (var dp in _photonDatabase.DataPoints)
                    {
                        photon.DP = dp;
                        _detectorController.Tally(photon);
                    }
                }
            }

            _detectorController.NormalizeDetectors(_databaseInput.N);

            var postProcessedOutput = new SimulationOutput(_databaseInput, _detectors);

            return postProcessedOutput;
        }
    }
}
