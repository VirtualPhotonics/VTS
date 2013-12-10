using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using GalaSoft.MvvmLight.Command;
using SLExtensions.Input;
using Vts.Factories;
using Vts.Gui.Silverlight.Input;
using Vts.Gui.Silverlight.Model;
using Vts.Gui.Silverlight.Extensions;
#if WHITELIST
using Vts.Gui.Silverlight.ViewModel.Application;
#endif
using Vts.SpectralMapping;

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

        // todo: This should be handled by a Controller, not dealt with directly
        // by the ViewModel. Job of ViewModel is just to "surface" Model information
        // to the View - shouldn't have to do any appreciable work/thinking/analysis.
        // Currently, the Factories class is serving this need, but I'm afraid it's 
        // getting too big...
        // public IForwardSolver ForwardSolver { get; set; }
        //private IAnalyzer Analyzer { get; set; }

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
            double[] independentValues = RangeVM.Values.ToArray();
            
            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                    ? new double[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0];

            double[] reflectance = ComputationFactory.ComputeReflectance(
                ForwardSolverTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.SelectedValue,
                ForwardAnalysisTypeOptionVM.SelectedValue,
                SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                independentValues,
                OpticalPropertyVM.GetOpticalProperties(),
                constantValues);

            var numPoints = independentValues.Length;
            if (SolutionDomainTypeOptionVM.IndependentAxisType == IndependentVariableAxis.Ft)
            {
                var points = new[] { new Point[numPoints], new Point[numPoints] };
                for (int i = 0; i < numPoints; i++)
                {
                    points[0][i] = new Point(independentValues[i], reflectance[i]);
                    points[1][i] = new Point(independentValues[i], reflectance[numPoints + i]); // Imaginary is stored after Real
                }
                return points;
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
    }
}
