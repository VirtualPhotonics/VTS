using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using System.Windows;
using SLExtensions.Input;
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

        private object _tissueInputVM; // either an OpticalPropertyViewModel or a MultiRegionTissueViewModel is stored here, and dynamically displayed
        
        // private fields to cache created instances of tissue inputs, created on-demand in GetTissueInputVM (vs up-front in constructor)
        private OpticalProperties _currentHomogeneousOpticalProperties;
        private SemiInfiniteTissueInput _currentSemiInfiniteTissueInput;
        private MultiLayerTissueInput _currentMultiLayerTissueInput;
        private SingleEllipsoidTissueInput _currentSingleEllipsoidTissueInput;
        
        public ForwardSolverViewModel()
        {
            RangeVM = new RangeViewModel { Title = "Detection Parameters:" };
            
            // right now, we're doing manual databinding to the selected item. need to enable databinding 
            // confused, though - do we need to use strings? or, how to make generics work with dependency properties?
#if WHITELIST 
            ForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Forward Model:",false, WhiteList.ForwardSolverTypes);
#else
            ForwardSolverTypeOptionVM = new OptionViewModel<ForwardSolverType>("Forward Model:", false);
#endif
            ForwardSolverTypeOptionVM.PropertyChanged += (sender, args) =>
            {
                OnPropertyChanged("IsGaussianForwardModel");
                OnPropertyChanged("ForwardSolver");

                OnPropertyChanged("IsMultiRegion");
                OnPropertyChanged("IsSemiInfinite");
                TissueInputVM = GetTissueInputVM(IsMultiRegion ? MonteCarlo.TissueType.MultiLayer : MonteCarlo.TissueType.SemiInfinite);
            };
            ForwardSolverTypeOptionVM.SelectedValue = ForwardSolverType.PointSourceSDA; // force the model choice here?

            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain:", SolutionDomainType.ROfRho);

            ForwardAnalysisTypeOptionVM = new OptionViewModel<ForwardAnalysisType>("Model/Analysis Output:", true);

            SolutionDomainTypeOptionVM.SolverType = SolverType.Forward;

            Commands.FS_ExecuteForwardSolver.Executed += ExecuteForwardSolver_Executed;
            Commands.FS_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;
        }

        public IForwardSolver ForwardSolver
        {
            get { return SolverFactory.GetForwardSolver(ForwardSolverTypeOptionVM.SelectedValue); }
        }

        public bool IsGaussianForwardModel
        {
            get { return ForwardSolverTypeOptionVM.SelectedValue.IsGaussianForwardModel(); }
        }

        public bool IsMultiRegion
        {
            get { return ForwardSolverTypeOptionVM.SelectedValue.IsMultiRegionForwardModel(); }
        }

        public bool IsSemiInfinite
        {
            get { return !ForwardSolverTypeOptionVM.SelectedValue.IsMultiRegionForwardModel(); }
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

        public object TissueInputVM
        {
            get { return _tissueInputVM; }
            private set
            {
                _tissueInputVM = value;
                OnPropertyChanged("TissueInputVM");
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

        void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is RangeViewModel)
            {
                RangeVM = (RangeViewModel)e.Parameter;
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

            Commands.TextOutput_PostMessage.Execute("Forward Solver: " + TissueInputVM + "\r"); // todo: override ToString() for MultiRegionTissueViewModel
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
        
        private object GetTissueInputVM(Vts.MonteCarlo.TissueType tissueType)
        {
            // ops to use as the basis for instantiating multi-region tissues based on homogeneous values (for differential comparison)
            if (_currentHomogeneousOpticalProperties == null)
            {
                _currentHomogeneousOpticalProperties = new OpticalProperties(0.01, 1, 0.8, 1.4);
            }

            switch (tissueType)
            {
                case MonteCarlo.TissueType.SemiInfinite:
                    if (_currentSemiInfiniteTissueInput == null)
                    {
                        _currentSemiInfiniteTissueInput = new SemiInfiniteTissueInput(new SemiInfiniteRegion(_currentHomogeneousOpticalProperties));
                    }
                    return new OpticalPropertyViewModel(
                        ((SemiInfiniteTissueInput)_currentSemiInfiniteTissueInput).Regions.First().RegionOP,
                         IndependentVariableAxisUnits.InverseMM.GetInternationalizedString(),
                        "Optical Properties:");
                    break;
                case MonteCarlo.TissueType.MultiLayer:
                    if (_currentMultiLayerTissueInput == null)
                    {
                        _currentMultiLayerTissueInput = new MultiLayerTissueInput(new ITissueRegion[]
                            { 
                                new LayerRegion(new DoubleRange(0, 2), _currentHomogeneousOpticalProperties.Clone() ), 
                                new LayerRegion(new DoubleRange(2, double.PositiveInfinity), _currentHomogeneousOpticalProperties.Clone() ), 
                            });
                    }
                    return new MultiRegionTissueViewModel(_currentMultiLayerTissueInput);
                case MonteCarlo.TissueType.SingleEllipsoid:
                    if (_currentSingleEllipsoidTissueInput == null)
                    {
                        _currentSingleEllipsoidTissueInput = new SingleEllipsoidTissueInput(
                            new EllipsoidRegion(new Position(0, 0, 10), 5, 5, 5, new OpticalProperties(0.05, 1.0, 0.8, 1.4)),
                            new ITissueRegion[]
                            { 
                                new LayerRegion(new DoubleRange(0, double.PositiveInfinity), _currentHomogeneousOpticalProperties.Clone()), 
                            });
                    }
                    return new MultiRegionTissueViewModel(_currentSingleEllipsoidTissueInput);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
                case ForwardSolverType.TwoLayerSDA:
                    modelString = "Model - 2 layer SDA \r";
                    break;
            }
            string opString = null;

            if (IsMultiRegion)
            {
                ITissueRegion[] regions = null;
                if (ForwardSolver is TwoLayerSDAForwardSolver)
                {
                    regions = ((MultiRegionTissueViewModel)TissueInputVM).GetTissueInput().Regions;
                    opString =
                        "μa1=" + regions[0].RegionOP.Mua + " μs'1=" + regions[0].RegionOP.Musp + "\r" +
                        "μa2=" + regions[1].RegionOP.Mua + " μs'2=" + regions[1].RegionOP.Musp; 
                }
            }
            else
            {
                var opticalProperties = ((OpticalPropertyViewModel)TissueInputVM).GetOpticalProperties();
                opString = "μa=" + opticalProperties.Mua + " \rμs'=" + opticalProperties.Musp;
            }

            return modelString + opString;
        }

        public Point[][] ExecuteForwardSolver()
        {
            double[] independentValues = RangeVM.Values.ToArray(); // ToList() necessary?

            double[] constantValues =
                ComputationFactory.IsSolverWithConstantValues(SolutionDomainTypeOptionVM.SelectedValue)
                    ? new double[] { SolutionDomainTypeOptionVM.ConstantAxisValue } : new double[0];

            double[] query = null;
            if (IsMultiRegion)
            {
                ITissueRegion[] regions = null;
                if (ForwardSolver is TwoLayerSDAForwardSolver)
                {
                    regions = ((MultiRegionTissueViewModel) TissueInputVM).GetTissueInput().Regions;
                }

                if (regions == null)
                {
                    return null;
                }

                query = ComputationFactory.ComputeReflectance(
                        ForwardSolverTypeOptionVM.SelectedValue,
                        SolutionDomainTypeOptionVM.SelectedValue,
                        ForwardAnalysisTypeOptionVM.SelectedValue,
                        SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                        independentValues,
                        regions,
                        constantValues);
            }
            else
            {
                var opticalProperties = ((OpticalPropertyViewModel) TissueInputVM).GetOpticalProperties();
                query = ComputationFactory.ComputeReflectance(
                     ForwardSolverTypeOptionVM.SelectedValue,
                     SolutionDomainTypeOptionVM.SelectedValue,
                     ForwardAnalysisTypeOptionVM.SelectedValue,
                     SolutionDomainTypeOptionVM.IndependentVariableAxisOptionVM.SelectedValue,
                     independentValues,
                     opticalProperties,
                     constantValues);
            }

            // if it's reporting Real + Imaginary, we need two vectors
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
                return new[] {
                    Enumerable.Zip(
                        independentValues,
                        query, 
                        (x, y) => new Point(x, y)).ToArray()
                };
            }
        }
    }
}
