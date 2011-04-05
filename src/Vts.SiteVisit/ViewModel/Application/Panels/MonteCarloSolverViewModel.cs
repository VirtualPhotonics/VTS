using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using SLExtensions.Input;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.Sources;
using Vts.MonteCarlo.Tissues;
using Vts.SiteVisit.Input;
using Vts.SiteVisit.Model;
using Vts.SiteVisit.ViewModel.Application.Panels;

namespace Vts.SiteVisit.ViewModel
{
    /// <summary> 
    /// View model implementing the Monte Carlo panel functionality (experimental)
    /// </summary>
    public class MonteCarloSolverViewModel : BindableObject
    {
        private SimulationInput _simulationInput;

        private SimulationInputViewModel _simulationInputVM;

        private RangeViewModel _independentVariableRangeVM;

        private SolutionDomainOptionViewModel _solutionDomainTypeOptionVM;

        public MonteCarloSolverViewModel()
        {
            _simulationInput = new SimulationInput
            {
                TissueInput = new MultiLayerTissueInput(
                    new List<ITissueRegion> 
                    { 
                        new LayerRegion(
                            new DoubleRange(double.NegativeInfinity, 0.0),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0)),
                        new LayerRegion(
                            new DoubleRange(0.0, 1.0),
                            new OpticalProperties(0.1, 1.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(1.0, 100.0),
                            new OpticalProperties(0.01, 2.0, 0.8, 1.4)),
                        new LayerRegion(
                            new DoubleRange(100.0, double.PositiveInfinity),
                            new OpticalProperties(1e-10, 0.0, 0.0, 1.0))
                    }
                ),
                OutputFileName = "MonteCarloOutput",
                N = 1000,
                Options = new SimulationOptions(
                    0, // Note seed = 0 is -1 in linux
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete),
                SourceInput = new PointSourceCollimatedInput(),
                DetectorInputs = new List<IDetectorInput>
                    {
                        new ROfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                    }
            };

            _simulationInputVM = new SimulationInputViewModel(_simulationInput); 

            var rho = ((ROfRhoDetectorInput)_simulationInput.DetectorInputs.
                Where(d => d.TallyType == TallyType.ROfRho).First()).Rho;

            _independentVariableRangeVM = new RangeViewModel(rho, "mm", "Independent Variable Range");

            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain", SolutionDomainType.RofRho);

            Commands.MC_ExecuteMonteCarloSolver.Executed += MC_ExecuteMonteCarloSolver_Executed;
            Commands.FS_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;


            ExecuteMonteCarloSolverCommand = new RelayCommand(() => MC_ExecuteMonteCarloSolver_Executed(null, null));
        }

        public RelayCommand ExecuteMonteCarloSolverCommand { get; private set; }


        public SolutionDomainOptionViewModel SolutionDomainTypeOptionVM
        {
            get { return _solutionDomainTypeOptionVM; }
            set
            {
                _solutionDomainTypeOptionVM = value;
                OnPropertyChanged("SolutionDomainTypeOptionVM");
            }
        }

        public SimulationInputViewModel SimulationInputVM
        {
            get { return _simulationInputVM; }
            set
            {
                _simulationInputVM = value;
                OnPropertyChanged("SimulationInputVM");
            }
        }

        public RangeViewModel IndependentVariableRangeVM
        {
            get { return _independentVariableRangeVM; }
            set
            {
                _independentVariableRangeVM = value;
                OnPropertyChanged("IndependentVariableRangeVM");
            }
        }

        public IEnumerable<Point> ExecuteMonteCarloSolver()
        {
            IEnumerable<double> independentValues = IndependentVariableRangeVM.Values.ToList();

            var input = _simulationInputVM.GetSimulationInput();

            var simulation = new MonteCarloSimulation(input);

            Output output = simulation.Run();

            return EnumerableEx.Zip(independentValues, output.R_r, (x, y) => new Point(x, y));
        }

        private void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is RangeViewModel)
            {
                IndependentVariableRangeVM = (RangeViewModel)e.Parameter;
            }
        }

        private void MC_ExecuteMonteCarloSolver_Executed(object sender, ExecutedEventArgs e)
        {
            IEnumerable<Point> points = ExecuteMonteCarloSolver();

            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            string plotLabel = GetPlotLabel();
            Commands.Plot_PlotValues.Execute(new PlotData(points, plotLabel));

            Commands.TextOutput_PostMessage.Execute("Monte Carlo Solver executed.\r");
        }

        private PlotAxesLabels GetPlotLabels()
        {
            var sd = this.SolutionDomainTypeOptionVM;
            PlotAxesLabels axesLabels = null;
            if (sd.IndependentVariableAxisOptionVM.Options.Count > 1)
            {
                axesLabels = new PlotAxesLabels(
                    sd.IndependentAxisLabel, sd.IndependentAxisUnits,
                    sd.SelectedDisplayName, "", sd.ConstantAxisLabel,
                    sd.ConstantAxisUnits, sd.ConstantAxisValue);
            }
            else
            {
                axesLabels = new PlotAxesLabels(sd.IndependentAxisLabel,
                    sd.IndependentAxisUnits, sd.SelectedDisplayName, "");
            }
            return axesLabels;
        }

        private string GetPlotLabel()
        {
            return "Model - MC";// + 
            //"\rμa=" + OpticalPropertyVM.Mua + 
            //"\rμs'=" + OpticalPropertyVM.Musp + 
            //"\rg=" + OpticalPropertyVM.G;
        }
    }
}
