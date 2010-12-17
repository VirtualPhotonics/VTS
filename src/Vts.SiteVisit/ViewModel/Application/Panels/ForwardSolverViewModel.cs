using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SLExtensions.Input;
using Vts.Extensions;
using Vts.Factories;
using Vts.SiteVisit.Extensions;
using Vts.SiteVisit.Input;
using Vts.SiteVisit.Model;
#if WHITELIST
using Vts.SiteVisit.ViewModel.Application;
#endif

namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// View model implementing Forward Solver panel functionality
    /// </summary>
    public partial class ForwardSolverViewModel : BindableObject
    {
        private SolutionDomainOptionViewModel _SolutionDomainTypeOptionVM;
        private OptionViewModel<ForwardSolverType> _ForwardSolverTypeOptionVM;
        private OptionViewModel<ForwardAnalysisType> _ForwardAnalysisTypeOptionVM;
        private RangeViewModel _RangeVM;
        private OpticalPropertyViewModel _OpticalPropertyVM;

        // todo: This should be handled by a Controller, not dealt with directly
        // by the ViewModel. Job of ViewModel is just to "surface" Model information
        // to the View - shouldn't have to do any appreciable work/thinking/analysis.
        // Currently, the Factories class is serving this need, but I'm afraid it's 
        // getting too big...
        // public IForwardSolver ForwardSolver { get; set; }
        //private IAnalyzer Analyzer { get; set; }

        public ForwardSolverViewModel()
        {
            RangeVM = new RangeViewModel { Title = "Detection Parameters:" };
            OpticalPropertyVM = new OpticalPropertyViewModel { Title = "Optical Properties:" };

            // right now, we're doing manual databinding to the selected item. need to enable databinding 
            // confused, though - do we need to use strings? or, how to make generics work with dependency properties?
#if WHITELIST 
            ForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Forward Model:",false, WhiteList.ForwardSolverTypes);
#else 
            ForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Forward Model:",false);
#endif
            //ForwardSolverType.DistributedPointSourceSDA,
                //ForwardSolverType.PointSourceSDA,
                //ForwardSolverType.DistributedGaussianSourceSDA,
                //ForwardSolverType.MonteCarlo); // explicitly enabling these for the workshop;
            ForwardSolverTypeOptionVM.PropertyChanged += (sender, args) =>
            {
            //    ForwardSolver = SolverFactory.GetForwardSolver(ForwardSolverTypeOptionVM.SelectedValue);
                  OnPropertyChanged("IsGaussianForwardModel");
                  OnPropertyChanged("ForwardSolver");
            };

            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain:", SolutionDomainType.RofRho);

            ForwardAnalysisTypeOptionVM = new OptionViewModel<ForwardAnalysisType>("Model/Analysis Output:");

            SolutionDomainTypeOptionVM.SolverType = SolverType.Forward;

            // model
            //UpdateModels();

            Commands.FS_ExecuteForwardSolver.Executed += ExecuteForwardSolver_Executed;
            Commands.FS_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;
        }

        #region Properties


        public IForwardSolver ForwardSolver
        {
            get
            {
                return SolverFactory.GetForwardSolver(
                    ForwardSolverTypeOptionVM.SelectedValue);
            }
        }
        public bool IsGaussianForwardModel
        {
            get { return ForwardSolverTypeOptionVM.SelectedValue.IsGaussianForwardModel(); }
        }

        public SolutionDomainOptionViewModel SolutionDomainTypeOptionVM
        {
            get { return _SolutionDomainTypeOptionVM; }
            set
            {
                _SolutionDomainTypeOptionVM = value;
                OnPropertyChanged("SolutionDomainTypeOptionVM");
            }
        }
        public OptionViewModel<ForwardSolverType> ForwardSolverTypeOptionVM
        {
           get { return _ForwardSolverTypeOptionVM; }
            set
            {
                _ForwardSolverTypeOptionVM = value;
                 OnPropertyChanged("ForwardSolverTypeOptionVM");
            }
        }
        public RangeViewModel RangeVM
        {
            get { return _RangeVM; }
            set
            {
                _RangeVM = value;
                OnPropertyChanged("RangeVM");
            }
        }
        public OpticalPropertyViewModel OpticalPropertyVM
        {
            get { return _OpticalPropertyVM; }
            set
            {
                _OpticalPropertyVM = value;
                OnPropertyChanged("OpticalPropertyVM");
            }
        }
        public OptionViewModel<ForwardAnalysisType> ForwardAnalysisTypeOptionVM
        {
            get { return _ForwardAnalysisTypeOptionVM; }
            set
            {
                _ForwardAnalysisTypeOptionVM = value;
                OnPropertyChanged("ForwardAnalysisTypeOptionVM");
            }
        }

        #endregion

        //private void UpdateModels()
        //{
        //    ForwardSolver = SolverFactory.GetForwardSolver(ForwardSolverTypeOptionVM.SelectedValue);
        //}

        void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is RangeViewModel)
            {
                RangeVM = (RangeViewModel)e.Parameter;
            }
        }

        void ExecuteForwardSolver_Executed(object sender, ExecutedEventArgs e)
        {
            IEnumerable<Point> points = ExecuteForwardSolver();

            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            string plotLabel = GetLegendLabel();
            Commands.Plot_PlotValues.Execute(new PlotData(points, plotLabel));

            Commands.TextOutput_PostMessage.Execute("Forward Solver: " + OpticalPropertyVM + "\r");
        }

        private PlotAxesLabels GetPlotLabels()
        {
            var sd = this.SolutionDomainTypeOptionVM;
            PlotAxesLabels axesLabels = null;
            if (sd.IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                axesLabels = new PlotAxesLabels(
                    sd.IndependentAxisLabel, sd.IndependentAxisUnits,
                    sd.SelectedDisplayName, sd.SelectedValue.GetUnits(), sd.ConstantAxisLabel,
                    sd.ConstantAxisUnits, sd.ConstantAxisValue);
            }
            else
            {
                axesLabels = new PlotAxesLabels(sd.IndependentAxisLabel,
                    sd.IndependentAxisUnits, sd.SelectedDisplayName, sd.SelectedValue.GetUnits());
            }
            return axesLabels;
        }

        // todo: rename? this was to get a concise name for the legend
        private string GetLegendLabel()
        {
            string modelString = null;
            switch (ForwardSolverTypeOptionVM.SelectedValue)
            {
                case ForwardSolverType.DistributedGaussianSourceSDA:
                case ForwardSolverType.DistributedPointSourceSDA:
                case ForwardSolverType.PointSourceSDA:
                    modelString = "Model - SDA \r";
                    break;
                case ForwardSolverType.MonteCarlo:
                    modelString = "Model - scaled MC \r";
                    break;
                case ForwardSolverType.pMC:
                    modelString = "Model - pMC \r";
                    break;
                case ForwardSolverType.Nurbs:
                    modelString = "Model - nurbs \r";
                    break;
            }
            string opString = "μa=" + OpticalPropertyVM.Mua + " \rμs'=" + OpticalPropertyVM.Musp;

            return modelString + opString;
        }

        public IEnumerable<Point> ExecuteForwardSolver()
        {
            IEnumerable<double> independentValues = RangeVM.Values.ToList(); // ToList() necessary?
            
            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                    ? new double[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0];

            IEnumerable<double> query = ComputationFactory.GetVectorizedIndependentVariableQueryNew(
                ForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                independentValues,
                OpticalPropertyVM.GetOpticalProperties(),
                constantValues);

            // if it's reporting Real + Imaginary, we need a vector twice as long
            if (ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue))
            {
                return independentValues.Concat(independentValues).Zip(query, (x, y) => new Point(x, y));
            }
            else
            {
                return independentValues.Zip(query, (x, y) => new Point(x, y));
            }
        }
    }
}
