using System.Collections.Generic;
using System.Threading.Tasks;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Factories;
using Vts.MonteCarlo.Extensions;

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
        /// Creates an instance of PhotonDatabasePostProcessor for standard (photon) database processing
        /// </summary>
        /// <param name="virtualBoundaryType"></param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        public PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            bool tallySecondMoment,
            pMCDatabase database,
            SimulationInput databaseInput)
            : this(virtualBoundaryType, detectorInputs,tallySecondMoment, databaseInput)
        {
            _pMCDatabase = database;
            _ispMCPostProcessor = true;
        }

        /// <summary>
        /// Creates an instance of PhotonDatabasePostProcessor for pMC database processing
        /// </summary>
        /// <param name="virtualBoundaryType"></param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        public PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            bool tallySecondMoment,
            PhotonDatabase photonDatabase,
            SimulationInput databaseInput)
            : this(virtualBoundaryType, detectorInputs, tallySecondMoment, databaseInput)
        {
            _photonDatabase = photonDatabase;
            _ispMCPostProcessor = false;
        }

        /// <summary>
        /// Shared constructor for both pMC and standard (photon) databases
        /// </summary>
        /// <param name="virtualBoundaryType"></param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        private PhotonDatabasePostProcessor(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            bool tallySecondMoment,
            SimulationInput databaseInput)
        {
            _virtualBoundaryType = virtualBoundaryType;

            _databaseInput = databaseInput;

            _tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput,
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType,
                databaseInput.Options.RussianRouletteWeightThreshold);

            _detectors = DetectorFactory.GetDetectors(detectorInputs, _tissue, tallySecondMoment);

            _detectorController = new DetectorController(_detectors);
        }

        /// <summary>
        /// Helper static method to run a group of post-processors in parallel
        /// </summary>
        /// <param name="postProcessors"></param>
        /// <returns></returns>
        public static Output[] RunAll(PhotonDatabasePostProcessor[] postProcessors)
        {
            var outputs = new Output[postProcessors.Length];

            Parallel.For(0, postProcessors.Length, index =>
            {
                outputs[index] = postProcessors[index].Run();
            });

            return outputs;
        }

        /// <summary>
        /// Executes the post-processor
        /// </summary>
        /// <returns></returns>
        public Output Run()
        {
            if (_virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                var photon = new Photon();
                if(_ispMCPostProcessor)
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

            var postProcessedOutput = new Output(_databaseInput, _detectors);

            return postProcessedOutput;
        }

        #region Static methods for post-processing, granfathered in (todo: remove usage)

        /// <summary>
        /// GenerateOutput takes detector inputs, reads PhotonExitHistory, and generates 
        /// Output.  This runs the conventional post-processing.
        /// </summary>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            bool tallySecondMoment,
            PhotonDatabase database,
            SimulationInput databaseInput)
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                virtualBoundaryType,
                detectorInputs,
                tallySecondMoment,
                database,
                databaseInput);

            var output = postProcessor.Run();

            return output;

            //ITissue tissue = Factories.TissueFactory.GetTissue(
            //    databaseInput.TissueInput,
            //    databaseInput.Options.AbsorptionWeightingType,
            //    databaseInput.Options.PhaseFunctionType,
            //    databaseInput.Options.RussianRouletteWeightThreshold);

            //var detectors = DetectorFactory.GetDetectors(detectorInputs, tissue, tallySecondMoment);

            //var detectorController = new DetectorController(detectors);

            //// DetectorController tallies for post-processing
            //if (virtualBoundaryType.IsSurfaceVirtualBoundary())
            //{
            //    var photon = new Photon();
            //    foreach (var dp in database.DataPoints)
            //    {
            //        photon.DP = dp;
            //        detectorController.Tally(photon); 
            //    }
            //}

            //detectorController.NormalizeDetectors(databaseInput.N);

            //var postProcessedOutput = new Output(databaseInput, detectors);

            //return postProcessedOutput;
        }

        /// <summary>
        /// pMC overload
        /// GenerateOutput takes IDetectorInput (which designates tallies),
        /// reads PhotonExitHistory, and generates Output.
        /// </summary>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            bool tallySecondMoment,
            pMCDatabase database,
            SimulationInput databaseInput)
        {
            var postProcessor = new PhotonDatabasePostProcessor(
                virtualBoundaryType,
                detectorInputs,
                tallySecondMoment,
                database,
                databaseInput);

            var output = postProcessor.Run();

            return output;

            //ITissue tissue = Factories.TissueFactory.GetTissue(
            //    databaseInput.TissueInput, 
            //    databaseInput.Options.AbsorptionWeightingType,
            //    databaseInput.Options.PhaseFunctionType,
            //    databaseInput.Options.RussianRouletteWeightThreshold);

            //var detectors = DetectorFactory.GetDetectors(detectorInputs, tissue, tallySecondMoment);

            //var detectorController = new DetectorController(detectors);                

            //if (virtualBoundaryType.IsSurfaceVirtualBoundary())
            //{
            //    var photon = new Photon();
            //    foreach (var dp in database.DataPoints)
            //    {
            //        photon.DP = dp.PhotonDataPoint;
            //        photon.History.SubRegionInfoList = dp.CollisionInfo;
            //        detectorController.Tally(photon);
            //    }
            //}

            //detectorController.NormalizeDetectors(databaseInput.N);

            //var postProcessedOutput = new Output(databaseInput, detectors);

            //return postProcessedOutput;
        }

        #endregion
    }
}
