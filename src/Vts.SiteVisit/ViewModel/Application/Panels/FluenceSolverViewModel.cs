using System;
using System.Collections.Generic;
using System.Linq;
using SLExtensions.Input;
using Vts.Common;
using Vts.Extensions;
using Vts.Factories;
using Vts.SiteVisit.Extensions;
using Vts.SiteVisit.Input;
using Vts.SiteVisit.Model;

namespace Vts.SiteVisit.ViewModel
{
    /// <summary>
    /// View model implementing Fluence Solver panel functionality
    /// </summary>
    public partial class FluenceSolverViewModel : BindableObject
    {
        #region Fields

        private OptionViewModel<MapType> _MapTypeOptionVM;
        private FluenceSolutionDomainOptionViewModel _SolutionDomainTypeOptionVM;
        private OptionViewModel<ForwardSolverType> _ForwardSolverTypeOptionVM;
        //private OptionViewModel<ForwardAnalysisType> _ForwardAnalysisTypeOptionVM;

        private RangeViewModel _RhoRangeVM;
        private RangeViewModel _ZRangeVM;
        private double _SourceDetectorSeparation;

        private OpticalPropertyViewModel _OpticalPropertyVM;

        // todo: This should be handled by a Controller, not dealt with directly
        // by the ViewModel. Job of ViewModel is just to "surface" Model information
        // to the View - shouldn't have to do any appreciable work/thinking/analysis.
        // Currently, the Factories class is serving this need, but I'm afraid it's 
        // getting too big...
        public IForwardSolver ForwardSolver { get; set; }
        //private IAnalyzer Analyzer { get; set; }

        #endregion

        #region Constructor

        public FluenceSolverViewModel()
        {
            RhoRangeVM = new RangeViewModel(new DoubleRange(0.1, 19.9, 100), "mm", "");
            ZRangeVM = new RangeViewModel(new DoubleRange(0.1, 19.9, 100), "mm", "");
            SourceDetectorSeparation = 10.0;

            OpticalPropertyVM = new OpticalPropertyViewModel() { Title = "Optical Properties:" };

            // right now, we're doing manual databinding to the selected item. need to enable databinding 
            // confused, though - do we need to use strings? or, how to make generics work with dependency properties?
            ForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>(
                "Forward Model:",
                false,
                ForwardSolverType.DistributedPointSDA,
                ForwardSolverType.PointSDA,
                ForwardSolverType.DistributedGaussianSDA); // explicitly enabling these for the workshop;

            ForwardSolverTypeOptionVM.PropertyChanged += (sender, args) =>
            {
                ForwardSolver = SolverFactory.GetForwardSolver(ForwardSolverTypeOptionVM.SelectedValue);
                this.OnPropertyChanged("IsGaussianForwardModel");
                this.OnPropertyChanged("ForwardSolver");
            };

            SolutionDomainTypeOptionVM = new FluenceSolutionDomainOptionViewModel("Solution Domain", FluenceSolutionDomainType.FluenceofRho);

            MapTypeOptionVM = new OptionViewModel<MapType>(
                "Map Type", 
                MapType.Fluence, 
                MapType.AbsorbedEnergy, 
                MapType.PhotonHittingDensity);

            MapTypeOptionVM.PropertyChanged += (sender, args) =>
            {
                this.OnPropertyChanged("IsFluence");
                this.OnPropertyChanged("IsAbsorbedEnergy");
                this.OnPropertyChanged("IsPhotonHittingDensity");
            };

            // ForwardAnalysisTypeOptionVM = new OptionViewModel<ForwardAnalysisType>("Model/Analysis Output");

            // model
            UpdateModels();

            Commands.FluenceSolver_ExecuteFluenceSolver.Executed += ExecuteFluenceSolver_Executed;
            Commands.FluenceSolver_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;
        }

        #endregion

        #region Properties

        public bool IsGaussianForwardModel
        {
            get { return ForwardSolverTypeOptionVM.SelectedValue.IsGaussianForwardModel(); }
        }

        public bool IsFluence { get { return MapTypeOptionVM.SelectedValue == MapType.Fluence; } }
        public bool IsAbsorbedEnergy { get { return MapTypeOptionVM.SelectedValue == MapType.AbsorbedEnergy; } }
        public bool IsPhotonHittingDensity { get { return MapTypeOptionVM.SelectedValue == MapType.PhotonHittingDensity; } }

