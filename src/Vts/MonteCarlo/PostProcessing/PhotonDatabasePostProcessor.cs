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

            var detectorController = new DetectorController(detectors);
 
            // DetectorController tallies for post-processing
            if (virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                var photon = new Photon();
                foreach (var dp in database.DataPoints)
                {
                    photon.DP = dp;
                    detectorController.Tally(photon); 
                }
            }

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

            var detectorController = new DetectorController(detectors);                

            if (virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                var photon = new Photon();
                foreach (var dp in database.DataPoints)
                {
                    photon.DP = dp.PhotonDataPoint;
                    photon.History.SubRegionInfoList = dp.CollisionInfo;
                    detectorController.Tally(photon);
                }
            }

            detectorController.NormalizeDetectors(databaseInput.N);

            var postProcessedOutput = new Output(databaseInput, detectors);

            return postProcessedOutput;
        }

    }
}
