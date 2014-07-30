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
    /// View model implementing Inverse Solver panel functionality
    /// </summary>
    public class InverseSolverViewModel : BindableObject
    {
        private SolutionDomainOptionViewModel _SolutionDomainTypeOptionVM;
        private RangeViewModel _RangeVM;
        private OptionViewModel<ForwardSolverType> _MeasuredForwardSolverTypeVM;
        private OptionViewModel<MeasuredDataType> _MeasuredDataTypeVM;
        private OptionViewModel<ForwardSolverType> _InverseForwardSolverTypeVM;
        private OptionViewModel<OptimizerType> _OptimizerTypeVM;
        private OptionViewModel<InverseFitType> _InverseFitTypeVM;

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

            RangeVM = new RangeViewModel { Title = "" };

            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain", SolutionDomainType.ROfRho)
            {
                EnableMultiAxis = false,
            };

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
                    if (SolutionDomainTypeOptionVM.UseSpectralInputs)
                    {
                        UseSpectralPanelData = true;
                    }
                    else
                    {
                        UseSpectralPanelData = false;
                    }
                }
                if (args.PropertyName == "IndependentAxisType")
                {
                    // if using spectral panel inputs, assign RangeVM, tissue, etc, accordingly
                    if (UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null)
                    {
                        //// if the independent axis is wavelength, then hide optical properties (because they come from spectral panel)
                        if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Wavelength)
                        {
                            ShowOpticalProperties = false; // don't show optical properties at all
                            RangeVM = SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM; // bind to same instance, not a copy
                        }
                        else // still show optical properties and wavelength, but link them to spectral panel and pull wavelength
                        {
                            ShowOpticalProperties = true;
                            RangeVM = SolutionDomainTypeOptionVM.IndependentAxisType.GetDefaultIndependentAxisRange();
                            updateSolutionDomainWithWavelength(SolverDemoViewModel.Current.SpectralMappingVM.Wavelength);
                        }
                        //_spectralPanelTissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue; ... (or, do this at execution time?)s
                    }
                    else
                    {
                        ShowOpticalProperties = true;
                        RangeVM = SolutionDomainTypeOptionVM.IndependentAxisType.GetDefaultIndependentAxisRange();
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

            MeasuredDataTypeOptionVM = new OptionViewModel<MeasuredDataType>("Measured Data Type");
            MeasuredDataTypeOptionVM.PropertyChanged += (sender, args) =>
                OnPropertyChanged("MeasuredForwardSolver");

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
                if (UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null)
                {
                    updateSolutionDomainWithWavelength(SolverDemoViewModel.Current.SpectralMappingVM.Wavelength);
                }
            };
            Commands.Spec_UpdateOpticalProperties.Executed += (sender, args) =>
            {
                if (UseSpectralPanelData && SolverDemoViewModel.Current != null && SolverDemoViewModel.Current.SpectralMappingVM != null)
                {
                    MeasuredOpticalPropertyVM.SetOpticalProperties(SolverDemoViewModel.Current.SpectralMappingVM.OpticalProperties);
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

        public RangeViewModel RangeVM
        {
            get { return _RangeVM; }
            set
            {
                _RangeVM = value;
                OnPropertyChanged("RangeVM");
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

        public OptionViewModel<MeasuredDataType> MeasuredDataTypeOptionVM
        {
            get { return _MeasuredDataTypeVM; }
            set
            {
                _MeasuredDataTypeVM = value;
                OnPropertyChanged("MeasuredDataTypeOptionVM");
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
        
        public Point[][] MeasuredDataPoints
        {
            get { return GetPoints(RangeVM.Values.ToArray(), MeasuredDataValues, ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue)); }
        }

        public double[] MeasuredDataValues
        {
            get { return _MeasuredDataValues; }
            set
            {
                _MeasuredDataValues = value; 
                OnPropertyChanged("MeasuredDataValues");
            }
        }

        public Point[][] InitialGuessDataPoints
        {
            get { return GetPoints(RangeVM.Values.ToArray(), InitialGuessDataValues, ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue)); }
        }

        public double[] InitialGuessDataValues
        {
            get { return _InitialGuessDataValues; }
            set
            {
                _InitialGuessDataValues = value; 
                OnPropertyChanged("InitialGuessDataValues");
            }
        }

        public Point[][] ResultDataPoints
        {
            get { return GetPoints(RangeVM.Values.ToArray(), ResultDataValues, ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue)); }
        }

        private static Point[][] GetPoints(double[] rangeValues, double[] results, bool isComplex)
        {
            // if it's reporting Real + Imaginary, we need a vector twice as long
            if (isComplex)
            {
                var numValues = rangeValues.Length;
                var real = results.Take(numValues);
                var imag = results.Skip(numValues).Take(numValues);
                return new[] {
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            rangeValues, real, (x, y) => new Point(x, y))),
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            rangeValues, imag, (x, y) => new Point(x, y)))
                    };
            }

            return new[] { new Point[rangeValues.Length].PopulateFromEnumerable2(Enumerable.Zip(rangeValues, results, (x, y) => new Point(x, y))) };
        }

        public double[] ResultDataValues
        {
            get { return _ResultDataValues; }
            set
            {
                _ResultDataValues = value; 
                OnPropertyChanged("ResultDataValues");
            }
        }

        void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e) // todo: delete? (who used to use this?)
        {
            if (e.Parameter is RangeViewModel)
            {
                RangeVM = (RangeViewModel)e.Parameter;
            }
        }

        void SimulateMeasuredDataCommand_Executed(object sender, ExecutedEventArgs e)
        {
            MeasuredDataValues = CalculateMeasuredData();
            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            PlotValues(MeasuredDataPoints, PlotDataType.Simulated);
            Commands.TextOutput_PostMessage.Execute("Simulated Measured Data: " + MeasuredOpticalPropertyVM + "\r");
        }

        private enum PlotDataType { Simulated, Guess, Calculated }

        private string GetLegendLabel(PlotDataType datatype)
        {
            string solverString = null;
            string modelString = null;
            OpticalPropertyViewModel op;
            string opString = "";
            switch (datatype)
            {
                case PlotDataType.Simulated:
                    solverString = "Simulated: \r";
                    modelString =
                        MeasuredForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedPointSourceSDA ||
                        MeasuredForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.PointSourceSDA ||
                        MeasuredForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedGaussianSourceSDA
                        ? "Model - SDA \r" : "Model - MC \r";
                    op = MeasuredOpticalPropertyVM;
                    break;
                case PlotDataType.Calculated:
                    solverString = "Calculated:\r";
                    modelString =
                        InverseForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedPointSourceSDA ||
                        InverseForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.PointSourceSDA ||
                        InverseForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedGaussianSourceSDA
                        ? "Model - SDA \r" : "Model - MC \r";
                    op = ResultOpticalPropertyVM;
                    break;
                case PlotDataType.Guess:
                    solverString = "Guess:\r";
                    modelString =
                        InverseForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedPointSourceSDA ||
                        InverseForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.PointSourceSDA ||
                        InverseForwardSolverTypeOptionVM.SelectedValue == ForwardSolverType.DistributedGaussianSourceSDA
                        ? "Model - SDA \r" : "Model - MC \r";
                    op = InitialGuessOpticalPropertyVM;
                    break;
                default:
                    solverString = "";
                    modelString = "";
                    opString = "";
                    break;
            }

            return solverString + modelString + opString;
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

        void CalculateInitialGuessCommand_Executed(object sender, ExecutedEventArgs e)
        {
            InitialGuessDataValues = CalculateInitialGuess();
            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            PlotValues(InitialGuessDataPoints, PlotDataType.Guess);
            Commands.TextOutput_PostMessage.Execute("Initial Guess: " + InitialGuessOpticalPropertyVM + " \r");
        }

        void SolveInverseCommand_Executed(object sender, ExecutedEventArgs e)
        {
            // Report inverse solver setup and results
            Commands.TextOutput_PostMessage.Execute("Inverse Solution Results: \r");
            Commands.TextOutput_PostMessage.Execute("   Optimization parameter(s): " + InverseFitTypeOptionVM.SelectedValue + " \r");
            Commands.TextOutput_PostMessage.Execute("   Exact: " + MeasuredOpticalPropertyVM + " \r");
            Commands.TextOutput_PostMessage.Execute("   Initial Guess: " + InitialGuessOpticalPropertyVM + " \r");

            SolveInverse();

            //Report the results
            PlotValues(ResultDataPoints, PlotDataType.Calculated);
            Commands.TextOutput_PostMessage.Execute("   At Converged Values: " + ResultOpticalPropertyVM + " \r");
        }

        void PlotValues(Point[][] points, PlotDataType dataType)
        {
            string plotLabel = GetLegendLabel(dataType);
            if (ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue))
            {
                var real = points[0];
                var imag = points[1];
                // convert Point to ComplexPoint
                var complexPoints = new ComplexDataPoint[real.Length];
                for (int i = 0; i < real.Length; i++)
                {
                    complexPoints[i] = new ComplexDataPoint(real[i].X, new Complex(real[i].Y, imag[i].Y));
                }
                Commands.Plot_PlotValues.Execute(new PlotData(new []{ complexPoints.ToArray() }, new []{ plotLabel }));
            }
            else
            {
                Commands.Plot_PlotValues.Execute(new PlotData(points, new []{ plotLabel }));
            }
        }

        public double[] CalculateMeasuredData()
        {
            switch (MeasuredDataTypeOptionVM.SelectedValue)
            {
                case MeasuredDataType.Simulated:
                    return GetSimulatedMeasuredData();
                    break;
                case MeasuredDataType.FromFile:
                    return GetMeasuredDataFromFile();
                    break;
                default:
                    throw new ArgumentException("SelectedValue");
            }
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

        //private object[] GetParametersInOrder(OpticalProperties[] opticalProperties)
        //{
        //    var parameters = new List<object>
        //    {
        //        opticalProperties
        //    };

        //    switch (SolutionDomainTypeOptionVM.SelectedValue)
        //    {
        //        case SolutionDomainType.ROfRho:
        //        case SolutionDomainType.ROfFx:
        //            switch (SolutionDomainTypeOptionVM.IndependentAxisType)
        //            {
        //                case IndependentVariableAxis.Rho:
        //                case IndependentVariableAxis.Fx:
        //                    parameters.Add(RangeVM.Values.ToArray());
        //                    break;
        //                case IndependentVariableAxis.Wavelength:
        //                    parameters.Add(new[] { SolutionDomainTypeOptionVM.ConstantAxisValue });
        //                    break;
        //                default:
        //                    throw new ArgumentOutOfRangeException();
        //            }
        //            break;
        //        case SolutionDomainType.ROfRhoAndTime:
        //        case SolutionDomainType.ROfFxAndTime:
        //        case SolutionDomainType.ROfRhoAndFt:
        //        case SolutionDomainType.ROfFxAndFt:
        //            switch (SolutionDomainTypeOptionVM.IndependentAxisType)
        //            {
        //                case IndependentVariableAxis.Rho:
        //                case IndependentVariableAxis.Fx:
        //                    parameters.Add(RangeVM.Values.ToArray());
        //                    parameters.Add(new[] { SolutionDomainTypeOptionVM.ConstantAxisValue });
        //                    break;
        //                case IndependentVariableAxis.Time:
        //                case IndependentVariableAxis.Ft:
        //                    parameters.Add(new[] { SolutionDomainTypeOptionVM.ConstantAxisValue });
        //                    parameters.Add(RangeVM.Values.ToArray());
        //                    break;
        //                case IndependentVariableAxis.Wavelength:
        //                    parameters.Add(new[] { SolutionDomainTypeOptionVM.ConstantAxisValue });
        //                    parameters.Add(new[] { SolutionDomainTypeOptionVM.ConstantAxisTwoValue });
        //                    break;
        //                default:
        //                    throw new ArgumentOutOfRangeException();
        //            }
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }

        //    return parameters.ToArray();
        //}

        private object GetMeasuredOpticalProperties()
        {
            if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Wavelength &&
                UseSpectralPanelData &&
                SolverDemoViewModel.Current != null &&
                SolverDemoViewModel.Current.SpectralMappingVM != null)
            {
                var tissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue;
                return tissue.GetOpticalProperties(RangeVM.Values.ToArray());
            }

            return new[] { MeasuredOpticalPropertyVM.GetOpticalProperties() };
        }

        private object GetInitialGuessOpticalProperties()
        {
            if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Wavelength &&
                UseSpectralPanelData &&
                SolverDemoViewModel.Current != null &&
                SolverDemoViewModel.Current.SpectralMappingVM != null)
            {
                var tissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue;
                return tissue.GetOpticalProperties(RangeVM.Values.ToArray());
            }

            return new[] { InitialGuessOpticalPropertyVM.GetOpticalProperties() };
        }

        private double[] GetMeasuredDataFromFile()
        {
            return RangeVM.Values.ToArray().AddNoise(PercentNoise);
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

        public void SolveInverse()
        {
            MeasuredDataValues = GetSimulatedMeasuredData();

            var independentValues = RangeVM.Values.ToArray();
            var dependentValues = MeasuredDataValues.ToArray();
            
            double[] constantValues = ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                ? (UseSpectralPanelData ? new[] { SolutionDomainTypeOptionVM.ConstantAxisValue, SolutionDomainTypeOptionVM.ConstantAxisTwoValue } : new[] { SolutionDomainTypeOptionVM.ConstantAxisValue })
                : (UseSpectralPanelData ? new[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0]);


            var opticalProperties = GetInitialGuessOpticalProperties();

            var paramters = GetParametersInOrder(opticalProperties);

            double[] fit = ComputationFactory.SolveInverse(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                OptimizerTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                dependentValues,
                dependentValues, // set standard deviation, sd, to measured (works w/ or w/o noise)
                InverseFitTypeOptionVM.SelectedValue,
                paramters.Values.ToArray());

            ResultOpticalPropertyVM.Mua = fit[0];
            ResultOpticalPropertyVM.Musp = fit[1];
            ResultOpticalPropertyVM.G = fit[2];
            ResultOpticalPropertyVM.N = fit[3];

            var fitOpticalProperties = new[] { ResultOpticalPropertyVM.GetOpticalProperties() };
            //// todo: refactor and re-use this code via method-call
            //var parameters = ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
            //                 && SolutionDomainTypeOptionVM.IndependentAxisType.IsTemporalAxis()
            //    ? new object[]
            //      {
            //          opticalProperties,
            //          new [] { SolutionDomainTypeOptionVM.ConstantAxisValue },
            //          independentValues,
            //      }
            //    : new object[]
            //      {
            //          opticalProperties,
            //          independentValues,
            //          new [] { SolutionDomainTypeOptionVM.ConstantAxisValue },
            //      };

            var parameters = GetParametersInOrder(fitOpticalProperties);

            ResultDataValues = ComputationFactory.ComputeReflectance(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisType.R,
                parameters.Values.ToArray());
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
                        return new[] { SolutionDomainTypeOptionVM.ConstantAxisValue };
                    case 1:
                        return new[] { SolutionDomainTypeOptionVM.ConstantAxisTwoValue };
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
                    //case 2:
                    //    switch (positionIndex)
                    //    {
                    //        case 0:
                    //        default:
                    //            return RangeTwoVM.Values.ToArray();
                    //        case 1:
                    //            return RangeVM.Values.ToArray();
                    //    }
                    //case 3:
                    //    switch (positionIndex)
                    //    {
                    //        case 0:
                    //        default:
                    //            return RangeThreeVM.Values.ToArray();
                    //        case 1:
                    //            return RangeTwoVM.Values.ToArray();
                    //        case 2:
                    //            return RangeVM.Values.ToArray();
                    //    }
                }
            }
        }
    }
}
