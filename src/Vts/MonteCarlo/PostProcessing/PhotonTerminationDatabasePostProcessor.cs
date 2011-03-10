using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Interfaces;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Tissues;

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
        /// <param name="peh">PhotonTerminationDatabase</param>
        /// <param name="databaseOutput">Database information needed for post-processing</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IList<IDetectorInput> detectorInputs, 
            PhotonDatabase peh, 
            Output databaseOutput)
        {
            Output postProcessedOutput = new Output();

            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseOutput.Input.TissueInput,
                databaseOutput.Input.Options.AbsorptionWeightingType,
                databaseOutput.Input.Options.PhaseFunctionType);

            DetectorController detectorController = Factories.DetectorControllerFactory.GetStandardDetectorController(detectorInputs, tissue);

            foreach (var dp in peh.DataPoints)
            {
                detectorController.TerminationTally(dp);     
            }

            detectorController.NormalizeDetectors(databaseOutput.Input.N);

            // todo: call output generation method on detectorController (once it's implemented)
            return postProcessedOutput;
        }

        /// <summary>
        /// pMC overload
        /// GenerateOutput takes IDetectorInput (which designates tallies),
        /// reads PhotonExitHistory, and generates Output.
        /// </summary>
        /// <param name="detectorInputs>List of IDetectorInputs designating binning</param>
        /// <param name="peh">PhotonTerminationDatabase</param>
        /// <param name="databaseOutput">Database information needed for post-processing</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="perturbedRegionsIndices">Indices of regions being perturbed</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IList<IpMCDetectorInput> detectorInputs, 
            PhotonDatabase peh, 
            Output databaseOutput,
            List<OpticalProperties> perturbedOps,
            List<int> perturbedRegionsIndices)
        {
            Output postProcessedOutput = new Output();

            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseOutput.Input.TissueInput, 
                databaseOutput.Input.Options.AbsorptionWeightingType,
                databaseOutput.Input.Options.PhaseFunctionType);

            pMCDetectorController detectorController = Factories.DetectorControllerFactory.GetpMCDetectorController(detectorInputs, tissue);
            IList<SubRegionCollisionInfo> collisionInfo = null; // todo: revisit
            foreach (var dp in peh.DataPoints)
            {
                detectorController.TerminationTally(dp, collisionInfo);
            }

            detectorController.NormalizeDetectors(databaseOutput.Input.N);
            
            // todo: call output generation method on detectorController (once it's implemented)
            return postProcessedOutput;
        }

    }
}
