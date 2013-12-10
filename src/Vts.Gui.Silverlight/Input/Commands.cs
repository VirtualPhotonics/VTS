using SLExtensions.Input;

namespace Vts.Gui.Silverlight.Input
{
    public static class Commands
    {
        static Commands()
        {
            SD_SetWavelength = new Command("SD_SetWavelength");

            Spec_UpdateWavelength = new Command("Spec_UpdateWavelength");
            Spec_UpdateOpticalProperties = new Command("Spec_UpdateOpticalProperties");

            Plot_PlotValues = new Command("Plot_PlotValues");
            Plot_AddLegendItem = new Command("Plot_AddLegendItem");
            Plot_SetAxesLabels = new Command("Plot_SetAxesLabels");
            Plot_SetRequestedIndependentVariableAxis = new Command("Plot_SetRequestedIndependentVariableAxis");

            Maps_PlotMap = new Command("Maps_PlotNewMap");

            Mesh_PlotMap = new Command("Mesh_PlotNewMap");

            TextOutput_PostMessage = new Command("TextOutput_PostMessage");

            IsoStorage_IncreaseSpaceQuery = new Command("IsoStorage_IncreaseSpaceQuery");

            Main_DuplicatePlotView = new Command("Main_DuplicatePlotView");
            Main_DuplicateMapView = new Command("Main_DuplicateMapView");
        }

        public static Command Main_DuplicatePlotView { get; private set; }
        public static Command Main_DuplicateMapView { get; private set; }
        
        // Solution domain commands
        public static Command SD_SetWavelength { get; private set; }

        //Spectra view commands
        public static Command Spec_UpdateWavelength { get; private set; }
        public static Command Spec_UpdateOpticalProperties { get; private set; }

        // Plot commmands
        public static Command Plot_PlotValues { get; private set; }
        public static Command Plot_AddLegendItem { get; private set; }
        public static Command Plot_SetAxesLabels { get; private set; }
        public static Command Plot_SetRequestedIndependentVariableAxis { get; private set; }

        // Fluence Map commands
        public static Command Maps_PlotMap { get; private set; }

        // FEM Mesh commands
        public static Command Mesh_PlotMap { get; private set; }

        // Text output commands
        public static Command TextOutput_PostMessage { get; private set; }

        // Isolated storage commands
        public static Command IsoStorage_IncreaseSpaceQuery { get; private set; }
    }
}
