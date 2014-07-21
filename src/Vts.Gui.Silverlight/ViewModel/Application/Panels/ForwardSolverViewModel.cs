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
            RangeTwoVM = new RangeViewModel { Title = "Detection Parameters 2" };
            RangeThreeVM = new RangeViewModel { Title = "Detection Parameters 3" };
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
                    }

                    if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Length == 2)
                    {
                        RangeVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[0] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisType.GetDefaultIndependentAxisRange();

                        RangeTwoVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[1] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisTwoType.GetDefaultIndependentAxisRange();

                        ShowIndependentVariable = true;
                        ShowIndependentVariableTwo = true;
                        ShowIndependentVariableThree = false;
                    }

                    if (SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.Length == 3)
                    {
                        RangeVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[0] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisType.GetDefaultIndependentAxisRange();

                        RangeTwoVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[1] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisTwoType.GetDefaultIndependentAxisRange();

                        RangeThreeVM = useSpectralPanelDataAndNotNull && SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues[2] == IndependentVariableAxis.Wavelength
                            ? SolverDemoViewModel.Current.SpectralMappingVM.WavelengthRangeVM // bind to same instance, not a copy
                            : SolutionDomainTypeOptionVM.IndependentAxisThreeType.GetDefaultIndependentAxisRange();

                        ShowIndependentVariable = true;
                        ShowIndependentVariableTwo = true;
                        ShowIndependentVariableThree = true;
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
            
            string plotLabel = GetLegendLabel();
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
                Commands.Plot_PlotValues.Execute(new PlotData(complexPoints, plotLabel));
            }
            else
            {
                Commands.Plot_PlotValues.Execute(new PlotData(points.First(), plotLabel));
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
                case ForwardSolverType.Nurbs:
                    modelString = "Model - nurbs \r";
                    break;
            }
            string opString = "μa=" + OpticalPropertyVM.Mua + " \rμs'=" + OpticalPropertyVM.Musp;

            return modelString + opString;
        }

        public Point[][] ExecuteForwardSolver()
        {
            var parameters = GetParametersInOrder();

            double[] reflectance = ComputationFactory.ComputeReflectance(
                    ForwardSolverTypeOptionVM.SelectedValue,
                    SolutionDomainTypeOptionVM.SelectedValue,
                    ForwardAnalysisTypeOptionVM.SelectedValue,
                    parameters);
                    
            var primaryIdependentValues = RangeVM.Values.ToArray();
            var numPointsPerCurve = primaryIdependentValues.Length;
            var numCurves =
                (ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue)
                    ? reflectance.Length/2 : reflectance.Length)
                / numPointsPerCurve;

            long globalIndex = 0;
            var points = new Point[numCurves][];
            for (int j = 0; j < numCurves; j++)
            {
                points[j] = new Point[numPointsPerCurve];
                for (int i = 0; i < numPointsPerCurve; i++, globalIndex++)
                {
                    points[j][i] = new Point(primaryIdependentValues[i], reflectance[globalIndex]);
                }
            }
            return points;
        }

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

        private object[] GetParametersInOrder()
        {
            Func<IndependentVariableAxis, int, object> getIVParameter = (iv, idx) =>
            {
                switch (idx)
                {
                    case 0:
                    default:
                        return RangeVM.Values.ToArray();
                    case 1:
                        return RangeTwoVM.Values.ToArray();
                    case 2:
                        return RangeThreeVM.Values.ToArray();
                }
            };

            Func<IndependentVariableAxis, int, object> getConstantParameter = (iv, idx) =>
            {
                switch (idx)
                {
                    case 0:
                    default:
                        return new [] {SolutionDomainTypeOptionVM.ConstantAxisValue};
                    case 1:
                        return new[] { SolutionDomainTypeOptionVM.ConstantAxisTwoValue };
                    //case 2:
                    //    return new[] { SolutionDomainTypeOptionVM.ConstantAxisThreeValue };
                }
            };
            
            // get all parameters to get arrays of
            // then, for each one, decide if it's an IV or a constant
            // then, call the appropriate parameter generator, defined above
            IEnumerable<object> allParameters = from iv in Enumerable.Concat(
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues)
                where iv != IndependentVariableAxis.Wavelength
                orderby GetParameterOrder(iv) descending 
                let isConstant = SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues.Contains(iv)
                select isConstant 
                        ? getConstantParameter(iv, SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.UnSelectedValues.IndexOf(iv))
                        : getIVParameter(iv, SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValues.IndexOf(iv));

            return ((object)GetOpticalProperties()).AsEnumerable().Concat(allParameters).ToArray();
        }

        private OpticalProperties[] GetOpticalProperties()
        {
            if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Wavelength &&
                UseSpectralPanelData && 
                SolverDemoViewModel.Current != null &&
                SolverDemoViewModel.Current.SpectralMappingVM != null)
            {
                var tissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue;
                return tissue.GetOpticalProperties(RangeVM.Values.ToArray()); // this should really depend on the IVs, but in practice, it's always first
            }

            return new[] { OpticalPropertyVM.GetOpticalProperties() };
        }
    }
}
