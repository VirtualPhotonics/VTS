using System.Linq;
using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
using Vts.MonteCarlo.PhotonData;

//using MathNet.Numerics.Interpolation;

namespace Vts.Modeling.ForwardSolvers
{
    /// <summary>
    /// class to load database for perturbation Monte Carlo results in gui
    /// </summary>
    public class pMCLoader
    {
        # region fields
        /// <summary>
        /// optical properties of the reference database
        /// </summary>
        public static OpticalProperties ReferenceOps;
        /// <summary>
        /// perturbation Monte Carlo database
        /// </summary>
        public static pMCDatabase PMCDatabase;
        /// <summary>
        /// SimulationInput used to generate the pMC database
        /// </summary>
        public static SimulationInput DatabaseInput;
        /// <summary>
        /// rho binning used in database generation
        /// </summary>
        public static DoubleRange databaseRhoRange;
        /// <summary>
        /// time binning used in database generation
        /// </summary>
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
