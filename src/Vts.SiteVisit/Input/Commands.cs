using SLExtensions.Input;

namespace Vts.SiteVisit.Input
{
    public static class Commands
    {
        static Commands()
        {
            //Modeling_SetGaussianBeamSize = new Command("Modeling_SetGaussianBeamSize");

            FS_SetIndependentVariableRange = new Command("FS_SetIndependentVariableRange");
            FS_ExecuteForwardSolver = new Command("FS_ExecuteForwardSolver");

            FluenceSolver_ExecuteFluenceSolver = new Command("FluenceSolver_ExecuteFluenceSolver");
            FluenceSolver_SetIndependentVariableRange = new Command("FluenceSolver_SetIndependentVariableRange");

            IS_SetIndependentVariableRange = new Command("IS_SetIndependentVariableRange");
            IS_SimulateMeasuredData = new Command("IS_SimulateMeasuredData");
            IS_CalculateInitialGuess = new Command("IS_CalculateInitialGuess");
            IS_SolveInverse = new Command("IS_SolveInverse");

            MC_ExecuteMonteCarloSolver = new Command("MC_ExecuteMonteCarloSolver");
            MC_PlotDataInResources = new Command("MC_PlotDataInResources");
            MC_PlotScaledMC = new Command("MC_PlotScaledMC");
            
            PlotMuaSpectra = new Command("PlotMuaSpectra");
            PlotMusprimeSpectra = new Command("PlotMusprimeSpectra");

            //Plot_PlotValuesLinearly = new Command("Plot_PlotValuesLinearly");
            Plot_PlotValues = new Command("Plot_PlotValues");
            Plot_ClearPlot = new Command("Plot_ClearPlot");
            Plot_ClearPlotSingle = new Command("Plot_ClearPlotSingle");
            Plot_ClearLegend = new Command("Plot_ClearLegend");
            Plot_AddLegendItem = new Command("Plot_AddLegendItem");
            Plot_SetAxesLabels = new Command("Plot_SetAxesLabels");
            Plot_ExportDataToText = new Command("Plot_ExportDataToText");

            Maps_CreateDemoMap = new Command("Maps_CreateNewMap");
            Maps_PlotMap = new Command("Maps_PlotNewMap");
            Maps_ExportDataToText = new Command("Maps_ExportDataToText");

            TextOutput_PostMessage = new Command("TextOutput_PostMessage");
        }

        //public static Command Modeling_SetGaussianBeamSize { get; private set; }

        // Forward solver commands
        public static Command FS_ExecuteForwardSolver { get; private set; }
        public static Command FS_SetIndependentVariableRange { get; private set; }

        // Fluence solver commands
        public static Command FluenceSolver_ExecuteFluenceSolver { get; private set; }
        public static Command FluenceSolver_SetIndependentVariableRange { get; private set; }

        // Inverse solver commands
        public static Command IS_SimulateMeasuredData { get; private set; }
        public static Command IS_CalculateInitialGuess { get; private set; }
        public static Command IS_SolveInverse { get; private set; }
        public static Command IS_SetIndependentVariableRange { get; private set; }

        // Monte Carlo solver commands
        public static Command MC_ExecuteMonteCarloSolver { get; private set; }
        public static Command MC_PlotDataInResources { get; private set; }
        public static Command MC_PlotScaledMC { get; private set; }

        //Spectra view commands
        public static Command PlotMuaSpectra { get; private set; }
        public static Command PlotMusprimeSpectra { get; private set; }

        // Plot commmands
        //public static Command Plot_PlotValuesLinearly { get; private set; }
        public static Command Plot_PlotValues { get; private set; }
        public static Command Plot_ClearPlot { get; private set; }
        public static Command Plot_ClearPlotSingle { get; private set; }
        public static Command Plot_ClearLegend { get; private set; }
        public static Command Plot_AddLegendItem { get; private set; }
        public static Command Plot_SetAxesLabels { get; private set; }
        public static Command Plot_ExportDataToText { get; private set; }

        // Fluence Map commands
        public static Command Maps_CreateDemoMap { get; private set; }
        public static Command Maps_PlotMap { get; private set; }
        public static Command Maps_ExportDataToText { get; private set; }

        // Text output commands
        public static Command TextOutput_PostMessage { get; private set; }
    }
}
