using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
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
using Vts.IO;

#if WHITELIST
using Vts.Gui.Silverlight.ViewModel.Application;
#endif

namespace Vts.Gui.Silverlight.ViewModel
{
    /// <summary>
    /// View model implementing Inverse Solver panel functionality
    /// </summary>
    public class InverseSolverViewModel : BindableObject
    {
        private SolutionDomainOptionViewModel _SolutionDomainTypeOptionVM;
        private OptionViewModel<ForwardSolverType> _MeasuredForwardSolverTypeVM;
        private OptionViewModel<ForwardSolverType> _InverseForwardSolverTypeVM;
        private OptionViewModel<OptimizerType> _OptimizerTypeVM;
        private OptionViewModel<InverseFitType> _InverseFitTypeVM;
        private RangeViewModel[] _allRangeVMs;

        private OpticalPropertyViewModel _MeasuredOpticalPropertyVM;
        private OpticalPropertyViewModel _InitialGuessOpticalPropertyVM;
        private OpticalPropertyViewModel _ResultOpticalPropertyVM;

        private double _PercentNoise;
        private double[] _MeasuredDataValues;
        private double[] _InitialGuessDataValues;
        private double[] _ResultDataValues;

        private bool _showOpticalProperties;
        private bool _useSpectralPanelData;

