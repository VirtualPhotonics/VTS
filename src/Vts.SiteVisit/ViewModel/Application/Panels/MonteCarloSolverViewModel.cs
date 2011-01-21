using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SLExtensions.Input;
using Vts.Extensions;
using Vts.Factories;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.SiteVisit.Input;
using Vts.SiteVisit.Model;

namespace Vts.SiteVisit.ViewModel
{
    /// <summary> 
    /// View model implementing the Monte Carlo panel functionality (experimental)
    /// </summary>
    public partial class MonteCarloSolverViewModel : BindableObject
    {
        #region Fields
        private SolutionDomainOptionViewModel _SolutionDomainTypeOptionVM;
        //private OptionViewModel<ForwardSolverType> _ForwardSolverTypeVM;

        private RangeViewModel _RangeVM;

        private OpticalPropertyViewModel _OpticalPropertyVM;

        // todo: this is the model - shouldn't be exposed by the viewmodel
        private IForwardSolver ForwardSolver { get; set; }
        //private IAnalyzer Analyzer { get; set; }

        #endregion

        #region Constructor

        public MonteCarloSolverViewModel()
        {
            RangeVM = new RangeViewModel() { Title = "" };
            OpticalPropertyVM = new OpticalPropertyViewModel() { Title = "Optical Properties:" };

            // right now, we're doing manual databinding to the selected item. need to enable databinding 
            // confused, though - do we need to use strings? or, how to make generics work with dependency properties?
            //ForwardSolverTypeViewModel = new OptionControlViewModel<ForwardSolverType>("Forward Model");
            //ForwardSolverTypeViewModel.PropertyChanged += (sender, args) =>
            //{
            //    ForwardSolver = Factories.GetForwardSolver(ForwardSolverTypeViewModel.SelectedValue);
            //};
            ForwardSolver = SolverFactory.GetForwardSolver(ForwardSolverType.MonteCarlo);

            //AnalyzerTypeViewModel = new OptionControlViewModel<AnalyzerType>("Analyzer");
            //AnalyzerTypeViewModel.PropertyChanged += (sender, args) =>
            //{
            //    Analyzer = Factories.GetAnalyzer(AnalyzerTypeViewModel.SelectedValue, ForwardSolver);
            ////};
            //Analyzer = Factories.GetAnalyzer(AnalyzerType.Numeric, ForwardSolver);

            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain", SolutionDomainType.RofRho);

            //ForwardAnalysisTypeViewModel = new OptionControlViewModel<ForwardAnalysisType>("Model/Analysis Output");

            // model
            //UpdateModels();

            Commands.MC_ExecuteMonteCarloSolver.Executed += MC_ExecuteMonteCarloSolver_Executed;
            Commands.FS_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;
        }

        #endregion

