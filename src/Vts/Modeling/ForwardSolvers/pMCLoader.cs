using System.Linq;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.PhotonData;

//using MathNet.Numerics.Interpolation;

namespace Vts.Modeling.ForwardSolvers
{
    public class pMCLoader
    {
        # region fields
        public static OpticalProperties ReferenceOps;
        public static PhotonDatabase PhotonTerminationDatabase;
        public static Output databaseOutput;
        public static DoubleRange databaseRhoRange;
        public static DoubleRange databaseTimeRange;
        #endregion

        public pMCLoader(string projectName, string folderName, string databaseName)
        {
            InitializeDatabase(projectName, folderName, databaseName);
        }
        /// <summary>
        /// InitializeDatabase reads in reference database and initializes data 
        /// </summary>
        private static void InitializeDatabase(string projectName,
            string folderName, string databaseName)
        {
            databaseOutput = Output.FromFolderInResources(folderName, projectName);
            // need to add the setting up of other ranges

            // todo: temp code to make this work with the new structure. revisit.
            var input = (ROfRhoAndTimeDetectorInput) databaseOutput.Input.DetectorInputs.Where(di => di.TallyType == TallyType.ROfRhoAndTime).First();

            databaseRhoRange = new DoubleRange(
                input.Rho.Start,
                input.Rho.Stop,
                input.Rho.Count);

            databaseTimeRange = new DoubleRange(
                input.Time.Start,
                input.Time.Stop,
                input.Time.Count);

            ReferenceOps = databaseOutput.Input.TissueInput.Regions[1].RegionOP;
   
            PhotonTerminationDatabase = PhotonDatabase.FromFileInResources(
                databaseName, projectName);
        }
    }
}