        public InverseSolverViewModel()
        {
            _showOpticalProperties = true;
            _useSpectralPanelData = false;

            _allRangeVMs = new[] { new RangeViewModel { Title = Resources.Strings.IndependentVariableAxis_Rho } };

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

#if WHITELIST
            MeasuredForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>(
                "Forward Model Engine",false, WhiteList.InverseForwardSolverTypes);
#else
             MeasuredForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>(
                "Forward Model Engine",false);
#endif
            
#if WHITELIST 
            InverseForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Inverse Model Engine",false, WhiteList.InverseForwardSolverTypes);
#else
            InverseForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Inverse Model Engine", false);
#endif
            InverseForwardSolverTypeOptionVM.PropertyChanged += (sender, args) =>
                OnPropertyChanged("InverseForwardSolver");

            OptimizerTypeOptionVM = new OptionViewModel<OptimizerType>("Optimizer Type", true);
            OptimizerTypeOptionVM.PropertyChanged += (sender, args) =>
                OnPropertyChanged("Optimizer");

            InverseFitTypeOptionVM = new OptionViewModel<InverseFitType>("Optimization Parameters", true);

            MeasuredOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "" };
            InitialGuessOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "" };
            ResultOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "" };

            SimulateMeasuredDataCommand = new RelayCommand(() => SimulateMeasuredDataCommand_Executed(null, null));
            CalculateInitialGuessCommand = new RelayCommand(() => CalculateInitialGuessCommand_Executed(null, null));
            SolveInverseCommand = new RelayCommand(() => SolveInverseCommand_Executed(null, null));
            
            Commands.Spec_UpdateWavelength.Executed += (sender, args) =>
            {
                //need to get the value from the checkbox in case UseSpectralPanelData has not yet been updated
                if (SolutionDomainTypeOptionVM != null)
                {
                    UseSpectralPanelData = SolutionDomainTypeOptionVM.UseSpectralInputs;
                }
                if (UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null)
                {
                    updateSolutionDomainWithWavelength(SolverDemoViewModel.Current.SpectralMappingVM.Wavelength);
                }
            };
            Commands.Spec_UpdateOpticalProperties.Executed += (sender, args) =>
            {
                //need to get the value from the checkbox in case UseSpectralPanelData has not yet been updated
                if (SolutionDomainTypeOptionVM != null)
                {
                    UseSpectralPanelData = SolutionDomainTypeOptionVM.UseSpectralInputs;
                }
                if (UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null)
                {
                    //if (IsMultiRegion && MultiRegionTissueVM != null)
                    //{
                    //    MultiRegionTissueVM.RegionsVM.ForEach(region =>
                    //        ((dynamic)region).OpticalPropertyVM.SetOpticalProperties(
                    //            SolverDemoViewModel.Current.SpectralMappingVM.OpticalProperties));
                    //}
                    //else
                    if (MeasuredOpticalPropertyVM != null)
                    {
                        MeasuredOpticalPropertyVM.SetOpticalProperties(SolverDemoViewModel.Current.SpectralMappingVM.OpticalProperties);
                    }
                }
            };
        }

        public RelayCommand SimulateMeasuredDataCommand { get; set; }
        public RelayCommand CalculateInitialGuessCommand { get; set; }
        public RelayCommand SolveInverseCommand { get; set; }
        
        public SolutionDomainOptionViewModel SolutionDomainTypeOptionVM
        {
            get { return _SolutionDomainTypeOptionVM; }
            set
            {
                _SolutionDomainTypeOptionVM = value;
                OnPropertyChanged("SolutionDomainTypeOptionVM");
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

        public OptionViewModel<ForwardSolverType> MeasuredForwardSolverTypeOptionVM
        {
            get { return _MeasuredForwardSolverTypeVM; }
            set
            {
                _MeasuredForwardSolverTypeVM = value;
                OnPropertyChanged("MeasuredForwardSolverTypeOptionVM");
            }
        }

        public OptionViewModel<ForwardSolverType> InverseForwardSolverTypeOptionVM
        {
            get { return _InverseForwardSolverTypeVM; }
            set
            {
                _InverseForwardSolverTypeVM = value;
                OnPropertyChanged("InverseForwardSolverTypeOptionVM");
            }
        }

        public OptionViewModel<OptimizerType> OptimizerTypeOptionVM
        {
            get { return _OptimizerTypeVM; }
            set
            {
                _OptimizerTypeVM = value;
                OnPropertyChanged("OptimizerTypeOptionVM");
            }
        }

        public OptionViewModel<InverseFitType> InverseFitTypeOptionVM
        {
            get { return _InverseFitTypeVM; }
            set
            {
                _InverseFitTypeVM = value;
                OnPropertyChanged("InverseFitTypeOptionVM");
            }
        }

        public OpticalPropertyViewModel MeasuredOpticalPropertyVM
        {
            get { return _MeasuredOpticalPropertyVM; }
            set
            {
                _MeasuredOpticalPropertyVM = value;
                OnPropertyChanged("MeasuredOpticalPropertyVM");
            }
        }

        public bool UseSpectralPanelData // for measured data
        {
            get { return _useSpectralPanelData; }
            set
            {
                _useSpectralPanelData = value;
                OnPropertyChanged("UseSpectralPanelData");
            }
        }

        public bool ShowOpticalProperties // for measured data
        {
            get { return _showOpticalProperties; }
            set
            {
                _showOpticalProperties = value;
                OnPropertyChanged("ShowOpticalProperties");
            }
        }

        public OpticalPropertyViewModel InitialGuessOpticalPropertyVM
        {
            get { return _InitialGuessOpticalPropertyVM; }
            set
            {
                _InitialGuessOpticalPropertyVM = value;
                OnPropertyChanged("InitialGuessOpticalPropertyVM");
            }
        }

        public OpticalPropertyViewModel ResultOpticalPropertyVM
        {
            get { return _ResultOpticalPropertyVM; }
            set
            {
                _ResultOpticalPropertyVM = value;
                OnPropertyChanged("ResultOpticalPropertyVM");
            }
        }

        public double PercentNoise
        {
            get { return _PercentNoise; }
            set
            {
                _PercentNoise = value;
                OnPropertyChanged("PercentNoise");
            }
        }

        public IForwardSolver MeasuredForwardSolver
        {
            get{ return SolverFactory.GetForwardSolver(MeasuredForwardSolverTypeOptionVM.SelectedValue); }
        }

        public IForwardSolver InverseForwardSolver
        {
            get{ return SolverFactory.GetForwardSolver(InverseForwardSolverTypeOptionVM.SelectedValue); }
        }

        public IOptimizer Optimizer
        {
            get { return SolverFactory.GetOptimizer(OptimizerTypeOptionVM.SelectedValue); }
        }

        void SimulateMeasuredDataCommand_Executed(object sender, ExecutedEventArgs e)
        {
            var measuredDataValues = GetSimulatedMeasuredData();
            var measuredDataPoints = GetDataPoints(measuredDataValues);

            //Report the results
            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);
            string[] plotLabels = GetLegendLabels(PlotDataType.Simulated);
            var plotData = Enumerable.Zip(measuredDataPoints, plotLabels, (p, el) => new PlotData(p, el)).ToArray();
            Commands.Plot_PlotValues.Execute(plotData);
            Commands.TextOutput_PostMessage.Execute("Simulated Measured Data: " + MeasuredOpticalPropertyVM + "\r");
        }

        private enum PlotDataType { Simulated, Guess, Calculated }

        private string[] GetLegendLabels(PlotDataType datatype)
        {
            string modelString = null;
            switch (MeasuredForwardSolverTypeOptionVM.SelectedValue)
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

            string solverString = null;

            //if (IsMultiRegion && MultiRegionTissueVM != null)
            //{
            //    var regions = MultiRegionTissueVM.GetTissueInput().Regions;
            //    opString = "\rμa1=" + regions[0].RegionOP.Mua.ToString("F4") + "\rμs'1=" + regions[0].RegionOP.Musp.ToString("F4") +
            //               "\rμa2=" + regions[1].RegionOP.Mua.ToString("F4") + "\rμs'2=" + regions[1].RegionOP.Musp.ToString("F4");
            //}
            //else
            //{
            OpticalProperties opticalProperties = null;
            //OpticalPropertyViewModel op;
            //}
            switch (datatype)
            {
                case PlotDataType.Simulated:
                    solverString = "Simulated: \r";
                    opticalProperties = MeasuredOpticalPropertyVM.GetOpticalProperties();
                    break;
                case PlotDataType.Calculated:
                    solverString = "Calculated:\r";
                    opticalProperties = ResultOpticalPropertyVM.GetOpticalProperties();
                    break;
                case PlotDataType.Guess:
                    solverString = "Guess:\r";
                    opticalProperties = InitialGuessOpticalPropertyVM.GetOpticalProperties();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("datatype");
            }
            var opString = "\rμa=" + opticalProperties.Mua.ToString("F4") + " \rμs'=" + opticalProperties.Musp.ToString("F4");
            
            if (_allRangeVMs.Length > 1)
            {
                var isWavelengthPlot = _allRangeVMs.Any(vm => vm.AxisType == IndependentVariableAxis.Wavelength);
                var secondaryRangeVM = isWavelengthPlot
                    ? _allRangeVMs.Where(vm => vm.AxisType != IndependentVariableAxis.Wavelength).First()
                    : _allRangeVMs.Where(vm => vm.AxisType != IndependentVariableAxis.Time && vm.AxisType != IndependentVariableAxis.Ft).First();

                string[] secondaryAxesStrings = secondaryRangeVM.Values.Select(value => "\r" + secondaryRangeVM.AxisType.GetInternationalizedString() + " = " + value.ToString()).ToArray();
                return secondaryAxesStrings.Select(sas => solverString + modelString + sas + (isWavelengthPlot ? "\r(spectral μa,μs')" : opString)).ToArray();
            }

            return new []{ solverString + modelString + opString };
        }

        private PlotAxesLabels GetPlotLabels()
        {
            var sd = this.SolutionDomainTypeOptionVM;
            PlotAxesLabels axesLabels = new PlotAxesLabels(
                sd.SelectedDisplayName, sd.SelectedValue.GetUnits(), 
                sd.IndependentAxesVMs.First(),
                sd.ConstantAxesVMs);
            return axesLabels;
        }

        private void CalculateInitialGuessCommand_Executed(object sender, ExecutedEventArgs e)
        {
            var initialGuessDataValues = CalculateInitialGuess();
            var initialGuessDataPoints = GetDataPoints(initialGuessDataValues);

            //Report the results
            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);
            string[] plotLabels = GetLegendLabels(PlotDataType.Guess);
            var plotData = Enumerable.Zip(initialGuessDataPoints, plotLabels, (p, el) => new PlotData(p, el)).ToArray();
            Commands.Plot_PlotValues.Execute(plotData);
            Commands.TextOutput_PostMessage.Execute("Initial Guess: " + InitialGuessOpticalPropertyVM + " \r");
        }

        private void SolveInverseCommand_Executed(object sender, ExecutedEventArgs e)
        {
            // Report inverse solver setup and results
            Commands.TextOutput_PostMessage.Execute("Inverse Solution Results: \r");
            Commands.TextOutput_PostMessage.Execute("   Optimization parameter(s): " + InverseFitTypeOptionVM.SelectedValue + " \r");
            Commands.TextOutput_PostMessage.Execute("   Initial Guess: " + InitialGuessOpticalPropertyVM + " \r");

            var inverseResult = SolveInverse();
            ResultOpticalPropertyVM.SetOpticalProperties(inverseResult.FitOpticalProperties.First()); // todo: this only works for one set of properties

            //Report the results
            if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Contains(IndependentVariableAxis.Wavelength) && 
                inverseResult.FitOpticalProperties.Length > 1) // If multi-valued OPs, the results aren't in the "scalar" VMs, need to parse OPs directly
            {
                var fitOPs = inverseResult.FitOpticalProperties;
                var measuredOPs = inverseResult.MeasuredOpticalProperties;
                var wavelengths = GetParameterValues(IndependentVariableAxis.Wavelength);
                var wvUnitString = IndependentVariableAxisUnits.NM.GetInternationalizedString(); 
                var opUnitString = IndependentVariableAxisUnits.InverseMM.GetInternationalizedString();
                var sb = new StringBuilder("\t[Wavelength (" + wvUnitString + ")]\t\t\t\t\t\t[Exact]\t\t\t\t\t\t[At Converged Values]\t\t\t\t\t\t[Units]\r");
                for (int i = 0; i < fitOPs.Length; i++)
                {
                    sb.Append("\t" + wavelengths[i] + "\t\t\t\t\t\t" + measuredOPs[i] + "\t\t\t" + fitOPs[i] + "\t\t\t" + opUnitString + " \r");
                }
                Commands.TextOutput_PostMessage.Execute(sb.ToString());
            }
            else
            {
                Commands.TextOutput_PostMessage.Execute("   Exact: " + MeasuredOpticalPropertyVM + " \r");
                Commands.TextOutput_PostMessage.Execute("   At Converged Values: " + ResultOpticalPropertyVM + " \r");
            }

            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);
            string[] plotLabels = GetLegendLabels(PlotDataType.Calculated);
            var plotData = Enumerable.Zip(inverseResult.FitDataPoints, plotLabels, (p, el) => new PlotData(p, el)).ToArray();
            Commands.Plot_PlotValues.Execute(plotData);
        }

        private double[] GetSimulatedMeasuredData()
        {
            var opticalProperties = GetMeasuredOpticalProperties();

            var parameters = GetParametersInOrder(opticalProperties);

            var measuredData = ComputationFactory.ComputeReflectance(
                MeasuredForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisType.R,
                parameters.Values.ToArray());

            return measuredData.AddNoise(PercentNoise);
        }
        
        private object GetMeasuredOpticalProperties()
        {
            return GetOpticalPropertiesFromSpectralPanel() ?? new[] { MeasuredOpticalPropertyVM.GetOpticalProperties() };
        }

        private object GetInitialGuessOpticalProperties()
        {
            return GetDuplicatedListofOpticalProperties() ?? new[] { InitialGuessOpticalPropertyVM.GetOpticalProperties() };
        }

        private OpticalProperties[] GetDuplicatedListofOpticalProperties()
        {
            var initialGuessOPs = InitialGuessOpticalPropertyVM.GetOpticalProperties();
            if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Contains(IndependentVariableAxis.Wavelength))
            {
                var wavelengths = GetParameterValues(IndependentVariableAxis.Wavelength);
                return wavelengths.Select(_ => initialGuessOPs.Clone()).ToArray();
            }

            return null;
        }

        private OpticalProperties[] GetOpticalPropertiesFromSpectralPanel()
        {
            if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Contains(IndependentVariableAxis.Wavelength) &&
                UseSpectralPanelData &&
                SolverDemoViewModel.Current != null &&
                SolverDemoViewModel.Current.SpectralMappingVM != null)
            {
                var tissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue;
                var wavelengths = GetParameterValues(IndependentVariableAxis.Wavelength);
                var ops = tissue.GetOpticalProperties(wavelengths);

                return ops;
            }

            return null;
        }

        public double[] CalculateInitialGuess()
        {
            var opticalProperties = GetInitialGuessOpticalProperties();

            var parameters = GetParametersInOrder(opticalProperties);

            return ComputationFactory.ComputeReflectance(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisType.R,
                parameters.Values.ToArray());
        }

        public class InverseSolutionResult
        {
            public IDataPoint[][] FitDataPoints { get; set; }
            public OpticalProperties[] MeasuredOpticalProperties { get; set; }
            public OpticalProperties[] GuessOpticalProperties { get; set; }
            public OpticalProperties[] FitOpticalProperties { get; set; }
        }

        public InverseSolutionResult SolveInverse()
        {
            var measuredOpticalProperties = GetMeasuredOpticalProperties();
            var measuredDataValues = GetSimulatedMeasuredData();

            var dependentValues = measuredDataValues.ToArray();
            var initGuessOpticalProperties = GetInitialGuessOpticalProperties();
            var initGuessParameters = GetParametersInOrder(initGuessOpticalProperties);

            double[] fit = ComputationFactory.SolveInverse(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                OptimizerTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                dependentValues,
                dependentValues, // set standard deviation, sd, to measured (works w/ or w/o noise)
                InverseFitTypeOptionVM.SelectedValue,
                initGuessParameters.Values.ToArray());

            var fitOpticalProperties = ComputationFactory.UnFlattenOpticalProperties(fit);

            var fitParameters = GetParametersInOrder(fitOpticalProperties);

            var resultDataValues = ComputationFactory.ComputeReflectance(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisType.R,
                fitParameters.Values.ToArray());

            var resultDataPoints = GetDataPoints(resultDataValues);

            return new InverseSolutionResult
            {
                FitDataPoints = resultDataPoints,
                MeasuredOpticalProperties = (OpticalProperties[])measuredOpticalProperties, // todo: currently only supports homog OPs
                GuessOpticalProperties = (OpticalProperties[])initGuessOpticalProperties, // todo: currently only supports homog OPss
                FitOpticalProperties = fitOpticalProperties
            };
        }

        private IDataPoint[][] GetDataPoints(double[] reflectance)
        {
            var plotIsVsWavelength = _allRangeVMs.Any(vm => vm.AxisType == IndependentVariableAxis.Wavelength);
            var isComplexPlot = ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue);
            var primaryIdependentValues = _allRangeVMs.First().Values.ToArray();
            var numPointsPerCurve = primaryIdependentValues.Length;
            var numForwardValues = isComplexPlot ? reflectance.Length / 2 : reflectance.Length;
            // complex reported as all reals, then all imaginaries
            var numCurves = numForwardValues / numPointsPerCurve;

            var points = new IDataPoint[numCurves][];
            Func<int, int, IDataPoint> getReflectanceAtIndex = (i, j) =>
            {
                // man, this is getting hacky...
                var index = plotIsVsWavelength
                    ? i * numCurves + j
                    : j * numPointsPerCurve + i;
                return isComplexPlot
                    ? (IDataPoint)
                        new ComplexDataPoint(primaryIdependentValues[i],
                            new Complex(reflectance[index], reflectance[index + numForwardValues]))
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
                                select new KeyValuePair<IndependentVariableAxis, object>(iv, GetParameterValues(iv));

            // OPs are always first in the list
            return (new KeyValuePair<IndependentVariableAxis, object>(IndependentVariableAxis.Wavelength, opticalProperties)).AsEnumerable()
                .Concat(allParameters).ToDictionary();
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
                        return new[] { SolutionDomainTypeOptionVM.ConstantAxesVMs[0].AxisValue };
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
    }
}
