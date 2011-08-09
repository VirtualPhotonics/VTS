using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Ionic.Zip;
using SLExtensions.Input;
using Vts.Common;
using Vts.Extensions;
using Vts.IO;
using Vts.MonteCarlo;
using Vts.MonteCarlo.Detectors;
using Vts.MonteCarlo.IO;
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
        //private SimulationInput _simulationInput;
        private MonteCarloSimulation _simulation;

        private Output _output;

        private SimulationInputViewModel _simulationInputVM;

        private RangeViewModel _independentVariableRangeVM;

        private SolutionDomainOptionViewModel _solutionDomainTypeOptionVM;

        public MonteCarloSolverViewModel()
        {
            var simulationInput = GetDefaultSimulationInput();

            _simulationInputVM = new SimulationInputViewModel(simulationInput);

            var rho = ((ROfRhoDetectorInput)simulationInput.VirtualBoundaryInputs.
                Where(g => g.VirtualBoundaryType == VirtualBoundaryType.DiffuseReflectance).First().
                DetectorInputs.Where(d => d.TallyType == TallyType.ROfRho).First()).Rho;
            //Where(d => d.TallyType == TallyType.ROfRho).First()).Rho;

            _independentVariableRangeVM = new RangeViewModel(rho, "mm", "Independent Variable Range");

            SolutionDomainTypeOptionVM = new SolutionDomainOptionViewModel("Solution Domain", SolutionDomainType.RofRho);

            //Commands.MC_ExecuteMonteCarloSolver.Executed += MC_ExecuteMonteCarloSolver_Executed;
            Commands.FS_SetIndependentVariableRange.Executed += SetIndependentVariableRange_Executed;

            ExecuteMonteCarloSolverCommand = new RelayCommand(() => MC_ExecuteMonteCarloSolver_Executed(null, null));
            LoadSimulationInputCommand = new RelayCommand(() => MC_LoadSimulationInput_Executed(null, null));
            DownloadDefaultSimulationInputCommand = new RelayCommand(() => MC_DownloadDefaultSimulationInput_Executed(null, null));
            SaveSimulationResultsCommand = new RelayCommand(() => MC_SaveSimulationResults_Executed(null, null));
        }

        public RelayCommand ExecuteMonteCarloSolverCommand { get; private set; }
        public RelayCommand LoadSimulationInputCommand { get; private set; }
        public RelayCommand DownloadDefaultSimulationInputCommand { get; private set; }
        public RelayCommand SaveSimulationResultsCommand { get; private set; }

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

        private SimulationInput GetDefaultSimulationInput()
        {
            var simulationInput = new SimulationInput
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
                OutputName = "MonteCarloOutput",
                N = 100,
                Options = new SimulationOptions(
                    0, // Note seed = 0 is -1 in linux
                    RandomNumberGeneratorType.MersenneTwister,
                    AbsorptionWeightingType.Discrete),
                SourceInput = new DirectionalPointSourceInput(),
                VirtualBoundaryInputs = new List<IVirtualBoundaryInput>
                {
                    new SurfaceVirtualBoundaryInput(
                        VirtualBoundaryType.DiffuseReflectance,
                        new List<IDetectorInput>
                        {
                            new ROfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                        },
                        false, // write database
                        VirtualBoundaryType.DiffuseReflectance.ToString()
                    ),
                    new SurfaceVirtualBoundaryInput(
                        VirtualBoundaryType.DiffuseTransmittance,
                        new List<IDetectorInput>
                        {
                            //new TOfRhoDetectorInput(new DoubleRange(0.0, 40.0, 201)), // rho: nr=200 dr=0.2mm used for workshop)
                        },
                        false, // write database
                        VirtualBoundaryType.DiffuseTransmittance.ToString()
                    )
                }
            };

            return simulationInput;
        }

        //private IEnumerable<Point> ExecuteMonteCarloSolver()
        //{
        //    IEnumerable<double> independentValues = IndependentVariableRangeVM.Values.ToList();

        //    var input = _simulationInputVM.SimulationInput;

        //    _simulation = new MonteCarloSimulation(input);

        //    _output = simulation.Run();

        //    return EnumerableEx.Zip(independentValues, output.R_r, (x, y) => new Point(x, y));
        //}

        private void SetIndependentVariableRange_Executed(object sender, ExecutedEventArgs e)
        {
            if (e.Parameter is RangeViewModel)
            {
                IndependentVariableRangeVM = (RangeViewModel)e.Parameter;
            }
        }

        private void MC_ExecuteMonteCarloSolver_Executed(object sender, ExecutedEventArgs e)
        {
            IEnumerable<double> independentValues = IndependentVariableRangeVM.Values.ToList();

            var input = _simulationInputVM.SimulationInput;

            _simulation = new MonteCarloSimulation(input);

            _output = _simulation.Run();

            IEnumerable<Point> points = EnumerableEx.Zip(independentValues, _output.R_r, (x, y) => new Point(x, y));

            //IEnumerable<Point> points = ExecuteMonteCarloSolver();

            PlotAxesLabels axesLabels = GetPlotLabels();
            Commands.Plot_SetAxesLabels.Execute(axesLabels);

            string plotLabel = GetPlotLabel();
            Commands.Plot_PlotValues.Execute(new PlotData(points, plotLabel));

            Commands.TextOutput_PostMessage.Execute("Monte Carlo Solver executed.\r");
        }

        private void MC_LoadSimulationInput_Executed(object sender, ExecutedEventArgs e)
        {
            using (var stream = StreamFinder.GetLocalFilestreamFromOpenFileDialog("xml"))
            {
                if (stream != null)
                {
                    var simulationInput = FileIO.ReadFromStream<SimulationInput>(stream);

                    var validationResult = SimulationInputValidation.ValidateInput(simulationInput);
                    if (validationResult.IsValid)
                    {
                        _simulationInputVM.SimulationInput = simulationInput;
                        Commands.TextOutput_PostMessage.Execute("Simulation input loaded.\r");
                    }
                    else
                    {
                        Commands.TextOutput_PostMessage.Execute("Simulation input not loaded - XML format not valid.\r");
                    }
                }
            }
        }

        private void MC_DownloadDefaultSimulationInput_Executed(object sender, ExecutedEventArgs e)
        {
            using (var stream = StreamFinder.GetLocalFilestreamFromSaveFileDialog("xml"))
            {
                if (stream != null)
                {
                    // todo: fix
                    // var simulationInput = FileIO.ReadFromXMLInResources<SimulationInput>("MonteCarlo/Resources/DataStructures/" + "infile_ROfRho.xml", "Vts");
                    var simulationInput = GetDefaultSimulationInput();

                    FileIO.WriteToStream(simulationInput, stream);

                    Commands.TextOutput_PostMessage.Execute("Simulation input exported to file.\r");
                }
            }
        }

        private void MC_SaveSimulationResults_Executed(object sender, ExecutedEventArgs e)
        {
            if (_output != null)
            {
                var input = _simulationInputVM.SimulationInput;
                string resultsFolder = input.OutputName;
                FileIO.CreateDirectory(resultsFolder);
                input.ToFile(resultsFolder + "\\" + input.OutputName + ".xml");
                foreach (var result in _output.ResultsDictionary.Values)
                {
                    // save all detector data to the specified folder
                    DetectorIO.WriteDetectorToFile(result, resultsFolder);
                }

                var store = IsolatedStorageFile.GetUserStoreForApplication();
                if (store.DirectoryExists(resultsFolder))
                {
                    var fileNames = store.GetFileNames(resultsFolder + @"\*");

                    // then, zip all these together and SaveFileDialog to .zip...
                    using (var stream = StreamFinder.GetLocalFilestreamFromSaveFileDialog("zip"))
                    {
                        if (stream != null)
                        {
                            FileIO.ZipFiles(fileNames, resultsFolder, stream);
                            Commands.TextOutput_PostMessage.Execute("Simulation results exported to file.\r");
                        }
                    }
                }
            }
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
