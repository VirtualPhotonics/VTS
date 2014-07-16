using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using SLExtensions.Input;
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
        private OpticalPropertyViewModel _OpticalPropertyVM;

        private bool _showOpticalProperties;
        private bool _useSpectralPanelData;

        public ForwardSolverViewModel()
        {
            _showOpticalProperties = true;
            _useSpectralPanelData = false;

            RangeVM = new RangeViewModel { Title = "Detection Parameters" };
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

            //// test code
            //var methodInfo = typeof(IForwardSolver).GetMethod("ROfRho");
            //var result = methodInfo.Invoke(SolverFactory.GetForwardSolver(ForwardSolverType.PointSourceSDA), parameters.ToArray());
            //Console.WriteLine(result);

            double[] reflectance = ComputationFactory.ComputeReflectance(
                    ForwardSolverTypeOptionVM.SelectedValue,
                    SolutionDomainTypeOptionVM.SelectedValue,
                    ForwardAnalysisTypeOptionVM.SelectedValue,
                    parameters);

            // this all needs to change if we add multi-axis ranges
            if (ComputationFactory.IsComplexSolver(SolutionDomainTypeOptionVM.SelectedValue))
            {
                var real = query.Take(independentValues.Length).ToArray();
                var imag = query.Skip(independentValues.Length).Take(independentValues.Length).ToArray();

                return new[] {
                    Enumerable.Zip(
                        independentValues,
                        real,
                        (x, y) => new Point(x, y)).ToArray(),
                    Enumerable.Zip(
                        independentValues,
                        imag, 
                        (x, y) => new Point(x, y)).ToArray()
                };
            }
            else
            {
                var points = new[] { new Point[numPoints] };
                for (int i = 0; i < numPoints; i++)
                {
                    points[0][i] = new Point(independentValues[i], reflectance[i]);
                }
                return points;
            }
        }

        private object[] GetParametersInOrder()
        {
            var opticalProperties = GetOpticalProperties();

            var parameters = new List<object>
            {
                opticalProperties
            };

            switch (SolutionDomainTypeOptionVM.SelectedValue)
            {
                case SolutionDomainType.ROfRho:
                case SolutionDomainType.ROfFx:
                    switch (SolutionDomainTypeOptionVM.IndependentAxisType)
                    {
                        case IndependentVariableAxis.Rho:
                        case IndependentVariableAxis.Fx:
                            parameters.Add(RangeVM.Values.ToArray());
                            break;
                        case IndependentVariableAxis.Wavelength:
                            parameters.Add(new[] {SolutionDomainTypeOptionVM.ConstantAxisValue});
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case SolutionDomainType.ROfRhoAndTime:
                case SolutionDomainType.ROfFxAndTime:
                case SolutionDomainType.ROfRhoAndFt:
                case SolutionDomainType.ROfFxAndFt:
                    switch (SolutionDomainTypeOptionVM.IndependentAxisType)
                    {
                        case IndependentVariableAxis.Rho:
                        case IndependentVariableAxis.Fx:
                            parameters.Add(RangeVM.Values.ToArray());
                            parameters.Add(new[] {SolutionDomainTypeOptionVM.ConstantAxisValue});
                            break;
                        case IndependentVariableAxis.Time:
                        case IndependentVariableAxis.Ft:
                            parameters.Add(new[] {SolutionDomainTypeOptionVM.ConstantAxisValue});
                            parameters.Add(RangeVM.Values.ToArray());
                            break;
                        case IndependentVariableAxis.Wavelength:
                            parameters.Add(new[] {SolutionDomainTypeOptionVM.ConstantAxisValue});
                            parameters.Add(new[] {SolutionDomainTypeOptionVM.ConstantAxisTwoValue});
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return parameters.ToArray();
        }

        private OpticalProperties[] GetOpticalProperties()
        {
            if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Wavelength &&
                UseSpectralPanelData && 
                SolverDemoViewModel.Current != null &&
                SolverDemoViewModel.Current.SpectralMappingVM != null)
            {
                var tissue = SolverDemoViewModel.Current.SpectralMappingVM.SelectedTissue;
                return tissue.GetOpticalProperties(RangeVM.Values.ToArray());
            }

            return new[] { OpticalPropertyVM.GetOpticalProperties() };
        }
    }
}
