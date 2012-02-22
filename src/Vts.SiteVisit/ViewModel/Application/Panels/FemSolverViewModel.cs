using System;
using System.Linq;
using Vts.FemModeling.MGRTE._2D;
using GalaSoft.MvvmLight.Command;
using Vts.SiteVisit.Input;
using Vts.FemModeling.MGRTE._2D.DataStructures;
using Vts.SiteVisit.Model;
using Vts.Common.Logging;

namespace Vts.SiteVisit.ViewModel
{
    public class FemSolverViewModel : BindableObject
    {
        private SimulationInputs _parameters;
        private OpticalPropertyViewModel _mediumOpticalPropertyVM;
        private OpticalPropertyViewModel _inclusionOpticalPropertyVM;

        ILogger logger = LoggerFactoryLocator.GetDefaultNLogFactory().Create(typeof(FemSolverViewModel));   

        public FemSolverViewModel(SimulationInputs parameters)
        {
            _parameters = parameters;
            MediumOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "Tissue Optical Properties:", G = 0.8, N = 1.33, EnableG = true };
            InclusionOpticalPropertyVM = new OpticalPropertyViewModel() { Title = "Inclusion:", G = MediumOpticalPropertyVM.G, N = MediumOpticalPropertyVM.N, EnableG = false, EnableN = false };

            Commands.FEM_ExecuteFemSolver.Executed += FEM_ExecuteFemSolver_Executed;

            ExecuteFemSolverCommand = new RelayCommand(() => FEM_ExecuteFemSolver_Executed(null, null));    
        }


        public FemSolverViewModel()
            : this(new SimulationInputs
            {
                NTissue = 1.0,
                NExt = 1.0,
                SMeshLevel = 3,
                AMeshLevel = 5,
                ConvTol = 1e-4,
                MgMethod = 6,
                NIterations = 100,
                Length = 1.0, 
                InRad = 0.5,
                InX = 0.1,
                InZ = 0.1,
            })
        {
        }

        public RelayCommand ExecuteFemSolverCommand { get; private set; }

        public OpticalPropertyViewModel MediumOpticalPropertyVM
        {
            get { return _mediumOpticalPropertyVM; }
            set
            {
                _mediumOpticalPropertyVM = value;
                OnPropertyChanged("MediumOpticalPropertyVM");
            }
        }

        public OpticalPropertyViewModel InclusionOpticalPropertyVM
        {
            get { return _inclusionOpticalPropertyVM; }
            set
            {
                _inclusionOpticalPropertyVM = value;
                OnPropertyChanged("InclusionOpticalPropertyVM");
            }
        }

        public SimulationInputs Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                OnPropertyChanged("Parameters");
            }
        }

        private void FEM_ExecuteFemSolver_Executed(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            ExecuteSolver();
        }

        private void ExecuteSolver()
        {
            // Purpose: this is the main function for RTE_2D.
            // Note: we assume the spatial domain has "nt" intervals,
            //       starting from "-x" to "+x" with increasing "x" coordinate;
            //       starting from "0" to "+z" with increasing "z" coordinate;   

            _parameters.MedG = MediumOpticalPropertyVM.G;
            _parameters.MedMua = MediumOpticalPropertyVM.Mua;
            _parameters.MedMusp = MediumOpticalPropertyVM.Musp;

            _parameters.InG = _parameters.MedG;
            _parameters.InMua = InclusionOpticalPropertyVM.Mua;
            _parameters.InMusp = InclusionOpticalPropertyVM.Musp;

            if ((_parameters.AMeshLevel > 8) || (_parameters.SMeshLevel > 8))
                logger.Info(() => "Angular or Spatial mesh level is larger than 8\n");
            else
            {
                Measurement measurement = SolverMGRTE.ExecuteMGRTE(_parameters);
                var meshData = new MapData(measurement.inten, measurement.xloc, measurement.zloc, measurement.dx,
                                           measurement.dz);
                Commands.Mesh_PlotMap.Execute(meshData);
            }
        }
    }
}
