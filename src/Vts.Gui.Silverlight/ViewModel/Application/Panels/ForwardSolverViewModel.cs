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
using Vts.Factories;
using Vts.Gui.Silverlight.Extensions;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;
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
        private OpticalPropertyViewModel _OpticalPropertyVM;

        private bool _showOpticalProperties;
        private bool _useSpectralPanelData;
        private bool _showIndependentVariable;
        private bool _showIndependentVariableTwo;
        private bool _showIndependentVariableThree;

        public ForwardSolverViewModel()
        {
            _showOpticalProperties = true;
            _useSpectralPanelData = false;
            _showIndependentVariable = true;
            _showIndependentVariableTwo = false;
            _showIndependentVariableThree = false;

            RangeVM = new RangeViewModel { Title = "Detection Parameters" };
            _allRangeVMs = new[] {RangeVM};
            OpticalPropertyVM = new OpticalPropertyViewModel { Title = "Optical Properties" };
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
            };

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
                    OpticalPropertyVM.SetOpticalProperties(SolverDemoViewModel.Current.SpectralMappingVM.OpticalProperties);
                }
            };
        }

        public RelayCommand ExecuteForwardSolverCommand { get; set; }
        
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

        public bool ShowOpticalProperties
        {
            get { return _showOpticalProperties; }
            set
            {
                _showOpticalProperties = value;
                OnPropertyChanged("ShowOpticalProperties");
            }
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

        void ExecuteForwardSolver_Executed(object sender, ExecutedEventArgs e)
        {
            Point[][] points = ExecuteForwardSolver();
            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);
            
            string[] plotLabels = GetLegendLabels();
            if (ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue))
            {
                var real = points[0];
                var imag = points[1];
                // convert Point to ComplexPoint
                var complexPoints = new List<ComplexPoint>();
                for (int i = 0; i < real.Length; i++)
                {
                    complexPoints.Add(new ComplexPoint(real[i].X, new Complex(real[i].Y, imag[i].Y)));
                }
                Commands.Plot_PlotValues.Execute(new PlotData(new [] { complexPoints.ToArray() }, plotLabels));
            }
            else
            {
                Commands.Plot_PlotValues.Execute(new PlotData(points, plotLabels));
            }

            Commands.TextOutput_PostMessage.Execute("Forward Solver: " + OpticalPropertyVM + "\r");
        }

        private PlotAxesLabels GetPlotLabels()
        {
            var sd = this.SolutionDomainTypeOptionVM;
            PlotAxesLabels axesLabels = null;
            if (sd.IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                axesLabels = new PlotAxesLabels(
                    sd.IndependentAxisLabel, sd.IndependentAxisUnits, sd.IndependentAxisType,
                    sd.SelectedDisplayName, sd.SelectedValue.GetUnits(),
                    sd.ConstantAxisLabel, sd.ConstantAxisUnits, sd.ConstantAxisValue,
                    sd.ConstantAxisTwoLabel, sd.ConstantAxisTwoUnits, sd.ConstantAxisTwoValue);
            }
            else
            {
                axesLabels = new PlotAxesLabels(sd.IndependentAxisLabel, sd.IndependentAxisUnits, 
                    sd.IndependentAxisType, sd.SelectedDisplayName, sd.SelectedValue.GetUnits());
            }
            return axesLabels;
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
                    modelString = "Model - SDA \r";
                    break;
                case ForwardSolverType.MonteCarlo:
                    modelString = "Model - scaled MC \r";
                    break;
                case ForwardSolverType.Nurbs:
                    modelString = "Model - nurbs \r";
                    break;
            }

            if (_allRangeVMs.Length > 1)
            {
                var isWavelengthPlot = _allRangeVMs.Any(vm => vm.AxisType == IndependentVariableAxis.Wavelength);
                var secondaryRangeVM = isWavelengthPlot
                    ? _allRangeVMs.Where(vm => vm.AxisType != IndependentVariableAxis.Wavelength).First()
                    : _allRangeVMs.Where(vm => vm.AxisType != IndependentVariableAxis.Time && vm.AxisType != IndependentVariableAxis.Ft).First();

                string[] secondaryAxesStrings = secondaryRangeVM.Values.Select(value => secondaryRangeVM.AxisType.GetInternationalizedString() + " = " + value.ToString() + secondaryRangeVM.AxisType.GetUnits()).ToArray();
                string opString = "μa=" + OpticalPropertyVM.Mua + " \rμs'=" + OpticalPropertyVM.Musp;
                return secondaryAxesStrings.Select(sas => modelString + sas + (isWavelengthPlot ? "(spectral μa,μs')" : "\r" + opString) + "\r").ToArray();
            }
            else
            {
                string opString = "μa=" + OpticalPropertyVM.Mua + " \rμs'=" + OpticalPropertyVM.Musp + "\r";
                return new []{ modelString + opString };
            }
        }

        public Point[][] ExecuteForwardSolver()
        {
            var parameters = GetParametersInOrder();

            double[] reflectance = ComputationFactory.ComputeReflectance(
                    ForwardSolverTypeOptionVM.SelectedValue,
                    SolutionDomainTypeOptionVM.SelectedValue,
                    ForwardAnalysisTypeOptionVM.SelectedValue,
                    parameters.Values.ToArray());

            var plotIsVsWavelength = _allRangeVMs.Any(vm => vm.AxisType == IndependentVariableAxis.Wavelength);

            //var plotVsTime = SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Any( value => value == IndependentVariableAxis.Time) &&
            //                       ((OpticalProperties[]) parameters[IndependentVariableAxis.Time]).Length > 1 ||
            //                 SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Any( value => value == IndependentVariableAxis.Ft) &&
            //                       ((OpticalProperties[]) parameters[IndependentVariableAxis.Ft]).Length > 1;
            //GetParameterValues( plotVsWavelength ? RangeVM

            var primaryIdependentValues = RangeVM.Values.ToArray();
            var numPointsPerCurve = primaryIdependentValues.Length;
            var numCurves =
                (ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue)
                    ? reflectance.Length/2 : reflectance.Length)
                / numPointsPerCurve;

            var points = new Point[numCurves][];
            for (int j = 0; j < numCurves; j++)
            {
                points[j] = new Point[numPointsPerCurve];
                if (plotIsVsWavelength) // man, this is getting hacky...
                {
                    for (int i = 0; i < numPointsPerCurve; i++)
                    {
                        points[j][i] = new Point(primaryIdependentValues[i], reflectance[i*numCurves + j]);
                    }
                }
                else
                {
                    for (int i = 0; i < numPointsPerCurve; i++)
                    {
                        points[j][i] = new Point(primaryIdependentValues[i], reflectance[j * numPointsPerCurve + i]);
                    }
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

        private IDictionary<IndependentVariableAxis, object> GetParametersInOrder()
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
            return (new KeyValuePair<IndependentVariableAxis, object>(IndependentVariableAxis.Wavelength, GetOpticalProperties())).AsEnumerable()
                .Concat(allParameters).ToDictionary();
        }

        private OpticalProperties[] GetOpticalProperties()
        {
            if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Contains(IndependentVariableAxis.Wavelength) &&
                UseSpectralPanelData && 
                SolverDemoViewModel.Current != null &&
                SolverDemoViewModel.Current.SpectralMappingVM != null)
            {
                var tissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue;
                var wavelengths = GetParameterValues(IndependentVariableAxis.Wavelength);
                return tissue.GetOpticalProperties(wavelengths);
            }

            return new[] { OpticalPropertyVM.GetOpticalProperties() };
        }
    }
}
