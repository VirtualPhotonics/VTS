//using SLExtensions.Input;

using System.Reflection;

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing highest-level panel functionality for the general-purpose ATK
    /// </summary>
    public class SolverDemoViewModel : BindableObject
    {
        public SolverDemoViewModel()
        {
            Current = this;

            ForwardSolverVM = new ForwardSolverViewModel();
            InverseSolverVM = new InverseSolverViewModel();
            FluenceSolverVM = new FluenceSolverViewModel();
            MonteCarloSolverVM = new MonteCarloSolverViewModel();
            FemSolverVM = new FemSolverViewModel();
            SpectralMappingVM = new SpectralMappingViewModel();
            PlotVM = new PlotViewModel();
            MapVM = new MapViewModel();
            MeshVM = new MeshViewModel();
            TextOutputVM = new TextOutputViewModel();
        }

        public static SolverDemoViewModel Current { get; set; }

        public ForwardSolverViewModel ForwardSolverVM { get; private set; }
        public InverseSolverViewModel InverseSolverVM { get; private set; }
        public FluenceSolverViewModel FluenceSolverVM { get; private set; }
        public MonteCarloSolverViewModel MonteCarloSolverVM { get; private set; }
        public FemSolverViewModel FemSolverVM { get; private set; }
        public SpectralMappingViewModel SpectralMappingVM { get; private set; }
        public PlotViewModel PlotVM { get; private set; }
        public MapViewModel MapVM { get; private set; }
        public MeshViewModel MeshVM { get; private set; }
        public TextOutputViewModel TextOutputVM { get; private set; }
        public string Version
        {
            get
            {
                var currentVersion = new AssemblyName(Assembly.GetExecutingAssembly().FullName);
                return currentVersion.Version.Major.ToString() + "." + currentVersion.Version.Minor.ToString() + "." + currentVersion.Version.Build.ToString();
                //              return currentVersion.Version.ToString(); // This line returns all 4 version numbers Major.Minor.Build.Revision
            }
        }
    }
}
