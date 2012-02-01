//using SLExtensions.Input;

namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// View model implementing highest-level panel functionality for the general-purpose ATK
    /// </summary>
    public class SolverDemoViewModel : BindableObject
    {
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

        public SolverDemoViewModel()
        {
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
    }
}