        void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is RangeViewModel)
            {
                RangeVM = (RangeViewModel)e.Parameter;
            }
        }

        void MC_ExecuteMonteCarloSolver_Executed(object sender, ExecutedEventArgs e)
        {
            IEnumerable<Point> points = ExecuteMonteCarloSolver();

            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            string plotLabel = GetPlotLabel();
            Commands.Plot_PlotValues.Execute(new PlotData(points, plotLabel));

            //Commands.ClearLegend.Execute();
            //Commands.AddLegendItem.Execute("test");

            Commands.TextOutput_PostMessage.Execute("Monte Carlo Solver: " + OpticalPropertyVM + "\r");
        }

        private PlotAxesLabels GetPlotLabels()
        {
            var sd = this.SolutionDomainTypeOptionVM;
            PlotAxesLabels axesLabels = null;
            if (sd.IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                axesLabels = new PlotAxesLabels(
                    sd.IndependentAxisLabel, sd.IndependentAxisUnits,
                    sd.SelectedDisplayName, "", sd.ConstantAxisLabel,
                    sd.ConstantAxisUnits, sd.ConstantAxisValue);
            }
            else
            {
                axesLabels = new PlotAxesLabels(sd.IndependentAxisLabel,
                    sd.IndependentAxisUnits, sd.SelectedDisplayName, "");
            }
            return axesLabels;
        }

        private string GetPlotLabel()
        {
            string modelString = "Model - MC\r";
            string opString = "μa=" + OpticalPropertyVM.Mua + "\rμs'=" + OpticalPropertyVM.Musp;

            return modelString + opString;
        }


        #region Properties

        public SolutionDomainOptionViewModel SolutionDomainTypeOptionVM
        {
            get { return _SolutionDomainTypeOptionVM; }
            set
            {
                _SolutionDomainTypeOptionVM = value;
                OnPropertyChanged("SolutionDomainTypeOptionVM");
            }
        }
        //public OptionControlViewModel<ForwardSolverType> ForwardSolverTypeViewModel
        //{
        //    get { return _ForwardSolverTypeViewModel; }
        //    set
        //    {
        //        _ForwardSolverTypeViewModel = value;
        //        OnPropertyChanged("ForwardSolverTypeViewModel");
        //    }
        //}
        public RangeViewModel RangeVM
        {
            get { return _RangeVM; }
            set
            {
                _RangeVM = value;
                OnPropertyChanged("RangeVM");
            }
        }
        //public OptionControlViewModel<AnalyzerType> AnalyzerTypeViewModel
        //{
        //    get { return _AnalyzerTypeViewModel; }
        //    set
        //    {
        //        _AnalyzerTypeViewModel = value;
        //        OnPropertyChanged("AnalyzerTypeViewModel");
        //    }
        //}
        public OpticalPropertyViewModel OpticalPropertyVM
        {
            get { return _OpticalPropertyVM; }
            set
            {
                _OpticalPropertyVM = value;
                OnPropertyChanged("OpticalPropertyVM");
            }
        }
        //public OptionControlViewModel<ForwardAnalysisType> ForwardAnalysisTypeViewModel
        //{
        //    get { return _ForwardAnalysisTypeViewModel; }
        //    set
        //    {
        //        _ForwardAnalysisTypeViewModel = value;
        //        OnPropertyChanged("ForwardAnalysisTypeViewModel");
        //    }
        //}

        #endregion

        //private void UpdateModels()
        //{
        //    ForwardSolver = Factories.GetForwardSolver(ForwardSolverTypeViewModel.SelectedValue);
        //    Analyzer = Factories.GetAnalyzer(AnalyzerTypeViewModel.SelectedValue, ForwardSolver);
        //}

        // so much branching! if we had Func<double, double[], double[]> these would all collapse!

        public IEnumerable<Point> ExecuteMonteCarloSolver()
        {
            IEnumerable<double> independentValues = RangeVM.Values.ToList();
            IEnumerable<double> query = null;

            SimulationInput input = new SimulationInput();
            input.OutputFileName = "MonteCarloOutput";
            input.N = 100;
            input.Options = new SimulationOptions(
                0, // Note seed = 0 is -1 in linux
                RandomNumberGeneratorType.MersenneTwister,
                AbsorptionWeightingType.Discrete);
            input.SourceInput = new CustomPointSourceInput();
            input.TissueInput = new MultiLayerTissueInput();
            input.DetectorInput = new DetectorInput();
            ////input.Tissue.num_layers = 1;
            
            //input.Tissue.Regions[0].n = 1.0; /* idx of outside medium */
            //input.Tissue.Regions[1].n = OpticalPropertyVM.N;
            //input.Tissue.Regions[1].g = OpticalPropertyVM.G;
            ////convert to cm-1
            //input.Tissue.Regions[1].mua = OpticalPropertyVM.Mua * 10;
            //input.Tissue.Regions[1].mus = OpticalPropertyVM.Musp * 10/(1.0-OpticalPropertyVM.G);

            //input.Tissue.Regions[1].d = 10.0;
            //input.Tissue.Regions[2].n = 1.0; /* idx of out bot med */

            //input.source.beamtype = "f";
            //input.source.beam_center_x = 0.0;
            //input.source.beam_radius = 0.0;
            //input.source.src_NA = 0.0;

            //input.detector.nr = (short)RangeVM.Number;
            //input.detector.dr = (RangeVM.Stop-RangeVM.Start)/RangeVM.Number;
            //input.detector.nz = 10;
            //input.detector.dz = 0.1;
            //input.detector.nx = 80;
            //input.detector.dx = 0.05;
            //input.detector.ny = 80;
            //input.detector.dy = 0.05;

            //input.source.num_photons = 10;
            //input.detector.nt = 20; // this needs to match MonteCarloDataLoader for now
            //input.detector.dt = 50.0; // this needs to match MonteCarloDataLoader for now 
            //input.tissptr.do_ellip_layer = 0; /* 0=no pert, 1=ellip, 2=layer, 3=ellip no pMC */
            //input.tissptr.ellip_x = 0.0;
            //input.tissptr.ellip_y = 0.0;
            //input.tissptr.ellip_z = 0.3;
            //input.tissptr.ellip_rad_x = 0.2;
            //input.tissptr.ellip_rad_y = 0.2;
            //input.tissptr.ellip_rad_z = 0.2;
            //input.tissptr.layer_z_min = 0.0;
            //input.tissptr.layer_z_max = 0.02;
            //input.detector.reflect_flag = true;
            //input.detector.det_ctr = new double[] { 0.0 }; /* THESE NEED TO MATCH PERT REGION */
            //input.detector.radius = 4.0;

            MonteCarloSimulation simulation = new MonteCarloSimulation(input);
            Output output = simulation.Run();
            query = output.R_r;

            //if (IsFourVariableSolver(SolutionDomainTypeViewModel.SelectedValue)) // if it's a 4-variable forward kernel
            //{
            //    var func4Var = Factories.GetFourVariableSolverFunc(ForwardSolver, SolutionDomainTypeViewModel.SelectedValue);
            //    query = Factories.GetFourVariableLinqQuery(
            //        ForwardAnalysisType.R,
            //        OpticalPropertyControlViewModel.GetOpticalProperties(),
            //        independentValues,
            //        func4Var,
            //        Analyzer);
            //}
            //else // if it's a 5-variable forward kernel
            //{
            //    var func5Var = Factories.GetFiveVariableSolverFunc(ForwardSolver, SolutionDomainTypeViewModel.SelectedValue);
            //    query = Factories.GetFiveVariableLinqQuery(
            //        ForwardAnalysisType.R,
            //        OpticalPropertyControlViewModel.GetOpticalProperties(),
            //        SolutionDomainTypeViewModel.IndependentVariableAxisViewModel.SelectedValue,
            //        independentValues,
            //        SolutionDomainTypeViewModel.ConstantAxisValue,
            //        func5Var,
            //        Analyzer);
            //}

            //var points = independentValues.Zip(query, (x, y) => new Point(x, y));
            //return points;
            return  EnumerableEx.Zip(independentValues, query, (x, y) => new Point(x, y));
        }

        private bool IsFourVariableSolver(SolutionDomainType solutionDomainType)
        {
            return
                solutionDomainType == SolutionDomainType.RofRho ||
                solutionDomainType == SolutionDomainType.RofFx;
        }
    }
}
