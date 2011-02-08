using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="peh">PhotonTerminationDatabase</param>
        /// <param name="databaseOutput">Database information needed for post-processing</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IDetectorInput tallies, 
            PhotonTerminationDatabase peh, 
            Output databaseOutput)
        {
            Output postProcessedOutput = new Output();
            ITissue tissue = Factories.TissueFactory.GetTissue(databaseOutput.input.TissueInput);
            IDetector detector = Factories.DetectorFactory.GetDetector(tallies, tissue);

            foreach (var dp in peh.DataPoints)
            {
                foreach (var t in detector.TerminationITallyList)
                {
                    if (t.ContainsPoint(dp))
                    {
                        t.Tally(dp);
                    }
                }          
            }
            detector.NormalizeTalliesToOutput(databaseOutput.input.N, postProcessedOutput);
            return postProcessedOutput;
        }

        /// <summary>
        /// pMC overload
        /// GenerateOutput takes IDetectorInput (which designates tallies),
        /// reads PhotonExitHistory, and generates Output.
        /// </summary>
        /// <param name="tallies">IDetectorInput designating binning</param>
        /// <param name="peh">PhotonTerminationDatabase</param>
        /// <param name="databaseOutput">Database information needed for post-processing</param>
        /// <param name="perturbedOps">Perturbed optical properties</param>
        /// <param name="perturbedRegionsIndices">Indices of regions being perturbed</param>
        /// <returns></returns>
        public static Output GenerateOutput(
            IDetectorInput tallies, 
            PhotonTerminationDatabase peh, 
            Output databaseOutput,
            List<OpticalProperties> perturbedOps,
            List<int> perturbedRegionsIndices)
        {
            Output postProcessedOutput = new Output();
            ITissue tissue = Factories.TissueFactory.GetTissue(databaseOutput.input.TissueInput);
            IDetector detector = Factories.DetectorFactory.GetDetector(tallies, tissue);

            int count = 0;
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
            }
            detector.NormalizeTalliesToOutput(databaseOutput.input.N, postProcessedOutput);
            return postProcessedOutput;
        }

    }
}
