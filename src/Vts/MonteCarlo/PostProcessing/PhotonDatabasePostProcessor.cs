using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Extensions;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.PhotonData;

namespace Vts.MonteCarlo.PostProcessing
{
    /// <summary>
    /// The <see cref="PostProcessing"/> namespace contains the Monte Carlo Post Processor classes for processing photon databases
    /// </summary>

    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    /// Sets up and post-processes Monte Carlo termination data that has been 
    /// saved in a database.
    /// </summary>
    public class PhotonDatabasePostProcessor
    {

        private readonly VirtualBoundaryType _virtualBoundaryType;
        private readonly IList<IDetector> _detectors;
        private readonly DetectorController _detectorController;
        private readonly pMCDatabase _pMcDatabase;
        private readonly PhotonDatabase _photonDatabase;
        private readonly SimulationInput _databaseInput;
        private readonly bool _ispMcPostProcessor;
        private readonly ITissue _tissue;

        /// <summary>
        /// Creates an instance of PhotonDatabasePostProcessor for pMC database processing
        /// </summary>
        /// <param name="virtualBoundaryType">virtual boundary type</param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">pMCDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        public PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IEnumerable<IDetectorInput> detectorInputs,
            pMCDatabase database,
            SimulationInput databaseInput)
            : this(virtualBoundaryType, detectorInputs, databaseInput)
        {
            _pMcDatabase = database;
            _ispMcPostProcessor = true;
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
            _ispMcPostProcessor = false;
        }

        /// <summary>
        /// Shared constructor for both pMC and standard (photon) databases
        /// </summary>
        /// <param name="virtualBoundaryType"></param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        private PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IEnumerable<IDetectorInput> detectorInputs,
            SimulationInput databaseInput)
        {
            var rng = RandomNumberGeneratorFactory.GetRandomNumberGenerator(
                RandomNumberGeneratorType.MersenneTwister, -1); // -1 = random seed

            _virtualBoundaryType = virtualBoundaryType;

            _databaseInput = databaseInput;

            _tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput,
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType,
                databaseInput.Options.RussianRouletteWeightThreshold);

            _detectors = DetectorFactory.GetDetectors(detectorInputs, _tissue, rng);

            _detectorController = new DetectorController(_detectors);
        }

        /// <summary>
        /// Helper static method to run a group of post-processors in parallel
        /// </summary>
        /// <param name="postProcessors">array of PhotonDatabaseProcessor classes to be run</param>
        /// <returns>array of SimulationOutput</returns>
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
        /// <returns>SimulationOutput class</returns>
        public SimulationOutput Run()
        {
            if (_virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                var photon = new Photon();
                if (_ispMcPostProcessor)
                {
                    foreach (var dp in _pMcDatabase.DataPoints)
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
                        if (_virtualBoundaryType == VirtualBoundaryType.DiffuseReflectance)
                            photon.CurrentRegionIndex = 0;
                        if (_virtualBoundaryType == VirtualBoundaryType.DiffuseTransmittance)
                            // does following work for SingleInclusionTissue or BoundedTissue?
                            photon.CurrentRegionIndex = _tissue.Regions.Count - 1;
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
