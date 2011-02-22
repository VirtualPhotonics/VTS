using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

//using MathNet.Numerics.Interpolation;

namespace Vts.Modeling.ForwardSolvers
{
    public class pMCLoader
    {
        # region fields
        public static OpticalProperties ReferenceOps;
        public static PhotonTerminationDatabase PhotonTerminationDatabase;
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
            databaseRhoRange = new DoubleRange(
                databaseOutput.Input.DetectorInput.Rho.Start,
                databaseOutput.Input.DetectorInput.Rho.Stop,
                databaseOutput.Input.DetectorInput.Rho.Count);
            databaseTimeRange = new DoubleRange(
                databaseOutput.Input.DetectorInput.Time.Start,
                databaseOutput.Input.DetectorInput.Time.Stop,
                databaseOutput.Input.DetectorInput.Time.Count);
            ReferenceOps = databaseOutput.Input.TissueInput.Regions[1].RegionOP;
   
            PhotonTerminationDatabase = PhotonTerminationDatabase.FromFileInResources(
                databaseName, projectName);
        }

    }
}
