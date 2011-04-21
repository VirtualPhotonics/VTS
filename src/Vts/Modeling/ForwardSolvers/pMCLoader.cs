using System.Linq;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;

//using MathNet.Numerics.Interpolation;

namespace Vts.Modeling.ForwardSolvers
{
    public class pMCLoader
    {
        # region fields
        public static OpticalProperties ReferenceOps;
        public static pMCDatabase PhotonTerminationDatabase;
        public static Output databaseOutput;
        public static DoubleRange databaseRhoRange;
        public static DoubleRange databaseTimeRange;
        #endregion

        public pMCLoader(string projectName, string folderName, string photonDatabaseName, string collisionInfoDatabaseName)
        {
            InitializeDatabase(projectName, folderName, photonDatabaseName, collisionInfoDatabaseName);
        }
        /// <summary>
        /// InitializeDatabase reads in reference database and initializes data 
        /// </summary>
        private static void InitializeDatabase(string projectName,
            string folderName, string PhotonDatabaseName, string collisionInfoDatabaseName)
        {
            // databaseOutput = Output.FromFolderInResources(folderName, projectName); // old IO
            //var detector = (ROfRhoAndTimeDetector)DetectorIO.ReadDetectorFromFileInResources(TallyType.ROfRhoAndTime, folderName, projectName); // new IO

            // old IO
            // need to add the setting up of other ranges
            //// todo: temp code to make this work with the new structure. revisit.
            //var input = (ROfRhoAndTimeDetectorInput) databaseOutput.Input.DetectorInputs.Where(di => di.TallyType == TallyType.ROfRhoAndTime).First();
            //databaseRhoRange = new DoubleRange(
            //    input.Rho.Start,
            //    input.Rho.Stop,
            //    input.Rho.Count);
            //databaseTimeRange = new DoubleRange(
            //    input.Time.Start,
            //    input.Time.Stop,
            //    input.Time.Count);

            //// new IO
            //databaseRhoRange = new DoubleRange(
            //    detector.Rho.Start,
            //    detector.Rho.Stop,
            //    detector.Rho.Count);

            //databaseTimeRange = new DoubleRange(
            //    detector.Time.Start,
            //    detector.Time.Stop,
            //    detector.Time.Count);

            //ReferenceOps = databaseOutput.Input.TissueInput.Regions[1].RegionOP;
   
            PhotonTerminationDatabase = pMCDatabase.FromFileInResources(
                PhotonDatabaseName, collisionInfoDatabaseName, projectName);
        }
    }
}
