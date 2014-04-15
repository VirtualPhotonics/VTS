using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using SLExtensions.Input;
using Vts.Extensions;
using Vts.Factories;
using Vts.Gui.Silverlight.Extensions;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;

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

        public InverseSolverViewModel()
        {
            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain:", SolutionDomainType.ROfRho);
            RangeVM = new RangeViewModel { Title = "" };

            SolutionDomainTypeOptionVM.SolverType = SolverType.Inverse;

            // todo: white-list solvers as in ForwardSolverViewModel
            MeasuredForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>(
                "Forward Model Engine",
                false);

            MeasuredDataTypeOptionVM = new OptionViewModel<MeasuredDataType>("Measured Data Type:");
            MeasuredDataTypeOptionVM.PropertyChanged += (sender, args) =>
                OnPropertyChanged("MeasuredForwardSolver");

            // todo: white-list solvers as in ForwardSolverViewModel
            InverseForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>(
                "Inverse Model Engine",
                false);
            InverseForwardSolverTypeOptionVM.PropertyChanged += (sender, args) =>
                OnPropertyChanged("InverseForwardSolver");

            OptimizerTypeOptionVM = new OptionViewModel<OptimizerType>("Optimizer Type:", true);
            OptimizerTypeOptionVM.PropertyChanged += (sender, args) =>
                OnPropertyChanged("Optimizer");

            InverseFitTypeOptionVM = new OptionViewModel<InverseFitType>("Optimization Parameters:", true);
            //InverseFitTypeOptionVM.PropertyChanged += (sender, args) => UpdateModels();

            MeasuredOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "" };
            InitialGuessOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "" };
            ResultOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "" };

            SimulateMeasuredDataCommand = new RelayCommand(() => SimulateMeasuredDataCommand_Executed(null, null));
            CalculateInitialGuessCommand = new RelayCommand(() => CalculateInitialGuessCommand_Executed(null, null));
            SolveInverseCommand = new RelayCommand(() => SolveInverseCommand_Executed(null, null));

            Commands.IS_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;
        }

        public RelayCommand SimulateMeasuredDataCommand { get; set; }
        public RelayCommand CalculateInitialGuessCommand { get; set; }
        public RelayCommand SolveInverseCommand { get; set; }

        #region Sub-View-Models

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
            get { return (OpticalPropertyViewModel)_MeasuredOpticalPropertyVM; }
            set
            {
                _MeasuredOpticalPropertyVM = value;
                OnPropertyChanged("MeasuredOpticalPropertyVM");
            }
        }
        public OpticalPropertyViewModel InitialGuessOpticalPropertyVM
        {
            get { return (OpticalPropertyViewModel)_InitialGuessOpticalPropertyVM; }
            set
            {
                _InitialGuessOpticalPropertyVM = value;
                OnPropertyChanged("InitialGuessOpticalPropertyVM");
            }
        }
        public OpticalPropertyViewModel ResultOpticalPropertyVM
        {
            get { return (OpticalPropertyViewModel)_ResultOpticalPropertyVM; }
            set
            {
                _ResultOpticalPropertyVM = value;
                OnPropertyChanged("ResultOpticalPropertyVM");
            }
        }

        #endregion

        #region Model-Related

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
            get
            {
                return SolverFactory.GetForwardSolver(
                    MeasuredForwardSolverTypeOptionVM.SelectedValue);
            }
        }

        public IForwardSolver InverseForwardSolver
        {
            get
            {
                return SolverFactory.GetForwardSolver(
                    InverseForwardSolverTypeOptionVM.SelectedValue);
            }
        }

        public IOptimizer Optimizer
        {
            get
            {
                return SolverFactory.GetOptimizer(
                    OptimizerTypeOptionVM.SelectedValue);
            }
        }

        #endregion
        
        public Point[][] MeasuredDataPoints
        {
            get
            {
                // if it's reporting Real + Imaginary, we need a vector twice as long
                if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Ft)
                {
                    var numValues = RangeVM.Number;
                    var real = MeasuredDataValues.Take(numValues);
                    var imag = MeasuredDataValues.Skip(numValues).Take(numValues);

                    return new[] {
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            RangeVM.Values, real, (x, y) => new Point(x, y))),
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            RangeVM.Values, imag, (x, y) => new Point(x, y)))
                    };
                }

                return new[] { new Point[RangeVM.Number].PopulateFromEnumerable2(Enumerable.Zip(RangeVM.Values, MeasuredDataValues, (x, y) => new Point(x, y))) };

            }
        }

        public double[] MeasuredDataValues
        {
            get { return _MeasuredDataValues; }
            set { _MeasuredDataValues = value; OnPropertyChanged("MeasuredDataValues"); }
        }

        public Point[][] InitialGuessDataPoints
        {
            get
            {
                // if it's reporting Real + Imaginary, we need a vector twice as long
                if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Ft)
                {
                    var numValues = RangeVM.Number;
                    var real = InitialGuessDataValues.Take(numValues);
                    var imag = InitialGuessDataValues.Skip(numValues).Take(numValues);
                    return new[] {
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            RangeVM.Values, real, (x, y) => new Point(x, y))),
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            RangeVM.Values, imag, (x, y) => new Point(x, y)))
                    };
                }

                return new[] {new Point[ RangeVM.Number].PopulateFromEnumerable2( Enumerable.Zip(RangeVM.Values, InitialGuessDataValues, (x, y) => new Point(x, y))) };
            }
        }

        public double[] InitialGuessDataValues
        {
            get { return _InitialGuessDataValues; }
            set { _InitialGuessDataValues = value; OnPropertyChanged("InitialGuessDataValues"); }
        }

        public Point[][] ResultDataPoints
        {
            get
            {
                // if it's reporting Real + Imaginary, we need a vector twice as long
                if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Ft)
                {
                    var numValues = RangeVM.Number;
                    var real = ResultDataValues.Take(numValues);
                    var imag = ResultDataValues.Skip(numValues).Take(numValues);
                    return new[] {
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            RangeVM.Values, real, (x, y) => new Point(x, y))),
                        new Point[numValues].PopulateFromEnumerable2(Enumerable.Zip(
                            RangeVM.Values, imag, (x, y) => new Point(x, y)))
                    };
                }

                return new[] { new Point[ RangeVM.Number].PopulateFromEnumerable2(Enumerable.Zip(RangeVM.Values, ResultDataValues, (x, y) => new Point(x, y))) };
            }
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

        void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e)
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
                    sd.SelectedDisplayName, sd.SelectedValue.GetUnits(), sd.ConstantAxisLabel,
                    sd.ConstantAxisUnits, sd.ConstantAxisValue);
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
            if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Ft)
            {
                var real = points[0];
                var imag = points[1];
                // convert Point to ComplexPoint
                var complexPoints = new List<ComplexPoint>();
                for (int i = 0; i < real.Length; i++)
                {
                    complexPoints.Add(new ComplexPoint(real[i].X, new Complex(real[i].Y, imag[i].Y)));
                }
                Commands.Plot_PlotValues.Execute(new PlotData(complexPoints, plotLabel));
            }
            else
            {
                Commands.Plot_PlotValues.Execute(new PlotData(points.First(), plotLabel));
            }
        }

        public double[] CalculateMeasuredData()
        {
            switch (MeasuredDataTypeOptionVM.SelectedValue)
            {
                case MeasuredDataType.Simulated:
                default:
                    return GetSimulatedMeasuredData();
                    break;
                case MeasuredDataType.FromFile:
                    return GetMeasuredDataFromFile();
                    break;
            }
        }

        private double[] GetSimulatedMeasuredData()
        {
            var independentValues = RangeVM.Values.ToArray();

            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                ? new double[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0];

            var measuredData = ComputationFactory.ComputeReflectance(
                MeasuredForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisType.R,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                independentValues,
                MeasuredOpticalPropertyVM.GetOpticalProperties(),
                constantValues).ToArray();

            return measuredData.AddNoise(PercentNoise);
        }

        private double[] GetMeasuredDataFromFile()
        {
            return RangeVM.Values.ToArray().AddNoise(PercentNoise);
        }

        public double[] CalculateInitialGuess()
        {
            var independentValues = RangeVM.Values.ToArray();

            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                ? new double[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0];

            return ComputationFactory.ComputeReflectance(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisType.R,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                independentValues,
                InitialGuessOpticalPropertyVM.GetOpticalProperties(),
                constantValues).ToArray();
        }

        public void SolveInverse()
        {
            MeasuredDataValues = GetSimulatedMeasuredData();

            //var op = ResultOpticalPropertyVM;
            // Solve the inverse problem for optical properties
            // todo: is this a good format for the solver?
            //var opGuess = InitialGuessOpticalPropertyVM;

            var independentValues = RangeVM.Values.ToArray();
            var dependentValues = MeasuredDataValues.ToArray();

            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                    ? new double[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0];

            double[] fit = ComputationFactory.SolveInverse(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                OptimizerTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                independentValues,
                dependentValues,
                dependentValues, // set standard deviation, sd, to measured (works w/ or w/o noise)
                InitialGuessOpticalPropertyVM.GetOpticalProperties(),
                InverseFitTypeOptionVM.SelectedValue,
                constantValues);

            ResultOpticalPropertyVM.Mua = fit[0];
            ResultOpticalPropertyVM.Musp = fit[1];
            ResultOpticalPropertyVM.G = fit[2];
            ResultOpticalPropertyVM.N = fit[3];

            ResultDataValues = ComputationFactory.ComputeReflectance(
                InverseForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisType.R,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                independentValues,
                ResultOpticalPropertyVM.GetOpticalProperties(),
                constantValues);
        }

    }
}
