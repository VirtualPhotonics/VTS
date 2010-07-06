using Vts.Common;
using Vts.MonteCarlo;
using Vts.MonteCarlo.PhotonData;

//using MathNet.Numerics.Interpolation;

namespace Vts.Modeling.ForwardSolvers
{
    public class pMCLoader
    {
        # region fields
        /// CKH TODO: automate pointer to reference data 
        public static string folder = "ReferenceData/N1e6mua0musp1g0p8dr0p2dt0p005/";
        public static OpticalProperties ReferenceOps;
        public static PhotonTerminationDatabase PhotonTerminationDatabase;
        public static Output databaseOutput;
        public static DoubleRange databaseRhoRange;
        public static DoubleRange databaseTimeRange;
        #endregion

        static pMCLoader()
        {
            InitializeVectorsAndInterpolators();
        }
        /// <summary>
        /// InitializeVectorsAndInterpolators reads in reference database and initializes data 
        /// </summary>
        private static void InitializeVectorsAndInterpolators()
        {
            databaseOutput = Output.FromFolderInResources("Modeling/Resources/" + folder, "Vts");
            databaseRhoRange = new DoubleRange(
                databaseOutput.input.DetectorInput.Rho.Start,
                databaseOutput.input.DetectorInput.Rho.Stop,
                databaseOutput.input.DetectorInput.Rho.Count);
            databaseTimeRange = new DoubleRange(
                databaseOutput.input.DetectorInput.Time.Start,
                databaseOutput.input.DetectorInput.Time.Stop,
                databaseOutput.input.DetectorInput.Time.Count);
            ReferenceOps = databaseOutput.input.TissueInput.Regions[1].RegionOP;
   
            PhotonTerminationDatabase = PhotonTerminationDatabase.FromFileInResources("photonBiographies1e6", "Vts.Resources");
            //PhotonTerminationDatabase = PhotonTerminationDatabase.FromFileInResources("Resources/photonBiographies1e6", "Vts");
        }
    }
}
