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
        /// <param name="tallies">IDetectorInput designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="detectorInputs">List of IDetectorInputs designating binning</param>
        /// <param name="peh">PhotonTerminationDatabase</param>
        /// <param name="databaseOutput">Database information needed for post-processing</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IDetectorInput tallies, 
            PhotonTerminationDatabase database, 
            IList<IDetectorInput> detectorInputs, 
            PhotonDatabase peh, 
            SimulationInput databaseInput)
        {
            Output postProcessedOutput = new Output();

            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput,
                databaseInput.Options.AbsorptionWeightingType);
                databaseInput.Options.AbsorptionWeightingType,
                databaseInput.Options.PhaseFunctionType);
            IDetector detector = Factories.DetectorFactory.GetDetector(tallies, tissue);

            foreach (var dp in database.DataPoints)
            DetectorController detectorController = Factories.DetectorControllerFactory.GetStandardDetectorController(detectorInputs, tissue);

            foreach (var dp in peh.DataPoints)
            {
                foreach (var t in detector.TerminationITallyList)
                {
                    if (t.ContainsPoint(dp))
                    {
                        t.Tally(dp);
                    }
                }          
                detectorController.TerminationTally(dp);     
            }
            postProcessedOutput.Input = databaseInput;
            detector.NormalizeTalliesToOutput(databaseInput.N, postProcessedOutput);

            detectorController.NormalizeDetectors(databaseInput.N);

            // todo: call output generation method on detectorController (once it's implemented)
            return postProcessedOutput;
        }

        /// <summary>
        /// pMC overload
        /// GenerateOutput takes IDetectorInput (which designates tallies),
        /// reads PhotonExitHistory, and generates Output.
        /// </summary>
        /// <param name="tallies">IDetectorInput designating binning</param>
        /// <param name="database">PhotonTerminationDatabase</param>
        /// <param name="detectorInputs>List of IDetectorInputs designating binning</param>
        /// <param name="peh">PhotonTerminationDatabase</param>
        /// <param name="databaseOutput">Database information needed for post-processing</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="perturbedRegionsIndices">Indices of regions being perturbed</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IDetectorInput tallies, 
            PhotonTerminationDatabase database, 
            SimulationInput databaseInput,
            IList<IpMCDetectorInput> detectorInputs, 
            PhotonDatabase peh, 
            Output databaseOutput,
            List<OpticalProperties> perturbedOps,
            List<int> perturbedRegionsIndices)
        {
            Output postProcessedOutput = new Output();

            ITissue tissue = Factories.TissueFactory.GetTissue(
                databaseInput.TissueInput, 
                databaseInput.Options.AbsorptionWeightingType);
            IDetector detector = Factories.DetectorFactory.GetDetector(tallies, tissue);
                databaseOutput.Input.TissueInput, 
                databaseOutput.Input.Options.AbsorptionWeightingType);

            int count = 0;
            foreach (var dp in database.DataPoints)
            pMCDetectorController detectorController = Factories.DetectorControllerFactory.GetpMCDetectorController(detectorInputs, tissue);
            IList<SubRegionCollisionInfo> collisionInfo = null; // todo: revisit
            foreach (var dp in peh.DataPoints)
            {
                foreach (var t in detector.TerminationITallyList)
			    {
                    if (t.ContainsPoint(dp))
                    {
                        t.Tally(dp);
                        ++count;
                    }
                }
                detectorController.TerminationTally(dp, collisionInfo);
            }
            postProcessedOutput.Input = databaseInput;
            detector.NormalizeTalliesToOutput(databaseInput.N, postProcessedOutput);

            detectorController.NormalizeDetectors(databaseOutput.Input.N);
            
            // todo: call output generation method on detectorController (once it's implemented)
            return postProcessedOutput;
        }

    }
}
