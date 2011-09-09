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
        public static pMCDatabase PMCDatabase;
        public static SimulationInput DatabaseInput;
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
            string folderName, string photonDatabaseName, string collisionInfoDatabaseName)
        {   
            PMCDatabase = pMCDatabase.FromFileInResources(
                photonDatabaseName, collisionInfoDatabaseName, projectName);
            DatabaseInput = SimulationInput.FromFileInResources("infile.xml", projectName);
            var detectorInput = (ROfRhoAndTimeDetectorInput)DatabaseInput.DetectorInputs[0];
            databaseRhoRange = detectorInput.Rho;
            databaseTimeRange = detectorInput.Time;
        }
    }
}
