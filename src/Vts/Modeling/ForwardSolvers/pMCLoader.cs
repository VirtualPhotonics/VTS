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
                databaseOutput.input.DetectorInput.Rho.Start,
                databaseOutput.input.DetectorInput.Rho.Stop,
                databaseOutput.input.DetectorInput.Rho.Count);
            databaseTimeRange = new DoubleRange(
                databaseOutput.input.DetectorInput.Time.Start,
                databaseOutput.input.DetectorInput.Time.Stop,
                databaseOutput.input.DetectorInput.Time.Count);
            ReferenceOps = databaseOutput.input.TissueInput.Regions[1].RegionOP;
   
            PhotonTerminationDatabase = PhotonTerminationDatabase.FromFileInResources(
                databaseName, projectName);
        }

    }
}