        public OptionViewModel<MapType> MapTypeOptionVM
        {
            get { return _MapTypeOptionVM; }
            set
            {
                _MapTypeOptionVM = value;
                OnPropertyChanged("MapTypeOptionVM");
            }
        }
        public FluenceSolutionDomainOptionViewModel SolutionDomainTypeOptionVM
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
        public RangeViewModel RhoRangeVM
        {
            get { return _RhoRangeVM; }
            set
            {
                _RhoRangeVM = value;
                OnPropertyChanged("RhoRangeVM");
            }
        }
        public double SourceDetectorSeparation
        {
            get { return _SourceDetectorSeparation; }
            set
            {
                _SourceDetectorSeparation = value;
                OnPropertyChanged("SourceDetectorSeparation");
            }
        }
        public RangeViewModel ZRangeVM
        {
            get { return _ZRangeVM; }
            set
            {
                _ZRangeVM = value;
                OnPropertyChanged("ZRangeVM");
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
        //public OptionViewModel<ForwardAnalysisType> ForwardAnalysisTypeOptionVM
        //{
        //    get { return _ForwardAnalysisTypeOptionVM; }
        //    set
        //    {
        //        _ForwardAnalysisTypeOptionVM = value;
        //        OnPropertyChanged("ForwardAnalysisTypeOptionVM");
        //    }
        //}

        #endregion

        private void UpdateModels()
        {
            ForwardSolver = SolverFactory.GetForwardSolver(ForwardSolverTypeOptionVM.SelectedValue);
        }

        void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is RangeViewModel)
            {
                RhoRangeVM = (RangeViewModel)e.Parameter;
            }
        }

        void ExecuteFluenceSolver_Executed(object sender, ExecutedEventArgs e)
        {
            var mapData = ExecuteForwardSolver();

            Commands.Maps_PlotMap.Execute(mapData);

            //PlotAxesLabels axesLabels = GetPlotLabels();
            //Commands.Plot_SetAxesLabels.Execute(axesLabels);

            //string plotLabel = GetLegendLabel();
            //Commands.Plot_PlotValues.Execute(new PlotData(points, plotLabel));

            Commands.TextOutput_PostMessage.Execute("Fluence Solver: " + OpticalPropertyVM + "\r");
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
            string modelString = 
                ForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedPointSDA ||
                ForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.PointSDA  ||
                ForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedGaussianSDA
                ? "Model - SDA\r" : "Model - MC scaled\r";
            string opString = "μa=" + OpticalPropertyVM.Mua + "\rμs'=" + OpticalPropertyVM.Musp;

            return modelString + opString;
        }

        public MapData ExecuteForwardSolver()
        {
            double[] rhos = RhoRangeVM.Values.Reverse().Select(rho => -rho).Concat(RhoRangeVM.Values).ToArray();
            double[] zs = ZRangeVM.Values.ToArray();

            IEnumerable<double>[] independentValues = new[] {rhos, zs };

            // todo: too much thinking at the VM layer?
            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                    ? new double[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0];

            IndependentVariableAxis[] independentAxes = 
                GetIndependentVariableAxesInOrder(
                    SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                    IndependentVariableAxis.Z);

            double[] fluence =
                ComputationFactory.GetVectorizedMultidimensionalIndependentVariableQueryNew(
                    ForwardSolver,
                    SolutionDomainTypeOptionVM.SelectedValue,
                    independentAxes,
                    independentValues,
                    OpticalPropertyVM.GetOpticalProperties(),
                    constantValues).ToArray();

            double[] results = null;
            switch (MapTypeOptionVM.SelectedValue)
            {
                case MapType.Fluence:
                default:
                    results = fluence;
                    break;
                case MapType.AbsorbedEnergy:
                    results = ComputationFactory.GetAbsorbedEnergy(fluence, OpticalPropertyVM.GetOpticalProperties().Mua).ToArray();
                    break;
                case MapType.PhotonHittingDensity:
                    results = ComputationFactory.GetPHD(
                        ForwardSolver,
                        fluence,
                        SourceDetectorSeparation,
                        OpticalPropertyVM.GetOpticalProperties().AsEnumerable(),
                        independentValues[0],
                        independentValues[1]).ToArray();
                    break;
            }

            // flip the array (since it goes over zs and then rhos, while map wants rhos and then zs
            double[] destinationArray = new double[results.Length];
            long index = 0;
            for (int rhoi = 0; rhoi < rhos.Length; rhoi++)
            {
                for (int zi = 0; zi < zs.Length; zi++)
                {
                    destinationArray[rhoi + rhos.Length * zi] = results[index++];
                }
            }

            return new MapData(destinationArray, rhos, zs);
        }

        private static IndependentVariableAxis[] GetIndependentVariableAxesInOrder(params IndependentVariableAxis[] axes)
        {
            if (axes.Length <= 0)
                throw new ArgumentNullException("axes");

            var sortedAxes = axes.OrderBy(ax => ax.GetMaxArgumentLocation()).ToArray();

            return sortedAxes;
        }
    }
}
