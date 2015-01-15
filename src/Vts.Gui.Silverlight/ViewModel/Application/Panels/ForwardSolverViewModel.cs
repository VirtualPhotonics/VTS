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
        private RangeViewModel _RangeVM;
        private RangeViewModel _RangeTwoVM;
        private RangeViewModel _RangeThreeVM;
        private RangeViewModel[] _allRangeVMs;

        private bool _showOpticalProperties;
        private bool _useSpectralPanelData;
        private bool _showIndependentVariable;
        private bool _showIndependentVariableTwo;
        private bool _showIndependentVariableThree;
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
            _showIndependentVariable = true;
            _showIndependentVariableTwo = false;
            _showIndependentVariableThree = false;
            
            RangeVM = new RangeViewModel { Title = "Detection Parameters" };
            _allRangeVMs = new[] {RangeVM};
            //OpticalPropertyVM = new OpticalPropertyViewModel { Title = "Optical Properties" };
            // right now, we're doing manual databinding to the selected item. need to enable databinding 
            // confused, though - do we need to use strings? or, how to make generics work with dependency properties?
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
                if (SolutionDomainTypeOptionVM.ConstantAxisType == IndependentVariableAxis.Wavelength)
                {
                    SolutionDomainTypeOptionVM.ConstantAxisValue = wv;
                }
                else if (SolutionDomainTypeOptionVM.ConstantAxisTwoType == IndependentVariableAxis.Wavelength)
                {
                    SolutionDomainTypeOptionVM.ConstantAxisTwoValue = wv;
                }
            };
            SolutionDomainTypeOptionVM.PropertyChanged += (sender, args) => 
            {
                if (args.PropertyName == "UseSpectralInputs")
                {
                    UseSpectralPanelData = SolutionDomainTypeOptionVM.UseSpectralInputs;
                }
                if (args.PropertyName == "IndependentAxisType" || args.PropertyName == "AllowMultiAxis" || args.PropertyName == "IndependentAxisTwoVisible" || args.PropertyName == "IndependentAxisThreeVisible")
                {
                    var useSpectralPanelDataAndNotNull = UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null;
                    if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Length == 1)
                    {
                        RangeVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[0] == IndependentVariableAxis.Wavelength 
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisType.GetDefaultIndependentAxisRange();
                        
                        ShowIndependentVariable = true;
                        ShowIndependentVariableTwo = false;
                        ShowIndependentVariableThree = false;

                        _allRangeVMs = new[] {RangeVM};
                    }

                    if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Length == 2)
                    {
                        RangeVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[1] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisTwoType.GetDefaultIndependentAxisRange();

                        RangeTwoVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[0] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisType.GetDefaultIndependentAxisRange();

                        ShowIndependentVariable = true;
                        ShowIndependentVariableTwo = true;
                        ShowIndependentVariableThree = false;

                        _allRangeVMs = new[] { RangeVM, RangeTwoVM };
                    }

                    if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Length == 3)
                    {
                        RangeVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[2] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisThreeType.GetDefaultIndependentAxisRange();

                        RangeTwoVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[1] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisTwoType.GetDefaultIndependentAxisRange();

                        RangeThreeVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[0] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisType.GetDefaultIndependentAxisRange();

                        ShowIndependentVariable = true;
                        ShowIndependentVariableTwo = true;
                        ShowIndependentVariableThree = true;

                        _allRangeVMs = new[] { RangeVM, RangeTwoVM, RangeThreeVM };
                    }

                    // if the independent axis is wavelength, then hide optical properties (because they come from spectral panel)
                    ShowOpticalProperties = !SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Any(value => value == IndependentVariableAxis.Wavelength);

                    // update solution domain wavelength constant if applicable
                    if (useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues.Contains(IndependentVariableAxis.Wavelength))
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

        public bool ShowIndependentVariable
        {
            get { return _showIndependentVariable; }
            set
            {
                _showIndependentVariable = value;
                OnPropertyChanged("ShowIndependentVariable");
            }
        }

        public bool ShowIndependentVariableTwo
        {
            get { return _showIndependentVariableTwo; }
            set
            {
                _showIndependentVariableTwo = value;
                OnPropertyChanged("ShowIndependentVariableTwo");
            }
        }

        public bool ShowIndependentVariableThree
        {
            get { return _showIndependentVariableThree; }
            set
            {
                _showIndependentVariableThree = value;
                OnPropertyChanged("ShowIndependentVariableThree");
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

        public RangeViewModel RangeVM
        {
            get { return _RangeVM; }
            set
            {
                _RangeVM = value;
                OnPropertyChanged("RangeVM");
            }
        }

        public RangeViewModel RangeTwoVM
        {
            get { return _RangeTwoVM; }
            set
            {
                _RangeTwoVM = value;
                OnPropertyChanged("RangeTwoVM");
            }
        }

        public RangeViewModel RangeThreeVM
        {
            get { return _RangeThreeVM; }
            set
            {
                _RangeThreeVM = value;
                OnPropertyChanged("RangeThreeVM");
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
            //if (ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue))
            //{
            //    var real = points[0];
            //    var imag = points[1];
            //    // convert Point to ComplexPoint
            //    var complexPoints = new List<ComplexPoint>();
            //    for (int i = 0; i < real.Length; i++)
            //    {
            //        complexPoints.Add(new ComplexPoint(real[i].X, new Complex(real[i].Y, imag[i].Y)));
            //    }
            //    Commands.Plot_PlotValues.Execute(new PlotData(new [] { complexPoints.ToArray() }, plotLabels));
            //}
            //else
            //{
                Commands.Plot_PlotValues.Execute(new PlotData(points, plotLabels));
            //}

            Commands.TextOutput_PostMessage.Execute("Forward Solver: " + TissueInputVM + "\r"); // todo: override ToString() for MultiRegionTissueViewModel
        }

        private PlotAxesLabels GetPlotLabels()
        {
            var sd = this.SolutionDomainTypeOptionVM;
            PlotAxesLabels axesLabels = null;
            if (sd.IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                var axisType = RangeVM.AxisType;

                if (sd.IndependentVariableAxisOptionVM.UnSelectedValues.Length > 1)
                {
                    axesLabels = new PlotAxesLabels(
                        axisType.GetInternationalizedString(), axisType.GetUnits(), axisType,
                        sd.SelectedDisplayName, sd.SelectedValue.GetUnits(),
                        sd.ConstantAxisLabel, sd.ConstantAxisUnits, sd.ConstantAxisValue,
                        sd.ConstantAxisTwoLabel, sd.ConstantAxisTwoUnits, sd.ConstantAxisTwoValue);
                }
                else
                {
                    axesLabels = new PlotAxesLabels(
                        axisType.GetInternationalizedString(), axisType.GetUnits(), axisType,
                        sd.SelectedDisplayName, sd.SelectedValue.GetUnits(),
                        sd.ConstantAxisLabel, sd.ConstantAxisUnits, sd.ConstantAxisValue);
                }
                //axesLabels = new PlotAxesLabels(
                //    sd.IndependentAxisLabel, sd.IndependentAxisUnits, axisType,
                //    axisType.GetInternationalizedString(), axisType.GetUnits(),
                //    sd.ConstantAxisLabel, sd.ConstantAxisUnits, sd.ConstantAxisValue,
                //    sd.ConstantAxisTwoLabel, sd.ConstantAxisTwoUnits, sd.ConstantAxisTwoValue);
            }
            else
            {
                axesLabels = new PlotAxesLabels(sd.IndependentAxisLabel, sd.IndependentAxisUnits,
                    sd.IndependentAxisType, sd.SelectedDisplayName, sd.SelectedValue.GetUnits());
            }
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

            var plotIsVsWavelength = _allRangeVMs.Any(vm => vm.AxisType == IndependentVariableAxis.Wavelength);
            var isComplexPlot = ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue);
            var primaryIdependentValues = RangeVM.Values.ToArray();
            var numPointsPerCurve = primaryIdependentValues.Length;
            var numForwardValues =  isComplexPlot ? reflectance.Length/2 : reflectance.Length; // complex reported as all reals, then all imaginaries
            var numCurves = numForwardValues / numPointsPerCurve;

            var points = new IDataPoint[numCurves][];
            Func<int, int, IDataPoint> getReflectanceAtIndex = (i, j) =>
            {
                // man, this is getting hacky...
                var index = plotIsVsWavelength
                    ? i*numCurves + j
                    : j*numPointsPerCurve + i;
                return isComplexPlot
                    ? (IDataPoint)new ComplexDataPoint(primaryIdependentValues[i], new Complex(reflectance[index], reflectance[index + numForwardValues]))
                    : (IDataPoint)new DoubleDataPoint(primaryIdependentValues[i], reflectance[index]);
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
                var positionIndex =
                    SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues.IndexOf(axis);
                switch (positionIndex)
                {
                    case 0:
                    default:
                        return new[] {SolutionDomainTypeOptionVM.ConstantAxisValue};
                    case 1:
                        return new[] {SolutionDomainTypeOptionVM.ConstantAxisTwoValue};
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
                        return RangeVM.Values.ToArray();
                    case 2:
                        switch (positionIndex)
                        {
                            case 0:
                            default:
                                return RangeTwoVM.Values.ToArray();
                            case 1:
                                return RangeVM.Values.ToArray();
                        }
                    case 3:
                        switch (positionIndex)
                        {
                            case 0:
                            default:
                                return RangeThreeVM.Values.ToArray();
                            case 1:
                                return RangeTwoVM.Values.ToArray();
                            case 2:
                                return RangeVM.Values.ToArray();
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
