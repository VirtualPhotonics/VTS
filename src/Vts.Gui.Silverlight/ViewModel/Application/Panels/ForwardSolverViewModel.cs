using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using SLExtensions;
using SLExtensions.Input;
using Vts.Extensions;
using Vts.Common;
using Vts.Factories;
using Vts.Gui.Silverlight.Extensions;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;
using Vts.IO;
using Vts.Modeling.ForwardSolvers;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Tissues;
#if WHITELIST
using Vts.Gui.Silverlight.ViewModel.Application;
#endif

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing Forward Solver panel functionality
    /// </summary>
    public class ForwardSolverViewModel : BindableObject
    {
        private SolutionDomainOptionViewModel _SolutionDomainTypeOptionVM;
        private OptionViewModel<ForwardSolverType> _ForwardSolverTypeOptionVM;
        private OptionViewModel<ForwardAnalysisType> _ForwardAnalysisTypeOptionVM;
        private RangeViewModel[] _allRangeVMs;

        private bool _showOpticalProperties;
        private bool _useSpectralPanelData;
        private object _tissueInputVM; // either an OpticalPropertyViewModel or a MultiRegionTissueViewModel is stored here, and dynamically displayed
        
        // private fields to cache created instances of tissue inputs, created on-demand in GetTissueInputVM (vs up-front in constructor)
        private OpticalProperties _currentHomogeneousOpticalProperties;
        private SemiInfiniteTissueInput _currentSemiInfiniteTissueInput;
        private MultiLayerTissueInput _currentMultiLayerTissueInput;
        private SingleEllipsoidTissueInput _currentSingleEllipsoidTissueInput;
        
        public ForwardSolverViewModel()
        {
            _showOpticalProperties = true;
            _useSpectralPanelData = false;
            
            _allRangeVMs = new[] { new RangeViewModel { Title = "Detection Parameters" } };

#if WHITELIST 
            ForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Forward Model",false, WhiteList.ForwardSolverTypes);
#else
            ForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Forward Model",false);
#endif
            ForwardSolverTypeOptionVM.PropertyChanged += (sender, args) =>
            {
                OnPropertyChanged("IsGaussianForwardModel");
                OnPropertyChanged("ForwardSolver");
                OnPropertyChanged("IsMultiRegion");
                OnPropertyChanged("IsSemiInfinite");
                TissueInputVM = GetTissueInputVM(IsMultiRegion ? "MultiLayer" : "SemiInfinite");
            };
            ForwardSolverTypeOptionVM.SelectedValue = ForwardSolverType.PointSourceSDA; // force the model choice here?

            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain", SolutionDomainType.ROfRho);
            
            Action<double> updateSolutionDomainWithWavelength = wv =>
            {
                var wvAxis = SolutionDomainTypeOptionVM.ConstantAxesVMs.FirstOrDefault(axis => axis.AxisType == IndependentVariableAxis.Wavelength);
                if (wvAxis != null)
                {
                    wvAxis.AxisValue = wv;
                }
            };

            SolutionDomainTypeOptionVM.PropertyChanged += (sender, args) => 
            {
                if (args.PropertyName == "UseSpectralInputs")
                {
                    UseSpectralPanelData = SolutionDomainTypeOptionVM.UseSpectralInputs;
                }
                if (args.PropertyName == "IndependentAxesVMs")
                {
                    var useSpectralPanelDataAndNotNull = UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null;

                    AllRangeVMs = (from i in Enumerable.Range(0, SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Length)
                                        orderby i descending // descending so that wavelength takes highest priority, then time/time frequency, then space/spatial frequency
                                        select useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[i] == IndependentVariableAxis.Wavelength
                                             ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                                             : SolutionDomainTypeOptionVM.IndependentAxesVMs[i].AxisRangeVM).ToArray();

                    // if the independent axis is wavelength, then hide optical properties (because they come from spectral panel)
                    ShowOpticalProperties = !_allRangeVMs.Any(value => value.AxisType == IndependentVariableAxis.Wavelength);

                    // update solution domain wavelength constant if applicable
                    if (useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.ConstantAxesVMs.Any(axis => axis.AxisType == IndependentVariableAxis.Wavelength))
                    {
                        updateSolutionDomainWithWavelength(SolverDemoViewModel.Current.SpectralMappingVM.Wavelength);
                    }
                }
            };

            ForwardAnalysisTypeOptionVM = new OptionViewModel<ForwardAnalysisType>("Model/Analysis Output", true);
            
            ExecuteForwardSolverCommand = new RelayCommand(() => ExecuteForwardSolver_Executed(null, null));

            Commands.Spec_UpdateWavelength.Executed += (sender, args) =>
            {
                if (UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null)
                {
                    updateSolutionDomainWithWavelength(SolverDemoViewModel.Current.SpectralMappingVM.Wavelength);
                }
            };
            Commands.Spec_UpdateOpticalProperties.Executed += (sender, args) =>
            {
                if (UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null)
                {
                    if (IsMultiRegion && MultiRegionTissueVM != null)
                    {
                        MultiRegionTissueVM.RegionsVM.ForEach(region =>
                            ((dynamic)region).OpticalPropertyVM.SetOpticalProperties(
                                SolverDemoViewModel.Current.SpectralMappingVM.OpticalProperties));
                    }
                    else if ( OpticalPropertyVM != null)
                    {
                        OpticalPropertyVM.SetOpticalProperties(SolverDemoViewModel.Current.SpectralMappingVM.OpticalProperties);
                    }
                }
            };
        }

        public RelayCommand ExecuteForwardSolverCommand { get; set; }

        public IForwardSolver ForwardSolver
        {
            get { return SolverFactory.GetForwardSolver(ForwardSolverTypeOptionVM.SelectedValue); }
        }

        public bool IsGaussianForwardModel
        {
            get { return ForwardSolverTypeOptionVM.SelectedValue.IsGaussianForwardModel(); }
        }

        public bool ShowOpticalProperties
        {
            get { return _showOpticalProperties; }
            set
            {
                _showOpticalProperties = value;
                OnPropertyChanged("ShowOpticalProperties");
            }
        }

        public bool IsMultiRegion
        {
            get { return ForwardSolverTypeOptionVM.SelectedValue.IsMultiRegionForwardModel(); }
        }
                
        public bool IsSemiInfinite
        {
            get { return !ForwardSolverTypeOptionVM.SelectedValue.IsMultiRegionForwardModel(); }
        }

        public bool UseSpectralPanelData
        {
            get { return _useSpectralPanelData; }
            set
            {
                _useSpectralPanelData = value;
                OnPropertyChanged("UseSpectralPanelData");
            }
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

        public RangeViewModel[] AllRangeVMs
        {
            get { return _allRangeVMs; }
            set
            {
                _allRangeVMs = value;
                OnPropertyChanged("AllRangeVMs");
            }
        }

        public object TissueInputVM
        {
            get { return _tissueInputVM; }
            private set
            {
                _tissueInputVM = value;
                OnPropertyChanged("TissueInputVM");
            }
        }

        private OpticalPropertyViewModel OpticalPropertyVM
        {
            get { return _tissueInputVM as OpticalPropertyViewModel; }
        }

        private MultiRegionTissueViewModel MultiRegionTissueVM
        {
            get { return _tissueInputVM as MultiRegionTissueViewModel; }
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

        void ExecuteForwardSolver_Executed(object sender, ExecutedEventArgs e)
        {
            IDataPoint[][] points = ExecuteForwardSolver();
            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            string[] plotLabels = GetLegendLabels();

            var plotData = Enumerable.Zip(points, plotLabels, (p, el) => new PlotData(p, el)).ToArray();
            Commands.Plot_PlotValues.Execute(plotData);

            Commands.TextOutput_PostMessage.Execute("Forward Solver: " + TissueInputVM + "\r"); // todo: override ToString() for MultiRegionTissueViewModel
        }

        private PlotAxesLabels GetPlotLabels()
        {
            var sd = this.SolutionDomainTypeOptionVM;
            PlotAxesLabels axesLabels = new PlotAxesLabels(
                sd.SelectedDisplayName, sd.SelectedValue.GetUnits(),
                sd.IndependentAxesVMs.Where(vm => vm.AxisType == AllRangeVMs.First().AxisType).First(),
                sd.ConstantAxesVMs); 
            return axesLabels;
        }
        
        private object GetTissueInputVM(string tissueType)
        {
            // ops to use as the basis for instantiating multi-region tissues based on homogeneous values (for differential comparison)
            if (_currentHomogeneousOpticalProperties == null)
            {
                _currentHomogeneousOpticalProperties = new OpticalProperties(0.01, 1, 0.8, 1.4);
            }

            switch (tissueType)
            {
                case "SemiInfinite":
                    if (_currentSemiInfiniteTissueInput == null)
                    {
                        _currentSemiInfiniteTissueInput = new SemiInfiniteTissueInput(new SemiInfiniteTissueRegion(_currentHomogeneousOpticalProperties));
                    }
                    return new OpticalPropertyViewModel(
                        ((SemiInfiniteTissueInput)_currentSemiInfiniteTissueInput).Regions.First().RegionOP,
                         IndependentVariableAxisUnits.InverseMM.GetInternationalizedString(),
                        "Optical Properties:");
                    break;
                case "MultiLayer":
                    if (_currentMultiLayerTissueInput == null)
                    {
                        _currentMultiLayerTissueInput = new MultiLayerTissueInput(new ITissueRegion[]
                            { 
                                new LayerTissueRegion(new DoubleRange(0, 2), _currentHomogeneousOpticalProperties.Clone() ), 
                                new LayerTissueRegion(new DoubleRange(2, double.PositiveInfinity), _currentHomogeneousOpticalProperties.Clone() ), 
                            });
                    }
                    return new MultiRegionTissueViewModel(_currentMultiLayerTissueInput);
                case "SingleEllipsoid":
                    if (_currentSingleEllipsoidTissueInput == null)
                    {
                        _currentSingleEllipsoidTissueInput = new SingleEllipsoidTissueInput(
                            new EllipsoidTissueRegion(new Position(0, 0, 10), 5, 5, 5, new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                            new ITissueRegion[]
                            { 
                                new LayerTissueRegion(new DoubleRange(0, double.PositiveInfinity), _currentHomogeneousOpticalProperties.Clone()), 
                            });
                    }
                    return new MultiRegionTissueViewModel(_currentSingleEllipsoidTissueInput);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // todo: rename? this was to get a concise name for the legend
        private string[] GetLegendLabels()
        {
            string modelString = null;
            switch (ForwardSolverTypeOptionVM.SelectedValue)
            {
                case ForwardSolverType.DistributedGaussianSourceSDA:
                case ForwardSolverType.DistributedPointSourceSDA:
                case ForwardSolverType.PointSourceSDA:
                    modelString = "\rModel - SDA";
                    break;
                case ForwardSolverType.MonteCarlo:
                    modelString = "\rModel - scaled MC";
                    break;
                case ForwardSolverType.Nurbs:
                    modelString = "\rModel - nurbs";
                    break;
                case ForwardSolverType.TwoLayerSDA:
                    modelString = "\rModel - 2 layer SDA";
                    break;
            }

            string opString = null;
            if (IsMultiRegion && MultiRegionTissueVM != null)
            {
                var regions = MultiRegionTissueVM.GetTissueInput().Regions;
                opString = "\rμa1=" + regions[0].RegionOP.Mua.ToString("F4") + "\rμs'1=" + regions[0].RegionOP.Musp.ToString("F4") +
                           "\rμa2=" + regions[1].RegionOP.Mua.ToString("F4") + "\rμs'2=" + regions[1].RegionOP.Musp.ToString("F4"); 
            }
            else
            {
                var opticalProperties = OpticalPropertyVM.GetOpticalProperties();
                opString = "\rμa=" + opticalProperties.Mua.ToString("F4") + " \rμs'=" + opticalProperties.Musp.ToString("F4");
            }

            if (_allRangeVMs.Length > 1)
            {
                var isWavelengthPlot = _allRangeVMs.Any(vm => vm.AxisType == IndependentVariableAxis.Wavelength);
                var secondaryRangeVM = isWavelengthPlot
                    ? _allRangeVMs.Where(vm => vm.AxisType != IndependentVariableAxis.Wavelength).First()
                    : _allRangeVMs.Where(vm => vm.AxisType != IndependentVariableAxis.Time && vm.AxisType != IndependentVariableAxis.Ft).First();

                string[] secondaryAxesStrings = secondaryRangeVM.Values.Select(value => "\r" + secondaryRangeVM.AxisType.GetInternationalizedString() + " = " + value.ToString()).ToArray();
                return secondaryAxesStrings.Select(sas => modelString + sas + (isWavelengthPlot ? "\r(spectral μa,μs')" : opString)).ToArray();
            }

            return new []{ modelString + opString };
        }

        public IDataPoint[][] ExecuteForwardSolver()
        {
            var opticalProperties = GetOpticalProperties();

            var parameters = GetParametersInOrder(opticalProperties);

            double[] reflectance = ComputationFactory.ComputeReflectance(                    
                    ForwardSolverTypeOptionVM.SelectedValue,
                    SolutionDomainTypeOptionVM.SelectedValue,
                    ForwardAnalysisTypeOptionVM.SelectedValue,
                    parameters.Values.ToArray());

            return GetDataPoints(reflectance);
        }

        private IDataPoint[][] GetDataPoints(double[] reflectance)
        {
            var plotIsVsWavelength = _allRangeVMs.Any(vm => vm.AxisType == IndependentVariableAxis.Wavelength);
            var isComplexPlot = ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue);
            var primaryIdependentValues = _allRangeVMs.First().Values.ToArray();
            var numPointsPerCurve = primaryIdependentValues.Length;
            var numForwardValues = isComplexPlot ? reflectance.Length/2 : reflectance.Length;
                // complex reported as all reals, then all imaginaries
            var numCurves = numForwardValues/numPointsPerCurve;

            var points = new IDataPoint[numCurves][];
            Func<int, int, IDataPoint> getReflectanceAtIndex = (i, j) =>
            {
                // man, this is getting hacky...
                var index = plotIsVsWavelength
                    ? i*numCurves + j
                    : j*numPointsPerCurve + i;
                return isComplexPlot
                    ? (IDataPoint)
                        new ComplexDataPoint(primaryIdependentValues[i],
                            new Complex(reflectance[index], reflectance[index + numForwardValues]))
                    : (IDataPoint) new DoubleDataPoint(primaryIdependentValues[i], reflectance[index]);
            };
            for (int j = 0; j < numCurves; j++)
            {
                points[j] = new IDataPoint[numPointsPerCurve];
                for (int i = 0; i < numPointsPerCurve; i++)
                {
                    points[j][i] = getReflectanceAtIndex(i, j);
                }
            }
            return points;
        }

        /// <summary>
        /// Function to provide ordering information for assembling forward calls
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private int GetParameterOrder(IndependentVariableAxis axis)
        {
            switch (axis)
            {
                case IndependentVariableAxis.Wavelength:
                    return 0;
                case IndependentVariableAxis.Rho:
                    return 1;
                case IndependentVariableAxis.Fx:
                    return 1;
                case IndependentVariableAxis.Time:
                    return 2;
                case IndependentVariableAxis.Ft:
                    return 2;
                case IndependentVariableAxis.Z:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException("axis");
            }
        }

        private double[] GetParameterValues(IndependentVariableAxis axis)
        {
            var isConstant = SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues.Contains(axis);
            if (isConstant)
            {
                var positionIndex = SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues.IndexOf(axis);
                switch (positionIndex)
                {
                    case 0:
                    default:
                        return new[] {SolutionDomainTypeOptionVM.ConstantAxesVMs[0].AxisValue};
                    case 1:
                        return new[] { SolutionDomainTypeOptionVM.ConstantAxesVMs[1].AxisValue };
                        //case 2:
                        //    return new[] { SolutionDomainTypeOptionVM.ConstantAxisThreeValue };
                }
            }
            else
            {
                var numAxes = SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Length;
                var positionIndex = SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.IndexOf(axis);
                switch (numAxes)
                {
                    case 1:
                    default:
                        return AllRangeVMs[0].Values.ToArray();
                    case 2:
                        switch (positionIndex)
                        {
                            case 0:
                            default:
                                return AllRangeVMs[1].Values.ToArray();
                            case 1:
                                return AllRangeVMs[0].Values.ToArray();
                        }
                    case 3:
                        switch (positionIndex)
                        {
                            case 0:
                            default:
                                return AllRangeVMs[2].Values.ToArray();
                            case 1:
                                return AllRangeVMs[1].Values.ToArray();
                            case 2:
                                return AllRangeVMs[0].Values.ToArray();
                        }
                }
            }
        }

        private IDictionary<IndependentVariableAxis, object> GetParametersInOrder(object opticalProperties)
        {
            // get all parameters to get arrays of
            // then, for each one, decide if it's an IV or a constant
            // then, call the appropriate parameter generator, defined above
            var allParameters = from iv in Enumerable.Concat(
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues)
                where iv != IndependentVariableAxis.Wavelength
                orderby GetParameterOrder(iv) 
                select new KeyValuePair<IndependentVariableAxis, object> (iv, GetParameterValues(iv));

            // OPs are always first in the list
            return (new KeyValuePair<IndependentVariableAxis, object>(IndependentVariableAxis.Wavelength, opticalProperties)).AsEnumerable()
                .Concat(allParameters).ToDictionary();
        }

        private object GetOpticalProperties()
        {     
            if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Contains(IndependentVariableAxis.Wavelength) &&
                UseSpectralPanelData && 
                SolverDemoViewModel.Current != null &&
                SolverDemoViewModel.Current.SpectralMappingVM != null)
            {
                var tissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue;
                var wavelengths = GetParameterValues(IndependentVariableAxis.Wavelength);
                var ops = tissue.GetOpticalProperties(wavelengths);

                if(IsMultiRegion && MultiRegionTissueVM != null)
                {
                    return ops.Select(op =>
                        {
                            var regions = MultiRegionTissueVM.GetTissueInput().Regions.Select(region => (IOpticalPropertyRegion)region).ToArray();
                            regions.ForEach(region =>
                                {
                                    region.RegionOP.Mua = op.Mua;
                                    region.RegionOP.Musp = op.Musp;
                                    region.RegionOP.G = op.G;
                                    region.RegionOP.N = op.N;
                                });
                            return regions.ToArray();
                        });
                }

                return ops;
            }

            if (IsMultiRegion && MultiRegionTissueVM != null)
            {
                return new[] { MultiRegionTissueVM.GetTissueInput().Regions.Select(region => (IOpticalPropertyRegion)region).ToArray() };
            }

            return new[] { OpticalPropertyVM.GetOpticalProperties() };
        }
    }
}
