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
        /// <param name="virtualBoundaryType">VirtualBoundaryType</param>
        /// <param name="detectorInputs">List of IDetectorInput</param>
        /// <param name="tallySecondMoment">boolean indicating whether to tally 2nd moment or not</param>
        /// <param name="database">PhotonDatabase (includes PhotonDataPoints)</param>
        /// <param name="databaseInput">SimulationInput used to generate database</param>
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
                databaseInput.Options.PhaseFunctionType,
                databaseInput.Options.RussianRouletteWeightThreshold);

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
        /// <param name="virtualBoundaryType">type of virtual boundary</param>
        /// <param name="detectorInputs">list of IDetectorInput</param>
        /// <param name="tallySecondMoment">boolean indicating whether to tally 2nd moment or not</param>
        /// <param name="trackStatistics">boolean indicating whether to tally statistics or not</param>
        /// <param name="database">pMCDatabase (includes PhotonDataPoints and SubRegionInfo)</param>
        /// <param name="databaseInput">SimulationInput used to generate database</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            VirtualBoundaryType virtualBoundaryType,
            IList<IDetectorInput> detectorInputs,
            bool tallySecondMoment,
            bool trackStatistics,  // track statistics only for subRegionCollisionInfo for now
            pMCDatabase database, 
            SimulationInput databaseInput)
        {
            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput, 
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType,
                databaseInput.Options.RussianRouletteWeightThreshold);

            var detectors = DetectorFactory.GetDetectors(detectorInputs, tissue, tallySecondMoment);

            var detectorController = new DetectorController(detectors);

            var statistics = new PostProcessorStatistics(databaseInput.TissueInput.Regions.Length);

            if (virtualBoundaryType.IsSurfaceVirtualBoundary())
            {
                var photon = new Photon();
                foreach (var dp in database.DataPoints)
                {
                    photon.DP = dp.PhotonDataPoint;
                    photon.History.SubRegionInfoList = dp.CollisionInfo;
                    detectorController.Tally(photon);
                    statistics.TallyStatistics(photon.History);
                }
            }

            detectorController.NormalizeDetectors(databaseInput.N);
            statistics.NormalizeStatistics(databaseInput.N);

            var postProcessedOutput = new Output(databaseInput, detectors);

            return postProcessedOutput;
        }

    }
}
