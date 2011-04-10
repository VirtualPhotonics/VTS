using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;
using Vts.MonteCarlo.Controllers;

namespace Vts.MonteCarlo.PostProcessing
{
    /// <summary>
    /// Sets up and postprocesses Monte Carlo termination data that has been 
    /// saved in a database.
    /// </summary>
    public class PhotonTerminationDatabasePostProcessor
    {
        /// <summary>
        /// GenerateOutput takes IDetectorInput (which designates tallies), reads PhotonExitHistory, and generates 
        /// Output.  This runs the conventional post-processing.
        /// </summary>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IList<IDetectorInput> detectorInputs, 
            PhotonDatabase database, 
            SimulationInput databaseInput)
        {
            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput,
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType);

            DetectorController detectorController = Factories.DetectorControllerFactory.GetStandardDetectorController(detectorInputs, tissue);

            foreach (var dp in database.DataPoints)
            {
                detectorController.TerminationTally(dp);     
            }

            detectorController.NormalizeDetectors(databaseInput.N);

            var postProcessedOutput = new Output(databaseInput, detectorController.Detectors);
            // todo: call output generation method on detectorController (once it's implemented)
            return postProcessedOutput;
        }

        /// <summary>
        /// pMC overload
        /// GenerateOutput takes IDetectorInput (which designates tallies),
        /// reads PhotonExitHistory, and generates Output.
        /// </summary>
        /// <param name="detectorInputs>List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="perturbedRegionsIndices">Indices of regions being perturbed</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IList<IpMCDetectorInput> detectorInputs, 
            pMCDatabase database, 
            SimulationInput databaseInput)
        {
            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput, 
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType);

            pMCDetectorController detectorController = Factories.DetectorControllerFactory.GetpMCDetectorController(detectorInputs, tissue);
            foreach (var dp in database.DataPoints)
            {
                detectorController.TerminationTally(dp.PhotonDataPoint, dp.CollisionInfo);
            }

            detectorController.NormalizeDetectors(databaseInput.N);

            var postProcessedOutput = new Output(databaseInput, detectorController.Detectors);

            return postProcessedOutput;
        }

    }
}
