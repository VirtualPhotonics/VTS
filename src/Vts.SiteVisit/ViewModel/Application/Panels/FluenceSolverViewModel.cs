using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private OptionViewModel<MapType> _MapTypeOptionVM;
        private FluenceSolutionDomainOptionViewModel _FluenceSolutionDomainTypeOptionVM;
        private FluenceSolutionDomainOptionViewModel _AbsorbedEnergySolutionDomainTypeOptionVM;
        private FluenceSolutionDomainOptionViewModel _PhotonHittingDensitySolutionDomainTypeOptionVM;
        private OptionViewModel<ForwardSolverType> _ForwardSolverTypeOptionVM;
        //private OptionViewModel<ForwardAnalysisType> _ForwardAnalysisTypeOptionVM;

        private RangeViewModel _RhoRangeVM;
        private RangeViewModel _ZRangeVM;
        private double _SourceDetectorSeparation;

        private OpticalPropertyViewModel _OpticalPropertyVM;

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
                new[]
                {
                    ForwardSolverType.DistributedPointSourceSDA,
                    ForwardSolverType.PointSourceSDA,
                    ForwardSolverType.DistributedGaussianSourceSDA
                }); // explicitly enabling these for the workshop;

            FluenceSolutionDomainTypeOptionVM = new FluenceSolutionDomainOptionViewModel("Fluence Solution Domain", FluenceSolutionDomainType.FluenceofRho);
            AbsorbedEnergySolutionDomainTypeOptionVM = new FluenceSolutionDomainOptionViewModel("Absorbed Energy Solution Domain", FluenceSolutionDomainType.FluenceofRho);
            PhotonHittingDensitySolutionDomainTypeOptionVM = new FluenceSolutionDomainOptionViewModel("PHD Solution Domain", FluenceSolutionDomainType.FluenceofRho);

            MapTypeOptionVM = new OptionViewModel<MapType>(
                "Map Type", 
                new[]
                {
                    MapType.Fluence, 
                    MapType.AbsorbedEnergy, 
                    MapType.PhotonHittingDensity
                });

            MapTypeOptionVM.PropertyChanged += (sender, args) =>
            {
                this.OnPropertyChanged("IsFluence");
                this.OnPropertyChanged("IsAbsorbedEnergy");
                this.OnPropertyChanged("IsPhotonHittingDensity");
            };

            ForwardSolverTypeOptionVM.PropertyChanged +=
                (sender, args) =>
                    {
                        OnPropertyChanged("ForwardSolver");
                        OnPropertyChanged("IsGaussianForwardModel");
                    };

            Commands.FluenceSolver_ExecuteFluenceSolver.Executed += ExecuteFluenceSolver_Executed;
            Commands.FluenceSolver_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;
        }

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
        public FluenceSolutionDomainOptionViewModel FluenceSolutionDomainTypeOptionVM
        {
            get { return _FluenceSolutionDomainTypeOptionVM; }
            set
            {
                _FluenceSolutionDomainTypeOptionVM = value;
                OnPropertyChanged("FluenceSolutionDomainTypeOptionVM");
            }
        }
        public FluenceSolutionDomainOptionViewModel AbsorbedEnergySolutionDomainTypeOptionVM
        {
            get { return _AbsorbedEnergySolutionDomainTypeOptionVM; }
            set
            {
                _AbsorbedEnergySolutionDomainTypeOptionVM = value;
                OnPropertyChanged("AbsorbedEnergySolutionDomainTypeOptionVM");
            }
        }
        public FluenceSolutionDomainOptionViewModel PhotonHittingDensitySolutionDomainTypeOptionVM
        {
            get { return _PhotonHittingDensitySolutionDomainTypeOptionVM; }
            set
            {
                _PhotonHittingDensitySolutionDomainTypeOptionVM = value;
                OnPropertyChanged("PhotonHittingDensitySolutionDomainTypeOptionVM");
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
            var sd = GetSelectedSolutionDomain();
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
                ForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedPointSourceSDA ||
                ForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.PointSourceSDA  ||
                ForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedGaussianSourceSDA
                ? "Model - SDA\r" : "Model - MC scaled\r";
            string opString = "μa=" + OpticalPropertyVM.Mua + "\rμs'=" + OpticalPropertyVM.Musp;

            return modelString + opString;
        }

        public MapData ExecuteForwardSolver()
        {
            double[] rhos = RhoRangeVM.Values.Reverse().Select(rho => -rho).Concat(RhoRangeVM.Values).ToArray();
            double[] zs = ZRangeVM.Values.ToArray();

            IEnumerable<double>[] independentValues = new[] {rhos, zs };

            var sd = GetSelectedSolutionDomain();
            // todo: too much thinking at the VM layer?
            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(sd.SelectedValue)
                    ? new double[] { sd.ConstantAxisValue } : new double[0];

            IndependentVariableAxis[] independentAxes = 
                GetIndependentVariableAxesInOrder(
                    sd.IndependentVariableAxisOptionVM.SelectedValue,
                    IndependentVariableAxis.Z);

            double[] fluence =
                ComputationFactory.GetVectorizedMultidimensionalIndependentVariableQueryNew(
                    ForwardSolverTypeOptionVM.SelectedValue,
                    sd.SelectedValue,
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
                        ForwardSolverTypeOptionVM.SelectedValue,
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

        private FluenceSolutionDomainOptionViewModel GetSelectedSolutionDomain()
        {
            switch (MapTypeOptionVM.SelectedValue)
            {
                case MapType.Fluence:
                    return this.FluenceSolutionDomainTypeOptionVM;
                case MapType.AbsorbedEnergy:
                    return AbsorbedEnergySolutionDomainTypeOptionVM;
                case MapType.PhotonHittingDensity:
                    return PhotonHittingDensitySolutionDomainTypeOptionVM;
                default:
                    throw new InvalidEnumArgumentException("No solution domain of the specified type exists.");
            }
        }
    }
}
