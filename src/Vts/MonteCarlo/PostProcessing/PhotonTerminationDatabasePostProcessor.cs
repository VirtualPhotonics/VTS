using System.Collections.Generic;
using Vts.MonteCarlo.PhotonData;
using Vts.MonteCarlo.Controllers;
using Vts.MonteCarlo.Factories;

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
            bool tallySecondMoment,
            PhotonDatabase database, 
            SimulationInput databaseInput)
        {
            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput,
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType);

            var detectors = DetectorFactory.GetDetectors(detectorInputs, tissue, tallySecondMoment);
 
            var detectorController = 
                Factories.DetectorControllerFactory.GetStandardDetectorController(detectors);

            var virtualBoundaryController = VirtualBoundaryControllerFactory.GetVirtualBoundaryController(
                detectorController.Detectors, tissue); 
 
            // CKH need to FIX!
            foreach (var dp in database.DataPoints)
            {
                foreach (var vb in virtualBoundaryController.VirtualBoundaries)
                {
                    virtualBoundaryController.TallyToTerminationDetectors(dp);
                }
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
            bool tallySecondMoment,
            pMCDatabase database, 
            SimulationInput databaseInput)
        {
            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput, 
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType);

            pMCDetectorController detectorController = 
                Factories.DetectorControllerFactory.GetpMCDetectorController(
                detectorInputs, tissue, databaseInput.Options.TallySecondMoment);
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
