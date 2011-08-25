using System.Collections.Generic;
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
            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput,
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType);

            var detectors = DetectorFactory.GetDetectors(detectorInputs, tissue, tallySecondMoment);

            var detectorController = 
                Factories.DetectorControllerFactory.GetDetectorController(
                virtualBoundaryType, detectors);
 
            // DetectorController tallies for post-processing
            if (virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                foreach (var dp in database.DataPoints)
                {
                    ((ISurfaceDetectorController)detectorController).Tally(dp);
                }
            }
            // need to add volumeDetectorController processing

            detectorController.NormalizeDetectors(databaseInput.N);

            var postProcessedOutput = new Output(databaseInput, detectors);

            return postProcessedOutput;
        }

        /// <summary>
        /// pMC overload
        /// GenerateOutput takes IDetectorInput (which designates tallies),
        /// reads PhotonExitHistory, and generates Output.
        /// </summary>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="databaseInput">Database information needed for post-processing</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="perturbedRegionsIndices">Indices of regions being perturbed</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            VirtualBoundaryType virtualBoundaryType,
            IList<IpMCDetectorInput> detectorInputs,
            bool tallySecondMoment,
            pMCDatabase database, 
            SimulationInput databaseInput)
        {
            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput, 
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType);

            var detectors = DetectorFactory.GetDetectors(detectorInputs, tissue, tallySecondMoment);

            var detectorController = Factories.DetectorControllerFactory.GetpMCDetectorController(
                virtualBoundaryType, detectors, tissue, databaseInput.Options.TallySecondMoment);

            if (virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                foreach (var dp in database.DataPoints)
                {
                    ((IpMCSurfaceDetectorController)detectorController).Tally(dp.PhotonDataPoint, dp.CollisionInfo);
                }
            }

            detectorController.NormalizeDetectors(databaseInput.N);

            var postProcessedOutput = new Output(databaseInput, detectors);

            return postProcessedOutput;
        }

    }
}
