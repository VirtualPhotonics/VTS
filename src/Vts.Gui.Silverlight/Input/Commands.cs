using SLExtensions.Input;

namespace Vts.Gui.Silverlight.Input
{
    public static class Commands
    {
        static Commands()
        {
            FS_SetIndependentVariableRange = new Command("FS_SetIndependentVariableRange");
            FS_ExecuteForwardSolver = new Command("FS_ExecuteForwardSolver");

            FluenceSolver_ExecuteFluenceSolver = new Command("FluenceSolver_ExecuteFluenceSolver");
            FluenceSolver_SetIndependentVariableRange = new Command("FluenceSolver_SetIndependentVariableRange");

            IS_SetIndependentVariableRange = new Command("IS_SetIndependentVariableRange");
            //IS_SimulateMeasuredData = new Command("IS_SimulateMeasuredData");
            //IS_CalculateInitialGuess = new Command("IS_CalculateInitialGuess");
            //IS_SolveInverse = new Command("IS_SolveInverse");
            


            FEM_ExecuteFemSolver = new Command("FEM_ExecuteMonteCarloSolver");
            
            PlotMuaSpectra = new Command("PlotMuaSpectra");
            PlotMusprimeSpectra = new Command("PlotMusprimeSpectra");
            UpdateOpticalProperties = new Command("UpdateOpticalProperties");

            Plot_PlotValues = new Command("Plot_PlotValues");

            Plot_AddLegendItem = new Command("Plot_AddLegendItem");
            Plot_SetAxesLabels = new Command("Plot_SetAxesLabels");
            Plot_ExportDataToText = new Command("Plot_ExportDataToText");

            Maps_PlotMap = new Command("Maps_PlotNewMap");

            Mesh_PlotMap = new Command("Mesh_PlotNewMap");
            Mesh_ExportDataToText = new Command("Mesh_ExportDataToText");

            TextOutput_PostMessage = new Command("TextOutput_PostMessage");

            IsoStorage_IncreaseSpaceQuery = new Command("IsoStorage_IncreaseSpaceQuery");

            Main_DuplicatePlotView = new Command("Main_DuplicatePlotView");
            Main_DuplicateMapView = new Command("Main_DuplicateMapView");
        }

        //public static Command Modeling_SetGaussianBeamSize { get; private set; }

        public static Command Main_DuplicatePlotView { get; private set; }
        public static Command Main_DuplicateMapView { get; private set; }

        // Forward solver commands
        public static Command FS_ExecuteForwardSolver { get; private set; }
        public static Command FS_SetIndependentVariableRange { get; private set; }

        // Fluence solver commands
        public static Command FluenceSolver_ExecuteFluenceSolver { get; private set; }
        public static Command FluenceSolver_SetIndependentVariableRange { get; private set; }

        // Inverse solver commands
        // public static Command IS_SimulateMeasuredData { get; private set; }
        // public static Command IS_CalculateInitialGuess { get; private set; }
        // public static Command IS_SolveInverse { get; private set; }
        public static Command IS_SetIndependentVariableRange { get; private set; }

        // Monte Carlo solver commands
        //public static Command MC_ExecuteMonteCarloSolver { get; private set; }
        //public static Command MC_PlotDataInResources { get; private set; }
        //public static Command MC_PlotScaledMC { get; private set; }

        // FEM Solver commands
        public static Command FEM_ExecuteFemSolver { get; private set; }

        //Spectra view commands
        public static Command PlotMuaSpectra { get; private set; }
        public static Command PlotMusprimeSpectra { get; private set; }
        public static Command UpdateOpticalProperties { get; private set; }

        // Plot commmands
        //public static Command Plot_PlotValuesLinearly { get; private set; }
        public static Command Plot_PlotValues { get; private set; }
        public static Command Plot_AddLegendItem { get; private set; }
        public static Command Plot_SetAxesLabels { get; private set; }
        public static Command Plot_ExportDataToText { get; private set; }

        // Fluence Map commands
        //public static Command Maps_CreateDemoMap { get; private set; }
        public static Command Maps_PlotMap { get; private set; }
        //public static Command Maps_ExportDataToText { get; private set; }

        // FEM Mesh commands
        public static Command Mesh_PlotMap { get; private set; }
        public static Command Mesh_ExportDataToText { get; private set; }

        // Text output commands
        public static Command TextOutput_PostMessage { get; private set; }

        // Isolated storage commands
        public static Command IsoStorage_IncreaseSpaceQuery { get; private set; }
    }
}
